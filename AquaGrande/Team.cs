namespace AquaGrande; 

public class Team
{
    public string Name { get; }

    public List<Game> PlayedGames { get; } = new List<Game>();

    private int gameCount;
    public bool IsCompleted => PlayedGames.Count >= gameCount;
    
    public int ExtraGamesPlayed  {
        get {
            if (!IsCompleted) {
                return 0;
            }
            return PlayedGames.Count - gameCount;
        }
    }
    
    public Team(string name, int gameCount)
    {
        this.Name = name;
        this.gameCount = gameCount;
    }

    public bool NotPlayed(Game game)
    {
        return !PlayedGames.Contains(game);
    }
}