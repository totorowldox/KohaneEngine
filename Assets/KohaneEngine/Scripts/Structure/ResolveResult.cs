namespace KohaneEngine.Scripts.Structure
{
    public class ResolveResult
    {
        public bool Success;
        public string Reason;
        public bool RequestEndResolving;

        public static ResolveResult SuccessResult(string reason = "", bool endResolving = false) => new()
        {
            Success = true,
            Reason = reason,
            RequestEndResolving =  endResolving
        };

        public static ResolveResult FailResult(string reason = "", bool endResolving = false) => new()
        {
            Success = false, 
            Reason = reason,
            RequestEndResolving = endResolving
        };
    }
}