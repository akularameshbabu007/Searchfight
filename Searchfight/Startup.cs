using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SearchfightEngine.Interfaces;
using SearchfightEngine.Services;
using System;

namespace Searchfight
{
    public class Startup
    {
        private readonly ILifetimeScope _webHostScope;
        private ILifetimeScope _aspNetScope;

        public Startup(ILifetimeScope webHostScope)
        {
            _webHostScope = webHostScope ?? throw new ArgumentNullException(nameof(webHostScope));
        }

        public IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddControllers();
            services.AddTransient<ISearchfightSummaryCounter, SearchfightSummaryCounter>();
            services.AddTransient<ISearchService, SearchService>();
            // doesn't work with Autofac 4.6.0 and child scopes.
            // An exception is thrown on startup: Cannot resolve parameter 'IOptionsFactory<KestrelServerOptions>'
            _aspNetScope = _webHostScope.BeginLifetimeScope(builder => builder.Populate(services));

           
            
            return new AutofacServiceProvider(_aspNetScope);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, Microsoft.AspNetCore.Hosting.IApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddFile("Searchfight_{Date}.txt");
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            appLifetime.ApplicationStopped.Register(() => _aspNetScope.Dispose());
        }
    }
}
