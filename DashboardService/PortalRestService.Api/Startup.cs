using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using PortalRestService.Api.Service;
using PortalRestService.Application.Handlers.Assets.QueryHandlers;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Repositories.Base;
using PortalRestService.Helpers;
using PortalRestService.Infrastructure.Repositories;
using PortalRestService.Infrastructure.Repositories.Assets;
using PortalRestService.Infrastructure.Repositories.Repository;
using System.Reflection;

namespace RestService.Assets
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
            Dictionary<string, string> myConfiguration = new Dictionary<string, string>
                {
                    {"AzureAd:Instance",Environment.GetEnvironmentVariable("AZUREAD_INSTANCE")},
                    {"AzureAd:Domain",Environment.GetEnvironmentVariable("AZUREAD_DOMAIN")},
                    {"AzureAd:clientId", Environment.GetEnvironmentVariable("AZUREAD_CID")},
                    {"AzureAd:TenantId", Environment.GetEnvironmentVariable("AZUREAD_TID")},
                    {"AzureAd:audience",Environment.GetEnvironmentVariable("AZUREAD_AUD")},
                };
            IConfiguration configurationENV = new ConfigurationBuilder().AddInMemoryCollection(myConfiguration).Build();
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            var dbName = Environment.GetEnvironmentVariable("DB_NAME");
            var dbPassword = Environment.GetEnvironmentVariable("DB_USER_PASSWORD");
            var dbUserName = Environment.GetEnvironmentVariable("DB_LOGIN_USERNAME");
            var connectionString = $"Data Source={dbHost};Initial Catalog={dbName};User ID={dbUserName};Password={dbPassword}";
            if (dbHost == null)
            {
                connectionString = Configuration.GetConnectionString("AssetsDB");
                configurationENV = new ConfigurationBuilder().AddInMemoryCollection(myConfiguration).Build();
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(Configuration.GetSection("AzureAd"));
                services.AddControllers();
                Environment.SetEnvironmentVariable("ASSETAPI", Configuration["ApplicationUrl:AssetBaseUrl"].ToString());
            }
            else
            {
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(configurationENV.GetSection("AzureAd"));
                services.AddControllers();
            }
            services.AddDbContext<PortalRestService.Infrastructure.DBContext.ocpp_dbContext>(
            m => m.UseSqlServer(connectionString), ServiceLifetime.Transient);

            services.AddCors();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DASHBOARD.API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "www.Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });
            services.AddAutoMapper(typeof(Startup));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(GetAllChargingSessionHandler).GetTypeInfo().Assembly));
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<IChargingSessionRepository, ChargingSessionRepository>();
            services.AddTransient<IChargerByLocationRepository, GetChargerByLocationIDRepository>();
            services.AddTransient<IEnergyUsedByLocationIDRepository, EnergyUsedByLocationIDRepository>();
            services.AddTransient<ILocationStatusByLocationIdRepository, LocationStatusByLocationIdRepository>();
            services.AddTransient<ILocationPerformingRepository, LocationPerformingRepository>();
            services.AddTransient<ILocationDispenserRepository, LocationDispenserRepository>();
            services.AddTransient<IVehicleDashboardRepository, VehicleDashboardRepository>();
            services.AddSingleton<IHttpHelper, HttpHelper>();
            services.AddTransient<IMilesAddedByLocationQueryRepository, MilesAddedByLocationRepository>();
            services.AddTransient<IVehicleRepository, VehicleRepository>();
            services.AddTransient<IEventLogByLocationRepository, EventLogBylocationRepository>();
            services.AddTransient<IGetAllAlertsRepository, GetAllAlertsRepository>();
            services.AddTransient<IUpdateIsReadEventLogByIDRepository, UpdateIsReadEventLogByIDRepository>();
            services.AddTransient<IUpdateOcppEventLogAndTaskNotificationRepository, UpdateOcppEventLogAndTaskNotificationRepository>();
            services.AddTransient<IChartDetailsListRepository, GetChartDetailsListRepository>();
            services.AddTransient<IGetChargerSessionDetailsListRepository, ChargerSessionDetailsListRepository>();
            services.AddTransient<IGetChargerInformationRepository, GetChargerInfoRepository>();
            services.AddTransient<IGetSummaryDataRepository, GetSummaryDataRepository>();
            services.AddTransient<IGetSummaryStatusRepository, GetSummaryStatusRepository>();
            services.AddTransient<IGetAllChargeBoxIDRepository, GetAllChargeBoxIDRepository>();
            services.AddTransient<IRfIdReaderRepository, RfIdReaderRepository>();
            services.AddTransient<IUpdateIsNotificationRepository, UpdateNotificationIsReadRepository>();
            services.AddTransient<INotificationRepository, NotificationRespository>();
            services.AddTransient<ILocationsDispenserRepository, LocationsDispenserRepository>();
            services.AddTransient<ILocationRepository, LocationRepository>();
            services.AddTransient<IDispenserDetailRepository, DispensersDetailRepository>();
            services.AddTransient<IGetLocationByIdRepository, GetLocationByIdRepository>();
            services.AddTransient<IChargingSessionAndPaymentTransactionRepository, ChargingSessionAndPaymentTransactionRepository>();
            services.AddScoped<PortalRestService.Infrastructure.Helper.TokenBase>();
            services.AddHealthChecks()
                .AddCheck<PortalHealthCheck>("example_health_check");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DASHBOARD.API v1"));
            //}

            // app.UseHttpsRedirection();

            app.UseCors(buider =>
            {
                buider
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                {
                    ResultStatusCodes =
                    {
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Degraded] = StatusCodes.Status200OK,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    }
                });
            });
        }
    }
}