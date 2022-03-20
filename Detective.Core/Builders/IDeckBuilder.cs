using Detective.Core.Cards;
using Detective.Core.Gameplay;

namespace Detective.Core.Builders;

public interface IDeckBuilder
{
    #region Properties
    IEnumerable<Card> Cards { get; }

    IEnumerable<Card> PlayerCards { get; }

    CrimeSolution? Solution { get; }
    #endregion

    void Build();
}
