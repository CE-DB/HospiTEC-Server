using HospiTec_Server.DBModels;
using HotChocolate.Types;

namespace HospiTec_Server.Logic.Graphql.Types
{
    public class PersonType : ObjectType<Person>
    {
        protected override void Configure(IObjectTypeDescriptor<Person> descriptor)
        {
            base.Configure(descriptor);

            descriptor.BindFieldsExplicitly();

            descriptor.Field(e => e.Identification)
                .Type<NonNullType<StringType>>();

            descriptor.Field(e => e.FirstName)
                .Type<NonNullType<StringType>>();

            descriptor.Field(e => e.LastName)
                .Type<NonNullType<StringType>>();

            descriptor.Field(e => e.PhoneNumber)
                .Type<NonNullType<StringType>>();

            descriptor.Field(e => e.Canton)
                .Type<NonNullType<StringType>>();

            descriptor.Field(e => e.Province)
                .Type<NonNullType<StringType>>();

            descriptor.Field(e => e.ExactAddress)
                .Type<NonNullType<StringType>>();

            descriptor.Field(e => e.BirthDate)
                .Type<NonNullType<DateType>>();
        }


    }
}
