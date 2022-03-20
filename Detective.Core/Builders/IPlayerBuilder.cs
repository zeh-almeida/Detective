using Detective.Core.Cards;
using Detective.Core.Players;

namespace Detective.Core.Builders;

public interface IPlayerBuilder
{
    IEnumerable<IPlayer> Build(int playerCount, IEnumerable<Card> cards);
}
