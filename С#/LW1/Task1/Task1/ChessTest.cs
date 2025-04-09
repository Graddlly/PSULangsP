namespace Task1;

public static class ChessTest
{
    /// <summary>
    /// Запускает комплексное тестирование классов CharacterPair и ChessCoordinate.
    /// </summary>
    public static void TestChess()
    {
        TestBaseClass();
        TestSecondaryClass();
    }
    
    /// <summary>
    /// Тестирует класс CharacterPair.
    /// </summary>
    private static void TestBaseClass()
    {
        Console.WriteLine("Тестирование базового класса - CharacterPair:");
            
        // Тестирование конструктора по умолчанию
        var defaultPair = new CharacterPair();
        Console.WriteLine($"Конструктор по умолчанию: {defaultPair}");
            
        // Тестирование конструктора с параметрами
        var customPair = new CharacterPair('A', 'B');
        Console.WriteLine($"Конструктор с параметрами: {customPair}");
            
        // Тестирование конструктора копирования
        var copiedPair = new CharacterPair(customPair);
        Console.WriteLine($"Конструктор копирования: {copiedPair}");
        
        customPair.FirstChar = 'X';
        customPair.SecondChar = 'Y';
        Console.WriteLine($"После изменения - оригинал: {customPair}, копия: {copiedPair}");
            
        // Тестирование ToString из базового класса
        Console.WriteLine($"Созданная строка (базового конструктора): {defaultPair}");
        Console.WriteLine($"Созданная строка ('скопированного' конструктора): {copiedPair}");
        Console.WriteLine($"Созданная строка (после изменения): {customPair}");
    }

    /// <summary>
    /// Тестирует класс ChessCoordinate.
    /// </summary>
    private static void TestSecondaryClass()
    {
        Console.WriteLine("\nТестирование дочернего класса - ChessCoordinate:");
            
        // Тестирование конструктора по умолчанию
        var defaultCoord = new ChessCoordinate();
        Console.WriteLine($"Конструктор по умолчанию: {defaultCoord}");
            
        // Тестирование конструктора с параметрами
        var customCoord = new ChessCoordinate('e', '4');
        Console.WriteLine($"Конструктор с параметрами: {customCoord}");
            
        // Тестирование конструктора копирования
        var copiedCoord = new ChessCoordinate(customCoord);
        Console.WriteLine($"Конструктор копирования: {copiedCoord}");
            
        // Тестирование методов SetFile/SetRank
        Console.WriteLine("\nПроверка методов установки координат:");
        customCoord.SetFile('h');
        customCoord.SetRank('8');
        Console.WriteLine($"После изменения: {customCoord}");

        var invalidFile = customCoord.SetFile('z');
        Console.WriteLine($"Попытка установить невалидную вертикаль 'z': {(invalidFile ? "Успешно" : "Ошибка")}");
        Console.WriteLine($"Текущая координата: {customCoord}");
            
        // Тестирование метода IsWhiteSquare
        Console.WriteLine("\nПроверка цвета полей на доске:");
        ChessCoordinate[] testPositions =
        [
            new ('a', '1'),
            new ('a', '2'),
            new ('b', '1'),
            new ('e', '4')
        ];
            
        foreach (var position in testPositions)
            Console.WriteLine($"Клетка {position}");
            
        // Тестирование метода CreateString
        Console.WriteLine($"\nМетод CreateString для координаты: {customCoord}");
    }
}