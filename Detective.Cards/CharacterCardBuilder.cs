using Detective.Core.Cards;

namespace Detective.Cards;

public sealed record CharacterCardBuilder : AbstractCardBuilder
{
    #region Properties
    public override CardType Type => CardType.Character;
    #endregion

    public override IEnumerable<Card> Build()
    {
        return LoadData("characters").Values
            .Select(value => new Card(CardType.Character, value))
            .ToArray();
    }
}
