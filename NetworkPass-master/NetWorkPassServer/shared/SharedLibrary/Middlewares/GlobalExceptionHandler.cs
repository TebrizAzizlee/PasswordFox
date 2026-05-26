using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SharedLibrary.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SharedLibrary.Middlewares;
public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ProblemDetails problem;
      
        

        switch (exception)
        {
       
            case ValidationException validationEx:
                var errors=validationEx.Errors
                .GroupBy(e=>e.PropertyName)
                .ToDictionary(
                g=>g.Key,
                g=>g.Select(x=>x.ErrorMessage).ToArray());

                problem=new ProblemDetails
                {
                    Title="Validation error",
                    Status=StatusCodes.Status400BadRequest,
                };
                problem.Extensions["errors"]=errors;
                break;
            case UnauthorizedException:
                problem = CreateProblem(httpContext,exception.Message, StatusCodes.Status401Unauthorized);
                break;
            case ForbiddenException:
                problem = CreateProblem(httpContext,exception.Message,StatusCodes.Status403Forbidden);
                break;

            case BadRequestException ex:
                problem = CreateProblem(httpContext, ex.Message, StatusCodes.Status400BadRequest);
                break;

            case NotFoundException ex:
                problem = CreateProblem(httpContext, ex.Message, StatusCodes.Status404NotFound);
                break;

            case RateLimitExceededException ex:
                httpContext.Response.Headers["Retry-After"] = ex.RetryAfterSeconds.ToString();
                problem = CreateProblem(httpContext, ex.Message, 429);
                break;
            case TokenException ex:
                problem = CreateProblem(httpContext, ex.Message ?? "Invalid token", StatusCodes.Status401Unauthorized);
                break;

            default:
                Log.Error(exception, "Unhandled exception occurred");

                problem = new ProblemDetails
                {
                    Title = "Server error",
                    Detail = "Gözlənilməz xəta baş verdi",
                    Status = StatusCodes.Status500InternalServerError
                };
                break;
        }

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = problem.Status ?? 500;

        var response = new
        {
            success = false,
            data = (object?)null,
            error = problem
        };

        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response), cancellationToken);
        return true; // handled
    }

    private static ProblemDetails CreateProblem(HttpContext context, string message, int status)
    {
        return new ProblemDetails
        {
            Title = message,
            Status = status,
            Instance = context.Request.Path,
            Extensions=
            {
                
                    ["traceId"]=context.TraceIdentifier,
                    ["timestamp"]=DateTime.UtcNow
                    
            }
        };
    }
}