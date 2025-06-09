using PascalCompiler.IOModule;

namespace PascalCompiler.SyntaxAnalyzer;

public class SyntaxProgram
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== СИНТАКСИЧЕСКИЙ АНАЛИЗАТОР PASCAL ===\n");
        
        const string fileName = "syntax_test_program.pas";
        CreateSyntaxTestFile(fileName);
        
        if (!InputOutput.Initialize(fileName))
        {
            Console.WriteLine("Ошибка инициализации модуля ввода-вывода!");
            return;
        }

        InputOutput.PrintErrorTable();
        
        var syntaxAnalyzer = new SyntaxAnalyzer();
        Console.WriteLine("=== НАЧАЛО СИНТАКСИЧЕСКОГО АНАЛИЗА ===");
        
        syntaxAnalyzer.AnalyzeProgram();
        InputOutput.PrintFinalSummary();
        
        Console.WriteLine("=== КОНЕЦ СИНТАКСИЧЕСКОГО АНАЛИЗА ===");
    }

    /// <summary>
    /// Создание тестового файла для синтаксического анализа
    /// </summary>
    /// <param name="fileName">Имя файла</param>
    private static void CreateSyntaxTestFile(string fileName)
    {
        const string testProgram = @"program SyntaxTest;
var 
x, y, z: integer;
arr1: array[1..10 of integer;
arr2: array[0..5 of real;
ch: char;
temperature: real;

{ Ошибки для тестирования }
duplicateVar: integer;
duplicateVar: real;     { Повторное объявление }

begin
{ Простые присваивания }
x := 10;
y := 20;
z := x + y;
ch := 'A';
temperature := 36.6;

{ Присваивания массивам }
arr1[1] := 100;
arr1[x] := y + z;
arr2[0] := 3.14159;
arr2[y] := temperature * 1.5;

{ Ошибки для тестирования }
undeclaredVar := 42;           { Необъявленная переменная }
x[1] := 10;                    { x не массив }
arr1 := 50                     { Пропущен оператор присваивания }
arr2[x := y;                   { Пропущена закрывающая скобка }

{ Составные операторы }
begin
    x := x + 1;
    y := y * 2;
    arr1[5] := arr1[3] + arr1[4];
end;

{ Вложенные составные операторы }
begin
    z := 0;
    begin
        x := 1;
        y := 2;
    end;
    z := x + y;
end;

end.

{ Дополнительные конструкции после end }
extra := 123;";

        File.WriteAllText(fileName, testProgram);
        Console.WriteLine($"Создан тестовый файл: {fileName}");
    }
}