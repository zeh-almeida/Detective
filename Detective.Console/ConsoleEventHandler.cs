using Detective.Core.Cards;
using Detective.Core.Gameplay;
using Detective.Core.Players;

namespace Detective.ConsoleUI;

public sealed record ConsoleEventHandler : IEventHandler
{
    #region Constants
    private const string Separator = "===============================================================================";
    #endregion

    public async Task OnPlayerSelect(IPlayer _)
    {
        // nothing
        await ResetColor();
    }

    public async Task OnGameStart()
    {
        await ResetColor();
        Console.WriteLine("Detective Game!");
    }

    public async Task OnGameEnd(int _)
    {
        await ResetColor();
        Console.WriteLine("\n\nGame over");
    }

    public async Task OnNewTurn(int turnNumber)
    {
        await ResetColor();
        Console.WriteLine($"Turn {turnNumber + 1}:");
    }

    public async Task OnTurnEnd(int _)
    {
        await ResetColor();
        Console.WriteLine(Separator);
    }

    public async Task OnGuessMade(Guess guess)
    {
        await ResetColor();
        Console.WriteLine($"\tPlayer '{guess.Guesser.Name}' made a guess: {guess}");
    }

    public async Task OnGuessMatched(Guess guess, Card? _shownCard)
    {
        await ResetColor();

        if (!guess.Guesser.Equals(guess.Responder))
        {
            Console.WriteLine($"\tPlayer '{guess.Responder?.Name}' answered guess by Player '{guess.Guesser.Name}'");
        }
        else
        {
            Console.WriteLine($"\tPlayer '{guess.Guesser.Name}' guess answered by itself");
        }
    }

    public async Task OnGuessIsSolution(Guess guess)
    {
        await ResetColor();

        Console.WriteLine(Separator);
        Console.WriteLine(Separator);
        Console.WriteLine($"Case solved: at turn {guess.Turn + 1}");

        Console.WriteLine($"Player '{guess.Guesser.Name}' cracked the case: {guess}");
    }

    private static async Task ResetColor()
    {
        await Console.Out.FlushAsync();

        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Black;

        await Console.Out.FlushAsync();
    }
}
