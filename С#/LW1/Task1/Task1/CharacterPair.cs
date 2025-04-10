namespace Task1;

public class CharacterPair
{
    private char _firstChar;
    private char _secondChar;
    
    /// <summary>
    /// Создает экземпляр класса CharacterPair с пустыми значениями.
    /// </summary>
    public CharacterPair()
    {
        _firstChar = ' ';
        _secondChar = ' ';
    }
    
    /// <summary>
    /// Создает экземпляр класса CharacterPair с указанными значениями.
    /// </summary>
    /// <param name="firstChar">Первый символ.</param>
    /// <param name="secondChar">Второй символ.</param>
    public CharacterPair(char firstChar, char secondChar)
    {
        _firstChar = firstChar;
        _secondChar = secondChar;
    }
    
    /// <summary>
    /// Создает копию экземпляра класса CharacterPair
    /// </summary>
    /// <param name="other">Экземпляр класса CharacterPair</param>
    public CharacterPair(CharacterPair other)
    {
        _firstChar = other._firstChar;
        _secondChar = other._secondChar;
    }
    
    /// <summary>
    /// Первый символ
    /// </summary>
    public char FirstChar 
    {
        get { return _firstChar; }
        set { _firstChar = value; } 
    }

    /// <summary>
    /// Второй символ
    /// </summary>
    public char SecondChar 
    {
        get { return _secondChar; }
        set { _secondChar = value; } 
    }
    
    /// <summary>
    /// Возвращает строковое представление двух символов.
    /// </summary>
    /// <returns>Строка в формате "[X, Y]"</returns>
    public override string ToString()
    {
        return $"[{_firstChar}, {_secondChar}]";
    }
}