using System;
using System.IO;

namespace Http2Client.Utilities;

/// <summary>
/// Helper methods for throwing common exceptions. Keeps validation code clean.
/// </summary>
internal static class ThrowException
{
    /// <summary>
    /// Throw <see cref="ArgumentNullException" /> if value is null.
    /// </summary>
    public static void Null<T>(T value, string paramName = "value")
    {
        if (value is null)
        {
            throw new ArgumentNullException(paramName);
        }
    }

    /// <summary>
    /// Throw <see cref="ArgumentException" /> if string is null or empty.
    /// </summary>
    public static void NullOrEmpty(string value, string paramName = "value")
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("Value cannot be null or empty.", paramName);
        }
    }

    /// <summary>
    /// Throw <see cref="FileNotFoundException" /> if file doesn't exist.
    /// </summary>
    public static void FileNotExists(string filePath, string paramName = "filePath")
    {
        NullOrEmpty(filePath, paramName);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found at path: '{filePath}'", filePath);
        }
    }
}