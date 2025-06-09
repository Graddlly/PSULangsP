namespace PascalCompiler.SyntaxAnalyzer.Assets;

/// <summary>
/// Исключение синтаксического анализа
/// </summary>
public class SyntaxException : Exception
{
    public SyntaxException(string message) : base(message) { }
}