namespace Detective.Core.Cards;

public sealed class Card : IEquatable<Card?>, IComparable<Card>
{
    #region Properties
    public CardType Type { get; }

    public string Value { get; }
    #endregion

    #region Constructors
    public Card(CardType type, string value)
    {
        this.Type = type;
        this.Value = value;
    }
    #endregion

    #region Type checks
    public bool IsCharacter()
    {
        return CardType.Character.Equals(this.Type);
    }

    public bool IsLocation()
    {
        return CardType.Location.Equals(this.Type);
    }

    public bool IsWeapon()
    {
        return CardType.Weapon.Equals(this.Type);
    }
    #endregion

    #region Equality
    public override bool Equals(object? obj)
    {
        return this.Equals(obj as Card);
    }

    public bool Equals(Card? other)
    {
        return other is not null
            && Equals(this.Type, other.Type)
            && string.Equals(this.Value, other.Value, StringComparison.InvariantCultureIgnoreCase);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Type, this.Value);
    }
    #endregion

    public override string ToString()
    {
        return this.Value;
    }

    public int CompareTo(Card? other)
    {
        return other is null ? 1 : this.Value.CompareTo(other.Value);
    }
}
