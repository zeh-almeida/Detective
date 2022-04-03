using Detective.Cards;
using Detective.Core.Builders;
using Detective.Core.Gameplay;
using Detective.Players;

namespace Detective.ConsoleUI;

public sealed record Program
{
    #region Constants
    const int numberPlayers = 4;
    #endregion

    public static async Task Main(string[] _args)
    {
        //var eventHandler = new DebugConsoleEventHandler();
        var eventHandler = new ConsoleEventHandler();
        var gameState = new GameState();

        var deckBuilder = new DeckBuilder(new ICardBuilder[] {
            new CharacterCardBuilder(),
            new LocationCardBuilder(),
            new WeaponCardBuilder()
        });

        var playerBuilder = new PlayerBuilder();

        gameState.Setup(numberPlayers, deckBuilder, playerBuilder);
        bool hasNextTurn;

        await ResetConsole();

        do
        {
            hasNextTurn = await gameState
                .ExecuteTurn(eventHandler)
                .ConfigureAwait(false);
        } while (hasNextTurn);
    }

    private static async Task ResetConsole()
    {
        await Console.Out.FlushAsync();

        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Black;

        await Console.Out.FlushAsync();
    }
}