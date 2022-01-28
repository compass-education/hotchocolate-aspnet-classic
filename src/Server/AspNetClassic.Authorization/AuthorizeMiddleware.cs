using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using HotChocolate.AspNetClassic.Authorization.Properties;
using HotChocolate.Resolvers;

namespace HotChocolate.AspNetClassic.Authorization
{
    internal class AuthorizeMiddleware
    {
        private readonly FieldDelegate _next;

        public AuthorizeMiddleware(FieldDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(IDirectiveContext context)
        {
            AuthorizeDirective directive = context.Directive.ToObject<AuthorizeDirective>();

            ClaimsPrincipal principal = null;
            var allowed = false;
            var authenticated = false;

            if (context.ContextData.TryGetValue(
                nameof(ClaimsPrincipal), out var o)
                && o is ClaimsPrincipal p)
            {
                principal = p;

                authenticated = allowed = p.Identities.Any(t => t.IsAuthenticated);
            }

            allowed = allowed && IsInAnyRole(principal, directive.Roles);

            if (allowed)
            {
                await _next(context).ConfigureAwait(false);
            }
            else if (context.Result == null)
            {
                context.Result = ErrorBuilder.New()
                    .SetMessage(
                        AuthResources.AuthorizeMiddleware_NotAuthorized)
                    .SetCode(authenticated
                        ? ErrorCodes.Authentication.NotAuthorized
                        : ErrorCodes.Authentication.NotAuthenticated)
                    .SetPath(context.Path)
                    .AddLocation(context.FieldSelection)
                    .Build();
            }
        }

        private static bool IsInAnyRole(
            IPrincipal principal,
            IReadOnlyList<string> roles)
        {
            if (roles == null || roles.Count == 0)
            {
                return true;
            }

            for (int i = 0; i < roles.Count; i++)
            {
                if (principal.IsInRole(roles[i]))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
