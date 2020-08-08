using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using HotChocolate;
using System;
using HotChocolate.Types;
using HotChocolate.AspNetCore;
using HospiTec_Server.Logic.Graphql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HospiTec_Server.database;
using HospiTec_Server.database.DBModels;

namespace HospiTec_Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddCors();
            services.AddControllers();

            /// This generates the parameters of validation for JWT Tokens sended to this server
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(config =>
                {
                    var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Constants.key));

                    config.RequireHttpsMetadata = false;
                    config.SaveToken = true;

                    config.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            ///This adds the policies for role admin, doctor, patient and nurse
            services.AddAuthorization(x =>
            {
                x.AddPolicy(Constants.adminRole, builder =>
                    builder
                        .RequireAuthenticatedUser()
                        .RequireRole(Constants.adminRole)
                        .Build()
                );

                x.AddPolicy(Constants.doctorRole, builder =>
                    builder
                        .RequireAuthenticatedUser()
                        .RequireRole(Constants.doctorRole)
                        .Build()
                );

                x.AddPolicy(Constants.nurseRole, builder =>
                    builder
                        .RequireAuthenticatedUser()
                        .RequireRole(Constants.nurseRole)
                        .Build()
                );

                x.AddPolicy(Constants.patientRole, builder =>
                    builder
                        .RequireAuthenticatedUser()
                        .RequireRole(Constants.patientRole)
                        .Build()
                );
            });

            /// This adds the DB Context of hospitec database to services available for use in other classes.
            services
              .AddDbContext<hospitecContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("postgresql")));

            /// This adds the DB Context of CoTEC-2020 database to services available for use in other classes.
            services
              .AddDbContext<CotecModels.CoTEC_DBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("sqlserver")));

            /// This adds the controller for mongodb database to services available for use in other classes.
            services
                .AddSingleton<MongoDatabase>();
            
            /// This configures the graphql service with the query an mutation class 
            /// Also, enbales the security policies.
            services
                .AddDataLoaderRegistry()
                .AddGraphQL(SchemaBuilder
                    .New()
                    .BindClrType<DateTime, DateType>()
                    // Here, we add the QueryType and MutationType
                    .AddQueryType<Query>()
                    .AddMutationType<Mutation>()
                    .AddAuthorizeDirectiveType()
                    .Create());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseMvc();

            app.UseAuthentication();

            /// This allows any client to connect to server
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());


            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseWebSockets();
            app.UseGraphQLHttpPost(new HttpPostMiddlewareOptions { Path = "/graphql" });
            app.UseGraphQL();
            app.UsePlayground();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
