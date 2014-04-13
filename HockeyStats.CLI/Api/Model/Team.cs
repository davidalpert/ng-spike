namespace HockeyStats.CLI.Api.Model
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public Team.Player[] Players { get; set; }

        public class Player
        {
            public int Id { get; set; }
            public string Name { get { return string.Format("{0} {1}", FirstName, LastName); } }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Position { get; set; }
            public string PositionAbbreviation { get; set; }
            public int Number { get; set; }
            public string Status { get; set; }
            public string Decoration { get; set; }
        }
    }
}