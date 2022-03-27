using Detective.Core.Cards;
using Detective.Core.Gameplay;
using Detective.Core.Players;

namespace Detective.Players;

public sealed class DumbPlayer : AbstractPlayer
{
    #region Properties
    private ISet<Card> SeenCards { get; }
    #endregion

    #region Constructors
    public DumbPlayer(string name)
        : base(name)
    {
        this.SeenCards = new HashSet<Card>();
    }
    #endregion

    public override Task<Guess> MakeGuess(
        int turnNumber,
        IEnumerable<Card> cards,
        IEnumerable<Guess> _pastGuesses)
    {
        return Task.Run(() =>
        {
            var missing = cards
                .Where(c => !this.SeenCards.Contains(c))
                .OrderBy(c => c)
                .ToArray();

            var weaponCard = missing.First(c => c.IsWeapon());
            var locationCard = missing.First(c => c.IsLocation());
            var characterCard = missing.First(c => c.IsCharacter());

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

    public override Task ReadMatchedCard(Guess guess, Card card)
    {
        _ = this.SeenCards.Add(card);
        return Task.CompletedTask;
    }

    public override Task<Card> ShowMatchedCard(Guess guess)
    {
        var randomized = this.GuessedCards(guess)
            .OrderBy(c => c)
            .First();

        return Task.FromResult(randomized);
    }

    public override string ToString()
    {
        return $"DumbPlayer {this.Name}";
    }

    protected override void WhenReady()
    {
        foreach (var card in this.Cards)
        {
            _ = this.SeenCards.Add(card);
        }
    }
}
