using Cysharp.Threading.Tasks;

namespace KohaneEngine.Scripts.Framework.States
{
    public class ReadyState : KohaneState
    {
        public ReadyState(KohaneStateManager stateManager) : base(stateManager)
        {
        }

        public override void OnEnter()
        {
            // Check auto skip
            switch (StateManager.CurrentPlayback)
            {
                case PlaybackMode.Skip:
                    UniTask.Delay((int) (Constants.SkipSpeed * 1000))
                        .ContinueWith(() =>
                        {
                            if (StateManager.CurrentPlayback == PlaybackMode.Skip)
                                RequestNextStep();
                        });
                    return;

                case PlaybackMode.Auto:
                    UniTask.Delay((int) (Constants.AutoSpeed * 1000))
                        .ContinueWith(() =>
                        {
                            if (StateManager.CurrentPlayback == PlaybackMode.Auto)
                                RequestNextStep();
                        });
                    break;
            }
        }

        public override void OnExit()
        {
        }
    }
}