using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace SharedLibrary;
public sealed class ServiceResult<T>: ServiceResultBase
{
    public T? Data { get; init; }
   

    public static ServiceResult<T> Success(T data)
        => new() { Data = data };

    public static ServiceResult<T> Failure(ProblemDetails error)
        => new() { Error = error };

    public static ServiceResult<T> Failure(string title, string detail, HttpStatusCode status)
        => new()
        {
            Error = new ProblemDetails
            {
                Title = title,
                Detail = detail,
                Status = (int)status
            }
        };
}

public sealed class ServiceResult : ServiceResultBase
{
    public static ServiceResult Success()
        => new();

    public static ServiceResult Failure(string title, string detail, HttpStatusCode status)
        => new()
        {
            Error = CreateProblem(title, detail, status)
        };
}
