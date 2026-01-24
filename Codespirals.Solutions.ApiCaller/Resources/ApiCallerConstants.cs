namespace Codespirals.Solutions.ApiCaller;

/// <summary>
/// Constants for the ApiCaller
/// </summary>
public static class ApiCallerConstants
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public const string ErrorCodeApiCallFailed = "APICALLER_ERROR_400";
    public const string ErrorCodeConversionError = "APICALLER_ERROR_418";
    public const string ErrorCodeNoContent = "APICALLER_ERROR_500";
    public const string ErrorCodeNotFound = "APICALLER_ERROR_404";

    public const string NewItem = "new";
    public const string EditItem = "edit";
    public const string Search = "search";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
