using Microsoft.AspNetCore.Builder;

namespace IL.VirtualViews.Extensions;

public static class WebAppBuilderExtensions
{
    public static WebApplicationBuilder AddVirtualViewsCapabilities(this WebApplicationBuilder builder)
    {
        builder.Services.AddVirtualViewsCapabilities();
        return builder;
    }
}