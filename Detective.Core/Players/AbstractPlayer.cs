using Detective.Core.Cards;
using Detective.Core.Gameplay;
using System.Security.Cryptography;

namespace Detective.Core.Players;

public abstract class AbstractPlayer : IPlayer
{
    #region Variables
    private readonly List<Card> _cards;
    #endregion

    #region Properties
    public string? Character { get; private set; }

    public IEnumerable<Card> Cards => this._cards.ToArray();

    public string Name { get; }

    protected IDictionary<Card, IPlayer> SeenCards { get; }

    private bool Ready { get; set; }
    #endregion

    #region Constructors
    protected AbstractPlayer(string name)
    {
        this.Name = name;

        this._cards = new List<Card>();
        this.SeenCards = new Dictionary<Card, IPlayer>();
    }
    #endregion

    #region Equality
    public override bool Equals(object? obj)
    {
        return this.Equals(obj as IPlayer);
    }

    public bool Equals(IPlayer? other)
    {
        return other is not null
            && Equals(this.Character, other.Character);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Character);
    }

    int IComparable<IPlayer?>.CompareTo(IPlayer? other)
    {
        if (other is null || other.Character is null)
        {
            return int.MaxValue;
        }

        return this.Character is null
             ? int.MinValue
             : this.Character.CompareTo(other.Character);
    }
    #endregion

    public override string ToString()
    {
        return $"{this.GetType().Name} {this.Name}";
    }

    public void SetCharacter(string character)
    {
        if (this.Ready)
        {
            throw new Exception("Player already initialized");
        }

        if (this.Character is not null)
        {
            throw new Exception("character already set");
        }

        this.Character = character;
    }

    public void GiveCard(Card card)
    {
        if (this.Ready)
        {
            throw new Exception("Player already initialized");
        }

        this._cards.Add(card);
    }

    public void SetReady()
    {
        if (this.Ready)
        {
            throw new Exception("Already ready!");
        }

        this.Ready = this._cards.Count > 0
                  && this.Character is not null;

        if (this.Ready)
        {
            this.WhenReady();
        }
    }

    public bool IsReady()
    {
        return this.Ready;
    }

    public Task<bool> MatchesGuess(Guess guess)
    {
        return Task.FromResult(this.GuessedCards(guess).Any());
    }

    public virtual Task ReadMatchedCard(Guess guess, Card card)
    {
        if (guess.Responder is null)
        {
            throw new ArgumentException("No responder set", nameof(guess));
        }

        this.SeenCards.Add(card, guess.Responder);
        return Task.CompletedTask;
    }

    #region Abstracts
    public abstract Task<Card?> ShowMatchedCard(Guess guess);

    public abstract Task<Guess> MakeGuess(
        int turnNumber,
        IPlayer nextPlayer,
        IEnumerable<Card> cards,
        IEnumerable<Guess> pastGuesses);
    #endregion

    protected IEnumerable<Card> GuessedCards(Guess guess)
    {
        return this._cards
            .Where(c => guess.WeaponCard.Equals(c)
                                  || guess.LocationCard.Equals(c)
                                  || guess.CharacterCard.Equals(c)).ToArray();
    }

    protected virtual void WhenReady()
    {
        foreach (var card in this._cards)
        {
            this.SeenCards.Add(card, this);
        }
    }

    protected static Card RandomlySelectCard(IEnumerable<Card> cards)
    {
        var missings = cards.ToArray();

        return missings
            .OrderBy(_ => RandomNumberGenerator.GetInt32(missings.Length))
            .First();
    }
}
