namespace HotChocolate.AspNetClassic.Warmup;

internal class WarmupSchema
{
    public WarmupSchema(NameString schemaName)
    {
        SchemaName = schemaName;
    }

    public NameString SchemaName { get; }
}
