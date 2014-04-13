using System;

namespace HockeyStats.CLI.Api.Model
{
    public class HockeyGameSummaryLineItem
    {
        public DateTime DatePlayed { get; set; }
        public TeamHeader AwayTeam { get; set; }
        public TeamHeader HomeTeam { get; set; }
        public string AwayTeamAbbreviation { get; set; }
        public string HomeTeamAbbreviation { get; set; }
    }
}