namespace Detective.Cards.Data;
public sealed record DataValues
{
    #region Properties
    public IEnumerable<string> Values { get; set; }
    #endregion

    #region Constructors
    public DataValues()
    {
        this.Values = Array.Empty<string>();
    }
    #endregion
}
