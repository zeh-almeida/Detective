using Detective.Core.Cards;

namespace Detective.Cards;

public sealed record LocationCardBuilder : AbstractCardBuilder
{
    #region Properties
    public override CardType Type => CardType.Location;
    #endregion

    public override IEnumerable<Card> Build()
    {
        return LoadData("locations").Values
            .Select(value => new Card(CardType.Location, value))
            .ToArray();
    }
}
