using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Linq;
using System.Xml.XPath;
using HockeyStats.CLI.Helpers;

namespace HockeyStats.CLI.Api
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

                var homeTeam = GetTeam(e, "home");
                var awayTeam = GetTeam(e, "away");

                return new HockeyGameSummaryLineItem
                {
                    DatePlayed = new DateTime(year, month, date),
                    HomeTeam = homeTeam.Name,
                    HomeTeamAbbreviation = homeTeam.Abbreviation,
                    AwayTeam = awayTeam.Name,
                    AwayTeamAbbreviation = awayTeam.Abbreviation,
                };
            });

            return games;
        }

        private static HockeyGameSummaryLineItem.Team GetTeam(XElement e, string alignment)
        {
            var team = e.Elements("team").First(t => t.Element("team-metadata").Attribute("alignment").Value == alignment);
            var name = team.Element("team-metadata").Element("name");
            var teamName = name.Attribute("full").Value;
            var abbreviation = name.Attribute("abbreviation").Value;
            return new HockeyGameSummaryLineItem.Team
            {
                Name = teamName,
                Abbreviation = abbreviation
            };
        }
    }

    public class HockeyGameSummaryLineItem
    {
        public DateTime DatePlayed { get; set; }
        public string AwayTeam { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeamAbbreviation { get; set; }
        public string HomeTeamAbbreviation { get; set; }

        public class Team
        {
            public string Name { get; set; }
            public string Abbreviation { get; set; }
        }
    }

    public class GamesController : ApiController
    {
        public IHttpActionResult Get()
        {
            return Json(DataRepo.GetGames());
        }
    }
}
