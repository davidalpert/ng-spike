using System;

namespace HockeyStats.CLI.Api.Model
{
    public class HockeyGameSummaryLineItem
    {
        public string EventKey { get; set; }
        public DateTime DatePlayed { get; set; }
        public TeamHeader AwayTeam { get; set; }
        public TeamHeader HomeTeam { get; set; }
    }
}