using SharedLibrary;

namespace NetWorkPassServer.WEBAPI.Extensions;

public static class ResultExtensions
{
    public static IResult ToResult<T>(this ServiceResult<T> result)
    {
        if (!result.IsSuccess)
        {
            return Results.Problem(
                title: result.Error!.Title,
                detail: result.Error.Detail,
                statusCode: result.Error.Status);
        }

        return Results.Ok(result.Data);
    }

    public static IResult ToCreatedResult<T>(this ServiceResult<T> result, string uri)
    {
        if (!result.IsSuccess)
        {
            return Results.Problem(
                title: result.Error!.Title,
                detail: result.Error.Detail,
                statusCode: result.Error.Status);
        }

        return Results.Created(uri, result.Data);
    }

    public static IResult ToNoContentResult(this ServiceResult result)
    {
        if (!result.IsSuccess)
        {
            return Results.Problem(
                title: result.Error!.Title,
                detail: result.Error.Detail,
                statusCode: result.Error.Status);
        }

        return Results.NoContent();
    }
}

