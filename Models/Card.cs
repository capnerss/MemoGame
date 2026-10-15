namespace MemoGame.Models;

public class Card
{
	public int Id { get; }
	public string Symbol { get; }
	public bool IsFaceUp { get; private set; }
	public bool IsMatched { get; private set; }

	public Card(int id, string symbol)
	{
		Id = id;
		Symbol = symbol;
		IsFaceUp = false;
		IsMatched = false;
	}

	public void Flip() => IsFaceUp = !IsFaceUp;
	public void SetFaceUp(bool state) => IsFaceUp = state;
	public void MarkAsMatched() => IsMatched = true;

	public void Reset()
	{
		IsFaceUp = false;
		IsMatched = false;
	}
}