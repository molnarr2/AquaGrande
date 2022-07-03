namespace AquaGrande; 

public class Round
{
    public int RoundNumber { get; set; }
    public List<TeamMatchUp> Matches { get; } = new List<TeamMatchUp>();
}

public class TeamMatchUp
{
    public Team Team1 { get; }
    public Team Team2 { get; }
    public Team? Team3 { get; }
    public Game Game { get; }

    public TeamMatchUp(Team team1, Team team2, Team team3, Game game)
    {
        this.Team1 = team1;
        this.Team2 = team2;
        this.Team3 = team3;
        this.Game = game;
    }
    
    public TeamMatchUp(Team team1, Team team2, Game game)
    {
        this.Team1 = team1;
        this.Team2 = team2;
        Team3 = null;
        this.Game = game;
    }

}
