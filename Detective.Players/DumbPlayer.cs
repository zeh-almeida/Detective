using Detective.Core.Cards;
using Detective.Core.Gameplay;
using Detective.Core.Players;
using System.Security.Cryptography;

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
        IEnumerable<Card> cards,
        IEnumerable<Guess> _pastGuesses)
    {
        return Task.Run(() =>
        {
            var missing = cards.Where(c => !this.SeenCards.Contains(c));

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
        return Task.FromResult(this.GuessedCards(guess).First());
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

    private static Card RandomlySelectCard(IEnumerable<Card> cards)
    {
        var missings = cards.ToArray();
        return missings.OrderBy(_ => RandomNumberGenerator.GetInt32(missings.Length)).First();
    }
}
