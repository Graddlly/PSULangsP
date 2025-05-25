namespace PascalCompiler.IOModule;

public class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("МОДУЛЬ ВВОДА-ВЫВОДА ДЛЯ УЧЕБНОГО КОМПИЛЯТОРА PASCAL");
        Console.WriteLine("==================================================\n");
        
        TestInputOutput.RunTests();

        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ReadKey();
    }
}