namespace MemoGame.Models;

public class Player
{
	public string Name { get; set; }
	public int Moves { get; private set; }
	public int MatchesFound { get; private set; }

	public Player(string name = "Mängija")
	{
		Name = name;
		Reset();
	}

	public void IncrementMoves() => Moves++;
	public void AddMatch() => MatchesFound++;

	public void Reset()
	{
		Moves = 0;
		MatchesFound = 0;
	}

	public void SaveBestScore(int seconds)
	{
		int bestMoves = Preferences.Default.Get("BestMoves", int.MaxValue);
		if (Moves < bestMoves)
		{
			Preferences.Default.Set("BestMoves", Moves);
			Preferences.Default.Set("BestTime", seconds);
		}
	}

	public static (int Moves, int Seconds) GetBestScore()
	{
		int moves = Preferences.Default.Get("BestMoves", -1);
		int seconds = Preferences.Default.Get("BestTime", -1);
		return (moves, seconds);
	}
}