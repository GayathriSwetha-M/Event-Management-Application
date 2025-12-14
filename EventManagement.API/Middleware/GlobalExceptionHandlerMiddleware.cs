using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EventManagement.API.Models;

namespace EventManagement.API.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
                _logger.LogError(ex, "An unhandled exception occurred. Message: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var apiResponse = new ApiResponse<object>
            {
                Success = false,
                Errors = new List<string>()
            };

            switch (exception)
            {
                case UnauthorizedAccessException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    apiResponse.Message = "Unauthorized access";
                    apiResponse.Errors.Add(exception.Message);
                    break;

                case KeyNotFoundException:
                case ArgumentNullException when exception.Message.Contains("not found"):
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    apiResponse.Message = "Resource not found";
                    apiResponse.Errors.Add(exception.Message);
                    break;

                case ArgumentException:
                case InvalidOperationException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse.Message = "Invalid request";
                    apiResponse.Errors.Add(exception.Message);
                    break;

                case DbUpdateException dbEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse.Message = "Database operation failed";
                    
                    // Handle unique constraint violations
                    if (dbEx.InnerException?.Message.Contains("duplicate") == true ||
                        dbEx.InnerException?.Message.Contains("unique") == true)
                    {
                        apiResponse.Message = "Duplicate entry detected";
                    }
                    
                    apiResponse.Errors.Add("A database error occurred. Please try again.");
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    apiResponse.Message = "An unexpected error occurred";
                    
                    // Only show detailed error in development
                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                    {
                        apiResponse.Errors.Add(exception.Message);
                        if (exception.StackTrace != null)
                        {
                            apiResponse.Errors.Add($"Stack Trace: {exception.StackTrace}");
                        }
                    }
                    else
                    {
                        apiResponse.Errors.Add("Please contact support if the problem persists.");
                    }
                    break;
            }

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var jsonResponse = JsonSerializer.Serialize(apiResponse, jsonOptions);
            return response.WriteAsync(jsonResponse);
        }
    }
}

