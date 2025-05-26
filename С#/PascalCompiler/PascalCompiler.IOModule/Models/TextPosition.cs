namespace PascalCompiler.IOModule.Models;

public struct TextPosition
{
    public uint lineNumber { get; set; }
    public byte charNumber { get; set; }
    
    public TextPosition(uint ln = 0, byte c = 0)
    {
        lineNumber = ln;
        charNumber = c;
    }

    public override string ToString()
    {
        return $"[{lineNumber}:{charNumber}]";
    }
}