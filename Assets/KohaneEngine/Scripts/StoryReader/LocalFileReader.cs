using System.IO;
using KohaneEngine.Scripts.Serializer;
using KohaneEngine.Scripts.Structure;

namespace KohaneEngine.Scripts.StoryReader
{
    public class LocalFileReader : IStoryReader
    {
        private readonly IKohaneRuntimeStructSerializer _serializer;
        
        public LocalFileReader(IKohaneRuntimeStructSerializer serializer)
        {
            _serializer = serializer;
        }

        public KohaneStruct ReadFrom(object src) => _serializer.Deserialize(File.ReadAllText((string)src));
    }
}