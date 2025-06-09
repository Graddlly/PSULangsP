using PascalCompiler.IOModule.Assets;

namespace PascalCompiler.IOModule;

public class TestInputOutput
{
    public static void RunTests()
    {
        Console.WriteLine("=== ТЕСТИРОВАНИЕ МОДУЛЯ ВВОДА-ВЫВОДА ===\n");
        
        InputOutput.PrintErrorTable();
        
        CreateTestFile("test_pascal.pas", @"program Test;
var
a, b: integer;
c: real;
begin
a := 10;
b := a + 5;
c := 3.14;
writeln('Hello Pascal!');
end.");
        
        Console.WriteLine("ТЕСТ 1: Чтение корректного Pascal файла");
        Console.WriteLine("========================================");
        TestFile("test_pascal.pas");
        
        CreateTestFile("test_errors.pas", @"program ErrorTest;
var
x: integer
y := 10;
begin
x = y;
writeln(x)
end");

        Console.WriteLine("\nТЕСТ 2: Чтение файла с синтаксическими ошибками");
        Console.WriteLine("===============================================");
        TestFileWithErrors("test_errors.pas");
        TestMultipleErrors();
        
        CreateTestFile("empty.pas", "");
        Console.WriteLine("\nТЕСТ 6: Пустой файл");
        Console.WriteLine("===================");
        TestFile("empty.pas");
        
        CreateTestFile("single_line.pas", "program SingleLine; begin end.");
        Console.WriteLine("\nТЕСТ 7: Файл с одной строкой");
        Console.WriteLine("============================");
        TestFile("single_line.pas");

        Console.WriteLine("\n=== ТЕСТИРОВАНИЕ ЗАВЕРШЕНО ===");
    }

    private static void CreateTestFile(string fileName, string content)
    {
        try
        {
            File.WriteAllText(fileName, content);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка создания файла {fileName}: {ex.Message}");
        }
    }

    private static void TestFile(string fileName)
    {
        if (InputOutput.Initialize(fileName))
        {
            var charCount = 0;
            while (!InputOutput.IsEndOfFile() && charCount < 1000)
            {
                Console.Write($"'{InputOutput.Ch}'");
                if (charCount % 20 == 19) Console.WriteLine();
                InputOutput.NextCh();
                charCount++;
            }
            Console.WriteLine($"\nОбработано символов: {charCount}");
        }
    }

    private static void TestFileWithErrors(string fileName)
    {
        Console.WriteLine("Тестирование файла с преднамеренными ошибками");
        Console.WriteLine("(имитация работы будущего анализатора)\n");
        
        if (InputOutput.Initialize(fileName))
        {
            var charCount = 0;

            Console.WriteLine("Добавляем преднамеренные ошибки для демонстрации:");
            
            InputOutput.Error(2, new TextPosition(3, 13));
            Console.WriteLine("- Добавлена ошибка: отсутствие ';' после 'integer' в строке 3");
            
            InputOutput.Error(13, new TextPosition(4, 6));
            Console.WriteLine("- Добавлена ошибка: неправильное объявление 'y := 10' в строке 4");
            
            InputOutput.Error(4, new TextPosition(6, 6));
            Console.WriteLine("- Добавлена ошибка: использование '=' вместо ':=' в строке 6");
            
            InputOutput.Error(2, new TextPosition(7, 13));
            Console.WriteLine("- Добавлена ошибка: отсутствие ';' после 'writeln(x)' в строке 7");
            
            InputOutput.Error(12, new TextPosition(8, 3));
            Console.WriteLine("- Добавлена ошибка: отсутствие '.' после 'end' в строке 8");
            
            Console.WriteLine("\nНачинаем чтение файла...\n");
            
            while (!InputOutput.IsEndOfFile() && charCount < 1000)
            {
                InputOutput.NextCh();
                charCount++;
            }
            Console.WriteLine($"Обработано символов: {charCount}");
        }
    }
    
    private static void TestMultipleErrors()
    {
        Console.WriteLine("\nТЕСТ 5: Файл с множественными ошибками");
        Console.WriteLine("======================================");
        
        CreateTestFile("multiple_errors.pas", @"program BadCode;
var
a b: integer;
c = 5;
123name: real;
begin
a == b;
c := 'unclosed string;
if x then
    writeln()
else
    write(
end");

        if (InputOutput.Initialize("multiple_errors.pas"))
        {
            Console.WriteLine("Добавляем множественные ошибки для демонстрации:");
            
            InputOutput.Error(2, new TextPosition(3, 5));
            InputOutput.Error(4, new TextPosition(4, 6));
            InputOutput.Error(8, new TextPosition(5, 4));
            InputOutput.Error(1, new TextPosition(7, 7));
            InputOutput.Error(7, new TextPosition(8, 21));
            InputOutput.Error(15, new TextPosition(9, 7));
            InputOutput.Error(9, new TextPosition(12, 14));
            
            Console.WriteLine("- Ошибка в строке 3: пропущена запятая между 'a' и 'b'");
            Console.WriteLine("- Ошибка в строке 4: использование '=' вместо ':=' в объявлении константы");
            Console.WriteLine("- Ошибка в строке 5: идентификатор '123name' начинается с цифры");
            Console.WriteLine("- Ошибка в строке 7: двойной знак равенства '=='");
            Console.WriteLine("- Ошибка в строке 8: незакрытая строка");
            Console.WriteLine("- Ошибка в строке 9: необъявленная переменная 'x'");
            Console.WriteLine("- Ошибка в строке 12: незакрытая скобка в 'write('");
            
            Console.WriteLine("\nНачинаем чтение файла...\n");
            
            var charCount = 0;
            while (!InputOutput.IsEndOfFile() && charCount < 1000)
            {
                InputOutput.NextCh();
                charCount++;
            }
            Console.WriteLine($"Обработано символов: {charCount}");
        }
    }
}