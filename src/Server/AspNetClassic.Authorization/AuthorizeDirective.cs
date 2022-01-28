using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using HotChocolate.Language;

namespace HotChocolate.AspNetClassic.Authorization
{
    public class AuthorizeDirective
        : ISerializable
    {
        public AuthorizeDirective()
        {
            Roles = Array.Empty<string>();
        }

        public AuthorizeDirective(IEnumerable<string> roles)
        {
            if (roles == null)
            {
                throw new ArgumentNullException(nameof(roles));
            }

            Roles = roles.ToArray();
        }

        public AuthorizeDirective(
            SerializationInfo info,
            StreamingContext context)
        {
            var node = info.GetValue(
                nameof(DirectiveNode),
                typeof(DirectiveNode))
                as DirectiveNode;

            if (node == null)
            {
                Roles = (string[])info.GetValue(
                    nameof(Roles),
                    typeof(string[]));
            }
            else
            {
                ArgumentNode rolesArgument = node.Arguments
                    .FirstOrDefault(t => t.Name.Value == "roles");

                Roles = Array.Empty<string>();
                if (rolesArgument != null)
                {
                    if (rolesArgument.Value is ListValueNode lv)
                    {
                        Roles = lv.Items.OfType<StringValueNode>()
                            .Select(t => t.Value?.Trim())
                            .Where(s => !string.IsNullOrEmpty(s))
                            .ToArray();
                    }
                    else if (rolesArgument.Value is StringValueNode svn)
                    {
                        Roles = new[] { svn.Value };
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets of roles that are allowed to access the resource.
        /// </summary>
        public IReadOnlyList<string> Roles { get; }


        public void GetObjectData(
            SerializationInfo info,
            StreamingContext context)
        {
            info.AddValue(nameof(Roles), Roles?.ToArray());
        }
    }
}
