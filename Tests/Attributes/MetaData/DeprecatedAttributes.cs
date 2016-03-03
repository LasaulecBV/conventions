using System.Linq;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.MetaData;
using Should;
using Xunit;

namespace Tests.Attributes.MetaData
{
    public class DeprecatedAttributes
    {
        #pragma warning disable 0649

        public class Types
        {
            [Fact]
            public void NotDeprecated()
            {
                var typeInfo = typeof(Data.NotDeprecatedClass);
                var entity = Entity.New(typeInfo);
                entity.IsDeprecated.ShouldBeFalse();
            }

            [Fact]
            public void Deprecated()
            {
                var typeInfo = typeof(Data.DeprecatedClass);
                var entity = Entity.New(typeInfo);
                entity.IsDeprecated.ShouldBeTrue();
                entity.DeprecationReason.ShouldEqual("NA");
            }

            class Data
            {
                public class NotDeprecatedClass { }

                [Deprecated("NA")]
                public class DeprecatedClass { }
            }
        }

        public class Properties
        {
            [Fact]
            public void NotDeprecated()
            {
                var memberInfo = typeof(Foo).GetProperty(nameof(Foo.NotDeprecatedProperty));
                var entity = Entity.New(memberInfo);
                entity.IsDeprecated.ShouldBeFalse();
            }

            [Fact]
            public void Deprecated()
            {
                var memberInfo = typeof(Foo).GetProperty(nameof(Foo.DeprecatedProperty));
                var entity = Entity.New(memberInfo);
                entity.IsDeprecated.ShouldBeTrue();
            }

            [Fact]
            public void InheritedAndNotDeprecated()
            {
                var memberInfo = typeof(Foo).GetProperty(nameof(Foo.NotDeprecatedInheritedProperty));
                var entity = Entity.New(memberInfo);
                entity.IsDeprecated.ShouldBeFalse();
            }

            [Fact]
            public void InheritedAndDeprecated()
            {
                var memberInfo = typeof(Foo).GetProperty(nameof(Foo.DeprecatedInheritedProperty));
                var entity = Entity.New(memberInfo);
                entity.IsDeprecated.ShouldBeTrue();
            }

            class FooBase
            {
                public string NotDeprecatedInheritedProperty { get; set; }

                [Deprecated("NA")]
                public string DeprecatedInheritedProperty { get; set; }
            }

            class Foo : FooBase
            {
                public string NotDeprecatedProperty { get; set; }

                [Deprecated("NA")]
                public string DeprecatedProperty { get; set; }
            }
        }

        public class Fields
        {
            [Fact]
            public void NotDeprecated()
            {
                var memberInfo = typeof(Foo).GetField(nameof(Foo.NotDeprecatedField));
                var entity = Entity.New(memberInfo);
                entity.IsDeprecated.ShouldBeFalse();
            }

            [Fact]
            public void Deprecated()
            {
                var memberInfo = typeof(Foo).GetField(nameof(Foo.DeprecatedField));
                var entity = Entity.New(memberInfo);
                entity.IsDeprecated.ShouldBeTrue();
            }

            [Fact]
            public void InheritedAndNotDeprecated()
            {
                var memberInfo = typeof(Foo).GetField(nameof(Foo.NotDeprecatedInheritedField));
                var entity = Entity.New(memberInfo);
                entity.IsDeprecated.ShouldBeFalse();
            }

            [Fact]
            public void InheritedAndDeprecated()
            {
                var memberInfo = typeof(Foo).GetField(nameof(Foo.DeprecatedInheritedField));
                var entity = Entity.New(memberInfo);
                entity.IsDeprecated.ShouldBeTrue();
            }

            class FooBase
            {
                public string NotDeprecatedInheritedField;

                [Deprecated("NA")]
                public string DeprecatedInheritedField;
            }

            class Foo : FooBase
            {
                public string NotDeprecatedField;

                [Deprecated("NA")]
                public string DeprecatedField;
            }
        }

        public class Methods
        {
            [Fact]
            public void NotDeprecated()
            {
                var memberInfo = typeof(Foo).GetMethod(nameof(Foo.NotDeprecatedMethod));
                var entity = Entity.New(memberInfo);
                entity.IsDeprecated.ShouldBeFalse();
            }

            [Fact]
            public void Deprecated()
            {
                var memberInfo = typeof(Foo).GetMethod(nameof(Foo.DeprecatedMethod));
                var entity = Entity.New(memberInfo);
                entity.IsDeprecated.ShouldBeTrue();
            }

            [Fact]
            public void InheritedAndNotDeprecated()
            {
                var memberInfo = typeof(Foo).GetMethod(nameof(Foo.NotDeprecatedInheritedMethod));
                var entity = Entity.New(memberInfo);
                entity.IsDeprecated.ShouldBeFalse();
            }

            [Fact]
            public void InheritedAndDeprecated()
            {
                var memberInfo = typeof(Foo).GetMethod(nameof(Foo.DeprecatedInheritedMethod));
                var entity = Entity.New(memberInfo);
                entity.IsDeprecated.ShouldBeTrue();
            }

            class FooBase
            {
                public void NotDeprecatedInheritedMethod() { }

                [Deprecated("NA")]
                public void DeprecatedInheritedMethod() { }
            }

            class Foo : FooBase
            {
                public void NotDeprecatedMethod() { }

                [Deprecated("NA")]
                public void DeprecatedMethod() { }
            }
        }

        public class Parameters
        {
            [Fact]
            public void NotDeprecated()
            {
                var methodInfo = typeof(Foo).GetMethod(nameof(Foo.NotDeprecatedParameter));
                var parameterInfo = methodInfo.GetParameters().First();
                var entity = Entity.New(parameterInfo);
                entity.IsDeprecated.ShouldBeFalse();
            }

            [Fact]
            public void Deprecated()
            {
                var methodInfo = typeof(Foo).GetMethod(nameof(Foo.DeprecatedParameter));
                var parameterInfo = methodInfo.GetParameters().First();
                var entity = Entity.New(parameterInfo);
                entity.IsDeprecated.ShouldBeTrue();
            }

            class Foo
            {
                public void NotDeprecatedParameter(
                    string parameter)
                { }

                public void DeprecatedParameter(
                    [Deprecated("NA")] string parameter)
                { }
            }
        }

        public class Enums
        {
            [Fact]
            public void NotDeprecated()
            {
                var memberInfo = typeof(Bar).GetMember(nameof(Bar.NotDeprecated)).First();
                var entity = Entity.New(memberInfo);
                entity.IsDeprecated.ShouldBeFalse();
            }

            [Fact]
            public void Deprecated()
            {
                var memberInfo = typeof(Bar).GetMember(nameof(Bar.Deprecated)).First();
                var entity = Entity.New(memberInfo);
                entity.IsDeprecated.ShouldBeTrue();
            }

            enum Bar
            {
                NotDeprecated,

                [Deprecated("NA")]
                Deprecated,
            }
        }

        public class Interfaces
        {
            [Fact]
            public void NotDeprecated()
            {
                var typeInfo = typeof(Data.NotDeprecatedInterface);
                var entity = Entity.New(typeInfo);
                entity.Interfaces.Count.ShouldEqual(1);
                entity.Interfaces.ShouldContain<Data.INotDeprecated>();
                entity.Interfaces.First().IsDeprecated.ShouldBeFalse();
            }

            [Fact]
            public void Deprecated()
            {
                var typeInfo = typeof(Data.DeprecatedInterface);
                var entity = Entity.New(typeInfo);
                entity.Interfaces.Count.ShouldEqual(1);
                entity.Interfaces.ShouldContain<Data.IDeprecated>();
                entity.Interfaces.First().IsDeprecated.ShouldBeTrue();
            }

            class Data
            {
                public class NotDeprecatedInterface : INotDeprecated { }

                public class DeprecatedInterface : IDeprecated { }

                public interface INotDeprecated { }

                [Deprecated("NA")]
                public interface IDeprecated { }
            }
        }

        public class Unions
        {
            [Fact]
            public void NotDeprecated()
            {
                var typeInfo = typeof(Data.UnionWithNotDeprecatedType);
                var entity = Entity.New(typeInfo);
                entity.UnionTypes.Count.ShouldEqual(1);
                entity.UnionTypes.ShouldContain<Data.NotDeprecatedType>();
                entity.UnionTypes.First().IsDeprecated.ShouldBeFalse();
            }

            [Fact]
            public void Deprecated()
            {
                var typeInfo = typeof(Data.UnionWithDeprecatedType);
                var entity = Entity.New(typeInfo);
                entity.UnionTypes.Count.ShouldEqual(1);
                entity.UnionTypes.ShouldContain<Data.DeprecatedType>();
                entity.UnionTypes.First().IsDeprecated.ShouldBeTrue();
            }

            class Data
            {
                [Union(typeof(NotDeprecatedType))]
                public class UnionWithNotDeprecatedType { }

                [Union(typeof(DeprecatedType))]
                public class UnionWithDeprecatedType { }

                public class NotDeprecatedType { }

                [Deprecated("NA")]
                public class DeprecatedType { }
            }
        }

        #pragma warning restore 0649
    }
}
