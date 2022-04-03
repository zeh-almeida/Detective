using Detective.Core.Cards;
using Detective.Core.Gameplay;
using Detective.Core.Players;

namespace Detective.Players;

public sealed class ConsolePlayer : AbstractPlayer
{
    #region Constants
    private const string PlayerStart = "\n< Player Start >";

    private const string PlayerEnd = "< Player End >\n";
    #endregion

    #region Constructors
    public ConsolePlayer(string name)
        : base(name)
    {
    }
    #endregion

    public override Task<Guess> MakeGuess(
        int turnNumber,
        IPlayer nextPlayer,
        IEnumerable<Card> cards,
        IEnumerable<Guess> pastGuesses)
    {
        return Task.Run(() =>
        {
            Console.WriteLine(PlayerStart);
            Console.WriteLine($"'{this}' should make a guess:");

            var guess = this.MakeAction(turnNumber, cards);
            Console.WriteLine(PlayerEnd);

            return guess ?? throw new Exception("Guess wasn't made?");
        });
    }

    public override Task<Card?> ShowMatchedCard(Guess guess)
    {
        return Task.Run(() =>
        {
            Console.WriteLine(PlayerStart);
            Console.WriteLine($"'{this}' must show a card:");

            Card? selectedCard = null;

            var cards = this.GuessedCards(guess)
                .OrderBy(c => c)
                .ToArray();

            if (cards.Length == 1)
            {
                selectedCard = cards.FirstOrDefault();
                Console.WriteLine($"\t\tSelected matched card: {selectedCard}");

                Console.Write("\tPress 'enter' to continue");
                _ = Console.Read();
            }
            else
            {
                var isSelected = false;
                while (!isSelected)
                {
                    Console.WriteLine("\n\tSelect matched card:");

                    for (var index = 0; index < cards.Length; index++)
                    {
                        Console.WriteLine($"\t\t{index + 1}- {cards[index]}");
                    }

                    var optionNumber = ValidateOption(1, cards.Length);

                    if (optionNumber == -1)
                    {
                        continue;
                    }

                    selectedCard = cards[optionNumber - 1];
                    Console.WriteLine($"\t\tSelected matched card: {selectedCard}");

                    isSelected = true;
                }
            }

            Console.WriteLine(PlayerEnd);
            return selectedCard;
        });
    }

    public override async Task ReadMatchedCard(Guess guess, Card card)
    {
        await base.ReadMatchedCard(guess, card);

        await Task.Run(() =>
        {
            Console.WriteLine(PlayerStart);
            Console.WriteLine($"'{guess.Responder}' matched with '{this}' guess:");
            Console.WriteLine($"\t'{card}' was shown\n");

            Console.Write("\tPress 'enter' to continue");
            _ = Console.Read();

            Console.WriteLine(PlayerEnd);
        });
    }

    private void PrintHandCards()
    {
        Console.WriteLine("\n\tYour cards:");
        var cards = this.Cards.OrderBy(c => c).ToArray();

        for (var index = 0; index < cards.Length; index++)
        {
            var card = cards[index];
            Console.WriteLine($"\t\t{index + 1}- {card}");
        }
    }

    private void PrintSeenCards()
    {
        Console.WriteLine("\n\tSeen cards:");
        var otherCards = this.SeenCards
            .Where(c => !this.Cards.Contains(c.Key))
            .OrderBy(p => p.Key)
            .ToArray();

        for (var index = 0; index < otherCards.Length; index++)
        {
            var pair = otherCards[index];
            Console.WriteLine($"\t\t{index + 1}- {pair.Key}\t'{pair.Value}'");
        }
    }

    private void PrintMissingCards(IEnumerable<Card> cards)
    {
        Console.WriteLine("\n\tMissing cards:");
        var otherCards = cards
            .Where(c => !this.SeenCards.ContainsKey(c))
            .OrderBy(c => c)
            .ToArray();

        for (var index = 0; index < otherCards.Length; index++)
        {
            var card = otherCards[index];
            Console.WriteLine($"\t\t{index + 1}- {card}");
        }
    }

    private Guess? MakeAction(int turnNumber, IEnumerable<Card> cards)
    {
        var hasOption = false;
        Guess? guess = null;

        while (!hasOption)
        {
            Console.WriteLine("\n\tWhat will you do?");
            Console.WriteLine("\t\tList cards at hand: 1");
            Console.WriteLine("\t\tList seen cards: 2");
            Console.WriteLine("\t\tList unknown cards: 3");
            Console.WriteLine("\t\tMake a guess: 4");

            var optionNumber = ValidateOption(1, 5);

            if (optionNumber == -1)
            {
                continue;
            }

            switch (optionNumber)
            {
                case 1:
                    this.PrintHandCards();
                    break;

                case 2:
                    this.PrintSeenCards();
                    break;

                case 3:
                    this.PrintMissingCards(cards);
                    break;

                case 4:
                    guess = this.SelectCardsGuess(turnNumber, cards);
                    hasOption = true;
                    break;

                default:
                    Console.WriteLine("\t\t -- Bad option, try again!");
                    break;
            }
        }

        return guess;
    }

    private Guess SelectCardsGuess(int turnNumber, IEnumerable<Card> cards)
    {
        var weaponCard = this.SelectGuessCard("weapon", cards.Where(c => c.IsWeapon()));
        var locationCard = this.SelectGuessCard("location", cards.Where(c => c.IsLocation()));
        var characterCard = this.SelectGuessCard("character", cards.Where(c => c.IsCharacter()));

        if (weaponCard is null)
        {
            throw new Exception("Weapon card is not a Weapon?");
        }

        if (locationCard is null)
        {
            throw new Exception("Location card is not a Location?");
        }

        if (characterCard is null)
        {
            throw new Exception("Character card is not a Character?");
        }

        return new Guess(
            turnNumber,
            this,
            characterCard,
            locationCard,
            weaponCard);
    }

    private Card? SelectGuessCard(string cardType, IEnumerable<Card> cards)
    {
        var isSelected = false;
        Card? selectedCard = null;

        while (!isSelected)
        {
            Console.WriteLine($"\n\tSelect a {cardType}:");
            Console.WriteLine("\t\tO - means you own this card");
            Console.WriteLine("\t\tS - means you have seen this card\n");

            var toSelect = cards
                .OrderBy(c => c)
                .ToArray();

            for (var index = 0; index < toSelect.Length; index++)
            {
                var card = toSelect[index];
                var owns = this.Cards.Contains(card) ? "O" : string.Empty;
                var seen = this.SeenCards.ContainsKey(card) ? "S" : string.Empty;

                Console.WriteLine($"\t\t{index + 1}- {card} {owns}{seen}");
            }

            var optionNumber = ValidateOption(1, toSelect.Length);

            if (optionNumber == -1)
            {
                continue;
            }

            selectedCard = toSelect[optionNumber - 1];
            Console.WriteLine($"\t\tSelected card: {selectedCard}");

            isSelected = true;
        }

        return selectedCard;
    }

    private static int ValidateOption(int minValue, int maxValue)
    {
        Console.Write("\n\t\tSelect your option: ");
        var option = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(option))
        {
            Console.WriteLine("\t\t-- Bad option, try again!");
            return -1;
        }

        var isParsed = int.TryParse(option, out var optionNumber);

        if (!isParsed || optionNumber < minValue || optionNumber > maxValue)
        {
            Console.WriteLine("\t\t-- Bad option, try again!");
            return -1;
        }

        return optionNumber;
    }
}
