namespace AdoNetHelpersLibrary.ConnectionHelpers;
public readonly struct CommandDefinition(string commandText, BasicList<DynamicParameters>? parameters = null, IDbTransaction? transaction = null, int? commandTimeout = null,
                             CommandType? commandType = null)
{
    /// <summary>
    /// The command (sql or a stored-procedure name) to execute
    /// </summary>
    public string CommandText { get; } = commandText;

    /// <summary>
    /// The parameters associated with the command
    /// </summary>
    public BasicList<DynamicParameters>? Parameters { get; } = parameters;

    /// <summary>
    /// The active transaction for the command
    /// </summary>
    public IDbTransaction? Transaction { get; } = transaction;

    /// <summary>
    /// The effective timeout for the command
    /// </summary>
    public int? CommandTimeout { get; } = commandTimeout;

    public CommandType CommandType => _commandTypeDirect;
    internal readonly CommandType _commandTypeDirect = commandType ?? CommandType.Text;
    //since i have no way of doing regex stuff, will assume if not given, then its text.  if you really want stored procedure, then you have to specify manually
}