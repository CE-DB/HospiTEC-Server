using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using HospiTec_Server.DBModels;
using Microsoft.EntityFrameworkCore;
using HotChocolate;
using System;
using HotChocolate.Types;
using HotChocolate.AspNetCore;
using HospiTec_Server.Logic.Graphql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
            services.AddCors();
            services.AddControllers();

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

                    //config.SaveToken = true;
                });

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

            services
              .AddDbContext<hospitecContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("postgresql")));

            services
              .AddDbContext<CotecModels.CoTEC_DBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("sqlserver")));

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
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());


            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });


            //app.UseHttpsRedirection();

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
