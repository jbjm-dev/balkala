namespace BalkalaInfra.Application.DTOs.Response;

public sealed record ApiErrorResponse(
    bool Success,
    string Error,
    string Message,
    int Code);
