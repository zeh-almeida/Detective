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

    public async Task<bool> ExecuteTurn(IEventHandler eventHandler)
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
            await this.EventHandler.OnGameStart();
        }

        await this.EventHandler.OnNewTurn(this.Turns);

        _ = await this.MoveToNextPlayer();
        _ = await this.MakeAGuess();
        _ = await this.ValidateGuess();
        _ = await this.ValidateSolution();
        _ = await this.ValidateNextTurn();

        return this.HasNextTurn;
    }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.
    private async Task<GameState> MoveToNextPlayer()
    {
        this.CurrentPlayer = this.Players.ElementAt(this.PlayerTurnIndex);
        await this.EventHandler?.OnPlayerSelect(this.CurrentPlayer);

        return this;
    }

    private async Task<GameState> MakeAGuess()
    {
        var nextPlayer = this.Players.ElementAt(this.NextPlayerIndex());

        var guess = await this.CurrentPlayer?.MakeGuess(
            this.Turns,
            nextPlayer,
            this.Cards.ToArray(),
            this.Guesses.ToArray());

        if (!this.Turns.Equals(guess.Turn))
        {
            throw new Exception($"Player '{this.CurrentPlayer}' tried to change turns on guess");
        }

        this.Guesses.Add(guess);
        this.CurrentGuess = guess;

        await this.EventHandler?.OnGuessMade(this.CurrentGuess);
        return this;
    }

    private async Task<GameState> ValidateGuess()
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

            this.GuessHasMatch = await otherPlayer.MatchesGuess(this.CurrentGuess);

            if (this.GuessHasMatch)
            {
                this.CurrentGuess.SetResponder(otherPlayer);
                Card? shownCard = null;

                if (!otherPlayer.Equals(this.CurrentPlayer))
                {
                    shownCard = await otherPlayer.ShowMatchedCard(this.CurrentGuess);
                    await this.CurrentPlayer?.ReadMatchedCard(this.CurrentGuess, shownCard);
                }

                await this.EventHandler?.OnGuessMatched(this.CurrentGuess, shownCard);
                break;
            }
        }

        return this;
    }

    private async Task<GameState> ValidateSolution()
    {
        if (!this.GuessHasMatch)
        {
            var isSolution = this.Solution.MatchesGuess(this.CurrentGuess);

            if (!isSolution)
            {
                throw new Exception("Guess matches no cards nor solution?");
            }

            await this.EventHandler?.OnGuessIsSolution(this.CurrentGuess);
            this.HasNextTurn = false;
        }

        return this;
    }

    private async Task<GameState> ValidateNextTurn()
    {
        if (!this.HasNextTurn)
        {
            await this.EventHandler?.OnGameEnd(this.Turns);
        }
        else
        {
            this.PlayerTurnIndex++;

            if (this.PlayerTurnIndex >= this.Players.Count())
            {
                this.PlayerTurnIndex = 0;
            }

            await this.EventHandler?.OnTurnEnd(this.Turns);
            this.Turns++;
        }

        return this;
    }

    private int NextPlayerIndex()
    {
        var nextIndex = this.PlayerTurnIndex + 1;
        if (nextIndex >= this.Players.Count())
        {
            nextIndex = 0;
        }

        return nextIndex;
    }
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
}
