using Reservation.Backend;
using Reservation.Frontend.Resources;

namespace Reservation.Frontend.Pages;

public class ErrorProvider
{
    private const string UnknownError = "UnknownErrorCode";
    public string GetMessage(BusinessException ex)
    {
        var errorMessage = ERRORS.ResourceManager.GetString(UnknownError);
        var message = ERRORS.ResourceManager.GetString(ex.Error);
        if (message is not null)
        {
            errorMessage = message;
        }

        return errorMessage;
    }

    public string GetDefaultMessage()
    {
        return ERRORS.ResourceManager.GetString(UnknownError);
    }
}