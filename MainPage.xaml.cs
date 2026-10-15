using MemoGame.Models;
using MemoGame.Services;

namespace MemoGame;

public partial class MainPage : ContentPage
{
	private readonly Game _game;
	private readonly List<Theme> _themes;
	private readonly Dictionary<Card, Border> _cardViews = new();

	public MainPage()
	{
		InitializeComponent();

		_game = new Game();
		_themes = Theme.GetAvailableThemes();

		ThemePicker.ItemsSource = _themes.Select(t => t.Name).ToList();
		ThemePicker.SelectedIndex = 0;

		_game.OnTick += () => TimerLabel.Text = $"{_game.SecondsElapsed}s";
		_game.OnGameFinished += HandleGameOver;

		StartNewGame();
	}

	private void StartNewGame()
	{
		_game.InitializeGame(Dispatcher);
		UpdateDashboard();
		RenderBoard();
	}

	private void RenderBoard()
	{
		BoardGrid.Children.Clear();
		_cardViews.Clear();

		for (int i = 0; i < _game.Cards.Count; i++)
		{
			var card = _game.Cards[i];
			int row = i / 4;
			int col = i % 4;

			var label = new Label
			{
				Text = "?",
				FontSize = 26,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				TextColor = Colors.White
			};

			var border = new Border
			{
				StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
				Content = label
			};
			border.SetDynamicResource(Border.BackgroundColorProperty, "CardClosedBrush");

			var tap = new TapGestureRecognizer();
			tap.Tapped += async (s, e) => await OnCardTapped(card, border, label);
			border.GestureRecognizers.Add(tap);

			_cardViews[card] = border;
			BoardGrid.Add(border, col, row);
		}
	}

	private async Task OnCardTapped(Card card, Border cardView, Label cardLabel)
	{
		if (_game.IsBusy || card.IsFaceUp || card.IsMatched) return;

		// Анимация первого полуоборота (скрытие рубашки)
		await cardView.RelRotateTo(90, 150, Easing.CubicIn);
		cardLabel.Text = card.Symbol;
		cardLabel.TextColor = (Color)Resources["TextColorBrush"];
		cardView.SetDynamicResource(Border.BackgroundColorProperty, "CardOpenBrush");
		await cardView.RelRotateTo(90, 150, Easing.CubicOut);

		var (isMatch, first, second) = await _game.HandleCardClickAsync(card);
		MovesLabel.Text = _game.CurrentPlayer.Moves.ToString();

		// Если открылись две карты и они не совпали — анимируем закрытие обратно
		if (!isMatch && first != null && second != null)
		{
			var view1 = _cardViews[first];
			var view2 = _cardViews[second];
			var label1 = (Label)view1.Content;
			var label2 = (Label)view2.Content;

			await Task.WhenAll(
				view1.RelRotateTo(90, 150, Easing.CubicIn),
				view2.RelRotateTo(90, 150, Easing.CubicIn)
			);

			label1.Text = "?";
			label2.Text = "?";
			label1.TextColor = Colors.White;
			label2.TextColor = Colors.White;
			view1.SetDynamicResource(Border.BackgroundColorProperty, "CardClosedBrush");
			view2.SetDynamicResource(Border.BackgroundColorProperty, "CardClosedBrush");

			await Task.WhenAll(
				view1.RelRotateTo(90, 150, Easing.CubicOut),
				view2.RelRotateTo(90, 150, Easing.CubicOut)
			);
		}
		else if (isMatch && first != null && second != null)
		{
			// Небольшая пульсация для совпавшей пары
			var view1 = _cardViews[first];
			var view2 = _cardViews[second];
			await Task.WhenAll(
				view1.ScaleTo(1.1, 100),
				view2.ScaleTo(1.1, 100)
			);
			await Task.WhenAll(
				view1.ScaleTo(1.0, 100),
				view2.ScaleTo(1.0, 100)
			);
		}
	}

	private void UpdateDashboard()
	{
		MovesLabel.Text = _game.CurrentPlayer.Moves.ToString();
		TimerLabel.Text = $"{_game.SecondsElapsed}s";

		var (bestMoves, bestTime) = Player.GetBestScore();
		BestScoreLabel.Text = bestMoves > 0
			? $"Rekord: {bestMoves} käiku ({bestTime}s)"
			: "Rekord: puudub";
	}

	private async void HandleGameOver(bool won)
	{
		await DisplayAlert(
			"Palju õnne!",
			$"Mäng läbi!\nKäike: {_game.CurrentPlayer.Moves}\nAeg: {_game.SecondsElapsed} sekundit.",
			"OK"
		);
		UpdateDashboard();
	}

	private void OnThemeChanged(object sender, EventArgs e)
	{
		if (ThemePicker.SelectedIndex >= 0 && ThemePicker.SelectedIndex < _themes.Count)
		{
			var selectedTheme = _themes[ThemePicker.SelectedIndex];
			selectedTheme.Apply(this);
		}
	}

	private void OnRestartClicked(object sender, EventArgs e) => StartNewGame();
}