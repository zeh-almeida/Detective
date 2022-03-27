using Detective.Core.Cards;
using Detective.Core.Players;

namespace Detective.Core.Gameplay;

public interface IEventHandler
{
    #region Game events
    Task OnGameStart();

    Task OnGameEnd(int turnNumber);
    #endregion

    #region Player events
    Task OnPlayerSelect(IPlayer player);
    #endregion

    #region Turn events
    Task OnNewTurn(int turnNumber);

    Task OnTurnEnd(int turnNumber);
    #endregion

    #region Guess events
    Task OnGuessMade(Guess guess);

    Task OnGuessMatched(Guess guess, Card? shownCard);

    Task OnGuessIsSolution(Guess guess);
    #endregion
}
