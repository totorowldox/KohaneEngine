namespace KohaneEngine.Scripts.Framework.States
{
    public class PausedState : KohaneState
    {
        public PausedState(KohaneStateManager stateManager): base(stateManager){}
        
        public override  void OnEnter() {}
        public override  void OnUpdate() {}
        public override  void OnExit() {}
    }
}