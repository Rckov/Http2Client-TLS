using System;
using System.IO;

namespace Http2Client.Utilities;

/// <summary>
/// Helper methods for common exception throwing.
/// </summary>
internal static class ThrowException
{
    /// <summary>
    /// Throws <see cref="ArgumentNullException" /> if value is null.
    /// </summary>
    public static void Null<T>(T? value, string paramName = "value")
    {
        if (value is null)
        {
            throw new ArgumentNullException(paramName);
        }
    }

    /// <summary>
    /// Throws <see cref="ArgumentException" /> if string is null or empty.
    /// </summary>
    public static void NullOrEmpty(string? value, string paramName = "value")
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("Value cannot be null or empty.", paramName);
        }
    }

    /// <summary>
    /// Throws <see cref="FileNotFoundException" /> if file doesn't exist.
    /// </summary>
    public static void FileNotExists(string? filePath, string paramName = "filePath")
    {
        NullOrEmpty(filePath, paramName);

        if (File.Exists(filePath))
        {
            return;
        }

        throw new FileNotFoundException($"File not found at path: '{filePath}'", filePath);
    }

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> if the provided string is not a valid absolute URL.
    /// </summary>
    public static void IsUri(string? url, string paramName = "url")
    {
        NullOrEmpty(url, nameof(paramName));

        if (Uri.TryCreate(url, UriKind.Absolute, out _))
        {
            return;
        }

        throw new ArgumentException("The provided string is not a valid URL.", nameof(paramName));
    }
}