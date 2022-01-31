namespace HotChocolate.AspNetClassic.Serialization;

internal interface IVariablePathSegment
{
    IVariablePathSegment? Next { get; }
}
