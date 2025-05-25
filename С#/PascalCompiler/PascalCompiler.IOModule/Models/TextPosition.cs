namespace PascalCompiler.IOModule.Models;

public struct TextPosition
{
    public TextPosition(uint ln = 0, byte c = 0)
    {
        lineNumber = ln;
        charNumber = c;
    }

    public uint lineNumber { get; set; }
    public byte charNumber { get; set; }

    public override string ToString()
    {
        return $"[{lineNumber}:{charNumber}]";
    }
}