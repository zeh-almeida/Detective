using Detective.Core.Gameplay;
using Detective.Core.Players;

namespace Detective.ConsoleUI;

public sealed record ConsoleEventHandler : IEventHandler
{
    public Task OnPlayerSelect(IPlayer _)
    {
        // nothing
        return Task.CompletedTask;
    }

    public Task OnGameStart()
    {
        Console.WriteLine("Detective Game!");
        return Task.CompletedTask;
    }

    public Task OnGameEnd()
    {
        Console.WriteLine("\n\nGame over");
        return Task.CompletedTask;
    }

    public Task OnNewTurn(int turnNumber)
    {
        Console.WriteLine($"Turn {turnNumber + 1}:");
        return Task.CompletedTask;
    }

    public Task OnTurnEnd(int _)
    {
        Console.WriteLine("---");
        return Task.CompletedTask;
    }

    public Task OnGuessMade(Guess guess)
    {
        Console.WriteLine($"\tPlayer '{guess.Guesser.Name}' made a guess: {guess}");
        return Task.CompletedTask;
    }

    public Task OnGuessMatched(Guess guess)
    {
        if (!guess.Guesser.Equals(guess.Responder))
        {
            Console.WriteLine($"\tPlayer '{guess.Guesser.Name}' guess answered by Player '{guess.Responder?.Name}'");
        }
        else
        {
            Console.WriteLine($"\tPlayer '{guess.Guesser.Name}' guess answered by itself");
        }

        return Task.CompletedTask;
    }

    public Task OnGuessIsSolution(Guess guess)
    {
        Console.WriteLine("---");
        Console.WriteLine("---");
        Console.WriteLine("Case solved:");

        Console.WriteLine($"Player '{guess.Guesser.Name}' cracked the case: {guess}");

        return Task.CompletedTask;
    }
}
