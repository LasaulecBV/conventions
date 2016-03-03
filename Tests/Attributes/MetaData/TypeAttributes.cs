using System.Linq;
using GraphQL.Conventions.Attributes.Data;
using Should;
using Xunit;

namespace Tests.Attributes.MetaData
{
    public class TypeAttributes
    {
#pragma warning disable 0649

        public class TypeRepresentation
        {
            [Fact]
            public void Type()
            {
                var memberInfo = typeof(Foo);
                var entity = Entity.New(memberInfo);
                entity.TypeRepresentation.ShouldEqual(typeof(Foo));
                entity.Kind.ShouldEqual(Kind.OutputType);
                entity.Fields.Count.ShouldBeGreaterThan(0);
                entity.Fields.First().DeclaringType.ShouldEqual(entity);
            }

            [Fact]
            public void Interface()
            {
                var memberInfo = typeof(IFoo);
                var entity = Entity.New(memberInfo);
                entity.TypeRepresentation.ShouldEqual(typeof(IFoo));
                entity.Kind.ShouldEqual(Kind.InterfaceType);
                entity.Fields.Count.ShouldBeGreaterThan(0);
                entity.Fields.First().DeclaringType.ShouldEqual(entity);
            }

            [Fact]
            public void Property()
            {
                var memberInfo = typeof(Foo).GetProperty(nameof(Foo.Property));
                var entity = Entity.New(memberInfo);
                entity.TypeRepresentation.ShouldEqual(typeof(string));
                entity.Kind.ShouldEqual(Kind.Field);
            }

            [Fact]
            public void Field()
            {
                var memberInfo = typeof(Foo).GetField(nameof(Foo.Field));
                var entity = Entity.New(memberInfo);
                entity.TypeRepresentation.ShouldEqual(typeof(string));
                entity.Kind.ShouldEqual(Kind.Field);
            }

            [Fact]
            public void Method()
            {
                var memberInfo = typeof(Foo).GetMethod(nameof(Foo.Method));
                var entity = Entity.New(memberInfo);
                entity.TypeRepresentation.ShouldEqual(typeof(string));
                entity.Kind.ShouldEqual(Kind.Field);
            }

            [Fact]
            public void Parameter()
            {
                var methodInfo = typeof(Foo).GetMethod(nameof(Foo.Parameter));
                var memberInfo = methodInfo.GetParameters().First();
                var entity = Entity.New(memberInfo);
                entity.TypeRepresentation.ShouldEqual(typeof(string));
                entity.Kind.ShouldEqual(Kind.Argument);
            }

            [Fact]
            public void EnumType()
            {
                var memberInfo = typeof(Enum);
                var entity = Entity.New(memberInfo);
                entity.TypeRepresentation.ShouldEqual(typeof(Enum));
                entity.Kind.ShouldEqual(Kind.EnumType);
                entity.Fields.Count.ShouldBeGreaterThan(0);
                entity.Fields.First().DeclaringType.ShouldEqual(entity);
            }

            [Fact]
            public void EnumMember()
            {
                var memberInfo = typeof(Enum).GetMember(nameof(Enum.Value)).First();
                var entity = Entity.New(memberInfo);
                entity.TypeRepresentation.ShouldEqual(typeof(Enum));
                entity.DefaultValue.ShouldEqual(Enum.Value);
                entity.Kind.ShouldEqual(Kind.EnumValue);
            }

            class Foo
            {
                public string Property { get; set; }

                public string Field;

                public string Method() { return string.Empty; }

                public void Parameter(string parameter) { }
            }

            interface IFoo
            {
                string Property { get; }
            }

            enum Enum
            {
                Value = 12345
            }
        }

        #pragma warning restore 0649
    }
}
