using Http2Client.Core.Enums;

using System;
using System.ComponentModel;

namespace Http2Client.Extensions;

/// <summary>
/// Extension methods for <see cref="BrowserType"/> enum. Handles string conversion.
/// </summary>
public static class BrowserTypeExtension
{
    /// <summary>
    /// Get the string value from the description attribute.
    /// </summary>
    public static string GetValue(this BrowserType identifier)
    {
        var memberInfo = typeof(BrowserType).GetMember(identifier.ToString())[0];
        var attribute = memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false)[0] as DescriptionAttribute;
        return attribute?.Description ?? identifier.ToString().ToLowerInvariant();
    }

    /// <summary>
    /// Parse string value back to enum. Handles both enum names and description values.
    /// </summary>
    public static BrowserType FromString(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("Value cannot be null or empty", nameof(value));
        }

        if (Enum.TryParse<BrowserType>(value.Replace("_", "").Replace("-", ""), true, out var result))
        {
            // Verify the description matches
            if (result.GetValue().Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                return result;
            }
        }

        // Fallback: search by description (rare case)
        foreach (BrowserType identifier in (BrowserType[])Enum.GetValues(typeof(BrowserType)))
        {
            if (identifier.GetValue().Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                return identifier;
            }
        }

        throw new ArgumentException($"Unknown browser type: {value}", nameof(value));
    }

    /// <summary>
    /// Check if a string is a valid browser type.
    /// </summary>
    public static bool IsValid(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        try
        {
            FromString(value);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}