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
            var team = DataRepo.GetTeam(16);

            Assert.NotNull(team);
        }
    }
}
