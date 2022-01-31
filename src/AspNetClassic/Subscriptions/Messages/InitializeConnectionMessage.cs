namespace HotChocolate.AspNetClassic.Subscriptions.Messages;

public sealed class InitializeConnectionMessage
    : OperationMessage<IReadOnlyDictionary<string, object?>?>
{
    public InitializeConnectionMessage(IReadOnlyDictionary<string, object?>? payload = null)
        : base(MessageTypes.Connection.Initialize, payload)
    {
    }
}
