using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.MetaData;
using GraphQL.Conventions.Reflection;
using GraphQL.Conventions.Types;
using GraphQL.Conventions.Types.Wrapping;
using GraphQL.Types;
using Should;
using Xunit;
using static GraphQL.Conventions.Reflection.TypeConstructor;

namespace Tests.Reflection
{
    public class TypeConstruction
    {
        public class OutputType
        {
            [Fact]
            public void Empty()
            {
                var typeConstructor = new TypeConstructor();
                var type = (ObjectGraphType)typeConstructor.Derive(Entity.New(typeof(Data.Empty)));
                type.ShouldBeType<ObjectType<Data.Empty>>();
                type.Name.ShouldEqual(nameof(Data.Empty));
                type.Description.ShouldEqual(null);
                type.Interfaces.ShouldBeEmpty();
                type.Fields.ShouldBeEmpty();
            }

            [Fact]
            public void OverriddenNameAndDescription()
            {
                var typeConstructor = new TypeConstructor();
                var type = (ObjectGraphType)typeConstructor.Derive(Entity.New(typeof(Data.OverriddenNameAndDescription)));
                type.ShouldBeType<ObjectType<Data.OverriddenNameAndDescription>>();
                type.Name.ShouldEqual("EmptyOverride");
                type.Description.ShouldEqual("Lorem ipsum");
                type.Interfaces.ShouldBeEmpty();
                type.Fields.ShouldBeEmpty();
            }

            [Fact]
            public void FieldTypes()
            {
                var typeConstructor = new TypeConstructor();
                var type = (ObjectGraphType)typeConstructor.Derive(Entity.New(typeof(Data.Foo)));
                type.ShouldBeType<ObjectType<Data.Foo>>();
                type.Interfaces.ShouldBeEmpty();
                type.Fields.Count().ShouldEqual(3);

                var field = type.ShouldHaveField("field");
                field.Name.ShouldEqual(NameAttribute.AsFieldName(nameof(Data.Foo.Field)));
                field.Description.ShouldEqual("Hello");
                field.DeprecationReason.ShouldEqual("World");
                field.Arguments.ShouldBeEmpty();
                field.DefaultValue.ShouldBeNull();
                field.Type.ShouldEqual(typeof(StringGraphType));

                var property = type.ShouldHaveField("property");
                property.Name.ShouldEqual(NameAttribute.AsFieldName(nameof(Data.Foo.Property)));
                property.Description.ShouldEqual(null);
                property.DeprecationReason.ShouldEqual(null);
                property.Arguments.ShouldBeEmpty();
                property.DefaultValue.ShouldBeNull();
                property.Type.ShouldEqual(typeof(NonNullGraphType<IntGraphType>));

                var method = type.ShouldHaveField("method");
                method.Name.ShouldEqual(NameAttribute.AsFieldName(nameof(Data.Foo.Method)));
                method.Description.ShouldEqual(null);
                method.DeprecationReason.ShouldEqual(null);
                method.Arguments.Count.ShouldEqual(2);
                method.DefaultValue.ShouldBeNull();
                method.Type.ShouldEqual(typeof(BooleanGraphType));

                var arg1 = method.Arguments[0];
                arg1.Name.ShouldEqual("a");
                arg1.Description.ShouldEqual("abc");
                arg1.Type.ShouldEqual(typeof(IntGraphType));
                arg1.DefaultValue.ShouldBeNull();

                var arg2 = method.Arguments[1];
                arg2.Name.ShouldEqual("b");
                arg2.Description.ShouldEqual(null);
                arg2.Type.ShouldEqual(typeof(NonNullGraphType<FloatGraphType>));
                arg1.DefaultValue.ShouldEqual(1.0f);
            }

            [Fact]
            public void Tasks()
            {
                var typeConstructor = new TypeConstructor();
                var type = (ObjectGraphType)typeConstructor.Derive(Entity.New(typeof(Data.Baz)));
                type.ShouldBeType<ObjectType<Data.Baz>>();
                type.Interfaces.ShouldBeEmpty();
                type.Fields.Count().ShouldEqual(1);

                var field = type.ShouldHaveField("findInt");
                field.Name.ShouldEqual(NameAttribute.AsFieldName(nameof(Data.Baz.FindInt)));
                field.Type.ShouldEqual(typeof(NonNullGraphType<IntGraphType>));
                field.Arguments.Count.ShouldEqual(1);

                var arg = field.Arguments[0];
                arg.Name.ShouldEqual("input");
                arg.Type.ShouldEqual(typeof(IntGraphType));
                arg.DefaultValue.ShouldBeNull();
            }

            class Data
            {
                public class Empty { }

                [Name("EmptyOverride")]
                [Description("Lorem ipsum")]
                public class OverriddenNameAndDescription { }

                public class Foo
                {
                    [Description("Hello")]
                    [Deprecated("World")]
                    public string Field = "12345";

                    public int Property { get; set; }

                    public bool? Method(
                        [Description("abc")] int? a,
                        float b = 1.0f)
                    {
                        return a.Value + b > 0;
                    }
                }

                public class Baz
                {
                    public Task<int> FindInt(int? input)
                    {
                        return Task.FromResult(input.GetValueOrDefault(0) + 1);
                    }
                }
            }
        }

        public class InputType
        {
            [Fact]
            public void Empty()
            {
                var typeConstructor = new TypeConstructor();
                var type = (ObjectGraphType)typeConstructor.Derive(Entity.New(typeof(Data.Empty), true));
                type.ShouldBeType<InputObjectType<Data.Empty>>();
                type.Name.ShouldEqual(nameof(Data.Empty) + "Input");
                type.Description.ShouldEqual(null);
                type.Interfaces.ShouldBeEmpty();
                type.Fields.ShouldBeEmpty();
            }

            [Fact]
            public void OverriddenNameAndDescription()
            {
                var typeConstructor = new TypeConstructor();
                var type = (ObjectGraphType)typeConstructor.Derive(Entity.New(typeof(Data.OverriddenNameAndDescription), true));
                type.ShouldBeType<InputObjectType<Data.OverriddenNameAndDescription>>();
                type.Name.ShouldEqual("EmptyOverrideInput");
                type.Description.ShouldEqual("Lorem ipsum");
                type.Interfaces.ShouldBeEmpty();
                type.Fields.ShouldBeEmpty();
            }

            [Fact]
            public void FieldTypes()
            {
                var typeConstructor = new TypeConstructor();
                var entity = Entity.New(typeof(Data.Foo));
                var type = (ObjectGraphType)typeConstructor.Derive(entity);
                type.ShouldBeType<ObjectType<Data.Foo>>();
                type.Interfaces.ShouldBeEmpty();
                type.Fields.Count().ShouldEqual(3);

                var field = type.ShouldHaveField("field");
                field.Name.ShouldEqual(NameAttribute.AsFieldName(nameof(Data.Foo.Field)));
                field.Description.ShouldEqual("Hello");
                field.DeprecationReason.ShouldEqual("World");
                field.Arguments.ShouldBeEmpty();
                field.DefaultValue.ShouldBeNull();
                field.Type.ShouldEqual(typeof(StringGraphType));

                var property = type.ShouldHaveField("property");
                property.Name.ShouldEqual(NameAttribute.AsFieldName(nameof(Data.Foo.Property)));
                property.Description.ShouldEqual(null);
                property.DeprecationReason.ShouldEqual(null);
                property.Arguments.ShouldBeEmpty();
                property.DefaultValue.ShouldBeNull();
                property.Type.ShouldEqual(typeof(NonNullGraphType<IntGraphType>));

                var method = type.ShouldHaveField("method");
                method.Name.ShouldEqual(NameAttribute.AsFieldName(nameof(Data.Foo.Method)));
                method.Description.ShouldEqual(null);
                method.DeprecationReason.ShouldEqual(null);
                method.Arguments.Count.ShouldEqual(2);
                method.DefaultValue.ShouldBeNull();
                method.Type.ShouldEqual(typeof(NonNullGraphType<EnumType<Data.Baz>>));

                var arg1 = method.Arguments[0];
                arg1.Name.ShouldEqual("foo");
                arg1.Description.ShouldEqual(null);
                arg1.Type.ShouldEqual(typeof(InputObjectType<Data.Foo>));
                arg1.DefaultValue.ShouldBeNull();

                var arg2 = method.Arguments[1];
                arg2.Name.ShouldEqual("baz");
                arg2.Description.ShouldEqual(null);
                arg2.Type.ShouldEqual(typeof(EnumType<Data.Baz>));
                arg2.DefaultValue.ShouldBeNull();

                var entityField = entity.Fields.First(f => f.Name == "method");

                var entityArg1 = entityField.Arguments[0];
                entityArg1.WrappedType.Entity.IsInput.ShouldBeTrue();

                var entityArg2 = entityField.Arguments[1];
                entityArg2.WrappedType.Entity.IsInput.ShouldBeTrue();
            }

            public void DictionaryToObject()
            {
                var entity = Entity.New(typeof(Data.InputObject));
                var wrappedType = new WrappedType(entity);

                var obj = wrappedType.FromDictionary(new Dictionary<string, object>
                {
                    { "b", 123 },
                });

                obj.ShouldNotBeNull();
            }

            class Data
            {
                #pragma warning disable 0649

                public class Empty { }

                [Name("EmptyOverride")]
                [Description("Lorem ipsum")]
                public class OverriddenNameAndDescription { }

                public class Foo
                {
                    public string Field = "12345";

                    public int Property { get; set; }

                    public Baz Method(Foo foo, Baz baz)
                    {
                        return string.IsNullOrEmpty(foo.Field) ? Baz.A : Baz.C;
                    }
                }

                public class InputObject
                {
                    public string A;

                    public int B { get; set; }

                    public bool? C { get; set; }

                    public AnotherObject Object { get; set; }

                    public NonNull<AnotherObject> NonNullObject { get; set; }

                    public List<AnotherObject> NullableList { get; set; }

                    public List<NonNull<AnotherObject>> NullableListOfNonNull { get; set; }

                    public NonNull<List<AnotherObject>> NonNullableList { get; set; }

                    public NonNull<List<NonNull<AnotherObject>>> NonNullableListOfNonNull { get; set; }
                }

                public class AnotherObject
                {
                    public double D { get; set; }
                }

                public enum Baz
                {
                    A, B, C
                }

                #pragma warning restore 0649
            }
        }
    }
}
