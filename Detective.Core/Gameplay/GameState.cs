using Detective.Core.Builders;
using Detective.Core.Cards;
using Detective.Core.Players;

namespace Detective.Core.Gameplay;

public sealed record GameState
{
    #region Properties
    public int Turns { get; private set; }

    private IList<Guess> Guesses { get; set; }

    private IEnumerable<Card> Cards { get; set; }

    private IEnumerable<IPlayer> Players { get; set; }

    private CrimeSolution? Solution { get; set; }

    private int PlayerTurnIndex { get; set; }

    private IPlayer? CurrentPlayer { get; set; }

    private Guess? CurrentGuess { get; set; }

    private bool GuessHasMatch { get; set; }

    private bool HasNextTurn { get; set; }

    private IEventHandler? EventHandler { get; set; }
    #endregion

    #region Constructors
    public GameState()
    {
        this.Solution = null;
        this.EventHandler = null;

        this.Guesses = new List<Guess>();
        this.Cards = Array.Empty<Card>();
        this.Players = Array.Empty<IPlayer>();
    }
    #endregion

    public bool IsReady()
    {
        return this.Solution is not null
            && this.Cards.Any()
            && this.Players.Any()
            && this.Players.All(p => p.IsReady());
    }

    public void Setup(
        int playerCount,
        IDeckBuilder deckBuilder,
        IPlayerBuilder playerBuilder)
    {
        if (this.IsReady())
        {
            throw new Exception("Game already setup");
        }

        deckBuilder.Build();

        this.Cards = deckBuilder.Cards;
        this.Solution = deckBuilder.Solution;

        this.Players = playerBuilder.Build(playerCount, deckBuilder.PlayerCards);

        this.HasNextTurn = true;
    }

    public bool ExecuteTurn(IEventHandler eventHandler)
    {
        if (eventHandler is null)
        {
            throw new ArgumentNullException(nameof(eventHandler), "Event Handler not found");
        }

        if (!this.IsReady())
        {
            throw new Exception("Game not ready yet");
        }

        this.EventHandler = eventHandler;

        if (0.Equals(this.Turns))
        {
            this.EventHandler.OnGameStart();
        }

        this.EventHandler.OnNewTurn(this.Turns);

        _ = this.MovetoNextPlayer()
                .MakeAGuess()
                .ValidateGuess()
                .ValidateSolution()
                .ValidateNextTurn();

        return this.HasNextTurn;
    }

    private GameState MovetoNextPlayer()
    {
        this.CurrentPlayer = this.Players.ElementAt(this.PlayerTurnIndex);
        this.EventHandler?.OnPlayerSelect(this.CurrentPlayer);

        return this;
    }

    private GameState MakeAGuess()
    {
        var guess = this.CurrentPlayer?.MakeGuess(
            this.Cards.ToArray(),
            this.Guesses.ToArray());

        guess.SetTurn(this.Turns);

        this.Guesses.Add(guess);
        this.CurrentGuess = guess;

        this.EventHandler?.OnGuessMade(this.CurrentGuess);
        return this;
    }

    private GameState ValidateGuess()
    {
        var playerCount = this.Players.Count();
        this.GuessHasMatch = false;

        for (var index = 0; index < playerCount; index++)
        {
            var otherPlayerIndex = this.PlayerTurnIndex + index + 1;

            if (otherPlayerIndex >= playerCount)
            {
                otherPlayerIndex -= playerCount;
            }

            var otherPlayer = this.Players.ElementAt(otherPlayerIndex);

            this.GuessHasMatch = otherPlayer.MatchesGuess(this.CurrentGuess);

            if (this.GuessHasMatch)
            {
                this.CurrentGuess.SetResponder(otherPlayer);

                if (!otherPlayer.Equals(this.CurrentPlayer))
                {
                    var card = otherPlayer.ShowMatchedCard(this.CurrentGuess);
                    this.CurrentPlayer?.ReadMatchedCard(this.CurrentGuess, card);
                }

                this.EventHandler?.OnGuessMatched(this.CurrentGuess);
                break;
            }
        }

        return this;
    }

    private GameState ValidateSolution()
    {
        if (!this.GuessHasMatch)
        {
            var isSolution = this.Solution.MatchesGuess(this.CurrentGuess);

            if (!isSolution)
            {
                throw new Exception("Guess matches no cards nor solution?");
            }

            this.EventHandler?.OnGuessIsSolution(this.CurrentGuess);
            this.HasNextTurn = false;
        }

        return this;
    }

    private GameState ValidateNextTurn()
    {
        if (!this.HasNextTurn)
        {
            this.EventHandler?.OnGameEnd();
        }
        else
        {
            this.PlayerTurnIndex++;

            if (this.PlayerTurnIndex >= this.Players.Count())
            {
                this.PlayerTurnIndex = 0;
            }

            this.EventHandler?.OnTurnEnd(this.Turns);
            this.Turns++;
        }

        return this;
    }
}
