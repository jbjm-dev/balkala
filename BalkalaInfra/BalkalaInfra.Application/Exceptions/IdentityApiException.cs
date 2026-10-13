namespace BalkalaInfra.Application.Exceptions;

public sealed class IdentityApiException(int statusCode, string error, string message, int code) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
    public string Error { get; } = error;
    public int Code { get; } = code;
}
