using System.Collections;

namespace CardsAgainstMySanity.Domain.Card
{
    class Deck : IEnumerable<ICard>
    {
        public IEnumerator<ICard> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
