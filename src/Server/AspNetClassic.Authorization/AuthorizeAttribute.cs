using System;
using System.Reflection;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;

namespace HotChocolate.AspNetClassic.Authorization
{
    [AttributeUsage(
        AttributeTargets.Class
        | AttributeTargets.Struct
        | AttributeTargets.Property
        | AttributeTargets.Method,
        Inherited = true,
        AllowMultiple = true)]
    public class AuthorizeAttribute : DescriptorAttribute
    {
        public string Policy { get; set; }

        public string[] Roles { get; set; }

        protected override void TryConfigure(
            IDescriptorContext context,
            IDescriptor descriptor,
            ICustomAttributeProvider element)
        {
            if (descriptor is IObjectTypeDescriptor type)
            {
                type.Directive(CreateDirective());
            }
            else if (descriptor is IObjectFieldDescriptor field)
            {
                field.Directive(CreateDirective());
            }
        }

        private AuthorizeDirective CreateDirective()
        {
            if (Roles is { })
            {
                return new AuthorizeDirective(Roles);
            }
            else
            {
                return new AuthorizeDirective();
            }
        }
    }
}
