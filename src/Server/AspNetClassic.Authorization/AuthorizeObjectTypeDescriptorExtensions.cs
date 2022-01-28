using System;
using HotChocolate.AspNetClassic.Authorization;

namespace HotChocolate.Types
{
    public static class AuthorizeObjectTypeDescriptorExtensions
    {
        public static IObjectTypeDescriptor Authorize(
            this IObjectTypeDescriptor self,
            params string[] roles)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            return self.Directive(new AuthorizeDirective(roles));
        }

        public static IObjectTypeDescriptor<T> Authorize<T>(
            this IObjectTypeDescriptor<T> self,
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
