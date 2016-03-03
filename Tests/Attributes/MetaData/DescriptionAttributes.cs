using System.Linq;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.MetaData;
using Should;
using Xunit;

namespace Tests.Attributes.MetaData
{
    public class DescriptionAttributes
    {
        #pragma warning disable 0649

        public class Types
        {
            [Fact]
            public void NotDescribed()
            {
                var typeInfo = typeof(Data.NotDescribedClass);
                var entity = Entity.New(typeInfo);
                entity.Description.ShouldBeNull();
            }

            [Fact]
            public void Described()
            {
                var typeInfo = typeof(Data.DescribedClass);
                var entity = Entity.New(typeInfo);
                entity.Description.ShouldEqual("NA");
            }

            class Data
            {
                public class NotDescribedClass { }

                [Description("NA")]
                public class DescribedClass { }
            }
        }

        public class Properties
        {
            [Fact]
            public void NotDescribed()
            {
                var memberInfo = typeof(Foo).GetProperty(nameof(Foo.NotDescribedProperty));
                var entity = Entity.New(memberInfo);
                entity.Description.ShouldBeNull();
            }

            [Fact]
            public void Described()
            {
                var memberInfo = typeof(Foo).GetProperty(nameof(Foo.DescribedProperty));
                var entity = Entity.New(memberInfo);
                entity.Description.ShouldEqual("NA");
            }

            [Fact]
            public void InheritedAndNotDescribed()
            {
                var memberInfo = typeof(Foo).GetProperty(nameof(Foo.NotDescribedInheritedProperty));
                var entity = Entity.New(memberInfo);
                entity.Description.ShouldBeNull();
            }

            [Fact]
            public void InheritedAndDescribed()
            {
                var memberInfo = typeof(Foo).GetProperty(nameof(Foo.DescribedInheritedProperty));
                var entity = Entity.New(memberInfo);
                entity.Description.ShouldEqual("NA");
            }

            class FooBase
            {
                public string NotDescribedInheritedProperty { get; set; }

                [Description("NA")]
                public string DescribedInheritedProperty { get; set; }
            }

            class Foo : FooBase
            {
                public string NotDescribedProperty { get; set; }

                [Description("NA")]
                public string DescribedProperty { get; set; }
            }
        }

        public class Fields
        {
            [Fact]
            public void NotDescribed()
            {
                var memberInfo = typeof(Foo).GetField(nameof(Foo.NotDescribedField));
                var entity = Entity.New(memberInfo);
                entity.Description.ShouldBeNull();
            }

            [Fact]
            public void Described()
            {
                var memberInfo = typeof(Foo).GetField(nameof(Foo.DescribedField));
                var entity = Entity.New(memberInfo);
                entity.Description.ShouldEqual("NA");
            }

            [Fact]
            public void InheritedAndNotDescribed()
            {
                var memberInfo = typeof(Foo).GetField(nameof(Foo.NotDescribedInheritedField));
                var entity = Entity.New(memberInfo);
                entity.Description.ShouldBeNull();
            }

            [Fact]
            public void InheritedAndDescribed()
            {
                var memberInfo = typeof(Foo).GetField(nameof(Foo.DescribedInheritedField));
                var entity = Entity.New(memberInfo);
                entity.Description.ShouldEqual("NA");
            }

            class FooBase
            {
                public string NotDescribedInheritedField;

                [Description("NA")]
                public string DescribedInheritedField;
            }

            class Foo : FooBase
            {
                public string NotDescribedField;

                [Description("NA")]
                public string DescribedField;
            }
        }

        public class Methods
        {
            [Fact]
            public void NotDescribed()
            {
                var memberInfo = typeof(Foo).GetMethod(nameof(Foo.NotDescribedMethod));
                var entity = Entity.New(memberInfo);
                entity.Description.ShouldBeNull();
            }

            [Fact]
            public void Described()
            {
                var memberInfo = typeof(Foo).GetMethod(nameof(Foo.DescribedMethod));
                var entity = Entity.New(memberInfo);
                entity.Description.ShouldEqual("NA");
            }

            [Fact]
            public void InheritedAndNotDescribed()
            {
                var memberInfo = typeof(Foo).GetMethod(nameof(Foo.NotDescribedInheritedMethod));
                var entity = Entity.New(memberInfo);
                entity.Description.ShouldBeNull();
            }

            [Fact]
            public void InheritedAndDescribed()
            {
                var memberInfo = typeof(Foo).GetMethod(nameof(Foo.DescribedInheritedMethod));
                var entity = Entity.New(memberInfo);
                entity.Description.ShouldEqual("NA");
            }

            class FooBase
            {
                public void NotDescribedInheritedMethod() { }

                [Description("NA")]
                public void DescribedInheritedMethod() { }
            }

            class Foo : FooBase
            {
                public void NotDescribedMethod() { }

                [Description("NA")]
                public void DescribedMethod() { }
            }
        }

        public class Parameters
        {
            [Fact]
            public void NotDescribed()
            {
                var methodInfo = typeof(Foo).GetMethod(nameof(Foo.NotDescribedParameter));
                var parameterInfo = methodInfo.GetParameters().First();
                var entity = Entity.New(parameterInfo);
                entity.Description.ShouldBeNull();
            }

            [Fact]
            public void Described()
            {
                var methodInfo = typeof(Foo).GetMethod(nameof(Foo.DescribedParameter));
                var parameterInfo = methodInfo.GetParameters().First();
                var entity = Entity.New(parameterInfo);
                entity.Description.ShouldEqual("NA");
            }

            class Foo
            {
                public void NotDescribedParameter(
                    string parameter)
                { }

                public void DescribedParameter(
                    [Description("NA")] string parameter)
                { }
            }
        }

        public class Enums
        {
            [Fact]
            public void NotDescribed()
            {
                var memberInfo = typeof(Bar).GetMember(nameof(Bar.NotDescribed)).First();
                var entity = Entity.New(memberInfo);
                entity.Description.ShouldBeNull();
            }

            [Fact]
            public void Described()
            {
                var memberInfo = typeof(Bar).GetMember(nameof(Bar.Described)).First();
                var entity = Entity.New(memberInfo);
                entity.Description.ShouldEqual("NA");
            }

            enum Bar
            {
                NotDescribed,

                [Description("NA")]
                Described,
            }
        }

        public class Interfaces
        {
            [Fact]
            public void NotDescribed()
            {
                var typeInfo = typeof(Data.NotDescribedInterface);
                var entity = Entity.New(typeInfo);
                entity.Interfaces.Count.ShouldEqual(1);
                entity.Interfaces.ShouldContain<Data.INotDescribed>();
                entity.Interfaces.First().Description.ShouldBeNull();
            }

            [Fact]
            public void Described()
            {
                var typeInfo = typeof(Data.DescribedInterface);
                var entity = Entity.New(typeInfo);
                entity.Interfaces.Count.ShouldEqual(1);
                entity.Interfaces.ShouldContain<Data.IDescribed>();
                entity.Interfaces.First().Description.ShouldEqual("NA");
            }

            class Data
            {
                public class NotDescribedInterface : INotDescribed { }

                public class DescribedInterface : IDescribed { }

                public interface INotDescribed { }

                [Description("NA")]
                public interface IDescribed { }
            }
        }

        public class Unions
        {
            [Fact]
            public void NotDescribed()
            {
                var typeInfo = typeof(Data.UnionWithNotDescribedType);
                var entity = Entity.New(typeInfo);
                entity.UnionTypes.Count.ShouldEqual(1);
                entity.UnionTypes.ShouldContain<Data.NotDescribedType>();
                entity.UnionTypes.First().Description.ShouldBeNull();
            }

            [Fact]
            public void Described()
            {
                var typeInfo = typeof(Data.UnionWithDescribedType);
                var entity = Entity.New(typeInfo);
                entity.UnionTypes.Count.ShouldEqual(1);
                entity.UnionTypes.ShouldContain<Data.DescribedType>();
                entity.UnionTypes.First().Description.ShouldEqual("NA");
            }

            class Data
            {
                [Union(typeof(NotDescribedType))]
                public class UnionWithNotDescribedType { }

                [Union(typeof(DescribedType))]
                public class UnionWithDescribedType { }

                public class NotDescribedType { }

                [Description("NA")]
                public class DescribedType { }
            }
        }

        #pragma warning restore 0649
    }
}
