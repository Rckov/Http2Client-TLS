using FluentAssertions;

using Http2Client.Core.Enums;
using Http2Client.Utilities;

using System.Text.Json;

namespace Http2Client.Test.Core.Converters;

public class JsonBrowserTypeConverterTests
{
    [Theory]
    [InlineData(BrowserType.Chrome131, "\"chrome_131\"")]
    [InlineData(BrowserType.Firefox132, "\"firefox_132\"")]
    [InlineData(BrowserType.Safari160, "\"safari_16_0\"")]
    public void Serialize_BrowserType_WritesCorrectString(BrowserType browserType, string expected)
    {
        var result = Serializer.Serialize(browserType);

        result.Should().Be(expected);
    }

    [Fact]
    public void Deserialize_ValidString_ReturnsCorrectEnum()
    {
        const string json = "\"chrome_131\"";

        var result = Serializer.Deserialize<BrowserType>(json);

        result.Should().Be(BrowserType.Chrome131);
    }

    [Fact]
    public void Deserialize_InvalidString_Throws()
    {
        const string json = "\"invalid_browser\"";

        var action = () => Serializer.Deserialize<BrowserType>(json);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void RoundTrip_AllBrowserTypes_Works()
    {
        var browserTypes = Enum.GetValues<BrowserType>();

        foreach (var browserType in browserTypes)
        {
            var json = Serializer.Serialize(browserType);
            var deserialized = Serializer.Deserialize<BrowserType>(json);

            deserialized.Should().Be(browserType);
        }
    }
}