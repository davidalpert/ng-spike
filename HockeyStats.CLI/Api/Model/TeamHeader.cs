using HockeyStats.CLI.DomainServices;

namespace HockeyStats.CLI.Api.Model
{
    public class TeamHeader
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string City { get; set; }
        public string Franchise { get; set; }

        public string GetKey() { return DataRepo.KeyRoots.Team + Id; }
    }
}