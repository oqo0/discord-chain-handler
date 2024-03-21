namespace Discord.Addons.ChainHandler;

public class InteractionResult : IResult
{
    public InteractionCommandError? Error { get; }
    public string ErrorReason { get; }
    public bool IsSuccess { get; }

    public InteractionResult(bool isSuccess, string errorReason, InteractionCommandError? error)
    {
        Error = error;
        ErrorReason = errorReason;
        IsSuccess = isSuccess;
    }

    public static IResult Success =>
        new InteractionResult(true, string.Empty, null);
    
    public static IResult UnhandledException =>
        new InteractionResult(false, "Unhandled Exception", InteractionCommandError.Exception);
}