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
        }
        
        public override ResolveResult Resolve(Block block)
        {
            switch (block.type)
            {
                case "startAsync":
                    _stateManager.AddFlag(KohaneFlag.AsyncResolving);
                    break;
                case "endAsync":
                    _stateManager.RemoveFlag(KohaneFlag.AsyncResolving);
                    break;
                case "wait":
                    _animator.AppendTweenInterval(block.GetArg<float>(0));
                    break;
                case "waitForClick":
                    _stateManager.SwitchState(KohaneState.ResolveEnd);
                    break;
                case "canSkip":
                    var canSkip = block.GetArg<bool>(0);
                    if (canSkip)
                    {
                        _stateManager.AddFlag(KohaneFlag.CannotSkip);
                    }
                    else
                    {
                        _stateManager.RemoveFlag(KohaneFlag.CannotSkip);
                    }
                    break;
            }

            return ResolveResult.SuccessResult();
        }
    }
}