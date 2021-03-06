using System.Runtime.Serialization;

namespace HotChocolate.AspNetClassic;

[Serializable]
public class GraphQLRequestException : GraphQLException
{
    public GraphQLRequestException(string message)
        : base(message)
    {
    }

    public GraphQLRequestException(IError error)
        : base(error)
    {
    }

    public GraphQLRequestException(params IError[] errors)
        : base(errors)
    {

    }

    public GraphQLRequestException(IEnumerable<IError> errors)
        : base(errors)
    {
    }

    protected GraphQLRequestException(
        SerializationInfo info,
        StreamingContext context)
        : base(info, context)
    {
    }
}
