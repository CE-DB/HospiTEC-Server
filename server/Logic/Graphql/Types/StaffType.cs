using HospiTec_Server.database.DBModels;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql.Types
{
    /// <summary>
    /// This maps the fields of the graphql type for the staff entity
    /// </summary>
    public class StaffType : ObjectType<Staff>
    {
        protected override void Configure(IObjectTypeDescriptor<Staff> descriptor)
        {
            base.Configure(descriptor);

            descriptor.BindFieldsExplicitly();

            descriptor.Field(e => e.Name)
                .Name("role")
                .Type<StringType>();

            descriptor.Field(e => e.AdmissionDate)
                .Type<DateType>();

            descriptor.Field("person")
                .Type<NonNullType<PersonType>>()
                .Resolver(ctx => {

                    ///This gets the person associated with this staff
                    return ctx.Service<hospitecContext>()
                    .Person
                    .Where(p => p.Identification.Equals(ctx.Parent<Staff>().Identification))
                    .FirstOrDefault();
                    
                });

        }
    }
}
