using Detective.Core.Cards;

namespace Detective.Core.Gameplay;
public sealed record CrimeSolution
{
    #region Properties
    public Card CharacterCard { get; }

    public Card LocationCard { get; }

    public Card WeaponCard { get; }
    #endregion

    #region Constructors
    public CrimeSolution(
        Card characterCard,
        Card locationCard,
        Card weaponCard)
    {
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
    }
    #endregion

    public bool MatchesGuess(Guess guess)
    {
        return this.WeaponCard.Equals(guess.WeaponCard)
            && this.LocationCard.Equals(guess.LocationCard)
            && this.CharacterCard.Equals(guess.CharacterCard);
    }

    public override string ToString()
    {
        return $"Solution: '{this.CharacterCard}' with '{this.WeaponCard}' at '{this.LocationCard}'";
    }
}
