using Detective.Core.Cards;

namespace Detective.Cards;

public sealed record WeaponCardBuilder : AbstractCardBuilder
{
    #region Properties
    public override CardType Type => CardType.Weapon;
    #endregion

    public override IEnumerable<Card> Build()
    {
        return LoadData("weapons").Values
            .Select(value => new Card(CardType.Weapon, value))
            .ToArray();
    }
}
