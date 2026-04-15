namespace Pos.Providers.GarantiBank;

public enum OperationResultStatus
{
    Success,
    Failed,
    Rejected
}

public class OperationResult<T>
{
    public OperationResultStatus Status { get; private set; }
    public T? Data { get; private set; }
    public string? Message { get; private set; }

    private OperationResult() { }

    public static OperationResult<T> Success(T? data = default) =>
        new() { Status = OperationResultStatus.Success, Data = data };

    public static OperationResult<T> Failed(string? message = null) =>
        new() { Status = OperationResultStatus.Failed, Message = message };

    public static OperationResult<T> Rejected(T? data = default, string? message = null) =>
        new() { Status = OperationResultStatus.Rejected, Data = data, Message = message };
}

