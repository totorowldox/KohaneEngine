namespace KohaneEngine.Scripts.Structure
{
    public class ResolveResult
    {
        public bool Success = false;
        public float WaitTime = 0.0f;

        public static ResolveResult SuccessResult(float waitTime = 0) => new()
        {
            Success = true,
            WaitTime = waitTime
        };
    }
}