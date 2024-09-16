namespace KohaneEngine.Scripts.Serializer
{
    /// <summary>
    /// Serializer & Deserializer interface
    /// </summary>
    public interface ISerializer<TSerialized, TDeserialized>
    {
        public TSerialized Serialize(TDeserialized obj);

        public TDeserialized Deserialize(TSerialized obj);
    }
}