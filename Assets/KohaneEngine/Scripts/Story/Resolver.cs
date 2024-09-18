using KohaneEngine.Scripts.Structure;

namespace KohaneEngine.Scripts.Story
{
    public abstract class Resolver
    {
        public static Resolver Instance;

        public abstract ResolveResult Resolve(Block block);
    }
}