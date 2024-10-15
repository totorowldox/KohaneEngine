using System;
using System.Text;
using DG.Tweening;
using KohaneEngine.Scripts.Framework;
using KohaneEngine.Scripts.Graphic.TypewriterAnimation;
using KohaneEngine.Scripts.Structure;
using KohaneEngine.Scripts.Utils;
using UnityEngine;

namespace KohaneEngine.Scripts.Story.Resolvers
{
    public class TypeWriter
    {
        public string Name;
        public readonly StringBuilder Text = new();
        public bool HasMore;

        public void Clear()
        {
            Name = "";
            Text.Clear();
            HasMore = false;
        }
    }
    
    public class TextResolver : Resolver
    {
        private readonly KohaneBinder _binder;
        private readonly KohaneStateManager _stateManager;
        private readonly KohaneAnimator _animator;
        private readonly TypeWriter _typeWriter;
        private readonly TypewriterAnimation _textAnimator;
        
        public TextResolver(KohaneBinder binder, KohaneStateManager stateManager, KohaneAnimator animator, TypewriterAnimation textAnimator)
        {
            _binder = binder;
            _stateManager = stateManager;
            _animator = animator;
            _textAnimator = textAnimator;
            _typeWriter = new TypeWriter();
            Functions.Add("__text_begin", TextBegin);
            Functions.Add("__text_type", TextType);
            Functions.Add("__text_end", TextEnd);
        }

        private ResolveResult TextBegin(Block block)
        {
            if (_typeWriter?.HasMore ?? false)
            {
                return ResolveResult.SuccessResult();
            }
            
            _typeWriter!.Clear();
            var name = block.GetArg<string>(0) ?? "";
            _typeWriter.Name = name;
            return ResolveResult.SuccessResult();
        }

        private ResolveResult TextEnd(Block block)
        {
            var hasMore = block.GetArg<bool>(0);
            _typeWriter.HasMore = hasMore;
            if (hasMore)
            {
                _typeWriter.Text.AppendLine();
                return ResolveResult.SuccessResult();
            }
            
            TypeAnimation();
            
            _stateManager.SwitchState(KohaneState.ResolveEnd);
            return ResolveResult.SuccessResult();
        }

        private ResolveResult TextType(Block block)
        {
            var text = block.GetArg<string>(0);
            _typeWriter.Text.Append(text);
            return ResolveResult.SuccessResult();
        }

        private void TypeAnimation()
        {
            var phase = 0f;
            var text = _typeWriter.Text.ToString();
            var duration = _textAnimator.GetDuration(text);
            _animator.AppendCallback(() =>
            {
                _textAnimator.InitializeAnimation(_typeWriter.Name, text);
            });
            var anim = DOTween.To(() => phase, (x) =>
            {
                phase = x;
                _textAnimator.UpdateAnimation(phase);
            }, duration, duration).SetEase(Ease.Linear);
            _animator.JoinAnimation(anim);
        }
    }
}