using Detective.Core.Players;

namespace Detective.Core.Gameplay;

public interface IEventHandler

{
    #region Game events
    void OnGameStart();

    void OnGameEnd();
    #endregion

    #region Player events
    void OnPlayerSelect(IPlayer player);
    #endregion

    #region Turn events
    void OnNewTurn(int turnNumber);

    void OnTurnEnd(int turnNumber);
    #endregion

    #region Guess events
    void OnGuessMade(Guess guess);

    void OnGuessMatched(Guess guess);

    void OnGuessIsSolution(Guess guess);
    #endregion
}
