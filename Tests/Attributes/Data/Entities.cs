using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.Interfaces;
using GraphQL.Conventions.Attributes.MetaData;
using Should;
using Xunit;

namespace Tests.Attributes.Data
{
    public class Entities
    {
        #pragma warning disable 0649

        public class Types
        {
            [Fact]
            public void Plain()
            {
                var typeInfo = typeof(Data.PlainClass);
                var entity = Entity.New(typeInfo);
                entity.Name.ShouldEqual(nameof(Data.PlainClass));
            }

            [Fact]
            public void Named()
            {
                var typeInfo = typeof(Data.NamedClass);
                var entity = Entity.New(typeInfo);
                entity.Name.ShouldEqual(OverriddenTypeName);
            }

            [Fact]
            public void Cached()
            {
                var typeInfo1 = typeof(Data.SomeClass<int>);
                var entity1 = Entity.New(typeInfo1);

                var typeInfo2 = typeof(Data.SomeClass<int>);
                var entity2 = entity1.Construct(typeInfo2);

                entity1.ShouldEqual(entity2);
            }

            [Fact]
            public void NotCached()
            {
                var typeInfo1 = typeof(Data.SomeClass<int>);
                var entity1 = Entity.New(typeInfo1);

                var typeInfo2 = typeof(Data.SomeClass<bool>);
                var entity2 = entity1.Construct(typeInfo2);

                entity1.ShouldNotEqual(entity2);
            }

            class Data
            {
                public class PlainClass { }

                [Name(OverriddenTypeName)]
                public class NamedClass { }

                public class SomeClass<T> { }
            }
        }

        public class Properties
        {
            [Fact]
            public void Plain()
            {
                var memberInfo = typeof(Foo).GetProperty(nameof(Foo.PlainProperty));
                var entity = Entity.New(memberInfo);

                entity.Name.ShouldEqualFieldName(nameof(Foo.PlainProperty));
                entity.Description.ShouldEqual(null);
                entity.IsDeprecated.ShouldEqual(false);
                entity.DeprecationReason.ShouldEqual(null);
            }

            [Fact]
            public void Named()
            {
                var memberInfo = typeof(Foo).GetProperty(nameof(Foo.NamedProperty));
                var entity = Entity.New(memberInfo);

                entity.Name.ShouldEqualFieldName(OverriddenName);
                entity.Description.ShouldEqual(null);
                entity.IsDeprecated.ShouldEqual(false);
                entity.DeprecationReason.ShouldEqual(null);
            }

            [Fact]
            public void Described()
            {
                var memberInfo = typeof(Foo).GetProperty(nameof(Foo.DescribedProperty));
                var entity = Entity.New(memberInfo);

                entity.Name.ShouldEqualFieldName(nameof(Foo.DescribedProperty));
                entity.Description.ShouldEqual(Description);
                entity.IsDeprecated.ShouldEqual(false);
                entity.DeprecationReason.ShouldEqual(null);
            }

            [Fact]
            public void NamedAndDescribed()
            {
                var memberInfo = typeof(Foo).GetProperty(nameof(Foo.NamedAndDescribedProperty));
                var entity = Entity.New(memberInfo);

                entity.Name.ShouldEqual(OverriddenName);
                entity.Description.ShouldEqual(Description);
                entity.IsDeprecated.ShouldEqual(false);
                entity.DeprecationReason.ShouldEqual(null);
            }

            [Fact]
            public void Deprecated()
            {
                var memberInfo = typeof(Foo).GetProperty(nameof(Foo.DeprecatedProperty));
                var entity = Entity.New(memberInfo);

                entity.Name.ShouldEqualFieldName(nameof(Foo.DeprecatedProperty));
                entity.Description.ShouldEqual(null);
                entity.IsDeprecated.ShouldEqual(true);
                entity.DeprecationReason.ShouldEqual(DeprecationReason);
            }

            class Foo
            {
                public string PlainProperty { get; set; }

                [Name(OverriddenName)]
                public string NamedProperty { get; set; }

                [Description(Description)]
                public string DescribedProperty { get; set; }

                [Name(OverriddenName)]
                [Description(Description)]
                public string NamedAndDescribedProperty { get; set; }

                [Deprecated(DeprecationReason)]
                public string DeprecatedProperty { get; set; }
            }
        }

        public class Fields
        {
            [Fact]
            public void Plain()
            {
                var memberInfo = typeof(Foo).GetField(nameof(Foo.PlainField));
                var entity = Entity.New(memberInfo);

                entity.Name.ShouldEqualFieldName(nameof(Foo.PlainField));
                entity.Description.ShouldEqual(null);
                entity.IsDeprecated.ShouldEqual(false);
                entity.DeprecationReason.ShouldEqual(null);
            }

            [Fact]
            public void Named()
            {
                var memberInfo = typeof(Foo).GetField(nameof(Foo.NamedField));
                var entity = Entity.New(memberInfo);

                entity.Name.ShouldEqual(OverriddenName);
                entity.Description.ShouldEqual(null);
                entity.IsDeprecated.ShouldEqual(false);
                entity.DeprecationReason.ShouldEqual(null);
            }

            [Fact]
            public void Described()
            {
                var memberInfo = typeof(Foo).GetField(nameof(Foo.DescribedField));
                var entity = Entity.New(memberInfo);

                entity.Name.ShouldEqualFieldName(nameof(Foo.DescribedField));
                entity.Description.ShouldEqual(Description);
                entity.IsDeprecated.ShouldEqual(false);
                entity.DeprecationReason.ShouldEqual(null);
            }

            [Fact]
            public void NamedAndDescribed()
            {
                var memberInfo = typeof(Foo).GetField(nameof(Foo.NamedAndDescribedField));
                var entity = Entity.New(memberInfo);

                entity.Name.ShouldEqual(OverriddenName);
                entity.Description.ShouldEqual(Description);
                entity.IsDeprecated.ShouldEqual(false);
                entity.DeprecationReason.ShouldEqual(null);
            }

            [Fact]
            public void Deprecated()
            {
                var memberInfo = typeof(Foo).GetField(nameof(Foo.DeprecatedField));
                var entity = Entity.New(memberInfo);

                entity.Name.ShouldEqualFieldName(nameof(Foo.DeprecatedField));
                entity.Description.ShouldEqual(null);
                entity.IsDeprecated.ShouldEqual(true);
                entity.DeprecationReason.ShouldEqual(DeprecationReason);
            }

            class Foo
            {
                public string PlainField;

                [Name(OverriddenName)]
                public string NamedField;

                [Description(Description)]
                public string DescribedField;

                [Name(OverriddenName)]
                [Description(Description)]
                public string NamedAndDescribedField;

                [Deprecated(DeprecationReason)]
                public string DeprecatedField;
            }
        }

        public class Methods
        {
            [Fact]
            public void Plain()
            {
                var memberInfo = typeof(Foo).GetMethod(nameof(Foo.PlainMethod));
                var entity = Entity.New(memberInfo);

                entity.Name.ShouldEqualFieldName(nameof(Foo.PlainMethod));
                entity.Description.ShouldEqual(null);
                entity.IsDeprecated.ShouldEqual(false);
                entity.DeprecationReason.ShouldEqual(null);
            }

            [Fact]
            public void Named()
            {
                var memberInfo = typeof(Foo).GetMethod(nameof(Foo.NamedMethod));
                var entity = Entity.New(memberInfo);

                entity.Name.ShouldEqual(OverriddenName);
                entity.Description.ShouldEqual(null);
                entity.IsDeprecated.ShouldEqual(false);
                entity.DeprecationReason.ShouldEqual(null);
            }

            [Fact]
            public void Described()
            {
                var memberInfo = typeof(Foo).GetMethod(nameof(Foo.DescribedMethod));
                var entity = Entity.New(memberInfo);

                entity.Name.ShouldEqualFieldName(nameof(Foo.DescribedMethod));
                entity.Description.ShouldEqual(Description);
                entity.IsDeprecated.ShouldEqual(false);
                entity.DeprecationReason.ShouldEqual(null);
            }

            [Fact]
            public void NamedAndDescribed()
            {
                var memberInfo = typeof(Foo).GetMethod(nameof(Foo.NamedAndDescribedMethod));
                var entity = Entity.New(memberInfo);

                entity.Name.ShouldEqual(OverriddenName);
                entity.Description.ShouldEqual(Description);
                entity.IsDeprecated.ShouldEqual(false);
                entity.DeprecationReason.ShouldEqual(null);
            }

            [Fact]
            public void Deprecated()
            {
                var memberInfo = typeof(Foo).GetMethod(nameof(Foo.DeprecatedMethod));
                var entity = Entity.New(memberInfo);

                entity.Name.ShouldEqualFieldName(nameof(Foo.DeprecatedMethod));
                entity.Description.ShouldEqual(null);
                entity.IsDeprecated.ShouldEqual(true);
                entity.DeprecationReason.ShouldEqual(DeprecationReason);
            }

            class Foo
            {
                public void PlainMethod() { }

                [Name(OverriddenName)]
                public void NamedMethod() { }

                [Description(Description)]
                public void DescribedMethod() { }

                [Name(OverriddenName)]
                [Description(Description)]
                public void NamedAndDescribedMethod() { }

                [Deprecated(DeprecationReason)]
                public void DeprecatedMethod() { }
            }
        }

        public class Parameters
        {
            [Fact]
            public void Plain()
            {
                var methodInfo = typeof(Foo).GetMethod(nameof(Foo.PlainParameter));
                var parameterInfo = methodInfo.GetParameters().First();
                var entity = Entity.New(parameterInfo);

                entity.Name.ShouldEqual(ParameterName);
                entity.Description.ShouldEqual(null);
                entity.IsDeprecated.ShouldEqual(false);
                entity.DeprecationReason.ShouldEqual(null);
            }

            [Fact]
            public void Named()
            {
                var methodInfo = typeof(Foo).GetMethod(nameof(Foo.NamedParameter));
                var parameterInfo = methodInfo.GetParameters().First();
                var entity = Entity.New(parameterInfo);

                entity.Name.ShouldNotEqual(ParameterName);
                entity.Name.ShouldEqual(OverriddenName);
                entity.Description.ShouldEqual(null);
                entity.IsDeprecated.ShouldEqual(false);
                entity.DeprecationReason.ShouldEqual(null);
            }

            [Fact]
            public void Described()
            {
                var methodInfo = typeof(Foo).GetMethod(nameof(Foo.DescribedParameter));
                var parameterInfo = methodInfo.GetParameters().First();
                var entity = Entity.New(parameterInfo);

                entity.Name.ShouldEqual(ParameterName);
                entity.Description.ShouldEqual(Description);
                entity.IsDeprecated.ShouldEqual(false);
                entity.DeprecationReason.ShouldEqual(null);
            }

            [Fact]
            public void NamedAndDescribed()
            {
                var methodInfo = typeof(Foo).GetMethod(nameof(Foo.NamedAndDescribedParameter));
                var parameterInfo = methodInfo.GetParameters().First();
                var entity = Entity.New(parameterInfo);

                entity.Name.ShouldEqual(OverriddenName);
                entity.Description.ShouldEqual(Description);
                entity.IsDeprecated.ShouldEqual(false);
                entity.DeprecationReason.ShouldEqual(null);
            }

            [Fact]
            public void Deprecated()
            {
                var methodInfo = typeof(Foo).GetMethod(nameof(Foo.DeprecatedParameter));
                var parameterInfo = methodInfo.GetParameters().First();
                var entity = Entity.New(parameterInfo);

                entity.Name.ShouldEqual(ParameterName);
                entity.Description.ShouldEqual(null);
                entity.IsDeprecated.ShouldEqual(true);
                entity.DeprecationReason.ShouldEqual(DeprecationReason);
            }

            class Foo
            {
                public void PlainParameter(
                    string parameter)
                { }

                public void NamedParameter(
                    [Name(OverriddenName)] string parameter)
                { }

                public void DescribedParameter(
                    [Description(Description)] string parameter)
                { }

                public void NamedAndDescribedParameter(
                    [Name(OverriddenName), Description(Description)] string parameter)
                { }

                public void DeprecatedParameter(
                    [Deprecated(DeprecationReason)] string parameter)
                { }
            }
        }

        public class Enums
        {
            [Fact]
            public void Plain()
            {
                var memberInfo = typeof(Bar).GetMember(nameof(Bar.PlainEnumMember)).First();
                var entity = Entity.New(memberInfo);

                entity.Name.ShouldEqualEnumValue(nameof(Bar.PlainEnumMember));
                entity.Description.ShouldEqual(null);
                entity.IsDeprecated.ShouldEqual(false);
                entity.DeprecationReason.ShouldEqual(null);
            }

            [Fact]
            public void Named()
            {
                var memberInfo = typeof(Bar).GetMember(nameof(Bar.NamedEnumMember)).First();
                var entity = Entity.New(memberInfo);

                entity.Name.ShouldEqual(OverriddenName);
                entity.Description.ShouldEqual(null);
                entity.IsDeprecated.ShouldEqual(false);
                entity.DeprecationReason.ShouldEqual(null);
            }

            [Fact]
            public void Described()
            {
                var memberInfo = typeof(Bar).GetMember(nameof(Bar.DescribedEnumMember)).First();
                var entity = Entity.New(memberInfo);

                entity.Name.ShouldEqualEnumValue(nameof(Bar.DescribedEnumMember));
                entity.Description.ShouldEqual(Description);
                entity.IsDeprecated.ShouldEqual(false);
                entity.DeprecationReason.ShouldEqual(null);
            }

            [Fact]
            public void Deprecated()
            {
                var memberInfo = typeof(Bar).GetMember(nameof(Bar.DeprecatedEnumMember)).First();
                var entity = Entity.New(memberInfo);

                entity.Name.ShouldEqualEnumValue(nameof(Bar.DeprecatedEnumMember));
                entity.Description.ShouldEqual(null);
                entity.IsDeprecated.ShouldEqual(true);
                entity.DeprecationReason.ShouldEqual(DeprecationReason);
            }

            enum Bar
            {
                PlainEnumMember,

                [Name(OverriddenName)]
                NamedEnumMember,

                [Description(Description)]
                DescribedEnumMember,

                [Deprecated(DeprecationReason)]
                DeprecatedEnumMember,
            }
        }

        public class OrderedInheritace
        {
            [Fact]
            public void Types()
            {
                var typeInfo = typeof(Data.Baz3);
                var entity = Entity.New(typeInfo);

                var indices = Data.TestAttribute.GetIndices(entity);
                indices.Count.ShouldEqual(3 + 3 + 4);

                for (var i = 0; i < indices.Count; i++)
                {
                    indices[i].ShouldEqual(i);
                }
            }

            [Fact]
            public void Fields()
            {
                var propertyInfo = typeof(Data.Baz3).GetProperty(nameof(Data.Baz1.Field));
                var entity = Entity.New(propertyInfo);

                var indices = Data.TestAttribute.GetIndices(entity);
                indices.Count.ShouldEqual(3 + 3 + 4);

                for (var i = 0; i < indices.Count; i++)
                {
                    indices[i].ShouldEqual(i);
                }
            }

            class Data
            {
                [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
                public class TestAttribute : Attribute, IMetaDataAttribute
                {
                    private const string IndicesField = "indicies";

                    public static List<int> GetIndices(Entity entity)
                    {
                        return entity.Get<List<int>>(IndicesField);
                    }

                    private readonly int _expectedIndex;

                    public TestAttribute(int expectedIndex)
                    {
                        _expectedIndex = expectedIndex;
                    }

                    public List<IMetaDataAttribute> AssociatedAttributes { get; } = new List<IMetaDataAttribute>();

                    public int Order { get; set; }

                    public virtual bool ShouldBeApplied(Entity entity)
                    {
                        return true;
                    }

                    public void DeriveMetaData(Entity entity)
                    {
                        var list = entity.Get<List<int>>(IndicesField);
                        if (list == null)
                        {
                            list = new List<int>();
                            entity.Set(IndicesField, list);
                        }
                        list.Add(_expectedIndex);
                    }
                }

                [Test(6)]
                [Test(7)]
                [Test(1, Order = -2)]
                public class Baz1
                {
                    [Test(6)]
                    [Test(7)]
                    [Test(1, Order = -2)]
                    public virtual bool Field { get; set; }
                }


                [Test(4)]
                [Test(9, Order = 1)]
                [Test(5)]
                public class Baz2 : Baz1
                {
                    [Test(4)]
                    [Test(9, Order = 1)]
                    [Test(5)]
                    public override bool Field { get; set; }
                }

                [Test(3)]
                [Test(8, Order = 1)]
                [Test(2, Order = -1)]
                [Test(0, Order = -3)]
                public class Baz3 : Baz2
                {
                    [Test(3)]
                    [Test(8, Order = 1)]
                    [Test(2, Order = -1)]
                    [Test(0, Order = -3)]
                    public override bool Field { get; set; }
                }
            }
        }

        public class Interfaces
        {
            [Fact]
            public void None()
            {
                var typeInfo = typeof(Data.NoInterfaces);
                var entity = Entity.New(typeInfo);
                entity.Interfaces.Count.ShouldEqual(0);
            }

            [Fact]
            public void One()
            {
                var typeInfo = typeof(Data.OneInterface);
                var entity = Entity.New(typeInfo);
                entity.Interfaces.Count.ShouldEqual(1);
                entity.Interfaces.ShouldContain<Data.IOne>();
            }

            [Fact]
            public void Two()
            {
                var typeInfo = typeof(Data.TwoInterfaces);
                var entity = Entity.New(typeInfo);
                entity.Interfaces.Count.ShouldEqual(2);
                entity.Interfaces.ShouldContain<Data.IOne>();
                entity.Interfaces.ShouldContain<Data.ITwo>();
            }

            [Fact]
            public void OnePlusTwo()
            {
                var typeInfo = typeof(Data.OnePlusTwoInterfaces);
                var entity = Entity.New(typeInfo);
                entity.Interfaces.Count.ShouldEqual(3);
                entity.Interfaces.ShouldContain<Data.IOne>();
                entity.Interfaces.ShouldContain<Data.ITwo>();
                entity.Interfaces.ShouldContain<Data.IThree>();
            }

            [Fact]
            public void TwoAnnotated()
            {
                var typeInfo = typeof(Data.TwoInterfacesAnnotated);
                var entity = Entity.New(typeInfo);
                entity.Interfaces.Count.ShouldEqual(2);
                entity.Interfaces.ShouldContain<Data.IOne>();
                entity.Interfaces.ShouldContain<Data.IThree>();
            }

            [Fact]
            public void PossibleTypes1()
            {
                var entity = Entity.New(typeof(Data.IOne));

                entity.Construct(typeof(Data.OneInterface));
                entity.Construct(typeof(Data.TwoInterfaces));
                entity.Construct(typeof(Data.OnePlusTwoInterfaces));
                entity.Construct(typeof(Data.TwoInterfacesAnnotated));

                entity.PossibleTypes.Count.ShouldEqual(4);
                entity.PossibleTypes.ShouldContain<Data.OneInterface>();
                entity.PossibleTypes.ShouldContain<Data.TwoInterfaces>();
                entity.PossibleTypes.ShouldContain<Data.OnePlusTwoInterfaces>();
                entity.PossibleTypes.ShouldContain<Data.TwoInterfacesAnnotated>();
            }

            [Fact]
            public void PossibleTypes2()
            {
                var entity = Entity.New(typeof(Data.ITwo));

                entity.Construct(typeof(Data.OneInterface));
                entity.Construct(typeof(Data.TwoInterfaces));
                entity.Construct(typeof(Data.OnePlusTwoInterfaces));
                entity.Construct(typeof(Data.TwoInterfacesAnnotated));

                entity.PossibleTypes.Count.ShouldEqual(2);
                entity.PossibleTypes.ShouldContain<Data.TwoInterfaces>();
                entity.PossibleTypes.ShouldContain<Data.OnePlusTwoInterfaces>();
            }

            [Fact]
            public void PossibleTypes3()
            {
                var entity = Entity.New(typeof(Data.IThree));

                entity.Construct(typeof(Data.OneInterface));
                entity.Construct(typeof(Data.TwoInterfaces));
                entity.Construct(typeof(Data.OnePlusTwoInterfaces));
                entity.Construct(typeof(Data.TwoInterfacesAnnotated));

                entity.PossibleTypes.Count.ShouldEqual(2);
                entity.PossibleTypes.ShouldContain<Data.OnePlusTwoInterfaces>();
                entity.PossibleTypes.ShouldContain<Data.TwoInterfacesAnnotated>();
            }

            class Data
            {
                public class NoInterfaces { }

                public class OneInterface : IOne { }

                public class TwoInterfaces : IOne, ITwo { }

                public class OnePlusTwoInterfaces : TwoInterfaces, IThree { }

                [Interface(typeof(IOne), typeof(IThree))]
                public class TwoInterfacesAnnotated { }

                public interface IOne { }

                public interface ITwo { }

                public interface IThree { }
            }
        }

        public class Unions
        {
            [Fact]
            public void None()
            {
                var typeInfo = typeof(Data.NoUnionTypes);
                var entity = Entity.New(typeInfo);
                entity.UnionTypes.Count.ShouldEqual(0);
            }

            [Fact]
            public void One()
            {
                var typeInfo = typeof(Data.OneUnionType);
                var entity = Entity.New(typeInfo);
                entity.UnionTypes.Count.ShouldEqual(1);
                entity.UnionTypes.ShouldContain<Data.TypeOne>();
            }

            [Fact]
            public void Two()
            {
                var typeInfo = typeof(Data.TwoUnionTypes);
                var entity = Entity.New(typeInfo);
                entity.UnionTypes.Count.ShouldEqual(2);
                entity.UnionTypes.ShouldContain<Data.TypeOne>();
                entity.UnionTypes.ShouldContain<Data.TypeTwo>();
            }

            [Fact]
            public void OnePlusTwo()
            {
                var typeInfo = typeof(Data.OnePlusTwoUnionTypes);
                var entity = Entity.New(typeInfo);
                entity.UnionTypes.Count.ShouldEqual(3);
                entity.UnionTypes.ShouldContain<Data.TypeOne>();
                entity.UnionTypes.ShouldContain<Data.TypeTwo>();
                entity.UnionTypes.ShouldContain<Data.TypeThree>();
            }

            [Fact]
            public void PossibleTypes1()
            {
                var entity = Entity.New(typeof(Data.TypeOne));
                entity.Construct(typeof(Data.OneUnionType));
                entity.Construct(typeof(Data.TwoUnionTypes));
                entity.Construct(typeof(Data.OnePlusTwoUnionTypes));

                entity.PossibleTypes.Count.ShouldEqual(3);
                entity.PossibleTypes.ShouldContain<Data.OneUnionType>();
                entity.PossibleTypes.ShouldContain<Data.TwoUnionTypes>();
                entity.PossibleTypes.ShouldContain<Data.OnePlusTwoUnionTypes>();
            }

            [Fact]
            public void PossibleTypes2()
            {
                var entity = Entity.New(typeof(Data.TypeTwo));
                entity.Construct(typeof(Data.OneUnionType));
                entity.Construct(typeof(Data.TwoUnionTypes));
                entity.Construct(typeof(Data.OnePlusTwoUnionTypes));

                entity.PossibleTypes.Count.ShouldEqual(2);
                entity.PossibleTypes.ShouldContain<Data.TwoUnionTypes>();
                entity.PossibleTypes.ShouldContain<Data.OnePlusTwoUnionTypes>();
            }

            [Fact]
            public void PossibleTypes3()
            {
                var entity = Entity.New(typeof(Data.TypeThree));
                entity.Construct(typeof(Data.OneUnionType));
                entity.Construct(typeof(Data.TwoUnionTypes));
                entity.Construct(typeof(Data.OnePlusTwoUnionTypes));

                entity.PossibleTypes.Count.ShouldEqual(1);
                entity.PossibleTypes.ShouldContain<Data.OnePlusTwoUnionTypes>();
            }

            class Data
            {
                public class NoUnionTypes { }

                [Union(typeof(TypeOne))]
                public class OneUnionType { }

                [Union(typeof(TypeOne), typeof(TypeTwo))]
                public class TwoUnionTypes { }

                [Union(typeof(TypeThree))]
                public class OnePlusTwoUnionTypes : TwoUnionTypes { }

                public class TypeOne { }

                public class TypeTwo { }

                public class TypeThree { }
            }
        }

        const string OverriddenTypeName = "AnotherName";
        const string ParameterName = "parameter";
        const string OverriddenName = "anotherName";
        const string Description = "This is a description";
        const string DeprecationReason = "This is a deprecation reason";

        #pragma warning restore 0649
    }
}
