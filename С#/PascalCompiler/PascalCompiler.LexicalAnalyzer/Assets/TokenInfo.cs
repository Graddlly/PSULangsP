using PascalCompiler.IOModule.Assets;

namespace PascalCompiler.LexicalAnalyzer.Assets;

/// <summary>
/// Структура для хранения информации о токене
/// </summary>
public struct TokenInfo
{
    public byte Code { get; }
    public string Value { get; }
    public TextPosition Position { get; }

    public TokenInfo(byte code, string value, TextPosition position)
    {
        Code = code;
        Value = value ?? "";
        Position = position;
    }
}