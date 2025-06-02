using PascalCompiler.IOModule;
using PascalCompiler.LexicalAnalyzer.Assets;

namespace PascalCompiler.LexicalAnalyzer;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== ЛЕКСИЧЕСКИЙ АНАЛИЗАТОР PASCAL ===\n");

        const string fileName = "test_program.pas";
        CreateTestFile(fileName);
        
        if (!InputOutput.Initialize(fileName))
        {
            Console.WriteLine("Ошибка инициализации модуля ввода-вывода!");
            return;
        }
        
        var lexer = new LexicalAnalyzer();
        var tokenAnalyzer = new TokenAnalyzer();
        
        lexer.PrintSymbolTable();

        Console.WriteLine("=== РЕЗУЛЬТАТ ЛЕКСИЧЕСКОГО АНАЛИЗА ===");
        var symbolCodes = lexer.AnalyzeProgram(tokenAnalyzer, out var totalTokens);
        
        Console.WriteLine("\nКоды символов:");
        for (var i = 0; i < symbolCodes.Count; i++)
        {
            Console.Write($"{symbolCodes[i]} ");
            if ((i + 1) % 20 == 0)
            {
                Console.WriteLine();
            }
        }
        Console.WriteLine();

        Console.WriteLine($"\n\nВсего обработано токенов: {totalTokens}");
        
#if DEBUG
        tokenAnalyzer.PrintDetailedAnalysis();
#endif
        
        InputOutput.PrintFinalSummary();
        Console.WriteLine("=== КОНЕЦ АНАЛИЗА ===");
        
        if (File.Exists(fileName))
        {
            File.Delete(fileName);
        }
    }

    /// <summary>
    /// Создание тестового файла с программой Pascal
    /// </summary>
    /// <param name="fileName">Имя файла</param>
    private static void CreateTestFile(string fileName)
    {
        const string testProgram = @"program TestProgram;
var 
x, y: integer;
z: real;
name: char;
const 
MAX = 32768;    { Эта константа превысит лимит Int16 }
PI = 3.14159;
msg = 'Hello World;
unclosed = 'This string is not closed
begin
x := 10;
y := 20;
z := x + y * 2.5;

{ Это правильный комментарий в фигурных скобках }
(* Это правильный комментарий в скобках *)

{ Этот комментарий не закрыт

(* А этот комментарий тоже не закрыт

if x < y then
    z := x
else
    z := y;

for x := 1 to MAX do
begin
    y := y + x;
end;

while x > 0 do
begin
    x := x - 1;
end;

repeat
    y := y div 2;
until y <= 0;

case x of
    1: y := 10;
    2..5: y := 20;
    else y := 0;
end;

@ # $ % ~ ` |   { Недопустимые символы }
end.";

        File.WriteAllText(fileName, testProgram);
        Console.WriteLine($"Создан тестовый файл: {fileName}");
    }
}