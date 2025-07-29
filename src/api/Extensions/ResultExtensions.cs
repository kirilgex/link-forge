using System.Net;

using LinkForge.Domain.Shared;

namespace LinkForge.API.Extensions;

public static class ResultExtensions
{
    public static IResult ToHttpResponse(this Result result)
    {
        return result.Match(
            onSuccess: TypedResults.Ok,
            onFailure: error => error.ToHttpProblemResponse());
    }
    
    public static IResult ToHttpResponse<T>(this Result<T> result)
    {
        return result.Match(
            onSuccess: TypedResults.Ok,
            onFailure: error => error.ToHttpProblemResponse());
    }

    public static IResult ToHttpProblemResponse(this Error error)
    {
        return error.StatusCode switch
        {
            HttpStatusCode.Unauthorized => TypedResults.Unauthorized(),
            HttpStatusCode.NotFound => TypedResults.NotFound(),
            _ => TypedResults.Problem(detail: error.Message, statusCode: (int)error.StatusCode)
        };
    }
}