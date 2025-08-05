using FluentAssertions;

using Http2Client.Core.Enums;
using Http2Client.Utilities;

using System.Text;

namespace Http2Client.Test.Utilities;

public class SerializerTests
{
    [Fact]
    public void SerializeToBytes_SimpleObject_ReturnsUtf8Bytes()
    {
        var obj = new { name = "test" };

        var result = Serializer.SerializeToBytes(obj);

        var json = Encoding.UTF8.GetString(result);
        json.Should().Contain("test");
    }

    [Fact]
    public void Deserialize_ValidJson_ReturnsObject()
    {
        const string json = """{"name":"test","value":123}""";

        var result = Serializer.Deserialize<TestObject>(json);

        result.Name.Should().Be("test");
        result.Value.Should().Be(123);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Deserialize_InvalidJson_Throws(string json)
    {
        var action = () => Serializer.Deserialize<TestObject>(json);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Serialize_BrowserType_UsesConverter()
    {
        var obj = new { browser = BrowserType.Chrome131 };

        var result = Serializer.Serialize(obj);

        result.Should().Contain("chrome_131");
    }

    private class TestObject
    {
        public string Name { get; set; } = "";
        public int Value { get; set; }
    }
}