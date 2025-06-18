using PascalCompiler.IOModule;
using PascalCompiler.LexicalAnalyzer.Assets;

namespace PascalCompiler.SemanticAnalyzer;

public class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("=== PASCAL КОМПИЛЯТОР ===");

        string fileName;
        
        if (args.Length > 0)
        {
            fileName = args[0];
        }
        else
        {
            Console.Write("Введите имя файла для анализа: ");
            fileName = Console.ReadLine() ?? "";
            
            if (string.IsNullOrWhiteSpace(fileName))
            {
                Console.WriteLine("Используется файл по умолчанию: test.pas");
                fileName = "test.pas";
            }
        }
        
        if (File.Exists("test.pas"))
        {
            File.Delete(fileName);
        }
        
        if (!File.Exists(fileName))
        {
            Console.WriteLine($"Ошибка: Файл '{fileName}' не найден!");
            Console.WriteLine("Создаем тестовый файл...");
            CreateTestFile(fileName);
        }
        
        if (!InputOutput.Initialize(fileName))
        {
            Console.WriteLine("Ошибка инициализации. Программа завершена.");
            return;
        }
        
        Console.WriteLine("\nВыберите режим анализа:");
        Console.WriteLine("1. Только лексический анализ");
        Console.WriteLine("2. Лексический + синтаксический анализ"); 
        Console.WriteLine("3. Полный анализ (лексический + синтаксический + семантический)");
        Console.Write("Ваш выбор (1-3): ");
        
        var choice = Console.ReadLine();
        
        Console.WriteLine();
        
        switch (choice)
        {
            case "1":
                RunLexicalAnalysis();
                break;
            case "2":
                RunSyntaxAnalysis();
                break;
            default:
                RunFullAnalysis();
                break;
        }
        
        InputOutput.PrintFinalSummary();
        
        Console.WriteLine("\nНажмите любую клавишу для завершения...");
        Console.ReadKey();
    }

    /// <summary>
    /// Запуск только лексического анализа
    /// </summary>
    private static void RunLexicalAnalysis()
    {
        Console.WriteLine("=== ЛЕКСИЧЕСКИЙ АНАЛИЗ ===");
        
        var lexer = new LexicalAnalyzer.LexicalAnalyzer();
        var tokenAnalyzer = new TokenAnalyzer();
        var symbolCodes = lexer.AnalyzeProgram(tokenAnalyzer, out var tokenCount);
        
        Console.WriteLine($"\nОбработано токенов: {tokenCount}");
        
        tokenAnalyzer.PrintDetailedAnalysis();
        lexer.PrintSymbolTable();
    }

    /// <summary>
    /// Запуск синтаксического анализа
    /// </summary>
    private static void RunSyntaxAnalysis()
    {
        Console.WriteLine("=== СИНТАКСИЧЕСКИЙ АНАЛИЗ ===");
        
        var syntaxAnalyzer = new SyntaxAnalyzer.SyntaxAnalyzer();
        syntaxAnalyzer.AnalyzeProgram();
    }

    /// <summary>
    /// Запуск полного анализа (с семантическим)
    /// </summary>
    private static void RunFullAnalysis()
    {
        Console.WriteLine("=== ПОЛНЫЙ АНАЛИЗ ===");
        
        var enhancedAnalyzer = new EnhancedSyntaxAnalyzer();
        enhancedAnalyzer.AnalyzeProgram();
    }

    /// <summary>
    /// Создание тестового файла для демонстрации
    /// </summary>
    private static void CreateTestFile(string fileName)
    {
        const string testProgram = @"program TestProgram;
var
    x, y: integer;
    z: real;
    arr: array[1..10] of integer;
    ch: char;
    flag: boolean;
begin
    x := 5;
    y := x + 10;
    z := x / 2.5;
    arr[1] := x;
    arr[2] := arr[1] * 2;
    ch := 'A';
    flag := true;
end.";

        try
        {
            File.WriteAllText(fileName, testProgram);
            Console.WriteLine($"Тестовый файл '{fileName}' создан.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка создания файла: {ex.Message}");
        }
    }
}