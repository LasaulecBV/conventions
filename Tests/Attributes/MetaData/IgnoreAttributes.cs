using System.Linq;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.MetaData;
using Should;
using Xunit;

namespace Tests.Attributes.MetaData
{
    public class IgnoreAttributes
    {
        #pragma warning disable 0649

        public class Types
        {
            [Fact]
            public void Included()
            {
                var typeInfo = typeof(Data.IncludedClass);
                var entity = Entity.New(typeInfo);
                entity.IsIgnored.ShouldBeFalse();
            }

            [Fact]
            public void Ignored()
            {
                var typeInfo = typeof(Data.IgnoredClass);
                var entity = Entity.New(typeInfo);
                entity.IsIgnored.ShouldBeTrue();
            }

            class Data
            {
                public class IncludedClass { }

                [Ignore]
                public class IgnoredClass { }
            }
        }

        public class Properties
        {
            [Fact]
            public void Included()
            {
                var memberInfo = typeof(Foo).GetProperty(nameof(Foo.IncludedProperty));
                var entity = Entity.New(memberInfo);
                entity.IsIgnored.ShouldBeFalse();
            }

            [Fact]
            public void Ignored()
            {
                var memberInfo = typeof(Foo).GetProperty(nameof(Foo.IgnoredProperty));
                var entity = Entity.New(memberInfo);
                entity.IsIgnored.ShouldBeTrue();
            }

            [Fact]
            public void InheritedAndIncluded()
            {
                var memberInfo = typeof(Foo).GetProperty(nameof(Foo.IncludedInheritedProperty));
                var entity = Entity.New(memberInfo);
                entity.IsIgnored.ShouldBeFalse();
            }

            [Fact]
            public void InheritedAndIgnored()
            {
                var memberInfo = typeof(Foo).GetProperty(nameof(Foo.IgnoredInheritedProperty));
                var entity = Entity.New(memberInfo);
                entity.IsIgnored.ShouldBeTrue();
            }

            class FooBase
            {
                public string IncludedInheritedProperty { get; set; }

                [Ignore]
                public string IgnoredInheritedProperty { get; set; }
            }

            class Foo : FooBase
            {
                public string IncludedProperty { get; set; }

                [Ignore]
                public string IgnoredProperty { get; set; }
            }
        }

        public class Fields
        {
            [Fact]
            public void Included()
            {
                var memberInfo = typeof(Foo).GetField(nameof(Foo.IncludedField));
                var entity = Entity.New(memberInfo);
                entity.IsIgnored.ShouldBeFalse();
            }

            [Fact]
            public void Ignored()
            {
                var memberInfo = typeof(Foo).GetField(nameof(Foo.IgnoredField));
                var entity = Entity.New(memberInfo);
                entity.IsIgnored.ShouldBeTrue();
            }

            [Fact]
            public void InheritedAndIncluded()
            {
                var memberInfo = typeof(Foo).GetField(nameof(Foo.IncludedInheritedField));
                var entity = Entity.New(memberInfo);
                entity.IsIgnored.ShouldBeFalse();
            }

            [Fact]
            public void InheritedAndIgnored()
            {
                var memberInfo = typeof(Foo).GetField(nameof(Foo.IgnoredInheritedField));
                var entity = Entity.New(memberInfo);
                entity.IsIgnored.ShouldBeTrue();
            }

            class FooBase
            {
                public string IncludedInheritedField;

                [Ignore]
                public string IgnoredInheritedField;
            }

            class Foo : FooBase
            {
                public string IncludedField;

                [Ignore]
                public string IgnoredField;
            }
        }

        public class Methods
        {
            [Fact]
            public void Included()
            {
                var memberInfo = typeof(Foo).GetMethod(nameof(Foo.IncludedMethod));
                var entity = Entity.New(memberInfo);
                entity.IsIgnored.ShouldBeFalse();
            }

            [Fact]
            public void Ignored()
            {
                var memberInfo = typeof(Foo).GetMethod(nameof(Foo.IgnoredMethod));
                var entity = Entity.New(memberInfo);
                entity.IsIgnored.ShouldBeTrue();
            }

            [Fact]
            public void InheritedAndIncluded()
            {
                var memberInfo = typeof(Foo).GetMethod(nameof(Foo.IncludedInheritedMethod));
                var entity = Entity.New(memberInfo);
                entity.IsIgnored.ShouldBeFalse();
            }

            [Fact]
            public void InheritedAndIgnored()
            {
                var memberInfo = typeof(Foo).GetMethod(nameof(Foo.IgnoredInheritedMethod));
                var entity = Entity.New(memberInfo);
                entity.IsIgnored.ShouldBeTrue();
            }

            class FooBase
            {
                public void IncludedInheritedMethod() { }

                [Ignore]
                public void IgnoredInheritedMethod() { }
            }

            class Foo : FooBase
            {
                public void IncludedMethod() { }

                [Ignore]
                public void IgnoredMethod() { }
            }
        }

        public class Parameters
        {
            [Fact]
            public void Included()
            {
                var methodInfo = typeof(Foo).GetMethod(nameof(Foo.IncludedParameter));
                var parameterInfo = methodInfo.GetParameters().First();
                var entity = Entity.New(parameterInfo);
                entity.IsIgnored.ShouldBeFalse();
            }

            [Fact]
            public void Ignored()
            {
                var methodInfo = typeof(Foo).GetMethod(nameof(Foo.IgnoredParameter));
                var parameterInfo = methodInfo.GetParameters().First();
                var entity = Entity.New(parameterInfo);
                entity.IsIgnored.ShouldBeTrue();
            }

            class Foo
            {
                public void IncludedParameter(
                    string parameter)
                { }

                public void IgnoredParameter(
                    [Ignore] string parameter)
                { }
            }
        }

        public class Enums
        {
            [Fact]
            public void Included()
            {
                var memberInfo = typeof(Bar).GetMember(nameof(Bar.Included)).First();
                var entity = Entity.New(memberInfo);
                entity.IsIgnored.ShouldBeFalse();
            }

            [Fact]
            public void Ignored()
            {
                var memberInfo = typeof(Bar).GetMember(nameof(Bar.Ignored)).First();
                var entity = Entity.New(memberInfo);
                entity.IsIgnored.ShouldBeTrue();
            }

            enum Bar
            {
                Included,

                [Ignore]
                Ignored,
            }
        }

        public class Interfaces
        {
            [Fact]
            public void Included()
            {
                var typeInfo = typeof(Data.IncludedInterface);
                var entity = Entity.New(typeInfo);
                entity.Interfaces.Count.ShouldEqual(1);
                entity.Interfaces.ShouldContain<Data.IIncluded>();
            }

            [Fact]
            public void Ignored()
            {
                var typeInfo = typeof(Data.IgnoredInterface);
                var entity = Entity.New(typeInfo);
                entity.Interfaces.Count.ShouldEqual(0);
            }

            class Data
            {
                public class IncludedInterface : IIncluded { }

                public class IgnoredInterface : IIgnored { }

                public interface IIncluded { }

                [Ignore]
                public interface IIgnored { }
            }
        }

        public class Unions
        {
            [Fact]
            public void Included()
            {
                var typeInfo = typeof(Data.UnionWithIncludedType);
                var entity = Entity.New(typeInfo);
                entity.UnionTypes.Count.ShouldEqual(1);
                entity.UnionTypes.ShouldContain<Data.IncludedType>();
            }

            [Fact]
            public void Ignored()
            {
                var typeInfo = typeof(Data.UnionWithIgnoredType);
                var entity = Entity.New(typeInfo);
                entity.UnionTypes.Count.ShouldEqual(0);
            }

            class Data
            {
                [Union(typeof(IncludedType))]
                public class UnionWithIncludedType { }

                [Union(typeof(IgnoredType))]
                public class UnionWithIgnoredType { }

                public class IncludedType { }

                [Ignore]
                public class IgnoredType { }
            }
        }

        #pragma warning restore 0649
    }
}
