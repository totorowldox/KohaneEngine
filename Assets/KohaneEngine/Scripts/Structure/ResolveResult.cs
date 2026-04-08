namespace KohaneEngine.Scripts.Structure
{
    public class ResolveResult
    {
        public ResultType Type;
        public string Reason;

        public static ResolveResult SuccessResult(string reason = "") => new()
        {
            Type = ResultType.Success,
            Reason = reason,
        };

        public static ResolveResult FailResult(string reason = "") => new()
        {
            Type = ResultType.Failure,
            Reason = reason,
        };

        public static ResolveResult EndResolvingResult(string reason = "") => new()
        {
            Type = ResultType.EndResolving,
            Reason = reason,
        };

        public static ResolveResult ChoiceResult(string reason = "") => new()
        {
            Type = ResultType.Choice,
            Reason = reason,
        };
    }

    public enum ResultType
    {
        Success,
        Failure,
        EndResolving,
        Choice
    }
}