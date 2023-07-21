﻿using Microsoft.AspNetCore.Mvc;

namespace Shipping
{
    public readonly struct ApiSuccessResponse<T>
        where T : class
    {
        public required T Data { get; init; }

        public static ApiSuccessResponse<T> From(AppResponse<T> source) => new()
        {
            Data = source.Data!
        };
    }


    public readonly struct ApiFailedResponse<T>
    where T : class
    {
        public required IEnumerable<string> ErrorMessages { get; init; }

        public static ApiFailedResponse<T> From(AppResponse<T> source) => new()
        {
            ErrorMessages = source.ErrorMessages
        };
    }


    public static class IActionResultFactory
    {
        public static IActionResult From<TController, TResponse>(this TController controller, AppResponse<TResponse> source)
            where TController : ControllerBase
            where TResponse : class
        {
            if (source.Success)
            {
                return controller.Ok(
                    ApiSuccessResponse<TResponse>.From(source));
            }

            return source.FailureReason switch
            {
                FailureReason.Unauthorized =>
                    controller.Unauthorized(
                        ApiFailedResponse<TResponse>.From(source)),

                _ =>
                    controller.BadRequest(
                        ApiFailedResponse<TResponse>.From(source)),
            };
        }
    }

}
