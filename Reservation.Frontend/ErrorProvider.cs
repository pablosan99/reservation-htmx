using Reservation.Backend;
using Reservation.Frontend.Resources;

namespace Reservation.Frontend.Pages;

public class ErrorProvider
{
    private readonly ILogger<ErrorProvider> _logger;
    private const string UnknownError = "UnknownErrorCode";

    public ErrorProvider(ILogger<ErrorProvider> logger)
    {
        _logger = logger;
    }
    
    public string GetMessage(BusinessException ex)
    {
        var errorMessage = ERRORS.ResourceManager.GetString(UnknownError);
        var message = ERRORS.ResourceManager.GetString(ex.Error);
        if (message is not null)
        {
            errorMessage = message;
        }
        _logger.LogError("Business exception {Code} / {Message}", ex.Error, errorMessage);
        return errorMessage;
    }

    public string GetDefaultMessage()
    {
        return ERRORS.ResourceManager.GetString(UnknownError);
    }
}