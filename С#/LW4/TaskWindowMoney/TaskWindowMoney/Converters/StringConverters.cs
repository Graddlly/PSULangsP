using Avalonia.Data.Converters;

namespace TaskWindowMoney.Converters;

public static class StringConverters
{
    public static readonly IValueConverter IsNotNullOrEmpty =
        new FuncValueConverter<string, bool>(s => !string.IsNullOrEmpty(s));
}