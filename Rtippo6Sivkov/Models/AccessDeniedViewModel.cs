namespace Rtippo6Sivkov.Models;

public class AccessDeniedViewModel
{
    public AccessDeniedViewModel(string message)
    {
        Message = message;
    }

    public string Message { get; set; }
}