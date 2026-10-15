using MemoGame.Models;

namespace MemoGame.Services;

public class Game
{
	private readonly List<string> _availableIcons = new() { "🚀", "🎮", "👾", "⚡", "💎", "🔥", "🐱", "🎲" };
	private IDispatcherTimer? _timer;

	public List<Card> Cards { get; private set; } = new();
	public Player CurrentPlayer { get; private set; }
	public int SecondsElapsed { get; private set; }
	public bool IsBusy { get; private set; }

	public event Action? OnTick;
	public event Action<bool>? OnGameFinished;

	public Game()
	{
		CurrentPlayer = new Player();
	}

	public void InitializeGame(IDispatcher dispatcher)
	{
		CurrentPlayer.Reset();
		SecondsElapsed = 0;
		IsBusy = false;

		// Собираем 8 пар (16 карт) и перемешиваем
		var selectedIcons = _availableIcons.Take(8).ToList();
		var cardPool = selectedIcons.Concat(selectedIcons).ToList();
		var rng = new Random();
		cardPool = cardPool.OrderBy(_ => rng.Next()).ToList();

		Cards.Clear();
		for (int i = 0; i < cardPool.Count; i++)
		{
			Cards.Add(new Card(i, cardPool[i]));
		}

		SetupTimer(dispatcher);
	}

	private void SetupTimer(IDispatcher dispatcher)
	{
		_timer?.Stop();
		_timer = dispatcher.CreateTimer();
		_timer.Interval = TimeSpan.FromSeconds(1);
		_timer.Tick += (s, e) =>
		{
			SecondsElapsed++;
			OnTick?.Invoke();
		};
		_timer.Start();
	}

	public void Stop() => _timer?.Stop();

	public async Task<(bool isMatch, Card? first, Card? second)> HandleCardClickAsync(Card clickedCard)
	{
		if (IsBusy || clickedCard.IsFaceUp || clickedCard.IsMatched)
			return (false, null, null);

		clickedCard.SetFaceUp(true);

		var faceUpCards = Cards.Where(c => c.IsFaceUp && !c.IsMatched).ToList();

		if (faceUpCards.Count < 2)
			return (false, clickedCard, null);

		// Открыто две карты — блокируем клики на время анализа
		IsBusy = true;
		CurrentPlayer.IncrementMoves();

		var first = faceUpCards[0];
		var second = faceUpCards[1];

		if (first.Symbol == second.Symbol)
		{
			first.MarkAsMatched();
			second.MarkAsMatched();
			CurrentPlayer.AddMatch();
			IsBusy = false;

			if (Cards.All(c => c.IsMatched))
			{
				Stop();
				CurrentPlayer.SaveBestScore(SecondsElapsed);
				OnGameFinished?.Invoke(true);
			}

			return (true, first, second);
		}

		// Не совпали — пауза для запоминания
		await Task.Delay(800);
		first.SetFaceUp(false);
		second.SetFaceUp(false);
		IsBusy = false;

		return (false, first, second);
	}
}