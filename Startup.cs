using AutoMapper;
using HAS.Profile.ApplicationServices.Messaging;
using HAS.Profile.Data;
using HAS.Profile.Feature.EventLog;
using IdentityServer4.AccessTokenValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HAS.Profile
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;

            var testConfig = Configuration["MPY:Version"];

            Environment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddAutoMapper(typeof(Startup));
            services.AddMediatR(typeof(Startup));

            services.AddCors();

            services.AddScoped<ProfileContext>();
            services.AddScoped<TribeContext>();
            services.AddSingleton<IQueueService>(opt =>
            {
                var queueService = AzureStorageQueueService.Create(Configuration["Azure:Storage:ConnectionString"]);
                queueService.CreateQueue(Configuration["Azure:Storage:Queue:LogEventMPY:Name"]).Wait();

                return queueService;
            });

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(EventLogBehavior<,>));

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = Configuration["MPY:IdentityServer:Authority"];
                    options.ApiName = "MPY.Profile";
                    if (Environment.IsDevelopment())
                    {
                        options.RequireHttpsMetadata = false;
                    }
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("student", policyAdmin => policyAdmin.RequireClaim("role", "student"));
                options.AddPolicy("instructor", policyAdmin => policyAdmin.RequireClaim("role", "instructor"));
                options.AddPolicy("admin", policyAdmin => policyAdmin.RequireClaim("role", "admin"));
                options.AddPolicy("superadmin", policyAdmin => policyAdmin.RequireClaim("role", "superadmin"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(options =>
            {
                options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
