using FluentValidation.Internal;

namespace DevKit.MediatR;

public sealed class NullMessageFormatter : MessageFormatter
{
    public override string BuildMessage(string messageTemplate)
    {
        if (!PlaceholderValues.TryGetValue("PropertyValue", out var value))
        {
            return base.BuildMessage(messageTemplate);
        }

        if (value is null)
        {
            PlaceholderValues["PropertyValue"] = "null";
        }

        return base.BuildMessage(messageTemplate);
    }
}
