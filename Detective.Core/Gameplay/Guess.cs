using Detective.Core.Cards;
using Detective.Core.Players;

namespace Detective.Core.Gameplay;

public sealed class Guess : IEquatable<Guess?>, IComparable<Guess?>
{
    #region Properties
    public IPlayer Guesser { get; }

    public IPlayer? Responder { get; private set; }

    public Card CharacterCard { get; }

    public Card LocationCard { get; }

    public Card WeaponCard { get; }

    public int Turn { get; }
    #endregion

    #region Constructors
    public Guess(
        int turnNumber,
        IPlayer guesser,
        Card characterCard,
        Card locationCard,
        Card weaponCard)
    {
        if (guesser is null)
        {
            throw new ArgumentNullException(nameof(guesser), "no guesser?");
        }

        this.Guesser = guesser;

        if (!characterCard.IsCharacter())
        {
            throw new ArgumentException("Character card is not a Character", nameof(characterCard));
        }

        if (!locationCard.IsLocation())
        {
            throw new ArgumentException("Location card is not a Location", nameof(characterCard));
        }

        if (!weaponCard.IsWeapon())
        {
            throw new ArgumentException("Weapon card is not a Weapon", nameof(characterCard));
        }

        this.CharacterCard = characterCard;
        this.LocationCard = locationCard;
        this.WeaponCard = weaponCard;
        this.Turn = turnNumber;
    }
    #endregion

    public void SetResponder(IPlayer player)
    {
        if (this.Responder is not null)
        {
            throw new ArgumentException("Responder already set", nameof(player));
        }

        this.Responder = player;
    }

    public override string ToString()
    {
        return $"'{this.CharacterCard}' with '{this.WeaponCard}' at '{this.LocationCard}'";
    }

    public override bool Equals(object? obj)
    {
        return this.Equals(obj as Guess);
    }

    public bool Equals(Guess? other)
    {
        return other is not null
            && this.Turn.Equals(other.Turn)
            && this.Guesser.Equals(other.Guesser);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Guesser, this.Turn);
    }

    public int CompareTo(Guess? other)
    {
        return other is null
             ? 1
             : this.Turn.CompareTo(other.Turn);
    }
}
