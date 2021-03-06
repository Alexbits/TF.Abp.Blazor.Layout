﻿using System;
using System.Net.Http;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using IdentityModel;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.AspNetCore.Components.WebAssembly.Theming.Routing;
using Volo.Abp.Autofac.WebAssembly;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Identity.Blazor;
using Volo.Abp.AutoMapper;
using Volo.Abp.TenantManagement.Blazor;
using TF.Abp.Blazor.Layout.AntDesignTheme;
using TF.Abp.Blazor.Layout.AntDesignTheme.Themes.Basic;
using AntDesign.Pro.Layout;
using TF.Abp.Blazor.Layout.AntDesignTheme.Setting;

namespace TF.Abp.Blazor.Layout.BlazoriseTheme.Demo
{
    [DependsOn(
        typeof(AbpAutofacWebAssemblyModule),
        typeof(LayoutHttpApiClientModule),
        typeof(TFAbpBlazorAntDesignThemeModule),//To be added for TF AntDesign Theme
        typeof(AbpIdentityBlazorModule),
        typeof(AbpTenantManagementBlazorModule)
    )]
    public class LayoutBlazorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var environment = context.Services.GetSingletonInstance<IWebAssemblyHostEnvironment>();
            var builder = context.Services.GetSingletonInstance<WebAssemblyHostBuilder>();

            ConfigureAuthentication(builder);
            ConfigureHttpClient(context, environment);
            ConfigureBlazorise(context);
            ConfigureAntDesign(context, builder);   //To be added for TF AntDesign Theme
            ConfigureRouter(context);
            ConfigureUI(builder);
            ConfigureMenu(context);
            ConfigureAutoMapper(context);
        }

        private void ConfigureRouter(ServiceConfigurationContext context)
        {
            Configure<AbpRouterOptions>(options =>
            {
                options.AppAssembly = typeof(LayoutBlazorModule).Assembly;
            });
        }

        private void ConfigureMenu(ServiceConfigurationContext context)
        {
            Configure<AbpNavigationOptions>(options =>
            {
                options.MenuContributors.Add(new LayoutMenuContributor(context.Services.GetConfiguration()));
            });
        }

        private void ConfigureBlazorise(ServiceConfigurationContext context)
        {
            context.Services
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();
        }
        /// <summary>
        /// To be added for TF AntDesign Theme
        /// Config AntDesign and Layout setting
        /// </summary>
        /// <param name="context"></param>
        /// <param name="builder"></param>
        private void ConfigureAntDesign(ServiceConfigurationContext context, WebAssemblyHostBuilder builder)
        {
            context.Services.AddAntDesign();
            context.Services.Configure<ProSettings>(builder.Configuration.GetSection("ProSettings"));
            context.Services.Configure<TFAntDesignSettings>(builder.Configuration.GetSection("TFAntDesignSettings"));
        }

        private static void ConfigureAuthentication(WebAssemblyHostBuilder builder)
        {
            builder.Services.AddOidcAuthentication(options =>
            {
                builder.Configuration.Bind("AuthServer", options.ProviderOptions);
                options.UserOptions.RoleClaim = JwtClaimTypes.Role;
                options.ProviderOptions.DefaultScopes.Add("Layout");
                options.ProviderOptions.DefaultScopes.Add("role");
                options.ProviderOptions.DefaultScopes.Add("email");
                options.ProviderOptions.DefaultScopes.Add("phone");
            });
        }

        private static void ConfigureUI(WebAssemblyHostBuilder builder)
        {
            builder.RootComponents.Add<App>("#ApplicationContainer");
        }

        private static void ConfigureHttpClient(ServiceConfigurationContext context, IWebAssemblyHostEnvironment environment)
        {
            context.Services.AddTransient(sp => new HttpClient
            {
                BaseAddress = new Uri(environment.BaseAddress)
            });
        }

        private void ConfigureAutoMapper(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<LayoutBlazorModule>();
            });
        }
    }
}
