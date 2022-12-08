using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using FBus.Business.BaseBusiness.Implements;
using FBus.Business.BaseBusiness.Interfaces;
using FBus.Data.Context;
using FBus.Data.Interfaces;
using FBus.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using FBus.API.Utilities.Swagger;
using Microsoft.AspNetCore.Builder.Extensions;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using FBus.Business.Authorization.Interfaces;
using IAuthorizationService = FBus.Business.Authorization.Interfaces.IAuthorizationService;
using FBus.Business.Authorization.Implements;
using FBus.Business.StudentManagement.Interface;
using FBus.Business.StudentManagement.Implements;
using FBus.Business.StationManagement.Interfaces;
using FBus.Business.StationManagement.Implements;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Azure.Storage.Blobs;
using FBus.Business.RouteManagement.Interfaces;
using FBus.Business.RouteManagement.Implements;
using FBus.Business.BusVehicleManagement.Interfaces;
using FBus.Business.BusVehicleManagement.Implements;
using Microsoft.AspNetCore.Mvc;
using FBus.API.Utilities.Validation;
using FBus.Business.DriverManagement.Interfaces;
using FBus.Business.DriverManagement.Implements;
using FBus.Business.TripManagement.Interfaces;
using FBus.Business.TripManagement.Implements;
using FBus.Business.StudentTripManagement.Interfaces;
using FBus.Business.StudentTripManagement.Implements;
using FBus.Business.DashboardManagement.Interface;
using FBus.Business.DashboardManagement.Implements;
using TourismSmartTransportation.API;

namespace FBus.API
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
            // Background service
            services.AddHostedService<FbusBackgroundService>();

            //Database EF
            services.AddDbContext<FBusContext>(
               options => options.UseSqlServer(Configuration.GetConnectionString("FBus")));

            #region Authentication
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(option =>
            {
                option.SaveToken = true;
                option.RequireHttpsMetadata = false;
                option.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
                };

                option.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/hub")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            #endregion

            services.AddControllers();

            services.AddCors(option =>
            {
                option.AddDefaultPolicy(builder => { builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
            });

            #region Format Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("admin", new OpenApiInfo { Title = "FBus.API Admin", Version = "admin" });
                c.SwaggerDoc("driver", new OpenApiInfo { Title = "FBus.API Driver", Version = "driver" });
                c.SwaggerDoc("student", new OpenApiInfo { Title = "FBus.API Student", Version = "student" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });

                c.DocumentFilter<CustomSwaggerFilter>();

                c.TagActionsBy(api =>
                {
                    var controllerActionDescriptor = api.ActionDescriptor as ControllerActionDescriptor;
                    string controllerName = controllerActionDescriptor.ControllerName;

                    if (api.GroupName != null)
                    {
                        var name = api.GroupName + controllerName.Replace("Controller", "");
                        name = Regex.Replace(name, "([a-z])([A-Z])", "$1 $2");
                        return new[] { name };
                    }

                    if (controllerActionDescriptor != null)
                    {
                        controllerName = Regex.Replace(controllerName, "([a-z])([A-Z])", "$1 $2");
                        return new[] { controllerName };
                    }

                    throw new InvalidOperationException("Unable to determine tag for endpoint.");
                });

                c.DocInclusionPredicate((name, api) => true);
            });

            #endregion

            #region Format Error Response With Annotation Model Validation

            // Custom Error Message for Model Validation
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext => new AnnotationCustomErrorResponse().ErrorResponse(actionContext);
            });

            #endregion

            #region Add Third-party-service

            // Firebase
            services.AddSingleton(FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(Configuration["Firebase:Admin"]),
            }));

            // Azure blob
            services.AddScoped(_ => new BlobServiceClient(Configuration.GetConnectionString("AzureBlobStorage")));

            #endregion

            #region Define Service Provider

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IAzureBlobService, AzureBlobService>();
            services.AddScoped<ISMSService, SMSService>();
            services.AddScoped<IAuthorizationService, AuthorizationService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IStationManagementService, StationManagementService>();
            services.AddScoped<IRouteManagementService, RouteManagementService>();
            services.AddScoped<IBusService, BusService>();
            services.AddScoped<IDriverService, DriverService>();
            services.AddScoped<ITripManagementService, TripManagementService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IStudentTripManagementService, StudentTripManagementService>();
            services.AddScoped<IDashboardService, DashboardService>();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/admin/swagger.json", "FBus.API Admin");
                c.SwaggerEndpoint("/swagger/driver/swagger.json", "FBus.API Driver");
                c.SwaggerEndpoint("/swagger/student/swagger.json", "FBus.API Student");
                c.RoutePrefix = "";
            });

            // app.UseHttpsRedirection();
            app.UseExceptionHandler(c => c.Run(async context =>
            {
                var exception = context.Features
                    .Get<IExceptionHandlerPathFeature>()
                    .Error;
                var response = new
                {
                    statusCode = 500,
                    message = $"Lỗi hệ thống: {exception.Message}"
                };
                await context.Response.WriteAsJsonAsync(response);
            }));

            app.UseRouting();

            app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                if (env.IsDevelopment())
                    endpoints.MapControllers().WithMetadata(new AllowAnonymousAttribute());
                else
                    endpoints.MapControllers();
            });
        }
    }
}
