using System;
using System.Collections.Generic;
using GraphQL.Conventions.Input;
using Should;
using Xunit;

namespace Tests.Input
{
    public class InputExtensions
    {
        [Fact]
        public void ToInputsEmpty()
        {
            ((string)null).ToInputs().ShouldEqual(new Dictionary<string, object>());
            string.Empty.ToInputs().ShouldEqual(new Dictionary<string, object>());
            "{}".ToInputs().ShouldEqual(new Dictionary<string, object>());
            " { } ".ToInputs().ShouldEqual(new Dictionary<string, object>());
        }

        [Fact]
        public void ToInputsInvalidJson()
        {
            try
            {
                "{".ToInputs();
                true.ShouldBeFalse();
            }
            catch (Exception ex)
            {
                ex.Message.ShouldEqual("Unexpected end when deserializing object. Path '', line 1, position 1.");
            }
        }

        [Fact]
        public void ToInputsSingleEntry()
        {
            var data = @"{ 'key': 'value' }";

            var dict = data.ToInputs();
            dict.Count.ShouldEqual(1);
            dict.ContainsKey("key").ShouldBeTrue();
            dict["key"].ShouldEqual("value");
        }

        [Fact]
        public void ToInputsMultipleEntries()
        {
            var data =
                @"{
                    'key1': 'value1',
                    'key2': 15,
                    'key3': 3.14159
                }";

            var dict = data.ToInputs();
            dict.Count.ShouldEqual(3);
            dict.ContainsKey("key1").ShouldBeTrue();
            dict["key1"].ShouldEqual("value1");
            dict.ContainsKey("key2").ShouldBeTrue();
            dict["key2"].ShouldEqual(15L);
            dict.ContainsKey("key3").ShouldBeTrue();
            dict["key3"].ShouldEqual(3.14159);
        }

        [Fact]
        public void ToInputsArrays()
        {
            var data =
                @"{
                    'key1': ['value1', 'value2'],
                    'key2': 'blah',
                    'key3': [1, 'two', 3.0]
                }";

            var dict = data.ToInputs();
            dict.Count.ShouldEqual(3);

            dict.ContainsKey("key1").ShouldBeTrue();
            ((List<object>)dict["key1"]).Count.ShouldEqual(2);
            ((List<object>)dict["key1"])[0].ShouldEqual("value1");
            ((List<object>)dict["key1"])[1].ShouldEqual("value2");

            dict.ContainsKey("key2").ShouldBeTrue();
            dict["key2"].ShouldEqual("blah");

            dict.ContainsKey("key3").ShouldBeTrue();
            ((List<object>)dict["key3"]).Count.ShouldEqual(3);
            ((List<object>)dict["key3"])[0].ShouldEqual(1L);
            ((List<object>)dict["key3"])[1].ShouldEqual("two");
            ((List<object>)dict["key3"])[2].ShouldEqual(3.0);
        }

        [Fact]
        public void ToInputsNestedEntries()
        {
            var data =
                @"{
                    'key1': [ { 'a': 1, 'b': 2, 'c': 3.0 } ],
                    'key2': { 'foo': 'bar' }
                }";

            var dict = data.ToInputs();
            dict.Count.ShouldEqual(2);

            dict.ContainsKey("key1").ShouldBeTrue();
            var elem1 = (List<object>)dict["key1"];
            elem1.Count.ShouldEqual(1);

            var elem1Obj1 = (Dictionary<string, object>)elem1[0];
            elem1Obj1.Count.ShouldEqual(3);
            elem1Obj1["a"].ShouldEqual(1L);
            elem1Obj1["b"].ShouldEqual(2L);
            elem1Obj1["c"].ShouldEqual(3.0);

            dict.ContainsKey("key2").ShouldBeTrue();
            var elem2 = (Dictionary<string, object>)dict["key2"];
            elem2.Count.ShouldEqual(1);
            elem2["foo"].ShouldEqual("bar");
        }
    }
}
