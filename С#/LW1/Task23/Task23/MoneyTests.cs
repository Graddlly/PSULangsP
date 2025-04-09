namespace Task23;

/// <summary>
/// Класс для тестирования функциональности Money.
/// </summary>
public static class MoneyTests
{
    /// <summary>
    /// Запускает комплексное тестирование класса Money.
    /// </summary>
    public static void TestMoney()
    {
        Console.WriteLine("Тестирование класса Money");
        Console.WriteLine("=========================");

        TestConstructors();
        TestProperties();
        TestSubtractMethod();
        TestUnaryOperations();
        TestTypeConversions();
        TestBinaryOperations();
        TestUserInput();
    }
    
    /// <summary>
    /// Тестирует конструкторы класса Money.
    /// </summary>
    private static void TestConstructors()
    {
        Console.WriteLine("Тестирование конструкторов:");
            
        // Конструктор по умолчанию
        var defaultMoney = new Money();
        Console.WriteLine($"Конструктор по умолчанию: {defaultMoney}");

        // Конструктор с параметрами
        var money1 = new Money(150, 75);
        Console.WriteLine($"Конструктор с параметрами (150, 75): {money1}");

        try
        {
            var invalidMoney = new Money(100, 100);
            Console.WriteLine("Ошибка: Должно быть исключение при копейках >= 100");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Успешно: Поймано исключение при неверных копейках: {ex.Message}");
        }
    }

    /// <summary>
    /// Тестирует свойства класса Money.
    /// </summary>
    private static void TestProperties()
    {
        Console.WriteLine("\nТестирование свойств:");
        var money2 = new Money();
        money2.Rubles = 200;
        money2.Kopeks = 50;
        Console.WriteLine($"Установка через свойства (200, 50): {money2}");

        try
        {
            money2.Kopeks = 100;
            Console.WriteLine("Ошибка: Должно быть исключение при копейках >= 100");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Успешно: Поймано исключение при неверных копейках: {ex.Message}");
        }
    }

    /// <summary>
    /// Тестирует метод вычитания.
    /// </summary>
    private static void TestSubtractMethod()
    {
        Console.WriteLine("\nТестирование метода вычитания:");
        var money3 = new Money(500, 30);
        var money4 = new Money(200, 10);
        var result = money3.Subtract(money4);
        Console.WriteLine($"{money3} - {money4} = {result}");

        // Тест с заёмом из рублей
        var money5 = new Money(500, 20);
        var money6 = new Money(200, 50);
        var result2 = money5.Subtract(money6);
        Console.WriteLine($"{money5} - {money6} = {result2}");

        try
        {
            var money7 = new Money(100, 50);
            var money8 = new Money(200, 60);
            var result3 = money7.Subtract(money8);
            Console.WriteLine("Ошибка: Должно быть исключение при вычитании большей суммы из меньшей");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Успешно: Поймано исключение при вычитании большей суммы: {ex.Message}");
        }
    }

    /// <summary>
    /// Тестирует унарные операции.
    /// </summary>
    private static void TestUnaryOperations()
    {
        Console.WriteLine("\nТестирование унарных операций:");
            
        // Тест операции ++
        Console.WriteLine("Тест операции ++:");
        var moneyInc = new Money(100, 99);
        Console.WriteLine($"Исходное значение: {moneyInc}");
        var moneyIncResult = moneyInc++;
        Console.WriteLine($"После moneyInc++: {moneyInc}");
        Console.WriteLine($"Результат операции: {moneyIncResult}");

        // Тест операции -- 
        Console.WriteLine("\nТест операции --:");
        var moneyDec = new Money(100, 0);
        Console.WriteLine($"Исходное значение: {moneyDec}");
        var moneyDecResult = moneyDec--;
        Console.WriteLine($"После moneyDec--: {moneyDec}");
        Console.WriteLine($"Результат операции: {moneyDecResult}");

        // Тест исключения при операции --
        Console.WriteLine("\nТест исключения при операции --:");
        var moneyDecZero = new Money(0, 0);
        Console.WriteLine($"Исходное значение: {moneyDecZero}");
        try
        {
            var moneyDecZeroResult = moneyDecZero--;
            Console.WriteLine("Ошибка: Должно быть исключение при вычитании из нуля");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Успешно: Поймано исключение: {ex.Message}");
        }
    }

    /// <summary>
    /// Тестирует операции приведения типа.
    /// </summary>
    private static void TestTypeConversions()
    {
        Console.WriteLine("\nТестирование операций приведения типа:");
            
        // Неявное приведение к uint
        var moneyForUint = new Money(123, 45);
        uint rubles = moneyForUint;
        Console.WriteLine($"Неявное приведение {moneyForUint} к uint: {rubles} руб.");

        // Явное приведение к double
        var moneyForDouble = new Money(123, 45);
        var kopeksAsRubles = (double)moneyForDouble;
        Console.WriteLine($"Явное приведение {moneyForDouble} к double: {kopeksAsRubles:F2} руб.");
    }

    /// <summary>
    /// Тестирует бинарные операции.
    /// </summary>
    private static void TestBinaryOperations()
    {
        Console.WriteLine("\nТестирование бинарных операций:");
            
        // Money - Money
        var binOp1 = new Money(500, 75);
        var binOp2 = new Money(200, 25);
        var binResult1 = binOp1 - binOp2;
        Console.WriteLine($"Money - Money: {binOp1} - {binOp2} = {binResult1}");

        // Money - uint
        var binOp3 = new Money(500, 75);
        const uint binOp4 = 200;
        var binResult2 = binOp3 - binOp4;
        Console.WriteLine($"Money - uint: {binOp3} - {binOp4} = {binResult2}");

        // uint - Money
        const uint binOp5 = 500;
        var binOp6 = new Money(200, 25);
        var binResult3 = binOp5 - binOp6;
        Console.WriteLine($"uint - Money: {binOp5} - {binOp6} = {binResult3}");

        // Тестирование исключения в бинарной операции
        Console.WriteLine("\nТестирование исключения в бинарной операции:");
        var binOp7 = new Money(100, 50);
        var binOp8 = new Money(200, 75);
        try
        {
            var binResult4 = binOp7 - binOp8;
            Console.WriteLine("Ошибка: Должно быть исключение при вычитании большей суммы из меньшей");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Успешно: Поймано исключение: {ex.Message}");
        }

        const uint binOp9 = 100;
        var binOp10 = new Money(200, 75);
        try
        {
            var binResult5 = binOp9 - binOp10;
            Console.WriteLine("Ошибка: Должно быть исключение при вычитании большей суммы из меньшей");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Успешно: Поймано исключение: {ex.Message}");
        }
    }

    /// <summary>
    /// Тестирует ввод данных пользователем.
    /// </summary>
    private static void TestUserInput()
    {
        try
        {
            Console.WriteLine("\nТестирование ввода пользователем:");
            Console.WriteLine("Введите сумму денег:");
            var money = ReadMoneyFromConsole();
            Console.WriteLine($"Вы ввели: {money}");

            Console.WriteLine("\nВыберите операцию:");
            Console.WriteLine("1. Прибавить копейку (++)");
            Console.WriteLine("2. Вычесть копейку (--)");
            Console.WriteLine("3. Вычесть другую сумму денег");
            Console.WriteLine("4. Вычесть целое число рублей");
            Console.WriteLine("5. Вычесть из целого числа рублей");
            Console.WriteLine("6. Получить только рубли (приведение к uint)");
            Console.WriteLine("7. Получить только копейки в рублях (приведение к double)\n");
            
            Console.Write("Ваш выбор: ");
            var switchCh = Console.ReadLine();
            Console.WriteLine();
            switch (switchCh)
            {
                case "1":
                    var incResult = money++;
                    Console.WriteLine($"Результат {money} (после операции)");
                    Console.WriteLine($"Возвращаемое значение: {incResult}");
                    break;
                case "2":
                    try
                    {
                        var decResult = money--;
                        Console.WriteLine($"Результат {money} (после операции)");
                        Console.WriteLine($"Возвращаемое значение: {decResult}");
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine($"Ошибка: {ex.Message}");
                    }
                    break;
                case "3":
                    Console.WriteLine("Введите вторую сумму для вычитания:");
                    var money2 = ReadMoneyFromConsole();
                    try
                    {
                        var result = money - money2;
                        Console.WriteLine($"Результат вычитания: {money} - {money2} = {result}");
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine($"Ошибка: {ex.Message}");
                    }
                    break;
                case "4":
                    Console.Write("Введите количество рублей для вычитания: ");
                    if (uint.TryParse(Console.ReadLine(), out uint rubToSubtract))
                    {
                        try
                        {
                            var result = money - rubToSubtract;
                            Console.WriteLine($"Результат вычитания: {money} - {rubToSubtract} = {result}");
                        }
                        catch (InvalidOperationException ex)
                        {
                            Console.WriteLine($"Ошибка: {ex.Message}");
                        }
                    }
                    else Console.WriteLine("Неверное значение для рублей.");
                    break;
                case "5":
                    Console.Write("Введите количество рублей, из которых нужно вычесть: ");
                    if (uint.TryParse(Console.ReadLine(), out uint rubFromSubtract))
                    {
                        try
                        {
                            var result = rubFromSubtract - money;
                            Console.WriteLine($"Результат вычитания: {rubFromSubtract} - {money} = {result}");
                        }
                        catch (InvalidOperationException ex)
                        {
                            Console.WriteLine($"Ошибка: {ex.Message}");
                        }
                    }
                    else Console.WriteLine("Неверное значение для рублей.");
                    break;
                case "6":
                    uint rubles = money;
                    Console.WriteLine($"Только рубли (копейки отброшены): {rubles}");
                    break;
                case "7":
                    var kopeksAsRubles = (double)money;
                    Console.WriteLine($"Только копейки в рублях: {kopeksAsRubles:F2}");
                    break;
                default:
                    Console.WriteLine("Неверный выбор операции.");
                    break;
                }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
    }

    /// <summary>
    /// Читает данные денежной суммы с консоли.
    /// </summary>
    /// <returns>Созданный объект Money.</returns>
    private static Money ReadMoneyFromConsole()
    {
        uint rubles = 0;
        byte kopeks = 0;
        var isValid = false;
        
        while (!isValid)
        {
            Console.Write("Введите рубли (целое неотрицательное число): ");
            var input = Console.ReadLine();
                
            if (uint.TryParse(input, out rubles)) isValid = true;
            else Console.WriteLine("Ошибка: Введите корректное неотрицательное целое число для рублей.");
        }
            
        // Сбрасываем флаг для ввода копеек
        isValid = false;
        
        while (!isValid)
        {
            Console.Write("Введите копейки (целое число от 0 до 99): ");
            var input = Console.ReadLine();
                
            if (byte.TryParse(input, out kopeks) && kopeks < 100) isValid = true;
            else Console.WriteLine("Ошибка: Введите корректное число для копеек (от 0 до 99).");
        }
            
        return new Money(rubles, kopeks);
    }
}