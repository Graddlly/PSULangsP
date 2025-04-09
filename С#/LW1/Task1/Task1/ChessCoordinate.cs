namespace Task1;

/// <summary>
/// Использует символьные поля для представления координаты на шахматной доске.
/// Первый символ - буква (a-h), обозначающая вертикаль.
/// Второй символ - цифра (1-8), обозначающая горизонталь.
/// </summary>
public class ChessCoordinate : CharacterPair
{
    /// <summary>
    /// Создает экземпляр класса ChessCoordinate со значением координат а1.
    /// </summary>
    public ChessCoordinate() : base('a', '1') { }
    
    /// <summary>
    /// Создает экземпляр класса ChessCoordinate с указанными значениями.
    /// </summary>
    /// <param name="file">Координата [a-h]</param>
    /// <param name="rank">Координата [1-8]</param>
    public ChessCoordinate(char file, char rank)
    {
        SetFile(file);
        SetRank(rank);
    }
    
    /// <summary>
    /// Создает копию экземпляра класса ChessCoordinate.
    /// </summary>
    public ChessCoordinate(ChessCoordinate other) : base(other) { }
    
    /// <summary>
    /// Метод для установки символа вертикали [a-h]
    /// </summary>
    public bool SetFile(char file)
    {
        if (file >= 'a' && file <= 'h')
        {
            FirstChar = file;
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Метод для установки символа горизонтали [1-8]
    /// </summary>
    public bool SetRank(char rank)
    {
        if (rank >= '1' && rank <= '8')
        {
            SecondChar = rank;
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Метод для проверки, находится ли координата на белом поле или на черном
    /// </summary>
    private bool IsWhiteSquare()
    {
        var file = FirstChar - 'a' + 1; // При FC = 'a': 'a' - 'a' + 1 = 0 + 1 = 1
        var rank = SecondChar - '0';    // При SC = '1': '1' - '0' = 49 - 48 = 1
            
        // Если сумма координат четная, то поле черное, иначе - белое
        return (file + rank) % 2 != 0;
    }
    
    /// <summary>
    /// Возвращает строковое представление нахождения на поле (белый\черный).
    /// </summary>
    /// <returns>Строка в формате "XY находится на [белом\черном] поле"</returns>
    public override string ToString()
    {
        var squareColor = IsWhiteSquare() ? "белом" : "черном";
        return $"{FirstChar}{SecondChar} находится на {squareColor} поле";
    }
}