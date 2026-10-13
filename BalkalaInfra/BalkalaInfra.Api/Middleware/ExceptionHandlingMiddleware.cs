using System.Text.Json;
using BalkalaInfra.Application.DTOs.Response;
using BalkalaInfra.Application.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace BalkalaInfra.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An unhandled exception occurred while processing {Method} {Path}.",
                context.Request.Method, context.Request.Path);

            var (statusCode, response) = exception switch
            {
                IdentityApiException identityException => (identityException.StatusCode,
                    new ApiErrorResponse(false, identityException.Error, identityException.Message, identityException.Code)),
                EmployeeCodeConflictException => (StatusCodes.Status409Conflict,
                    new ApiErrorResponse(false, "Conflict", exception.Message, 40901)),
                KeyNotFoundException => (StatusCodes.Status404NotFound,
                    new ApiErrorResponse(false, "Not Found", "The requested resource was not found.", 40404)),
                ValidationException => (StatusCodes.Status400BadRequest,
                    new ApiErrorResponse(false, "Bad Request", exception.Message, 40012)),
                ArgumentException => (StatusCodes.Status400BadRequest,
                    new ApiErrorResponse(false, "Bad Request", exception.Message, 40012)),
                BadHttpRequestException => (StatusCodes.Status400BadRequest,
                    new ApiErrorResponse(false, "Bad Request", "The request could not be processed.", 40012)),
                _ => (StatusCodes.Status500InternalServerError,
                    new ApiErrorResponse(false, "Internal Server Error", "An unexpected error occurred.", 500))
            };

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await JsonSerializer.SerializeAsync(context.Response.Body, response, JsonOptions,
                context.RequestAborted);
        }
    }
}
