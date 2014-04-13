using System;
using System.Collections.Generic;
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
                    var datePlayed = e.Elements("event-metadata")
                                      .Select(x => x.Attribute("start-date-time").Value)
                                      .First();

                    var year = Int32.Parse(datePlayed.Substring(0, 4));
                    var month = Int32.Parse(datePlayed.Substring(4, 2));
                    var date = Int32.Parse(datePlayed.Substring(6, 2));

                    var homeTeam = GetTeamHeaderByAlignment(e, "home");
                    var awayTeam = GetTeamHeaderByAlignment(e, "away");

                    return new HockeyGameSummaryLineItem
                        {
                            DatePlayed = new DateTime(year, month, date),
                            HomeTeam = homeTeam,
                            AwayTeam = awayTeam,
                        };
                });

            return games;
        }

        private static TeamHeader GetTeamHeaderByAlignment(XElement e, string alignment)
        {
            var team = e.Elements("team").First(t => t.Element("team-metadata").Attribute("alignment").Value == alignment);
            return ParseTeamHeader(team);
        }

        private static TeamHeader ParseTeamHeader(XElement team)
        {
            var metadata = team.Element("team-metadata");
            var id = metadata.Attribute("team-key").Value.Replace(KeyRoots.Team, "");
            var name = metadata.Element("name");
            var teamName = name.Attribute("full").Value;
            var abbreviation = name.Attribute("abbreviation").Value;
            return new TeamHeader
                {
                    Id = Convert.ToInt32(id),
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
            var positionAbbreviation = metaDataXml.Attribute(xts+"position-abbreviation").Value;
            var decoration = metaDataXml.Attribute(xts+"captain").Value;
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
    }
}