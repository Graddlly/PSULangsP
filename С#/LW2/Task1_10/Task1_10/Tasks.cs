namespace Task1_10;

using static Methods;

public class Tasks
{
    /// <summary>
    /// Задание 1: Найти произведение максимального и минимального элементов в текстовом файле.
    /// </summary>
    public static void Task1()
    {
        const string FILE_NAME = "task1.txt";
        GenerateTextFileWithNumbers(FILE_NAME, 10, 1);
        
        var min = int.MaxValue;
        var max = int.MinValue;
        
        using (var reader = new StreamReader(FILE_NAME))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (int.TryParse(line, out var number))
                {
                    if (number < min)
                        min = number;
                    if (number > max)
                        max = number;
                }
            }
        }
        
        var product = (long)min * max;
        Console.WriteLine($"Минимум: {min}; Максимум: {max}; Произведение: {product}");
    }
    
    /// <summary>
    /// Задание 2: Вычислить количество нечётных элементов в текстовом файле.
    /// </summary>
    public static void Task2()
    {
        const string FILE_NAME = "task2.txt";
        GenerateTextFileWithNumbers(FILE_NAME, 20, 5);
        
        var oddCount = 0;
        using (var reader = new StreamReader(FILE_NAME))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                var numbers = line.Split(' ');
                foreach (var numStr in numbers)
                    if (int.TryParse(numStr, out var number) && number % 2 != 0)
                        oddCount++;
            }
        }
        
        Console.WriteLine($"Количество нечётных элементов: {oddCount}");
    }
    
    /// <summary>
    /// Задание 3: Создать новый текстовый файл, каждая строка которого содержит длину строки исходного файла.
    /// </summary>
    public static void Task3()
    {
        const string SOURCE_FILE_NAME = "task3_source.txt";
        const string RESULT_FILE_NAME = "task3_result.txt";
        
        if (!File.Exists(SOURCE_FILE_NAME))
        {
            using (var writer = new StreamWriter(SOURCE_FILE_NAME))
            {
                writer.WriteLine("Это тестовый файл.");
                writer.WriteLine("Он содержит в себе несколько строк текста");
                writer.WriteLine("различной длины.");
                writer.WriteLine("Новый файл будет содержать длину каждой строки из этого.");
            }
        }
        
        using (var reader = new StreamReader(SOURCE_FILE_NAME))
        using (var writer = new StreamWriter(RESULT_FILE_NAME))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
                writer.WriteLine(line.Length);
        }
        
        Console.WriteLine($"Создан {RESULT_FILE_NAME} с содержанием: длины строк из {SOURCE_FILE_NAME}");
        
        Console.WriteLine("\nИсходный файл содержит:");
        DisplayFileContents(SOURCE_FILE_NAME);
        
        Console.WriteLine("\nРезультирующий файл содержит:");
        DisplayFileContents(RESULT_FILE_NAME);
    }
    
    /// <summary>
    /// Задание 4: Сформировать новый файл на основе исходного с прогрессивным включением элементов.
    /// </summary>
    public static void Task4()
    {
        const string SOURCE_FILE_NAME = "task4_source.bin";
        const string RESULT_FILE_NAME = "task4_result.bin";
        
        GenerateBinaryFileWithNumbers(SOURCE_FILE_NAME, 5);
        
        var numbers = ReadNumbersFromBinaryFile(SOURCE_FILE_NAME);
        using (var writer = new BinaryWriter(File.Open(RESULT_FILE_NAME, FileMode.Create)))
        {
            for (var i = 0; i < numbers.Length; i++)
                for (var j = 0; j <= i; j++)
                    writer.Write(numbers[j]);
        }
        
        Console.WriteLine($"Создан файл {RESULT_FILE_NAME} с прогрессивным включением элементов из {SOURCE_FILE_NAME}");
            
        Console.WriteLine("\nСодержимое исходного файла:");
        DisplayBinaryFileContents(SOURCE_FILE_NAME);
            
        Console.WriteLine("\nСодержимое результирующего файла:");
        DisplayBinaryFileContents(RESULT_FILE_NAME);
    }
    
    /// <summary>
    /// Задание 5: Получить названия игрушек, подходящих детям как четырех лет, так и десяти лет.
    /// </summary>
    public static void Task5()
    {
        const string SOURCE_FILE_NAME = "task5_source.bin";
        
        GenerateToysFile(SOURCE_FILE_NAME);
        
        var toys = ReadToysFromFile(SOURCE_FILE_NAME);
        var suitableToys = new List<string>();
        foreach (var toy in toys)
            if (toy.MinAge <= 4 && toy.MaxAge >= 4 && toy.MinAge <= 10 && toy.MaxAge >= 10)
                suitableToys.Add(toy.Name);
        
        Console.WriteLine("Игрушки, подходящие как для детей 4 лет, так и для детей 10 лет:");
        if (suitableToys.Count > 0)
            foreach (var toyName in suitableToys)
                Console.WriteLine(toyName);
        else
            Console.WriteLine("Подходящие игрушки не найдены.");
    }

    /// <summary>
    /// Задание 6: Вставить в список L за первым вхождением элемента E все элементы списка L, если E входит в L.
    /// </summary>
    public static void Task6()
    {
        var list = new List<int> { 1, 2, 3, 4, 5 };
        const int ELEMENT_TO_FIND = 3;

        Console.WriteLine("\nИсходный список:");
        DisplayList(list);
        Console.WriteLine($"Элемент {ELEMENT_TO_FIND} выбран для работы с ним!");
            
        InsertAllAfterFirstOccurrence(list, ELEMENT_TO_FIND);
            
        Console.WriteLine($"\nСписок после вставки всех элементов за первым вхождением элемента {ELEMENT_TO_FIND}:");
        DisplayList(list);
    }

    /// <summary>
    /// Задание 7: Добавить в начало и конец списка L новый элемент E.
    /// </summary>
    public static void Task7()
    {
        var list = new LinkedList<int>([1, 2, 3, 4, 5]);
        const int ELEMENT_TO_FIND = 10;
            
        Console.WriteLine("\nИсходный список:");
        DisplayLinkedList(list);
        Console.WriteLine($"Элемент {ELEMENT_TO_FIND} выбран для работы с ним!");
            
        AddElementToStartAndEnd(list, ELEMENT_TO_FIND);
            
        Console.WriteLine($"\nСписок после добавления элемента {ELEMENT_TO_FIND} в начало и конец:");
        DisplayLinkedList(list);
    }

    /// <summary>
    /// Задание 8: Определить для каждой книги, какие из них прочли все из n читателей,
    /// какие — некоторые, и какие — никто.
    /// </summary>
    public static void Task8()
    {
        var allBooks = new List<string> { "Книга1", "Книга2", "Книга3", "Книга4", "Книга5" };
        
        var readersBooks = new HashSet<string>[3];
        readersBooks[0] = ["Книга1", "Книга2", "Книга3"];
        readersBooks[1] = ["Книга1", "Книга3", "Книга4"];
        readersBooks[2] = ["Книга1", "Книга5"];
        
        var booksReadByAll = new HashSet<string>(readersBooks[0]);
        for (var i = 1; i < readersBooks.Length; i++)
            booksReadByAll.IntersectWith(readersBooks[i]);
        
        var booksReadBySome = new HashSet<string>();
        foreach (var readerBooks in readersBooks)
            booksReadBySome.UnionWith(readerBooks);
        
        var booksReadByNone = new HashSet<string>(allBooks);
        booksReadByNone.ExceptWith(booksReadBySome);
        
        Console.WriteLine("Книги, прочитанные всеми читателями:");
        DisplayHashSet(booksReadByAll);
        
        Console.WriteLine("\nКниги, прочитанные некоторыми читателями:");
        var onlySomeReaders = new HashSet<string>(booksReadBySome);
        onlySomeReaders.ExceptWith(booksReadByAll);
        DisplayHashSet(onlySomeReaders);
        
        Console.WriteLine("\nКниги, не прочитанные никем:");
        DisplayHashSet(booksReadByNone);
    }

    /// <summary>
    /// Задание 9: Найти символы, которых нет в первом слове, но они присутствуют в каждом из других.
    /// </summary>
    public static void Task9()
    {
        const string FILE_NAME = "task9.txt";
        
        if (!File.Exists(FILE_NAME))
            using (var writer = new StreamWriter(FILE_NAME))
                writer.WriteLine("Привет, мир! Мы здесь программируем на c#");
        
        var text = File.ReadAllText(FILE_NAME);
        var words = text.Split([' ', '\t', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries);
        
        if (words.Length < 2)
        {
            Console.WriteLine("В тексте меньше двух слов.");
            return;
        }
        
        var firstWordChars = new HashSet<char>();
        foreach (var c in words[0])
            firstWordChars.Add(c);
        
        HashSet<char> commonChars = null!;
        for (var i = 1; i < words.Length; i++)
        {
            var currentWordChars = new HashSet<char>();
            foreach (var c in words[i])
                currentWordChars.Add(c);
            
            if (commonChars == null)
                commonChars = new HashSet<char>(currentWordChars);
            else
                commonChars.IntersectWith(currentWordChars);
        }
        
        commonChars.ExceptWith(firstWordChars);
        
        Console.WriteLine("Символы, которых нет в первом слове, но они присутствуют во всех остальных словах:");
        DisplayHashSet(commonChars);
    }

    /// <summary>
    /// Задание 10: Определить минимальный балл призера олимпиады и количество призеров по параллелям.
    /// </summary>
    public static void Task10()
    {
        const string FILE_NAME = "task10.txt";
        
        if (!File.Exists(FILE_NAME))
            GenerateOlympiadResults(FILE_NAME);
        
        var participants = new List<Participant>();
        using (var reader = new StreamReader(FILE_NAME))
        {
            var line = reader.ReadLine();
            if (int.TryParse(line, out var n))
                for (var i = 0; i < n; i++)
                {
                    line = reader.ReadLine();
                    if (line != null)
                    {
                        var parts = line.Split(' ');
                        if (parts.Length >= 4)
                        {
                            var lastName = parts[0];
                            var firstName = parts[1];
                            var grade = int.Parse(parts[2]);
                            var score = int.Parse(parts[3]);
                            
                            participants.Add(new Participant(lastName, firstName, grade, score));
                        }
                    }
                }
        }
        
        participants.Sort(CompareByScoreDescending);
        
        var thresholdIndex = (int)Math.Ceiling(participants.Count * 0.25) - 1;
        if (thresholdIndex < 0)
            thresholdIndex = 0;
        
        var thresholdScore = participants[thresholdIndex].Score;
        var lastIndex = thresholdIndex;
        while (lastIndex + 1 < participants.Count && participants[lastIndex + 1].Score == thresholdScore)
            lastIndex++;
        
        int minWinnerScore;
        if (lastIndex > thresholdIndex)
        {
            if (thresholdScore > 70 / 2)
                minWinnerScore = thresholdScore;
            else
            {
                var nextHigherScore = -1;
                for (var i = thresholdIndex - 1; i >= 0; i--)
                {
                    if (participants[i].Score > thresholdScore)
                    {
                        nextHigherScore = participants[i].Score;
                        break;
                    }
                }
                minWinnerScore = nextHigherScore;
            }
        }
        else
            minWinnerScore = thresholdScore;
        
        var winnersByGrade = new int[5];
        foreach (var p in participants)
            if (p.Score >= minWinnerScore)
                winnersByGrade[p.Grade - 7]++;
        
        Console.WriteLine(minWinnerScore);
        Console.WriteLine($"{winnersByGrade[0]} {winnersByGrade[1]} {winnersByGrade[2]} {winnersByGrade[3]} {winnersByGrade[4]}");
    }
}