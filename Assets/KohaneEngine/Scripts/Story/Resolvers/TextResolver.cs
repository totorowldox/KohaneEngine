using KohaneEngine.Scripts.Structure;

namespace KohaneEngine.Scripts.Story.Resolvers
{
    public class TextResolver : Resolver
    {
        private readonly KohaneUIBinder _binder;

        public TextResolver()
        {
            _binder = KohaneEngine.GetUIBinder();
        }
        
        public override ResolveResult Resolve(Block block)
        {
            _binder.speaker.text = block.name;
            _binder.text.text = block.text;
            return ResolveResult.SuccessResult();
        }
    }
}