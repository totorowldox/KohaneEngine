using System.Text;
using KohaneEngine.Scripts.Framework;
using KohaneEngine.Scripts.Structure;
using KohaneEngine.Scripts.Utils;

namespace KohaneEngine.Scripts.Story.Resolvers
{
    public class TypeWriter
    {
        public string Name;
        public StringBuilder Text = new();
        public bool HasMore = false;
    }
    
    public class TextResolver : Resolver
    {
        private readonly KohaneBinder _binder;
        private readonly KohaneStateManager _stateManager;
        private TypeWriter _typeWriter;
        
        public TextResolver(KohaneBinder binder, KohaneStateManager stateManager)
        {
            _binder = binder;
            _stateManager = stateManager;
        }
        
        public override ResolveResult Resolve(Block block)
        {
            switch (block.type)
            {
                case "__text_begin":
                    TextBegin(block);
                    break;
                case "__text_type":
                    TextType(block);
                    break;
                case "__text_end":
                    TextEnd(block);
                    break;
            }
            
            return ResolveResult.SuccessResult();
        }

        private void TextBegin(Block block)
        {
            if (_typeWriter?.HasMore ?? false)
            {
                return;
            }
            
            _typeWriter = new TypeWriter();
            var name = block.GetArg<string>(0) ?? "";
            _typeWriter.Name = name;
        }

        private void TextEnd(Block block)
        {
            var hasMore = block.GetArg<bool>(0);
            _typeWriter.HasMore = hasMore;
            if (hasMore)
            {
                _typeWriter.Text.AppendLine();
                return;
            }
            
            _binder.speaker.text = _typeWriter.Name;
            _binder.text.text = _typeWriter.Text.ToString();
            
            _stateManager.SwitchState(KohaneState.WaitingForClick);
        }

        private void TextType(Block block)
        {
            var text = block.GetArg<string>(0);
            _typeWriter.Text.Append(text);
        }
    }
}