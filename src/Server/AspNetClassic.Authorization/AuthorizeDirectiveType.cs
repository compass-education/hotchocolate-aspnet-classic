using HotChocolate.Types;

namespace HotChocolate.AspNetClassic.Authorization
{
    public class AuthorizeDirectiveType
        : DirectiveType<AuthorizeDirective>
    {
        protected override void Configure(
            IDirectiveTypeDescriptor<AuthorizeDirective> descriptor)
        {
            descriptor.Name("authorize");

            descriptor.Location(DirectiveLocation.Object)
                .Location(DirectiveLocation.FieldDefinition);

            // TODO :resources
            descriptor.Argument(t => t.Roles)
                .Description(
                    "Roles that are allowed to access to the " +
                    "annotated resource.")
                .Type<ListType<NonNullType<StringType>>>();

            descriptor.Repeatable();

            descriptor.Use<AuthorizeMiddleware>();
        }
    }
}
