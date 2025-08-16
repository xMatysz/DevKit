namespace DevKit.Core.Extensions;

public static class StringExtensions
{
    public static bool IsNullOrWhiteSpace(this string? value) => string.IsNullOrWhiteSpace(value);
    public static bool IsNotNullOrWhiteSpace(this string? value) => !IsNullOrWhiteSpace(value);
}
