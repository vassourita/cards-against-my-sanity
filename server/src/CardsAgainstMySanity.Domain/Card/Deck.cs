namespace CardsAgainstMySanity.Domain.Card;

using System.Collections;

internal class Deck : IEnumerable<ICard>
{
    public IEnumerator<ICard> GetEnumerator() => throw new NotImplementedException();

    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

    private readonly IEnumerable<ICard> Cards;
    public IEnumerable<ICard> GetCards() => this.Cards;
}
