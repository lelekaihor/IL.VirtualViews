using Microsoft.AspNetCore.Builder;

namespace Il.ClassViewRendering.Extensions;

public static class WebAppBuilderExtensions
{
    public static WebApplicationBuilder AddVirtualViewsCapabilities(this WebApplicationBuilder builder)
    {
        builder.Services.AddVirtualViewsCapabilities();
        return builder;
    }
}