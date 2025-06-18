namespace PascalCompiler.SemanticAnalyzer.Assets;

/// <summary>
/// Класс для представления семантической ошибки
/// </summary>
public class SemanticError
{
    public byte ErrorCode { get; }
    public string Message { get; }
    public DateTime Timestamp { get; }

    public SemanticError(byte errorCode, string message)
    {
        ErrorCode = errorCode;
        Message = message;
        Timestamp = DateTime.Now;
    }

    public override string ToString()
    {
        return $"Сематическая ошибка {ErrorCode}: {Message}";
    }
}