using Detective.Cards.Data;
using Detective.Core.Builders;
using Detective.Core.Cards;
using System.Text.Json;

namespace Detective.Cards;

public abstract record AbstractCardBuilder : ICardBuilder
{
    #region Properties
    public abstract CardType Type { get; }
    #endregion

    public abstract IEnumerable<Card> Build();

    protected static DataValues LoadData(string dataFile)
    {
        if (DataFiles.ResourceManager.GetObject(dataFile) is not byte[] contents || contents.Length == 0)
        {
            throw new Exception("No data found");
        }

        var data = JsonSerializer.Deserialize<DataValues>(contents, new JsonSerializerOptions(JsonSerializerDefaults.Web));

        if (data is null || !data.Values.Any())
        {
            throw new Exception("Could not parse");
        }

        return data;
    }
}
