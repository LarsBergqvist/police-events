using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;
using Core.DI;
using Infrastructure.DI;

namespace Api;

public class Startup
{
    private readonly string _corsPolicy = "appCorsPolicy";

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(_corsPolicy,
                builder =>
                {
                    builder.WithOrigins(Configuration["AllowedOrigins"].Split(";")).AllowAnyHeader().AllowAnyMethod();
                });
        });

        services
            .AddCoreServices()
            .AddInfrastructureServices()
            .AddControllersWithViews()
            ;

        services.AddRazorPages();

        services.AddSwaggerGen(setupAction =>
        {
            setupAction.SwaggerDoc("api", new Microsoft.OpenApi.Models.OpenApiInfo()
            {
                Title = "Police Events API",
                Version = "1",
                Description = "An API for accessing police events data"
            });
            var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
            setupAction.IncludeXmlComments(xmlCommentsFullPath);
        });

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();


        app.UseSwagger();

        app.UseSwaggerUI(setupAction =>
        {
            setupAction.SwaggerEndpoint("/swagger/api/swagger.json", "Police Events API");
            setupAction.RoutePrefix = "swagger";
        });

        app.UseRouting();

        app.UseCors(_corsPolicy);

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapFallbackToFile("index.html");
        });
    }
}