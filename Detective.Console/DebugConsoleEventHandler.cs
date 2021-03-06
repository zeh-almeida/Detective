using Detective.Core.Cards;
using Detective.Core.Gameplay;
using Detective.Core.Players;

namespace Detective.ConsoleUI;

public sealed record DebugConsoleEventHandler : IEventHandler
{
    #region Constants
    private const string Separator = "===============================================================================";
    #endregion

    public Task OnPlayerSelect(IPlayer _)
    {
        // nothing
        return ResetColor();
    }

    public Task OnGameStart()
    {
        Console.WriteLine("DEBUG: Detective Game!");
        return ResetColor();
    }

    public Task OnGameEnd(int _)
    {
        Console.WriteLine("\n\nGame over");
        return ResetColor();
    }

    public Task OnNewTurn(int turnNumber)
    {
        Console.WriteLine($"Turn {turnNumber + 1}:");
        return ResetColor();
    }

    public Task OnTurnEnd(int _)
    {
        Console.WriteLine(Separator);
        return ResetColor();
    }

    public Task OnGuessMade(Guess guess)
    {
        Console.WriteLine($"\tPlayer '{guess.Guesser.Name}' made a guess: {guess}");
        return ResetColor();
    }

    public Task OnGuessMatched(Guess guess, Card? shownCard)
    {
        if (guess.Responder is null)
        {
            throw new Exception("Guess matched but not answered?");
        }

        if (!guess.Guesser.Equals(guess.Responder))
        {
            Console.WriteLine($"\tPlayer '{guess.Responder?.Name}' answered guess by Player '{guess.Guesser.Name}'");

            var cards = guess.Responder?.Cards.OrderBy(c => c).ToArray();
            Console.WriteLine("\t\tResponder cards");

            foreach (var card in cards)
            {
                var isMatch = card.Equals(shownCard) ? "* " : "  ";
                Console.WriteLine($"\n\t\t\t{isMatch}{card}");
            }
        }
        else
        {
            Console.WriteLine($"\tPlayer '{guess.Guesser.Name}' guess answered by itself");
        }

        return ResetColor();
    }

    public Task OnGuessIsSolution(Guess guess)
    {
        Console.WriteLine(Separator);
        Console.WriteLine(Separator);
        Console.WriteLine($"Case solved: at turn {guess.Turn + 1}");

        Console.WriteLine($"Player '{guess.Guesser.Name}' cracked the case: {guess}");

        return ResetColor();
    }

    private static Task ResetColor()
    {
        Console.ResetColor();
        return Console.Out.FlushAsync();
    }
}
