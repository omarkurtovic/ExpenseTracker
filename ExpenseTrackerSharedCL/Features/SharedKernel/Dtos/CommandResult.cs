namespace ExpenseTrackerSharedCL.Features.SharedKernel.Dtos
{
    public class CommandResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
    }

    public class CommandResult<T> : CommandResult
    {
        public T Data { get; set; }
    }
}
