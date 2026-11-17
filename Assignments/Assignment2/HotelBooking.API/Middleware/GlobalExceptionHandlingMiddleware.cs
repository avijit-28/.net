namespace HotelBooking.API.Middleware;

using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred during request execution: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Instance = context.Request.Path
        };

        if (exception is SqlException sqlEx)
        {
            switch (sqlEx.Number)
            {
                case 50001: // Concurrency / Double-booking Conflict
                    problemDetails.Status = (int)HttpStatusCode.Conflict;
                    problemDetails.Title = "Conflict / Double-Booking Detected";
                    problemDetails.Detail = sqlEx.Message;
                    break;
                case 50002: // Business Validation Failure
                    problemDetails.Status = (int)HttpStatusCode.BadRequest;
                    problemDetails.Title = "Validation Error";
                    problemDetails.Detail = sqlEx.Message;
                    break;
                case 50003: // Resource Not Found
                    problemDetails.Status = (int)HttpStatusCode.NotFound;
                    problemDetails.Title = "Not Found";
                    problemDetails.Detail = sqlEx.Message;
                    break;
                case 50004: // Already Processed State
                case 50006: // Active Booking Integrity
                case 50008: // Date Policy Violation
                    problemDetails.Status = (int)HttpStatusCode.BadRequest;
                    problemDetails.Title = "Business Rule Violation";
                    problemDetails.Detail = sqlEx.Message;
                    break;
                case 50005: // Unique Constraint Violation
                case 2601:
                case 2627:
                    problemDetails.Status = (int)HttpStatusCode.Conflict;
                    problemDetails.Title = "Unique Constraint Violation";
                    problemDetails.Detail = sqlEx.Message;
                    break;
                default:
                    problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                    problemDetails.Title = "Database Execution Error";
                    problemDetails.Detail = sqlEx.Message;
                    break;
            }
        }
        else if (exception is UnauthorizedAccessException unauthEx)
        {
            problemDetails.Status = (int)HttpStatusCode.Unauthorized;
            problemDetails.Title = "Unauthorized";
            problemDetails.Detail = unauthEx.Message;
        }
        else if (exception is KeyNotFoundException notFoundEx)
        {
            problemDetails.Status = (int)HttpStatusCode.NotFound;
            problemDetails.Title = "Not Found";
            problemDetails.Detail = notFoundEx.Message;
        }
        else if (exception is ArgumentException argEx)
        {
            problemDetails.Status = (int)HttpStatusCode.BadRequest;
            problemDetails.Title = "Bad Request";
            problemDetails.Detail = argEx.Message;
        }
        else
        {
            problemDetails.Status = (int)HttpStatusCode.InternalServerError;
            problemDetails.Title = "Internal Server Error";
            problemDetails.Detail = exception.Message;
        }

        context.Response.StatusCode = problemDetails.Status.Value;
        var json = JsonSerializer.Serialize(problemDetails);
        await context.Response.WriteAsync(json);
    }
}
