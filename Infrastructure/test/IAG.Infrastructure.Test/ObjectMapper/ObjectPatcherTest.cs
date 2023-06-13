using System;

using IAG.Infrastructure.ObjectMapper;

using JetBrains.Annotations;

using Newtonsoft.Json.Linq;

using Xunit;

namespace IAG.Infrastructure.Test.ObjectMapper;

public class ObjectPatcherTest
{
    [Fact]
    public void PatchPropertyTest()
    {
        var testObject = new TestObject() { TextProperty = "Untouched" };
        testObject = ObjectPatcher.Patch(testObject, JObject.Parse(@"{
                NumberProperty: 5, 
                ReferenceProperty: { NumberProperty: 23, TextProperty: ""InnerText"" }
            }"));

        Assert.Equal(5, testObject.NumberProperty);
        Assert.Equal("Untouched", testObject.TextProperty);
        Assert.NotNull(testObject.ReferenceProperty);
        Assert.Equal(23, testObject.ReferenceProperty.NumberProperty);
        Assert.Equal("InnerText", testObject.ReferenceProperty.TextProperty);
    }

    [Fact]
    public void PatchNullTest()
    {
        var testObject = new TestObject() { NumberProperty = 42, TextProperty = "Untouched" };
        testObject = ObjectPatcher.Patch(testObject, null);

        Assert.Equal(42, testObject.NumberProperty);
        Assert.Equal("Untouched", testObject.TextProperty);
    }

    [Fact]
    public void PatchArgumentNullTest()
    {
        Assert.Throws<ArgumentNullException>(() => ObjectPatcher.Patch((TestObject)null, JObject.Parse("{NumberProperty:5}")));
    }

    [Fact]
    public void PatchInvalidPropertyTest()
    {
        var testObject = new TestObject() { NumberProperty = 42, TextProperty = "Untouched" };
        testObject = ObjectPatcher.Patch(testObject, JObject.Parse("{UnknownProperty:5}"));

        Assert.Equal(42, testObject.NumberProperty);
        Assert.Equal("Untouched", testObject.TextProperty);
    }


    private class TestObject
    {
        public int NumberProperty { get; set; }
        public string TextProperty { get; set; }
        public TestObject ReferenceProperty { get; [UsedImplicitly] set; }
    }
}