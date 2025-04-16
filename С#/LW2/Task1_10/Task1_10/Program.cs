namespace Task1_10;

using static Tasks;

class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Выберете задание (1 - 10): ");
        if (int.TryParse(Console.ReadLine(), out var taskNumber))
        {
            Console.WriteLine();
            switch (taskNumber)
            {
                case 1:
                    Task1();
                    break;
                case 2:
                    Task2();
                    break;
                case 3:
                    Task3();
                    break;
                case 4:
                    Task4();
                    break;
                case 5:
                    Task5();
                    break;
                case 6:
                    Task6();
                    break;
                case 7:
                    Task7();
                    break;
                case 8:
                    Task8();
                    break;
                case 9:
                    Task9();
                    break;
                case 10:
                    Task10();
                    break;
                default:
                    Console.WriteLine("Ошибка: неправильный ввод номера задания. Завершение работы.");
                    break;
            }
        }
        else
            Console.WriteLine("Ошибка: неправильный ввод. Завершение работы.");
    }
}