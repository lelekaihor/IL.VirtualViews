using IL.VirtualViews.ContentProvider;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.DependencyInjection;

namespace IL.VirtualViews.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVirtualViewsCapabilities(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddControllersWithViews()
            .AddRazorRuntimeCompilation();

        serviceCollection.Configure<MvcRazorRuntimeCompilationOptions>(options =>
        {
            options.FileProviders.Add(new VirtualViewsProvider());
        });

        return serviceCollection;
    }
}