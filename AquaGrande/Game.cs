namespace AquaGrande; 

public class Game
{
    public string Name { get; }
    public bool threeWay { get; set; }
    
    public Game(string name)
    {
        Name = name;
    }
}