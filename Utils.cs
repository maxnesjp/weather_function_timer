using System;
using System.Text.RegularExpressions;

public static class Utils
{
    public static string GetErrorMessage(Exception ex) => ex.Message;

    public static bool ValidateEmailFormat(string email)
    {
        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        return emailRegex.IsMatch(email);
    }

    public static bool ValidateString(string str, int maxLength)
    {
        return !string.IsNullOrEmpty(str) && str.Length <= maxLength;
    }
}
