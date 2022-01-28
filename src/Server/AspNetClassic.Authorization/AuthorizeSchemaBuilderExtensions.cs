using System;
using HotChocolate.AspNetClassic.Authorization;

namespace HotChocolate
{
    public static class AuthorizeSchemaBuilderExtensions
    {
        public static ISchemaBuilder AddAuthorizeDirectiveType(
            this ISchemaBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.AddDirectiveType<AuthorizeDirectiveType>();
        }
    }
}
