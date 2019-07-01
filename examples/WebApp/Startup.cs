using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Unsinedz.SimpleJsonLocalization;
using Unsinedz.SimpleJsonLocalization.Strings;

namespace WebApp
{
    public class Startup
    {
        public IHostingEnvironment Environment { get; private set; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<StringLocalizationOptions>(options =>
            {
                options.DefaultCulture = new CultureInfo("en-US");
                options.AllowFallbackToDefaultCulture = true;
                options.Providers = new[]
                {
                     new JsonStringProvider(@".\Resources\Strings.en-US.json"),
                     new JsonStringProvider(@".\Resources\Strings.es.json"),
                     new JsonStringProvider(@".\Resources\Strings.de-DE.json")
                };
            }).AddSimpleJsonLocalization();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRequestLocalization(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(culture: "en-US");
                options.SupportedCultures = options.SupportedUICultures = new[]
                {
                    new CultureInfo("en"), // will fallback to default "en-US"
                    new CultureInfo("en-US"),
                    new CultureInfo("es"),
                    new CultureInfo("es-ES"), // will fallback to neutral "es"
                    new CultureInfo("de"), // will fallback to default "en-US"
                    new CultureInfo("de-DE")
                };
            });

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseMvc();
        }
    }
}
