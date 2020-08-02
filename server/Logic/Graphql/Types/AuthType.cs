using HotChocolate.Types;

namespace HospiTec_Server.Logic.Graphql.Types
{
    public class AuthType : ObjectType
    {
        protected override void Configure(IObjectTypeDescriptor descriptor)
        {
            base.Configure(descriptor);

            descriptor.Field("accessKey")
                .Type<NonNullType<StringType>>();

            descriptor.Field("role")
                .Type<NonNullType<StringType>>();
        }
    }
}
