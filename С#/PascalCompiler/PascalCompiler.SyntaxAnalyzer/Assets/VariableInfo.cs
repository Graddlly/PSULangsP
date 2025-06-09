namespace PascalCompiler.SyntaxAnalyzer.Assets;

/// <summary>
/// Информация о переменной
/// </summary>
public struct VariableInfo
{
    public VariableType Type { get; }
    public bool IsArray { get; }

    public VariableInfo(VariableType type, bool isArray)
    {
        Type = type;
        IsArray = isArray;
    }

    public override string ToString()
    {
        return IsArray ? $"array of {Type}" : Type.ToString();
    }
}