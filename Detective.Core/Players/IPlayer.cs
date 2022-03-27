using Detective.Core.Cards;
using Detective.Core.Gameplay;

namespace Detective.Core.Players;
public interface IPlayer : IEquatable<IPlayer?>, IComparable<IPlayer?>
{
    #region Properties
    public string? Character { get; }

    public IEnumerable<Card> Cards { get; }

    public string Name { get; }
    #endregion

    void SetCharacter(string character);

    void GiveCard(Card card);

    void SetReady();

    bool IsReady();

    Task<Card> ShowMatchedCard(Guess guess);

    Task<bool> MatchesGuess(Guess guess);

    Task ReadMatchedCard(Guess guess, Card card);

    Task<Guess> MakeGuess(
        int turnNumber,
        IPlayer nextPlayer,
        IEnumerable<Card> cards,
        IEnumerable<Guess> pastGuesses);
}
