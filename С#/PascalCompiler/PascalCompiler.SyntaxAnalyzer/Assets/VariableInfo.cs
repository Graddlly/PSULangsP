namespace PascalCompiler.SyntaxAnalyzer.Assets;

/// <summary>
/// Информация о переменной
/// </summary>
public struct VariableInfo
{
    public VariableType Type { get; }
    public bool IsArray { get; }
    public bool IsInitialized { get; set; }

    public VariableInfo(VariableType type, bool isArray)
    {
        Type = type;
        IsArray = isArray;
        IsInitialized = false;
    }

    public override string ToString()
    {
        var arrayStr = IsArray ? "array of " : "";
        var initStr = IsInitialized ? " (initialized)" : " (not initialized)";
        return $"{arrayStr}{Type}{initStr}";
    }
}