namespace GraphQL.Conventions.Types.Relay
{
    public interface IRelayMutationObject<TObject>
        where TObject : class
    {
        Id<TObject> ClientMutationId { get; set; }
    }

    static class RelayMutationObject
    {
        public static bool Input;
    }
}
