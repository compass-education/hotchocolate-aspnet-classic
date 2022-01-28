using System;
using HotChocolate.AspNetClassic.Authorization;

namespace HotChocolate.Types
{
    public static class AuthorizeObjectFieldDescriptorExtensions
    {
        public static IObjectFieldDescriptor Authorize(
            this IObjectFieldDescriptor self,
            params string[] roles)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            return self.Directive(new AuthorizeDirective(roles));
        }
    }
}
