using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HockeyStats.CLI.DomainServices;
using NUnit.Framework;

namespace HockeyStats.Tests
{
    [TestFixture]
    public class DataRepoTests
    {
        [Test]
        public void CanLoadTheBlackhawks()
        {
            var blackhawksId = 16;

            var team = DataRepo.GetTeam(blackhawksId);

            Assert.NotNull(team);
        }

        [Test]
        public void CanLoadGameSummary()
        {
            var eventKey = "l.nhl.com-2008-e.21100";

            var game = DataRepo.GetGameDetail(eventKey);

            Assert.NotNull(game);

            Assert.AreEqual("Sharks", game.AwayTeam.Franchise);
            Assert.AreEqual("Blackhawks", game.HomeTeam.Franchise);

            Assert.AreEqual(2009, game.Date.Year);
            Assert.AreEqual(03, game.Date.Month);
            Assert.AreEqual(25, game.Date.Day);

            Assert.AreEqual(5, game.Periods.Length);

            Assert.AreEqual("SO", game.Periods.Last().Name);
        }
    }
}
