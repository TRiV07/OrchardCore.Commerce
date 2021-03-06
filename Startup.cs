using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Money;
using Money.Abstractions;
using OrchardCore.Commerce.Abstractions;
using OrchardCore.Commerce.Drivers;
using OrchardCore.Commerce.Fields;
using OrchardCore.Commerce.Handlers;
using OrchardCore.Commerce.Indexes;
using OrchardCore.Commerce.Migrations;
using OrchardCore.Commerce.Models;
using OrchardCore.Commerce.Services;
using OrchardCore.Commerce.Settings;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.ContentTypes.Editors;
using OrchardCore.Data.Migration;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.Security.Permissions;
using OrchardCore.Settings;
using YesSql.Indexes;

namespace OrchardCore.Commerce
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            // Product
            services.AddSingleton<IIndexProvider, ProductPartIndexProvider>();
            services.AddScoped<IDataMigration, ProductMigrations>();
            services.AddScoped<IContentAliasProvider, ProductPartContentAliasProvider>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IContentPartDisplayDriver, ProductPartDisplayDriver>();
            services.AddContentPart<ProductPart>();
            // Attributes
            services.AddContentField<BooleanProductAttributeField>();
            services.AddScoped<IContentFieldDisplayDriver, BooleanProductAttributeFieldDriver>();
            services.AddScoped<IContentPartFieldDefinitionDisplayDriver, BooleanProductAttributeFieldSettingsDriver>();
            services.AddContentField<NumericProductAttributeField>();
            services.AddScoped<IContentFieldDisplayDriver, NumericProductAttributeFieldDriver>();
            services.AddScoped<IContentPartFieldDefinitionDisplayDriver, NumericProductAttributeFieldSettingsDriver>();
            services.AddContentField<TextProductAttributeField>();
            services.AddScoped<IContentFieldDisplayDriver, TextProductAttributeFieldDriver>();
            services.AddScoped<IContentPartFieldDefinitionDisplayDriver, TextProductAttributeFieldSettingsDriver>();
            services.AddScoped<IProductAttributeProvider, ProductAttributeProvider>();
            services.AddScoped<IProductAttributeService, ProductAttributeService>();
            services.AddScoped<IPredefinedValuesProductAttributeService, PredefinedValuesProductAttributeService>();
            // Price
            services.AddScoped<IDataMigration, PriceMigrations>();
            services.AddScoped<IContentPartHandler, PricePartHandler>();
            services.AddScoped<IContentPartDisplayDriver, PricePartDisplayDriver>();
            services.AddScoped<IContentTypePartDefinitionDisplayDriver, PricePartSettingsDisplayDriver>();
            services.AddContentPart<PricePart>();
            services.AddScoped<IPriceProvider, PriceProvider>();
            services.AddScoped<IPriceService, PriceService>();
            services.AddScoped<IPriceSelectionStrategy, SimplePriceStrategy>();
            // Price Variants
            services.AddScoped<IDataMigration, PriceVariantsMigrations>();
            services.AddScoped<IContentPartHandler, PriceVariantsPartHandler>();
            services.AddScoped<IContentPartDisplayDriver, PriceVariantsPartDisplayDriver>();
            services.AddContentPart<PriceVariantsPart>();
            services.AddScoped<IPriceProvider, PriceVariantProvider>();
            services.AddScoped<IPriceVariantsService, PriceVariantsService>();
            // Currency
            services.AddScoped<ICurrencyProvider, CurrencyProvider>();
            services.AddScoped<IMoneyService, MoneyService>();
            // Shopping cart
            services.AddScoped<IShoppingCartHelpers, ShoppingCartHelpers>();
            // Settings
            services.AddScoped<IPermissionProvider, Permissions>();
            services.AddScoped<IDisplayDriver<ISite>, CommerceSettingsDisplayDriver>();
            services.AddScoped<INavigationProvider, AdminMenu>();
            services.AddTransient<IConfigureOptions<CommerceSettings>, CommerceSettingsConfiguration>();
        }
    }

    [RequireFeatures(CommerceConstants.Features.SessionCartStorage)]
    public class SessionCartStorageStartup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSession(options => { });
            // Shopping Cart
            services.AddScoped<IShoppingCartPersistence, SessionShoppingCartPersistence>();
        }

        public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            base.Configure(app, routes, serviceProvider);
            app.UseSession();
            routes.MapAreaControllerRoute(
                name: "ShoppingCart",
                areaName: "OrchardCore.Commerce",
                pattern: "shoppingcart/{action}",
                defaults: new { controller = "ShoppingCart", action = "Index" }
            );
        }
    }
}
