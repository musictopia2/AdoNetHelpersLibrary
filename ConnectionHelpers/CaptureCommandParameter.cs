namespace AdoNetHelpersLibrary.ConnectionHelpers;
internal class CaptureCommandParameter : ICaptureCommandParameter, IDisposable
{
    private readonly IConnector _connector;
    public CaptureCommandParameter(IConnector connector)
    {
        _connector = connector;
        CurrentConnection = _connector.GetConnection();
        CurrentConnection.Open();
    }
    public IDbConnection CurrentConnection { get; }
    EnumDatabaseCategory ICaptureCommandParameter.Category => _connector.Category;
    public void Dispose()
    {
        CurrentConnection.Close();
        CurrentConnection.Dispose();
    }
    public IDbCommand GetCommand()
    {
        return _connector.GetConnector.GetCommand();
    }
    public DbParameter GetParameter()
    {
        return _connector.GetConnector.GetParameter();
    }
}