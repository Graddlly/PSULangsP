using PascalCompiler.IOModule.Models;

namespace PascalCompiler.IOModule;

public class InputOutput
{
    const byte ERR_MAX = 9;
    
    private static string _line = "";
    private static byte _lastInLine;
    private static TextPosition _positionNow;
    private static List<Err> _err = [];
    private static List<Err> _currentErr = [];
    private static StreamReader? File { get; set; }
    private static uint _errCount;
    private static bool _isInitialized;
    private static bool _endOfFile;
    private static readonly Dictionary<byte, string> ErrorTable = new()
    {
        { 1, "Неожиданный символ" },
        { 2, "Ожидается точка с запятой (или точка в конце программы)" },
        { 3, "Ожидается идентификатор" },
        { 4, "Ожидается знак равенства" },
        { 5, "Недопустимый символ в числе" },
        { 6, "Слишком длинное число" },
        { 7, "Незакрытая строка" },
        { 8, "Недопустимый символ в идентификаторе" },
        { 9, "Ожидается закрывающая скобка" },
        { 10, "Ожидается ключевое слово BEGIN" },
        { 11, "Ожидается ключевое слово END" },
        { 12, "Неожиданный конец файла (незакрытый комментарий)" },
        { 13, "Недопустимый тип данных" },
        { 14, "Повторное объявление идентификатора" },
        { 15, "Необъявленный идентификатор" },
        { 203, "Константа превышает предел" }
    };
    
    public static char Ch { get; private set; } = '\0';
    public static TextPosition PositionNow { get; }

    /// <summary>
    /// Инициализация модуля ввода-вывода
    /// </summary>
    /// <param name="fileName">Имя файла для чтения</param>
    /// <returns>true, если инициализация прошла успешно</returns>
    public static bool Initialize(string fileName)
    {
        try
        {
            File?.Close();

            File = new StreamReader(fileName);
            _positionNow = new TextPosition(1, 0);
            _err = [];
            _errCount = 0;
            _endOfFile = false;
            _isInitialized = true;

            // TODO: Раскомментировать, если нужен вывод исходного кода
            /*Console.WriteLine("=== ИСХОДНЫЙ КОД ===");
            
            var allText = File.ReadToEnd();
            Console.WriteLine(allText);
            Console.WriteLine("=== КОНЕЦ ИСХОДНОГО КОДА ===\n");*/
            
            File.Close();
            File = new StreamReader(fileName);

            ReadNextLine();
            if (!_endOfFile && _line.Length > 0)
            {
                _lastInLine = (byte)(_line.Length - 1);
                Ch = _line[0];
            }
            else
            {
                Ch = '\0';
                _endOfFile = true;
            }

            Console.WriteLine("=== НАЧАЛО АНАЛИЗА ===");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка инициализации: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Переход к следующему символу
    /// </summary>
    public static void NextCh()
    {
        if (!_isInitialized || _endOfFile)
        {
            Ch = '\0';
            return;
        }

        if (_positionNow.charNumber == _lastInLine)
        {
            ListThisLine();
            
            _currentErr = _err.FindAll(e => e.errorPosition.lineNumber == _positionNow.lineNumber);
            
            if (_currentErr.Count > 0)
                ListErrors();
            
            ReadNextLine();
            if (!_endOfFile)
            {
                _positionNow.lineNumber += 1;
                _positionNow.charNumber = 0;
                if (_line.Length > 0)
                {
                    _lastInLine = (byte)(_line.Length - 1);
                    Ch = _line[0];
                }
                else
                {
                    Ch = '\n';
                }
            }
            else
            {
                Ch = '\0';
            }
        }
        else
        {
            ++_positionNow.charNumber;
            Ch = _positionNow.charNumber < _line.Length ? _line[_positionNow.charNumber] : '\n';
        }
    }

    private static void ListThisLine()
    {
        var lineNumber = $"{_positionNow.lineNumber,3} ";
        Console.WriteLine(lineNumber + _line);
    }

    private static void ReadNextLine()
    {
        if (File != null && !File.EndOfStream)
        {
            _line = File.ReadLine() ?? "";
            _lastInLine = (byte)Math.Max(0, _line.Length - 1);
        }
        else
        {
            _endOfFile = true;
            End();
        }
    }

    private static void End()
    {
        Console.WriteLine();
        Console.WriteLine($"\nКомпиляция завершена: ошибок — {_errCount}!");
        if (File != null)
        {
            File.Close();
            File = null;
        }
    }

    private static void ListErrors()
    {
        foreach (var item in _currentErr)
        {
            ++_errCount;
            var lineNumberSpaces = new string(' ', 4);
            var errorPositionSpaces = new string(' ', item.errorPosition.charNumber);
            var message = ErrorTable.GetValueOrDefault(item.errorCode, "Неизвестная ошибка");

            Console.WriteLine($"{lineNumberSpaces}{errorPositionSpaces}^ **{_errCount:00}**: ошибка {item.errorCode} - {message}");
        }
    }

    /// <summary>
    /// Добавление ошибки в список
    /// </summary>
    /// <param name="errorCode">Код ошибки</param>
    /// <param name="position">Позиция ошибки</param>
    public static void Error(byte errorCode, TextPosition position)
    {
        if (_err.Count <= ERR_MAX)
        {
            var e = new Err(position, errorCode);
            _err.Add(e);
            
#if DEBUG
            Console.WriteLine($"[DEBUG] Добавлена ошибка {errorCode} в позицию {position} - всего ошибок: {_err.Count}");
#endif
        }
    }

    /// <summary>
    /// Добавление ошибки в текущей позиции
    /// </summary>
    /// <param name="errorCode">Код ошибки</param>
    public static void Error(byte errorCode)
    {
        Error(errorCode, _positionNow);
    }

    /// <summary>
    /// Проверка, достигнут ли конец файла
    /// </summary>
    /// <returns>true, если достигнут конец файла</returns>
    public static bool IsEndOfFile()
    {
        return _endOfFile || Ch == '\0';
    }

    /// <summary>
    /// Получение описания ошибки по коду
    /// </summary>
    /// <param name="errorCode">Код ошибки</param>
    /// <returns>Описание ошибки</returns>
    public static string GetErrorDescription(byte errorCode)
    {
        return ErrorTable.GetValueOrDefault(errorCode, "Неизвестная ошибка");
    }

    /// <summary>
    /// Вывод таблицы ошибок
    /// </summary>
    public static void PrintErrorTable()
    {
        Console.WriteLine("=== ТАБЛИЦА ОШИБОК ===");
        foreach (var kvp in ErrorTable)
        {
            Console.WriteLine($"{kvp.Key,2}: {kvp.Value}");
        }
        Console.WriteLine("=====================\n");
    }
}