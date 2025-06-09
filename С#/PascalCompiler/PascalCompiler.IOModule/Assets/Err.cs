namespace PascalCompiler.IOModule.Assets;

public struct Err
{
    public TextPosition errorPosition { get; set; }
    public byte errorCode { get; set; }
    
    public Err(TextPosition errorPosition, byte errorCode)
    {
        this.errorPosition = errorPosition;
        this.errorCode = errorCode;
    }
}