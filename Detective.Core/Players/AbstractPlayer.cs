using Detective.Core.Cards;
using Detective.Core.Gameplay;

namespace Detective.Core.Players;

public abstract class AbstractPlayer : IPlayer
{
    #region Variables
    private readonly List<Card> _cards;
    #endregion

    #region Properties
    public string? Character { get; private set; }

    public IEnumerable<Card> Cards => this._cards.ToArray();

    private bool Ready { get; set; }

    public string Name { get; }
    #endregion

    #region Constructors
    protected AbstractPlayer(string name)
    {
        this.Name = name;
        this._cards = new List<Card>();
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

        this.Ready = this.Cards.Any()
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

    #region Guesses
    public Task<bool> MatchesGuess(Guess guess)
    {
        return Task.FromResult(this.GuessedCards(guess).Any());
    }

    protected IEnumerable<Card> GuessedCards(Guess guess)
    {
        return this.Cards
            .Where(c => guess.WeaponCard.Equals(c)
                                  || guess.LocationCard.Equals(c)
                                  || guess.CharacterCard.Equals(c)).ToArray();
    }
    #endregion

    public override string ToString()
    {
        return $"Player {this.Name}";
    }

    #region Abstracts
    public abstract Task<Card> ShowMatchedCard(Guess guess);

    public abstract Task<Guess> MakeGuess(
        IEnumerable<Card> cards,
        IEnumerable<Guess> pastGuesses);

    public abstract Task ReadMatchedCard(Guess guess, Card card);

    protected abstract void WhenReady();
    #endregion
}
