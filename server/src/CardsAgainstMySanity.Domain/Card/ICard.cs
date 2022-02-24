namespace CardsAgainstMySanity.Domain.Card;

public interface ICard
{
    int Id { get; }
    string Text { get; }
    int DeckId { get; }
}
