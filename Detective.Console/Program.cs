using Detective.Cards;
using Detective.Core.Builders;
using Detective.Core.Gameplay;
using Detective.Players;

const int numberPlayers = 3;

var eventHandler = new Detective.Console.ConsoleEventHandler(Console.Out);

var gameState = new GameState();

var deckBuilder = new DeckBuilder(new ICardBuilder[] {
    new CharacterCardBuilder(),
    new LocationCardBuilder(),
    new WeaponCardBuilder()
});

var playerBuilder = new PlayerBuilder();

gameState.Setup(numberPlayers, deckBuilder, playerBuilder);
bool hasNextTurn;

do
{
    hasNextTurn = gameState.ExecuteTurn(eventHandler);
} while (hasNextTurn);