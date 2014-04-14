using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management.Instrumentation;
using System.Xml.Linq;
using HockeyStats.CLI.Api.Model;
using HockeyStats.CLI.Helpers;

namespace HockeyStats.CLI.DomainServices
{
    public static class DataRepo
    {
        private static XDocument _xml;

        static DataRepo()
        {
            var a = @"Data\xt.nhl.21100-EVS-20090325T221959-0500";
            var b = @"HockeyStats.CLI.Data.xt.nhl.21100-EVS-20090325T221959-0500.xml";
            using (var s = ManifestResourceHelper.GetManifestResourcesStream(b))
            {
                _xml = XDocument.Load(s);
            }
        }

        public static IEnumerable<HockeyGameSummaryLineItem> GetGames()
        {
            //var x = _xml.CreateNavigator().Select("/sports-content/sports-event/event-//metadata/@start-date-time");
            var events =
                _xml.Elements("sports-content")
                    .Elements("sports-event");

            var games = events.Select(e =>
                {
                    var eventMetaData = e.Elements("event-metadata").First();
                    var datePlayed = eventMetaData.Attribute("start-date-time").Value;
                    var eventKey = eventMetaData.Attribute("event-key").Value;

                    var gameDate = ParseDatePlayed(datePlayed);

                    var homeTeam = GetTeamHeaderByAlignment(e, "home");
                    var awayTeam = GetTeamHeaderByAlignment(e, "away");

                    return new HockeyGameSummaryLineItem
                        {
                            EventKey = eventKey,
                            DatePlayed = gameDate,
                            HomeTeam = homeTeam,
                            AwayTeam = awayTeam,
                        };
                });

            return games;
        }

        private static DateTime ParseDatePlayed(string datePlayed)
        {
            var year = Int32.Parse(datePlayed.Substring(0, 4));
            var month = Int32.Parse(datePlayed.Substring(4, 2));
            var date = Int32.Parse(datePlayed.Substring(6, 2));

            return new DateTime(year, month, date);
        }

        private static TeamHeader GetTeamHeaderByAlignment(XElement e, string alignment)
        {
            var team =
                e.Elements("team").First(t => t.Element("team-metadata").Attribute("alignment").Value == alignment);
            return ParseTeamHeader(team);
        }

        private static TeamHeader ParseTeamHeader(XElement team)
        {
            var metadata = team.Element("team-metadata");
            var id = metadata.Attribute("team-key").Value.Replace(KeyRoots.Team, "");
            var name = metadata.Element("name");
            var firstName = name.Attribute("first").Value;
            var lastName = name.Attribute("last").Value;
            var teamName = name.Attribute("full").Value;
            var abbreviation = name.Attribute("abbreviation").Value;
            return new TeamHeader
                {
                    Id = Convert.ToInt32(id),
                    City = firstName,
                    Franchise = lastName,
                    Name = teamName,
                    Abbreviation = abbreviation
                };
        }

        public static class KeyRoots
        {
            public static string Team = "l.nhl.com-t.";
            public static string Player = "l.nhl.com-p.";
        }

        public static Team GetTeam(int id)
        {
            var teams = _xml.Element("sports-content").Elements("sports-event").Elements("team");
            var teamKey = KeyRoots.Team + id.ToString();
            var theTeam = teams.FirstOrDefault(t => t.Element("team-metadata").Attribute("team-key").Value == teamKey);
            if (theTeam == null)
            {
                throw new InstanceNotFoundException("Could not find team " + teamKey);
            }
            var header = ParseTeamHeader(theTeam);
            return new Team
                {
                    Id = header.Id,
                    Name = header.Name,
                    Abbreviation = header.Name,
                    Players = theTeam.Elements("player")
                                     .Select(GetPlayerFromPlayerXml)
                                     .OrderBy(p => p.Number)
                                     .ToArray()
                };
        }

        public static XNamespace xts = "http://www.xmlteam.com/";

        private static Team.Player GetPlayerFromPlayerXml(XElement playerXml)
        {
            var id = playerXml.Attribute("id").Value.Replace(KeyRoots.Player, "");
            var metaDataXml = playerXml.Element("player-metadata");
            var nameXml = metaDataXml.Element("name");
            var positionAbbreviation = metaDataXml.Attribute(xts + "position-abbreviation").Value;
            var decoration = metaDataXml.Attribute(xts + "captain").Value;
            return new Team.Player
                {
                    Id = int.Parse(id),
                    FirstName = nameXml.Attribute("first").Value,
                    LastName = nameXml.Attribute("last").Value,
                    Position = metaDataXml.Attribute("position-event").Value,
                    PositionAbbreviation = positionAbbreviation,
                    Number = int.Parse(metaDataXml.Attribute("uniform-number").Value),
                    Status = metaDataXml.Attribute("status").Value,
                    Decoration = decoration
                };
        }

        public static GameDetail GetGameDetail(string eventKey)
        {
            var game =
                _xml.Elements("sports-content")
                    .Elements("sports-event")
                    .FirstOrDefault(e => e.Element("event-metadata").Attribute("event-key").Value == eventKey);

            if (game == null) throw new InstanceNotFoundException();

            var datePlayed = game.Element("event-metadata").Attribute("start-date-time").Value;

            var eventListParent = game.Element("event-actions")
                                      .Element("event-actions-ice-hockey");

            var homeTeam = GetTeamHeaderByAlignment(game, "home");
            var awayTeam = GetTeamHeaderByAlignment(game, "away");

            var events = eventListParent
                .Elements("action-ice-hockey-play")
                .Select(e => GetGameEventFromXML(e, awayTeam, homeTeam));
            var scores = eventListParent
                .Elements("action-ice-hockey-score")
                .Select(e => GetScoreFromXml(e, awayTeam, homeTeam));

            var periods = scores.GroupBy(s => s.Period).Select(g =>
                {
                    var goalsAway = g.Count(s => s.Team == TeamRole.Away);
                    var goalsHome = g.Count(s => s.Team == TeamRole.Home);
                    return GameSummary.PeriodSummary.Create(g.Key, goalsAway, goalsHome);
                })
                .ToDictionary(p => p.Number, p => p);

            var numberOfPeriods = Math.Max(3, periods.Keys.Max());

            return new GameDetail()
                {
                    HomeTeam = GetTeamHeaderByAlignment(game, "home"),
                    AwayTeam = GetTeamHeaderByAlignment(game, "away"),
                    Date = ParseDatePlayed(datePlayed),
                    Periods = Enumerable.Range(1, numberOfPeriods)
                                        .Select(i => periods.ContainsKey(i) 
                                                        ? periods[i] 
                                                        : GameSummary.PeriodSummary.Empty(i))
                                        .OrderBy(p => p.Number)
                                        .ToArray()
                };
        }

       private static GameEvent GetGameEventFromXML(XElement x, TeamHeader awayTeam, TeamHeader homeTeam)
        {
            var playType = x.Attribute("play-type").Value;
            if (playType.StartsWith("shot-"))
            {
                return GetShotFromXml(x, awayTeam, homeTeam);
            }
            else
            {
                var period = x.Attribute("period-value").Value;
                var timeRemaining = x.Attribute("period-time-remaining").Value;
                var timeElapsed = x.Attribute("period-time-elapsed").Value;
                var scoreAway = x.Attribute(xts + "score-team-away").Value;
                var scoreHome = x.Attribute(xts + "score-team-home").Value;
                return new GameEvent
                    {
                        PlayType = playType,
                        Period = int.Parse(period),
                        TimeRemaining = TimeSpan.Parse(timeRemaining),
                        TimeElapsed = TimeSpan.Parse(timeElapsed),
                        ScoreAway = int.Parse(scoreAway),
                        ScoreHome = int.Parse(scoreHome)
                    };
            }
        }

        private static ShotEvent GetShotFromXml(XElement x, TeamHeader awayTeam, TeamHeader homeTeam, string playType = null)
        {
            playType = playType ?? x.Attribute("play-type").Value;
            var period = x.Attribute("period-value").Value;
            var timeRemaining = x.Attribute("period-time-remaining").Value;
            var timeElapsed = x.Attribute("period-time-elapsed").Value;
            var teamId = x.Attribute("team-idref").Value;
            var teamRole = teamId == homeTeam.GetKey() ? TeamRole.Home : TeamRole.Away;
            var scoreAway = x.Attribute(xts + "score-team-away").Value;
            var scoreHome = x.Attribute(xts + "score-team-home").Value;
            var shotLocation = x.Attribute("location").Value.Split(',');
            var shotZone = x.Attribute("zone").Value;
            var shotDistance = x.Attribute("shot-distance").Value;
            var attemptType = x.Attribute("score-attempt-type").Value;
            var shotType = x.Attribute("shot-type").Value;
            var strength = x.Attribute("strength").Value;
            var commentAttribute = x.Attribute("comment");
            var comment = commentAttribute != null ? commentAttribute.Value : string.Empty;
            return new ShotEvent
                {
                    PlayType = playType,
                    Period = int.Parse(period),
                    Team = teamRole,
                    TimeRemaining = TimeSpan.Parse(timeRemaining),
                    TimeElapsed = TimeSpan.Parse(timeElapsed),
                    ScoreAway = int.Parse(scoreAway),
                    ScoreHome = int.Parse(scoreHome),
                    Location = new Point(int.Parse(shotLocation[0]), int.Parse(shotLocation[1])),
                    Strength = strength,
                    ScoreAttemptType = attemptType,
                    ShotType = shotType
                };
        }

        private static ShotEvent GetScoreFromXml(XElement x, TeamHeader awayTeam, TeamHeader homeTeam)
        {
            var shot = GetShotFromXml(x, awayTeam, homeTeam, "score");
            shot.Goal = true;
            return shot;
        }
    }

    public class GameSummary
    {
        public TeamHeader HomeTeam { get; set; }
        public TeamHeader AwayTeam { get; set; }
        public DateTime Date { get; set; }
        public GameSummary.PeriodSummary[] Periods { get; set; }

        public class PeriodSummary
        {
            public int Number { get; set; }

            public string Name
            {
                get { return GetNameOfPeriod(this.Number); }
            }

            public int GoalsAway { get; set; }
            public int GoalsHome { get; set; }

            public static PeriodSummary Create(int key, int goalsAway, int goalsHome)
            {
                return new PeriodSummary
                    {
                        Number = key,
                        GoalsAway = key < 5
                                        ? goalsAway
                                        : goalsAway > goalsHome ? 1 : 0,
                        GoalsHome = key < 5
                                        ? goalsHome
                                        : goalsHome > goalsAway ? 1 : 0,
                    };
            }

            public static PeriodSummary Empty(int number)
            {
                return Create(number, 0, 0);
            }

            private static string GetNameOfPeriod(int key)
            {
                switch (key)
                {
                    case 1:
                        return "1st";
                    case 2:
                        return "2nd";
                    case 3:
                        return "3rd";
                    case 4:
                        return "OT";
                    case 5:
                        return "SO";
                    default:
                        throw new NotSupportedException("Unrecognized period: " + key);
                }
            }
        }
    }

    public class GameEvent
    {
        public int Id { get; set; }
        public string PlayType { get; set; }
        public int Period { get; set; }
        public TimeSpan TimeElapsed { get; set; }
        public TimeSpan TimeRemaining { get; set; }
        public int ScoreAway { get; set; }
        public int ScoreHome { get; set; }
    }

    public class ShotEvent : GameEvent
    {
        public TeamRole Team { get; set; }
        public Point Location { get; set; }
        public string ShotType { get; set; }
        public string ScoreAttemptType { get; set; }
        public string Strength { get; set; }
        public bool Goal { get; set; }
    }

    public enum TeamRole
    {
        Home,
        Away
    }

    public class GameDetail
    {
        public TeamHeader HomeTeam { get; set; }
        public TeamHeader AwayTeam { get; set; }
        public DateTime Date { get; set; }
        public GameSummary.PeriodSummary[] Periods { get; set; }

        public int AwayGoals
        {
            get { return Periods.Sum(p => p.GoalsAway); }
        }

        public int HomeGoals
        {
            get { return Periods.Sum(p => p.GoalsHome); }
        }
    }
}