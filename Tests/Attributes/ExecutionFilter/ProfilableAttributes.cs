using GraphQL.Conventions.Attributes.Data;
using GraphQL.Conventions.Attributes.ExecutionFilter;
using Should;
using Xunit;

namespace Tests.Attributes.ExecutionFilter
{
    public class ProfilableAttributes
    {
        #pragma warning disable 0649

        public class Fields
        {
            [Fact]
            public void Undecorated()
            {
                var entity = Entity.New(typeof(Foo).GetField(nameof(Foo.UndecoratedField)));
                entity.IsProfilable.ShouldBeFalse();
            }

            [Fact]
            public void Decorated()
            {
                var entity = Entity.New(typeof(Foo).GetField(nameof(Foo.DecoratedField)));
                entity.IsProfilable.ShouldBeFalse();
            }

            class Foo
            {
                public string UndecoratedField;

                [Profilable]
                public string DecoratedField;
            }
        }

        public class Properties
        {
            [Fact]
            public void Undecorated()
            {
                var entity = Entity.New(typeof(Foo).GetProperty(nameof(Foo.UndecoratedProperty)));
                entity.IsProfilable.ShouldBeFalse();
            }

            [Fact]
            public void Decorated()
            {
                var entity = Entity.New(typeof(Foo).GetProperty(nameof(Foo.DecoratedProperty)));
                entity.IsProfilable.ShouldBeFalse();
            }

            class Foo
            {
                public string UndecoratedProperty { get; }

                [Profilable]
                public string DecoratedProperty { get; }
            }
        }

        public class Methods
        {
            [Fact]
            public void Undecorated()
            {
                var entity = Entity.New(typeof(Foo).GetMethod(nameof(Foo.UndecoratedMethod)));
                entity.IsProfilable.ShouldBeTrue();
            }

            [Fact]
            public void Decorated()
            {
                var entity = Entity.New(typeof(Foo).GetMethod(nameof(Foo.DecoratedMethod)));
                entity.IsProfilable.ShouldBeFalse();
            }

            class Foo
            {
                public string UndecoratedMethod() { return string.Empty; }

                [Profilable(false)]
                public string DecoratedMethod() { return string.Empty; }
            }
        }

        #pragma warning restore 0649
    }
}
