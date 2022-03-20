using Detective.Core.Builders;
using Detective.Core.Cards;
using Detective.Core.Gameplay;
using System.Security.Cryptography;

namespace Detective.Cards;

public sealed record DeckBuilder : IDeckBuilder
{
    #region Properties
    private IEnumerable<ICardBuilder> Builders { get; }

    public IEnumerable<Card> Cards { get; private set; }

    public CrimeSolution? Solution { get; private set; }

    public IEnumerable<Card> PlayerCards { get; private set; }
    #endregion

    #region Constructors
    public DeckBuilder(IEnumerable<ICardBuilder> builders)
    {
        var typeCount = builders
            .Select(b => b.Type)
            .Distinct()
            .Count();

        if (!typeCount.Equals(Enum.GetValues<CardType>().Length))
        {
            throw new ArgumentException("Does not have all necessary card types", nameof(builders));
        }

        this.Solution = null;
        this.Builders = builders;

        this.Cards = Array.Empty<Card>();
        this.PlayerCards = Array.Empty<Card>();
    }
    #endregion

    public void Build()
    {
        this.BuildDeck();
        this.BuildSolution();
        this.SeparatePlayerCards();
    }

    private void BuildDeck()
    {
        var cards = new List<Card>();

        foreach (var builder in this.Builders)
        {
            cards.AddRange(builder.Build());
        }

        this.Cards = cards.ToArray();
    }

    private void BuildSolution()
    {
        var weapons = this.Cards.Where(c => c.IsWeapon()).ToArray();
        var locations = this.Cards.Where(c => c.IsLocation()).ToArray();
        var characters = this.Cards.Where(c => c.IsCharacter()).ToArray();

        var weapon = weapons[RandomNumberGenerator.GetInt32(weapons.Length)];
        var location = locations[RandomNumberGenerator.GetInt32(locations.Length)];
        var character = characters[RandomNumberGenerator.GetInt32(characters.Length)];

        this.Solution = new CrimeSolution(character, location, weapon);
    }

    private void SeparatePlayerCards()
    {
        this.PlayerCards = this.Cards
            .Where(c => !this.Solution.WeaponCard.Equals(c)
                                  && !this.Solution.CharacterCard.Equals(c)
                                  && !this.Solution.LocationCard.Equals(c))
            .ToArray();
    }
}
