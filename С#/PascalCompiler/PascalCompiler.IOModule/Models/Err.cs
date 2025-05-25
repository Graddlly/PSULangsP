namespace PascalCompiler.IOModule.Models;

public struct Err
{
    public Err(TextPosition errorPosition, byte errorCode)
    {
        this.errorPosition = errorPosition;
        this.errorCode = errorCode;
    }

    public TextPosition errorPosition { get; set; }
    public byte errorCode { get; set; }
}