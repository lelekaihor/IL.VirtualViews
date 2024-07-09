# IL.VirtualViews
Library which allows virtually register *.cshtml file as .cs classes with special attributes.

# How to use

* Reference IL.VirtualViews in your project
* Use registration extensions coming with library to activate functionality: `(IServiceCollection)services.AddVirtualViewsCapabilities()` or `(WebApplicationBuilder)builder.AddVirtualViewsCapabilities()`
* Use `[VirtualViewPath("")]` attribute for your classes with path to a view you want to register virtually
* Derive your class from `IVirtualView` interface and implement the content which view is supposed to have

## Examples
```
[VirtualViewPath("/Views/Test.cshtml")]
class Sample : IVirtualView
{
    public string Content {get;} =
    """
    @model string

    @Html.Raw(Model)
    """;
}
```