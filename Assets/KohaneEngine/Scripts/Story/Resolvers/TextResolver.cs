using System.Text;
using DG.Tweening;
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
        private readonly KohaneAnimator _animator;
        private TypeWriter _typeWriter;
        
        public TextResolver(KohaneBinder binder, KohaneStateManager stateManager, KohaneAnimator animator)
        {
            _binder = binder;
            _stateManager = stateManager;
            _animator = animator;
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
            
            TypeAnimation();
            
            _stateManager.SwitchState(KohaneState.ResolveEnd);
        }

        private void TextType(Block block)
        {
            var text = block.GetArg<string>(0);
            _typeWriter.Text.Append(text);
        }

        private void TypeAnimation()
        {
            _binder.speaker.text = _typeWriter.Name;
            
            var text = _typeWriter.Text.ToString();
            var length = text.Length;
            var duration = length * Constants.TypeAnimationSpeed;
            var showingCharCount = 0;

            var anim = DOTween.To(() => showingCharCount, (x) =>
            {
                showingCharCount = x;
                _binder.text.text = text[..x];
            }, length, duration).SetEase(Ease.Linear);
            _animator.AppendTweenAnimation(anim);
        }
    }
}