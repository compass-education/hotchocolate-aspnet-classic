using HotChocolate.Language;

namespace HotChocolate.AspNetClassic.Subscriptions.Messages;

public sealed class DataStartMessage : OperationMessage<GraphQLRequest>
{
    public DataStartMessage(string id, GraphQLRequest request)
        : base(MessageTypes.Subscription.Start, id, request)
    {
    }

    public override string Id => base.Id!;
}
