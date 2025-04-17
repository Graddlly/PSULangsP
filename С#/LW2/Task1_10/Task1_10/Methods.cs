using System.Text;
using System.Xml.Serialization;

namespace Task1_10;

public class Methods
{
    /// <summary>
    /// Генерирует текстовый файл с числами.
    /// </summary>
    /// <param name="fileName">Имя файла.</param>
    /// <param name="count">Количество строк.</param>
    /// <param name="numbersPerLine">Количество чисел в строке.</param>
    public static void GenerateTextFileWithNumbers(string fileName, int count, int numbersPerLine)
    {
        var random = new Random();
        using (var writer = new StreamWriter(fileName))
        {
            for (var i = 0; i < count; i++)
            {
                var line = new StringBuilder();
                for (var j = 0; j < numbersPerLine; j++)
                {
                    if (j > 0)
                        line.Append(' ');
                    line.Append(random.Next(-100, 101));
                }
                writer.WriteLine(line.ToString());
            }
        }
    }
    
    /// <summary>
    /// Генерирует бинарный файл со случайными целыми числами.
    /// </summary>
    /// <param name="fileName">Имя файла.</param>
    /// <param name="count">Количество чисел.</param>
    public static void GenerateBinaryFileWithNumbers(string fileName, int count)
    {
        var random = new Random();
        using (var writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
        {
            for (var i = 0; i < count; i++)
                writer.Write(random.Next(-100, 101));
        }
    }
    
    /// <summary>
    /// Читает целые числа из бинарного файла.
    /// </summary>
    /// <param name="fileName">Имя файла.</param>
    /// <returns>Массив целых чисел.</returns>
    public static int[] ReadNumbersFromBinaryFile(string fileName)
    {
        var numbers = new List<int>();
        using (var reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
        {
            while (reader.BaseStream.Position < reader.BaseStream.Length)
                numbers.Add(reader.ReadInt32());
        }
        return numbers.ToArray();
    }
    
    /// <summary>
    /// Отображает содержимое текстового файла.
    /// </summary>
    /// <param name="fileName">Имя файла.</param>
    public static void DisplayFileContents(string fileName)
    {
        using (var reader = new StreamReader(fileName))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
                Console.WriteLine(line);
        }
    }
    
    /// <summary>
    /// Отображает содержимое бинарного файла с целыми числами.
    /// </summary>
    /// <param name="fileName">Имя файла.</param>
    public static void DisplayBinaryFileContents(string fileName)
    {
        using (var reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
        {
            while (reader.BaseStream.Position < reader.BaseStream.Length)
                Console.Write($"{reader.ReadInt32()} ");
            Console.WriteLine();
        }
    }
    
    /// <summary>
    /// Генерирует файл с данными об игрушках.
    /// </summary>
    /// <param name="fileName">Имя файла.</param>
    public static void GenerateToysFile(string fileName)
    {
        var toys = new List<Toy>
        {
            new() { Name = "Плюшевый мишка", Price = 500, MinAge = 1, MaxAge = 12 },
            new() { Name = "Лего части", Price = 1200, MinAge = 3, MaxAge = 15 },
            new() { Name = "Пазл", Price = 800, MinAge = 4, MaxAge = 10 },
            new() { Name = "Кукла", Price = 600, MinAge = 2, MaxAge = 8 },
            new() { Name = "Машинка на дист. управлении", Price = 1500, MinAge = 6, MaxAge = 14 }
        };
        
        var serializer = new XmlSerializer(typeof(List<Toy>));
        using (var ms = new MemoryStream())
        {
            serializer.Serialize(ms, toys);
            
            using (var fs = new FileStream(fileName, FileMode.Create))
            {
                ms.Seek(0, SeekOrigin.Begin);
                var buffer = new byte[ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                fs.Write(buffer, 0, buffer.Length);
            }
        }
    }
    
    /// <summary>
    /// Читает информацию об игрушках из файла.
    /// </summary>
    /// <param name="fileName">Имя файла.</param>
    /// <returns>Список игрушек.</returns>
    public static List<Toy> ReadToysFromFile(string fileName)
    {
        var serializer = new XmlSerializer(typeof(List<Toy>));
        using (var fs = new FileStream(fileName, FileMode.Open))
            return (List<Toy>)serializer.Deserialize(fs);
    }
    
    /// <summary>
    /// Добавляет элемент в начало и конец связного списка.
    /// </summary>
    /// <typeparam name="T">Тип элементов списка.</typeparam>
    /// <param name="list">Связный список.</param>
    /// <param name="element">Элемент для добавления.</param>
    public static void AddElementToStartAndEnd<T>(LinkedList<T> list, T element)
    {
        list.AddFirst(element);
        list.AddLast(element);
    }
    
    /// <summary>
    /// Вставляет в список все его элементы за первым вхождением указанного элемента.
    /// </summary>
    /// <typeparam name="T">Тип элементов списка.</typeparam>
    /// <param name="list">Исходный список.</param>
    /// <param name="element">Элемент, после которого выполняется вставка.</param>
    public static void InsertAllAfterFirstOccurrence<T>(List<T> list, T element)
    {
        var index = list.IndexOf(element);
        if (index >= 0)
        {
            var copyOfList = new List<T>(list);
            list.InsertRange(index + 1, copyOfList);
        }
    }
    
    /// <summary>
    /// Отображает содержимое списка.
    /// </summary>
    /// <typeparam name="T">Тип элементов списка.</typeparam>
    /// <param name="list">Список.</param>
    public static void DisplayList<T>(List<T> list)
    {
        foreach (var item in list)
            Console.Write($"{item} ");
        Console.WriteLine();
    }
    
    /// <summary>
    /// Отображает содержимое связного списка.
    /// </summary>
    /// <typeparam name="T">Тип элементов списка.</typeparam>
    /// <param name="list">Связный список.</param>
    public static void DisplayLinkedList<T>(LinkedList<T> list)
    {
        var current = list.First;
        while (current != null)
        {
            Console.Write($"{current.Value} ");
            current = current.Next;
        }
        Console.WriteLine();
    }
    
    /// <summary>
    /// Отображает содержимое множества.
    /// </summary>
    /// <typeparam name="T">Тип элементов множества.</typeparam>
    /// <param name="set">Множество.</param>
    public static void DisplayHashSet<T>(HashSet<T> set)
    {
        if (set.Count == 0)
            Console.WriteLine("(Пустое множество)");
        else
            foreach (var item in set)
                Console.WriteLine(item);
    }
    
    /// <summary>
    /// Генерирует результаты олимпиады.
    /// </summary>
    /// <param name="fileName">Имя файла.</param>
    public static void GenerateOlympiadResults(string fileName)
    {
        var random = new Random();
        const int PARTICIPANTS_COUNT = 100;
        
        using (var writer = new StreamWriter(fileName))
        {
            writer.WriteLine(PARTICIPANTS_COUNT);
            
            string[] firstNames = ["Иван", "Петр", "Анна", "Мария", "Алексей", "Ольга", "Дмитрий", "Светлана"];
            string[] lastNames = ["Иванов", "Петров", "Сидоров", "Смирнов", "Козлов", "Морозов", "Волков", "Соколов"];
            
            for (var i = 0; i < PARTICIPANTS_COUNT; i++)
            {
                var lastName = lastNames[random.Next(lastNames.Length)];
                var firstName = firstNames[random.Next(firstNames.Length)];
                var grade = random.Next(7, 12);
                var score = random.Next(0, 71);
                
                writer.WriteLine($"{lastName} {firstName} {grade} {score}");
            }
        }
    }
}