using Detective.Core.Cards;

namespace Detective.Core.Builders;

public interface ICardBuilder
{
    #region Properties
    CardType Type { get; }
    #endregion

    IEnumerable<Card> Build();
}
