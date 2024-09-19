namespace KohaneEngine.Scripts.Structure
{
    public class ResolveResult
    {
        public bool Success = false;
        public string Reason;

        public static ResolveResult SuccessResult(string reason = "") => new()
        {
            Success = true,
            Reason = reason
        };

        public static ResolveResult FailResult(string reason = "") => new()
        {
            Success = false, 
            Reason = reason
        };
    }
}