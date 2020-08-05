using HotChocolate.Types;

namespace HospiTec_Server.Logic.Graphql.Types
{
    public class AuthType : ObjectType<Auth>
    {
        protected override void Configure(IObjectTypeDescriptor<Auth> descriptor)
        {
            base.Configure(descriptor);

            descriptor.Field(e =>e.accessKey)
                .Type<NonNullType<StringType>>();

            descriptor.Field(e => e.role)
                .Type<NonNullType<StringType>>();
        }
    }
}
