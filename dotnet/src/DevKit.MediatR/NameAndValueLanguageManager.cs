using System.Globalization;
using FluentValidation.Resources;

namespace DevKit.MediatR;

public sealed class NameAndValueLanguageManager : LanguageManager
{
    public override string GetString(string key, CultureInfo? culture = null)
    {
        var original = base.GetString(key, culture);

        if (string.IsNullOrWhiteSpace(original) || original.Contains("{PropertyValue}", StringComparison.Ordinal))
        {
            return original;
        }

        return original.Replace("{PropertyName}", "{PropertyName} ({PropertyValue})", StringComparison.Ordinal);
    }
}
