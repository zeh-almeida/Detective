using Detective.Core.Builders;
using Detective.Core.Cards;
using Detective.Core.Players;
using System.Security.Cryptography;

namespace Detective.Players;

public sealed record PlayerBuilder : IPlayerBuilder
{
    public IEnumerable<IPlayer> Build(int playerCount, IEnumerable<Card> cards)
    {
        var players = InstantiatePlayers(playerCount, cards);

        SetPlayerCharacter(players, cards);
        DistributeCards(players, cards);

        foreach (var player in players)
        {
            player.SetReady();
        }

        return players.ToArray();
    }

    private static IEnumerable<IPlayer> InstantiatePlayers(int playerCount, IEnumerable<Card> cards)
    {
        if (playerCount > cards.Count(c => c.IsCharacter()))
        {
            throw new Exception("More players than characters");
        }

        var players = new List<IPlayer>(playerCount);

        for (var index = 0; index < playerCount; index++)
        {
            var player = new DumbPlayer($"P{index}");
            players.Add(player);
        }

        return players;
    }

    private static void SetPlayerCharacter(IEnumerable<IPlayer> players, IEnumerable<Card> cards)
    {
        var playerCount = players.Count();
        var charCount = cards.Count(c => c.IsCharacter());

        var usedCharacters = cards
            .Where(c => c.IsCharacter())
            .OrderBy(_ => RandomNumberGenerator.GetInt32(charCount))
            .Take(playerCount)
            .ToArray();

        if (charCount <= 0)
        {
            throw new ArgumentException("No characters cards", nameof(cards));
        }

        if (playerCount > charCount)
        {
            throw new ArgumentException("More players than characters", nameof(players));
        }

        for (var index = 0; index < usedCharacters.Length; index++)
        {
            var character = usedCharacters[index].Value;
            var player = players.ElementAt(index);

            player.SetCharacter(character);
        }
    }

    private static void DistributeCards(IEnumerable<IPlayer> players, IEnumerable<Card> cards)
    {
        var cardCount = cards.Count();
        var playerCount = players.Count();

        if (cardCount <= 0)
        {
            throw new ArgumentException("No cards", nameof(cards));
        }

        if (playerCount > cardCount)
        {
            throw new ArgumentException("Not enough cards", nameof(cards));
        }

        var usedCards = new List<Card>(cards)
            .OrderBy(_ => RandomNumberGenerator.GetInt32(cardCount))
            .ToArray();

        var playerIndex = 0;

        foreach (var card in usedCards)
        {
            if (playerIndex >= playerCount)
            {
                playerIndex = 0;
            }

            var player = players.ElementAt(playerIndex);
            player.GiveCard(card);

            playerIndex++;
        }
    }
}
