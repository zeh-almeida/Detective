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

    public override async Task<Guess> MakeGuess(
        int turnNumber,
        IPlayer nextPlayer,
        IEnumerable<Card> cards,
        IEnumerable<Guess> pastGuesses)
    {
        await UsePlayerColor();

        Console.WriteLine(PlayerStart);
        Console.WriteLine($"'{this}' should make a guess:");

        var guess = this.MakeAction(turnNumber, cards);

        Console.WriteLine(PlayerEnd);
        await ResetColor();

        return guess ?? throw new Exception("Guess wasn't made?");
    }

    public override async Task<Card?> ShowMatchedCard(Guess guess)
    {
        await UsePlayerColor();

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
                Console.Write($"\t\tSelected matched card: {selectedCard}");

                isSelected = true;
            }
        }

        Console.WriteLine($"\n{PlayerEnd}");
        await ResetColor();

        return selectedCard;
    }

    public override async Task ReadMatchedCard(Guess guess, Card card)
    {
        await base.ReadMatchedCard(guess, card);

        await UsePlayerColor();

        Console.WriteLine(PlayerStart);
        Console.WriteLine($"'{guess.Responder}' matched with '{this}' guess:");
        Console.WriteLine($"\t'{card}' was shown\n");

        Console.Write("\tPress 'enter' to continue");
        _ = Console.Read();

        Console.WriteLine(PlayerEnd);
        await ResetColor();
    }

    private void PrintHandCards()
    {
        Console.WriteLine("\n\tYour cards:");
        var cards = this.Cards.OrderBy(c => c).ToArray();

        for (var index = 0; index < cards.Length; index++)
        {
            var card = cards[index];
            Console.WriteLine($"\t\t{WriteIndex(index)}- {card}");
        }
    }

    private void PrintSeenCards()
    {
        Console.WriteLine("\n\tSeen cards:");
        var otherCards = this.SeenCards
            .Where(c => !this.Cards.Contains(c.Key))
            .OrderBy(p => p.Key)
            .ToArray();

        if (otherCards.Length == 0)
        {
            Console.WriteLine("\t\tHaven't seen a card yet");
            return;
        }

        var maxKeyLength = otherCards.Max(c => c.Key.ToString().Length);

        for (var index = 0; index < otherCards.Length; index++)
        {
            var pair = otherCards[index];

            var key = AddSpacesToValue(pair.Key.ToString(), maxKeyLength);
            Console.WriteLine($"\t\t{WriteIndex(index)}- {key}'{pair.Value}'");
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
            Console.WriteLine($"\t\t{WriteIndex(index)}- {card}");
        }
    }

    private Guess? MakeAction(int turnNumber, IEnumerable<Card> cards)
    {
        var hasOption = false;
        Guess? guess = null;

        while (!hasOption)
        {
            var options = new string[] {
                "List cards at hand",
                "List seen cards",
                "List unknown cards",
                "Make a guess",
            };

            Console.WriteLine("\n\tWhat will you do?");

            var maxKeyLength = options.Max(c => c.Length);
            for (var index = 0; index < options.Length; index++)
            {
                Console.WriteLine($"\t\t{WriteIndex(index)}- {options[index]}");
            }

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

            var maxKeyLength = toSelect.Max(c => c.ToString().Length);

            for (var index = 0; index < toSelect.Length; index++)
            {
                var card = toSelect[index];

                var state = string.Empty;

                if (this.Cards.Contains(card))
                {
                    state = "O";
                }
                else if (this.SeenCards.ContainsKey(card))
                {
                    state = "S";
                }

                var key = AddSpacesToValue(card.ToString(), maxKeyLength);
                Console.WriteLine($"\t\t{WriteIndex(index)}- {key}{state}");
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

    private static string AddSpacesToValue(string value, int numberMaxChars)
    {
        var spaces = new string(' ', numberMaxChars - value.Length + 2);
        return $"{value}{spaces}";
    }

    private static string WriteIndex(int index)
    {
        var value = (index + 1).ToString();
        return value.PadLeft(2, '0');
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

    private static async Task UsePlayerColor()
    {
        await Console.Out.FlushAsync();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.BackgroundColor = ConsoleColor.Black;

        await Console.Out.FlushAsync();
    }

    private static async Task ResetColor()
    {
        await Console.Out.FlushAsync();

        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Black;

        await Console.Out.FlushAsync();
    }
}
