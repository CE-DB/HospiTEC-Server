using HospiTec_Server.database.DBModels;
using HotChocolate.Types;

namespace HospiTec_Server.Logic.Graphql.Types
{
    /// <summary>
    /// This maps the fields of the graphql type for the person entity
    /// </summary>
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
                .Type<StringType>();

            descriptor.Field(e => e.Canton)
                .Type<StringType>();

            descriptor.Field(e => e.Province)
                .Type<NonNullType<StringType>>();

            descriptor.Field(e => e.ExactAddress)
                .Type<StringType>();

            descriptor.Field(e => e.BirthDate)
                .Type<DateType>();

            //This field is to mark the patients who are from CoTEC-2020 database
            descriptor.Field(e => e.External)
                .Type<NonNullType<BooleanType>>();
        }


    }
}
