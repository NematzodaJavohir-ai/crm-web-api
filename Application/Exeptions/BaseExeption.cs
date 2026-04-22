namespace Application.Exeptions;
using System.Net;

public abstract class BaseException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) 
: Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}
public class NotFoundException(string message = "Resource not found") 
    : BaseException(message, HttpStatusCode.NotFound);

public class ValidationException(string message = "Validation failed") 
    : BaseException(message, HttpStatusCode.UnprocessableEntity);

public class ConflictException(string message = "Entity already exists") 
    : BaseException(message, HttpStatusCode.Conflict);

public class UnauthorizedException(string message = "Unauthorized access") 
    : BaseException(message, HttpStatusCode.Unauthorized);

public class BadRequestException(string message = "Bad request") 
    : BaseException(message, HttpStatusCode.BadRequest);

public class NoChangeException(string message = "No changes were detected") 
    : BaseException(message, HttpStatusCode.BadRequest);

public class UnknownException(string message = "An unknown error occurred") 
    : BaseException(message, HttpStatusCode.InternalServerError);

public record ErrorResponse(int Status, string Message);
