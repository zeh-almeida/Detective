using Detective.Core.Cards;
using Detective.Core.Gameplay;
using Detective.Core.Players;

namespace Detective.Players;

public sealed class RandomPlayer : AbstractPlayer
{
    #region Constructors
    public RandomPlayer(string name)
        : base(name)
    {
    }
    #endregion

    public override Task<Guess> MakeGuess(
        int turnNumber,
        IPlayer _nextPlayer,
        IEnumerable<Card> cards,
        IEnumerable<Guess> _pastGuesses)
    {
        return Task.Run(() =>
        {
            var missing = cards.Where(c => !this.SeenCards.ContainsKey(c));

            var weaponCard = RandomlySelectCard(missing.Where(c => c.IsWeapon()));
            var locationCard = RandomlySelectCard(missing.Where(c => c.IsLocation()));
            var characterCard = RandomlySelectCard(missing.Where(c => c.IsCharacter()));

            if (weaponCard is null)
            {
                throw new Exception("Weapon card is not a Weapon?");
            }

            if (locationCard is null)
            {
                throw new Exception("Location card is not a Location?");
            }

            if (characterCard is null)
            {
                throw new Exception("Character card is not a Character?");
            }

            return new Guess(
                turnNumber,
                this,
                characterCard,
                locationCard,
                weaponCard);
        });
    }

    public override Task<Card> ShowMatchedCard(Guess guess)
    {
        var matchingCards = this.GuessedCards(guess);
        var randomized = RandomlySelectCard(matchingCards);

        return Task.FromResult(randomized);
    }
}
