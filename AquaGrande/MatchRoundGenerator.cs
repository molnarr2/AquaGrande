namespace AquaGrande; 

public class MatchRoundGenerator
{
    List<Team> teams;
    List<Game> games;
    
    public MatchRoundGenerator(List<Team> teams, List<Game> games)
    {
        this.teams = teams;
        this.games = games;
    }

    public List<Round> Generate(int randomRounds)
    {
        // initialize with first generated.
        var bestGenerated = Generate ();
        var bestExtraGamesPlayed = teams.HowManyExtraGamesWerePlayed ();
        teams.ResetTeamsPlayedGames ();
        
        for (var i = 0; i < randomRounds; i++)
        {
            var generated = Generate();
            if (teams.DidEveryTeamPlayAllGames () && teams.HowManyExtraGamesWerePlayed () < bestExtraGamesPlayed) {
                bestGenerated = generated;
                bestExtraGamesPlayed = teams.HowManyExtraGamesWerePlayed ();
            }
            teams.ResetTeamsPlayedGames ();
        }

        return bestGenerated;
    }

    List<Team> TeamsNotPlayedInRound () {
        var teamsNotPlayedInRound = new List<Team> ();
        foreach (var team in teams) {
            if (!team.IsCompleted) {
                teamsNotPlayedInRound.Add(team);
            }
        }
        return teamsNotPlayedInRound;
    }

    List<Game> NotPlayedGamesInRound () {
        var notPlayedGames = new List<Game>();
        foreach (var g in games) {
            notPlayedGames.Add(g);
        }
        return notPlayedGames;
    }
    
    List<Round> Generate()
    {
        var rounds = new List<Round>();
        for (var round = 0; round < games.Count+1; round++) {
            var rnd = new Round();
            rounds.Add(rnd);

            var teamsNotPlayedInRound = teams.ToList ();
            var notPlayedGames = NotPlayedGamesInRound ();
            
            while (true) {
                // Teams that need to play a game and can play one. So if a team has completed all
                // games then they are not in this list.
                var teamsThatNeedToPlay = teamsNotPlayedInRound.TeamsThatNeedToPlay (notPlayedGames);
                if (teamsThatNeedToPlay.Count == 0) {
                    break;
                }
                
                // Create a match from open teams and open games.
                var match = teamsThatNeedToPlay.CreateMatch (notPlayedGames, teamsNotPlayedInRound);
                if (match == null) {
                    // Can't make any more matches.
                    break;
                }
                
                // These teams have played this game.
                match.Team1.PlayedGames.Add (match.Game);
                match.Team2.PlayedGames.Add (match.Game);
                match.Team3?.PlayedGames.Add (match.Game);

                // This game has been played and can't be played again during this round.
                notPlayedGames.Remove (match.Game);
                
                // These teams have played for this round so cannot play again until next round.
                teamsNotPlayedInRound.Remove (match.Team1);
                teamsNotPlayedInRound.Remove (match.Team2);
                if (match.Team3 != null) {
                    teamsNotPlayedInRound.Remove (match.Team3);
                }
                
                rnd.Matches.Add (match);
            }
        }

        return rounds;
    }
}

public static class ListGameExtensions
{
    private static Random rnd = new ();
    
    // Check that one game in the notPlayedGame does NOT exist in the team's played games.
    public static bool NotPlayedGame(this List<Game> notPlayedGames, List<Game> playedGames)
    {
        foreach (var notPlayed in notPlayedGames)
        {
            if (!playedGames.Contains(notPlayed))
            {
                return true;
            }
        }

        return false;
    }
}

public static class ListTeamExtensions
{
    static Random rnd = new Random();

    public static bool DidEveryTeamPlayAllGames (this List<Team> teams) {
        return teams.All (x => x.IsCompleted);
    }

    public static int HowManyExtraGamesWerePlayed (this List<Team> teams) {
        return teams.Sum (x => x.ExtraGamesPlayed);
    }

    public static void ResetTeamsPlayedGames (this List<Team> teams) {
        foreach (var team in teams) {
            team.PlayedGames.Clear ();
        }
    }
    
    public static List<Team> TeamsThatNeedToPlay (this List<Team> teamsNotPlayedInRound, List<Game> notPlayedGames) {
        var teamsOpenToPlay = new List<Team> ();
        
        foreach (var game in notPlayedGames) {
            foreach (var team in teamsNotPlayedInRound) {
                if (!team.IsCompleted && !team.PlayedGames.Contains (game)) {
                    teamsOpenToPlay.Add (team);
                }
            }
        }

        return teamsOpenToPlay.Distinct().ToList ();
    }

    public static TeamMatchUp? CreateMatch (this List<Team> teamsThatNeedToPlay, List<Game> notPlayedGames, List<Team> teamsNotPlayedInRound) {
        notPlayedGames.Shuffle ();
        foreach (var game in notPlayedGames) {
            var notPlayedGame = teamsThatNeedToPlay.NotPlayedGame (game);
            notPlayedGame.Shuffle ();
            
            // Do we need another team to play this game? If so then:
            // Add teams that haven't played yet that are comsouplete and have played this game one time.
            if (notPlayedGame.Count == 1) {
                var completedTeams = teamsNotPlayedInRound.CompleteTeamsPlayedGameOnce (game);
                notPlayedGame.AddRange (completedTeams);
            }
            
            if (game.threeWay && notPlayedGame.Count >= 3) {
                return new TeamMatchUp (notPlayedGame[0], notPlayedGame[1], notPlayedGame[2], game);
            } else if (!game.threeWay && notPlayedGame.Count >= 2) {
                return new TeamMatchUp (notPlayedGame[0], notPlayedGame[1], game);
            }
        }
        return null;
    }

    public static List<Team> CompleteTeamsPlayedGameOnce (this List<Team> teamsNotPlayedInRound, Game game) {
        var completedTeams = new List<Team> ();
        foreach (var notPlayedTeam in teamsNotPlayedInRound) {
            if (notPlayedTeam.IsCompleted && notPlayedTeam.PlayedGames.Count (x => x == game) == 1) {
                completedTeams.Add (notPlayedTeam);
            }
        }
        
        return completedTeams;
    }
    
    public static List<Team> NotPlayedGame (this List<Team> teamsNotPlayedInRound, Game game) {
        var notPlayedGame = new List<Team> ();
        foreach (var team in teamsNotPlayedInRound) {
            if (!team.PlayedGames.Contains (game)) {
                notPlayedGame.Add (team);
            }
        }

        return notPlayedGame;
    }
}
