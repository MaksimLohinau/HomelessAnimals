using HomelessAnimals.Shared.Exceptions;
using System.Net;
using FluentValidation;
using HomelessAnimals.Models;

namespace HomelessAnimals.Middleware
{
    public class ExceptionMappingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMappingMiddleware> _logger;

        public ExceptionMappingMiddleware(RequestDelegate next, ILogger<ExceptionMappingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception e)
            {
                string message;
                HttpStatusCode statusCode;

                switch (e)
                {
                    case FailedAuthenticationException:
                        message = "Invalid email address or password";
                        statusCode = HttpStatusCode.BadRequest;
                        break;
                    case ResetPasswordFailedException:
                        message = "Your password reset link is invalid or has expired, please try again";
                        statusCode = HttpStatusCode.BadRequest;
                        break;
                    case NotPermittedException:
                        message = "You are not permitted to perform this operation";
                        statusCode = HttpStatusCode.Forbidden;
                        break;
                    case ValidationException ex:
                        message = ex.Errors.FirstOrDefault()?.ErrorMessage ?? ex.Message;
                        statusCode = HttpStatusCode.BadRequest;
                        break;
                    case AccountAlreadyExistsException ex:
                        message = ex.Message;
                        statusCode = HttpStatusCode.BadRequest;
                        break;
                    case NotFoundException ex:
                        message = ex.Message;
                        statusCode = HttpStatusCode.NotFound;
                        break;
                    case OperationNotPossibleException ex:
                        message = ex.Message;
                        statusCode = HttpStatusCode.BadRequest;
                        break;
                    default:
                        message = "An unexpected error occured";
                        statusCode = HttpStatusCode.InternalServerError;
                        break;
                }

                httpContext.Response.StatusCode = (int)statusCode;

                _logger.LogError(e, "Exception caught");

                await httpContext.Response.WriteAsJsonAsync(
                    new ErrorResponse { ErrorMessage = message });
            }
        }
    }
}
