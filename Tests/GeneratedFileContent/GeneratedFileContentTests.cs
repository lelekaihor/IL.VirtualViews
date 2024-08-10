using IL.VirtualViews.Tests.VirtualViewsFolder;
using Xunit;

namespace IL.VirtualViews.Tests.GeneratedFileContent;

public class GeneratedFileContentTests
{
    [Fact]
    public void Generated_Cs_Class_Outputs_Correct_Content()
    {
        const string expected = """
                                @model string

                                <p>Test 123</p>
                                """;
        var result = new Test().ViewContent();
        
        Assert.Equal(expected, result);
    }
}