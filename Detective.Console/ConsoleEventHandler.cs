using Detective.Core.Gameplay;
using Detective.Core.Players;

namespace Detective.Console;

public sealed record ConsoleEventHandler : IEventHandler
{
    #region Properties
    private TextWriter Writer { get; }
    #endregion

    #region Constructors
    public ConsoleEventHandler(TextWriter writer)
    {
        this.Writer = writer;
    }
    #endregion

    public void OnPlayerSelect(IPlayer _)
    {
        // nothing
    }

    public void OnGameStart()
    {
        this.Writer.WriteLine("Detective Game!");
    }

    public void OnGameEnd()
    {
        this.Writer.WriteLine("\n\nGame over");
    }

    public void OnNewTurn(int turnNumber)
    {
        this.Writer.WriteLine("---");
        this.Writer.WriteLine($"Turn {turnNumber + 1}:");
    }

    public void OnTurnEnd(int _)
    {
        this.Writer.WriteLine();
    }

    public void OnGuessMade(Guess guess)
    {
        this.Writer.WriteLine($"\tPlayer '{guess.Guesser.Name}' made a guess: {guess}");
    }

    public void OnGuessMatched(Guess guess)
    {
        if (!guess.Guesser.Equals(guess.Responder))
        {
            this.Writer.WriteLine($"\tPlayer '{guess.Guesser.Name}' guess answered by Player '{guess.Responder?.Name}'");
        }
        else
        {
            this.Writer.WriteLine($"\tPlayer '{guess.Guesser.Name}' guess answered by itself");
        }
    }

    public void OnGuessIsSolution(Guess guess)
    {
        this.Writer.WriteLine("---");
        this.Writer.WriteLine("---");
        this.Writer.WriteLine("Case solved:");

        this.Writer.WriteLine($"Player '{guess.Guesser.Name}' cracked the case: {guess}");
    }
}
