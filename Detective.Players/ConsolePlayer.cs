using Detective.Core.Cards;
using Detective.Core.Gameplay;
using Detective.Core.Players;

namespace Detective.Players;

public sealed class ConsolePlayer : AbstractPlayer
{
    #region Constants
    private const string PlayerStart = "\n< Player Start >";

    private const string PlayerEnd = "< Player End >\n";

    private static string[] Options { get; } = new string[]
    {
        "List cards",
        "Make a guess",
    };
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
            _ = Console.ReadLine();
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
        _ = Console.ReadLine();

        Console.WriteLine(PlayerEnd);
        await ResetColor();
    }

    private Guess? MakeAction(int turnNumber, IEnumerable<Card> cards)
    {
        var hasOption = false;
        Guess? guess = null;

        while (!hasOption)
        {
            Console.WriteLine("\n\tWhat will you do?");

            var maxKeyLength = Options.Max(c => c.Length);
            for (var index = 0; index < Options.Length; index++)
            {
                Console.WriteLine($"\t\t{WriteIndex(index)}- {Options[index]}");
            }

            var optionNumber = ValidateOption(1, Options.Length);

            if (optionNumber == -1)
            {
                continue;
            }

            switch (optionNumber)
            {
                case 1:
                    Console.WriteLine($"\n\t\t--{Options[optionNumber - 1]}");
                    this.PrintAllCards(cards);
                    break;

                case 2:
                    Console.WriteLine($"\n\t\t--{Options[optionNumber - 1]}");

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
            Console.WriteLine($"\n\t\tSelect a {cardType}:");

            var toSelect = cards
                .OrderBy(c => c)
                .ToArray();

            this.PrintCards(toSelect);

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

    private void PrintAllCards(IEnumerable<Card> cards)
    {
        var weapons = cards
            .Where(c => c.IsWeapon());

        var locations = cards
            .Where(c => c.IsLocation());

        var characters = cards
            .Where(c => c.IsCharacter());

        Console.WriteLine("\n\t\tWeapons:");
        this.PrintCards(weapons);

        Console.WriteLine("\n\t\tLocations:");
        this.PrintCards(locations);

        Console.WriteLine("\n\t\tCharacters:");
        this.PrintCards(characters);
    }

    private void PrintCards(IEnumerable<Card> cards)
    {
        Console.WriteLine("\t\t\tO - means you own this card");
        Console.WriteLine("\t\t\tS - means you have seen this card\n");

        var toSelect = cards.OrderBy(c => c).ToArray();
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
                state = $"S -> '{this.SeenCards[card]}'";
            }

            var key = AddSpacesToValue(card.ToString(), maxKeyLength);
            Console.WriteLine($"\t\t\t{WriteIndex(index)}- {key}{state}");
        }
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
