// See https://aka.ms/new-console-template for more information

// 6 Teams:
// White Knights
// Black Ninjas
// Orange Crush
// Purple Power
// Read Rockets
// Blue Bombers

// 6 games:
// Obstacle Course
// Water War Zone
//    Bombs Away
// 3 Way Volleyball ---> Requires 3 teams
// Water Kickball
// Bumper Ball

using AquaGrande;
using ConsoleTables;

var games = new List<Game>
{
    new Game("Obstacle Course"),
    new Game("Water War Zone"),
    new Game("Bombs Away"),
    new Game("Water Kickball"),
    new Game("Bumper Ball"),
    new Game("3 Way Volleyball") {threeWay = true},
};
var gameCount = games.Count;

var teams = new List<Team>
{
    new Team("White Knights", gameCount),
    new Team("Black Ninjas", gameCount),
    new Team("Orange Crush", gameCount),
    new Team("Purple Power", gameCount),
    new Team("Read Rockets", gameCount),
    new Team("Blue Bombers", gameCount)
};

var matchRound = new MatchRoundGenerator(teams, games);
var rounds = matchRound.Generate(10000);

var table = new ConsoleTable ("Round", "Game", "Team 1", "Team 2", "Team 3");
for (var i = 0; i < rounds.Count; i++) {
    foreach (var match in rounds[i].Matches) {
        var team3 = "";
        if (match.Team3 != null) {
            team3 = match.Team3.Name;
        }

        table.AddRow ($"Round {i + 1}", match.Game.Name, match.Team1.Name, match.Team2.Name, team3);
    }
}
table.Write();
Console.WriteLine();


