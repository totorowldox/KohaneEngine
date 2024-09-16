using KohaneEngine.Scripts.Structure;

namespace KohaneEngine.Scripts.StoryReader
{
    public interface IStoryReader
    {
        public KohaneStruct ReadFrom(object src);
    }
}