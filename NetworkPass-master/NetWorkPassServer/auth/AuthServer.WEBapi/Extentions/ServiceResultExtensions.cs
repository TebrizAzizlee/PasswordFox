using SharedLibrary;
using System.Net;

namespace AuthServer.WEBapi.Extentions;

public static class ServiceResultExtensions
{
   
        public static IResult ToResult<T>(this ServiceResult<T> result)
        {
        if (!result.IsSuccess)
        {
            return Results.Problem(
                title: result.Error!.Title,
                detail: result.Error.Detail,
                statusCode: result.Error.Status,
                extensions: result.Error.Extensions);
              
                
            }

            return Results.Ok(result.Data);
        }


    public static IResult ToResult(this ServiceResult result)
    {
        if (!result.IsSuccess)
        {
            return Results.Problem(
                title: result.Error!.Title,
                detail: result.Error.Detail,
                statusCode: result.Error.Status,
                  extensions:result.Error.Extensions
            );
        }

        return Results.Ok();
    }
}
