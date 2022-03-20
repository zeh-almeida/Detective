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

    public int Turn { get; private set; }
    #endregion

    #region Constructors
    public Guess(
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
        this.Turn = -1;
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
            && this.CharacterCard.Equals(other.CharacterCard)
            && this.LocationCard.Equals(other.LocationCard)
            && this.WeaponCard.Equals(other.WeaponCard)
            && this.Guesser.Equals(other.Guesser)
            && this.Turn.Equals(other.Turn)
            && Equals(this.Responder, other.Responder);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Guesser, this.Responder, this.CharacterCard, this.LocationCard, this.WeaponCard);
    }

    public void SetTurn(int turn)
    {
        if (!this.Turn.Equals(-1))
        {
            throw new Exception("Guess turn already set");
        }

        this.Turn = turn;
    }

    public int CompareTo(Guess? other)
    {
        return other is null
             ? 1
             : this.Turn.CompareTo(other.Turn);
    }
}
