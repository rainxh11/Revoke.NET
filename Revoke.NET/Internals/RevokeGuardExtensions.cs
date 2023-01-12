namespace Revoke.NET.Internals;

using System;

internal static class Ensure
{
    public static class Is
    {
        public static void NotNullOrWhiteSpace(
            string input,
            string paramName = null)
        {
            string errorMessage = "Input parameter cannot be null or whitespace";
            if (!string.IsNullOrWhiteSpace(input))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(paramName))
            {
                errorMessage = errorMessage.Replace("Input", paramName);
            }
                
            throw new ArgumentException(errorMessage, paramName);
        }
    }
}