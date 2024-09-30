using KohaneEngine.Scripts.Framework;
using KohaneEngine.Scripts.Structure;
using KohaneEngine.Scripts.Utils;

namespace KohaneEngine.Scripts.Story.Resolvers
{
    public class EtcResolver : Resolver
    {
        private readonly KohaneStateManager _stateManager;
        private readonly KohaneAnimator _animator;

        public EtcResolver(KohaneStateManager stateManager, KohaneAnimator animator)
        {
            _stateManager = stateManager;
            _animator = animator;
            Functions.Add("startAsync", StartAsync);
            Functions.Add("endAsync", EndAsync);
            Functions.Add("wait", Wait);
            Functions.Add("waitForClick", WaitForClick);
            Functions.Add("canSkip", CanSkip);
        }

        private ResolveResult CanSkip(Block block)
        {
            var canSkip = block.GetArg<bool>(0);
            if (!canSkip)
            {
                _animator.AppendCallback(() =>_stateManager.AddFlag(KohaneFlag.CannotSkip));
            }
            else
            {
                _animator.AppendCallback(() =>_stateManager.RemoveFlag(KohaneFlag.CannotSkip));
            }
            return ResolveResult.SuccessResult();
        }

        private ResolveResult WaitForClick(Block block)
        {
            _stateManager.SwitchState(KohaneState.ResolveEnd);
            return ResolveResult.SuccessResult();
        }

        private ResolveResult Wait(Block block)
        {
            _animator.AppendTweenInterval(block.GetArg<float>(0));
            return ResolveResult.SuccessResult();
        }

        private ResolveResult EndAsync(Block block)
        {
            _stateManager.RemoveFlag(KohaneFlag.AsyncResolving);
            return ResolveResult.SuccessResult();
        }

        private ResolveResult StartAsync(Block block)
        {
            _stateManager.AddFlag(KohaneFlag.AsyncResolving);
            return ResolveResult.SuccessResult();
        }
    }
}