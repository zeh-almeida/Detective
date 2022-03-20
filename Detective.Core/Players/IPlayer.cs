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

    Card ShowMatchedCard(Guess guess);

    bool MatchesGuess(Guess guess);

    void ReadMatchedCard(Guess guess, Card card);

    Guess MakeGuess(
        IEnumerable<Card> cards,
        IEnumerable<Guess> pastGuesses);
}
