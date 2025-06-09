using PascalCompiler.IOModule.Assets;

namespace PascalCompiler.LexicalAnalyzer.Assets;

/// <summary>
/// Класс для анализа и представления результатов лексического анализа
/// </summary>
public class TokenAnalyzer
{
    private readonly Dictionary<byte, string> _symbolNames;
    private readonly List<TokenInfo> _tokens;

    public TokenAnalyzer()
    {
        _symbolNames = InitializeSymbolNames();
        _tokens = [];
    }

    /// <summary>
    /// Добавление токена в список для анализа
    /// </summary>
    /// <param name="code">Код символа</param>
    /// <param name="value">Значение (для идентификаторов, констант)</param>
    /// <param name="position">Позиция в тексте</param>
    public void AddToken(byte code, string value, TextPosition position)
    {
        _tokens.Add(new TokenInfo(code, value, position));
    }

    /// <summary>
    /// Вывод детального анализа токенов
    /// </summary>
    public void PrintDetailedAnalysis()
    {
        Console.WriteLine("\n=== ДЕТАЛЬНЫЙ АНАЛИЗ ТОКЕНОВ ===");
        Console.WriteLine($"{"№",-4} {"Позиция",-10} {"Код",-4} {"Тип",-20} {"Значение",-15}");
        Console.WriteLine(new string('-', 60));

        for (var i = 0; i < _tokens.Count; i++)
        {
            var token = _tokens[i];
            var typeName = GetTokenTypeName(token.Code);
            var position = $"[{token.Position.lineNumber}:{token.Position.charNumber}]";
            
            Console.WriteLine($"{i + 1,-4} {position,-10} {token.Code,-4} {typeName,-20} {token.Value,-15}");
        }

        Console.WriteLine(new string('-', 60));
        Console.WriteLine($"Всего токенов: {_tokens.Count}");
        PrintStatistics();
    }

    /// <summary>
    /// Вывод статистики по типам токенов
    /// </summary>
    private void PrintStatistics()
    {
        Console.WriteLine("\n=== СТАТИСТИКА ТОКЕНОВ ===");
        
        var statistics = new Dictionary<string, int>();
        
        foreach (var typeName in _tokens.Select(token => GetTokenTypeName(token.Code)))
        {
            if (statistics.TryGetValue(typeName, out var value))
                statistics[typeName] = ++value;
            else
                statistics[typeName] = 1;
        }

        foreach (var kvp in statistics)
        {
            Console.WriteLine($"{kvp.Key}: {kvp.Value}");
        }
        Console.WriteLine("========================\n");
    }

    /// <summary>
    /// Получение названия типа токена по коду
    /// </summary>
    /// <param name="code">Код токена</param>
    /// <returns>Название типа</returns>
    private string GetTokenTypeName(byte code)
    {
        if (_symbolNames.TryGetValue(code, out var value))
            return value;

        return code switch
        {
            >= 100 and <= 127 => "Ключевое слово",
            LexicalAnalyzer.ident => "Идентификатор",
            LexicalAnalyzer.intc => "Целая константа",
            LexicalAnalyzer.floatc => "Вещ. константа",
            LexicalAnalyzer.charc => "Символ. константа",
            _ => "Неизвестный"
        };
    }

    /// <summary>
    /// Инициализация названий символов
    /// </summary>
    /// <returns>Словарь соответствий кодов и названий</returns>
    private static Dictionary<byte, string> InitializeSymbolNames()
    {
        return new Dictionary<byte, string>
        {
            { LexicalAnalyzer.star, "Умножение (*)" },
            { LexicalAnalyzer.slash, "Деление (/)" },
            { LexicalAnalyzer.equal, "Равенство (=)" },
            { LexicalAnalyzer.comma, "Запятая (,)" },
            { LexicalAnalyzer.semicolon, "Точка с запятой (;)" },
            { LexicalAnalyzer.colon, "Двоеточие (:)" },
            { LexicalAnalyzer.point, "Точка (.)" },
            { LexicalAnalyzer.arrow, "Указатель (^)" },
            { LexicalAnalyzer.leftpar, "Левая скобка ((" },
            { LexicalAnalyzer.rightpar, "Правая скобка ())" },
            { LexicalAnalyzer.lbracket, "Левая кв. скобка ([)" },
            { LexicalAnalyzer.rbracket, "Правая кв. скобка (])" },
            { LexicalAnalyzer.flpar, "Левая фиг. скобка ({)" },
            { LexicalAnalyzer.frpar, "Правая фиг. скобка (})" },
            { LexicalAnalyzer.later, "Меньше (<)" },
            { LexicalAnalyzer.greater, "Больше (>)" },
            { LexicalAnalyzer.laterequal, "Меньше равно (<=)" },
            { LexicalAnalyzer.greaterequal, "Больше равно (>=)" },
            { LexicalAnalyzer.latergreater, "Не равно (<>)" },
            { LexicalAnalyzer.plus, "Сложение (+)" },
            { LexicalAnalyzer.minus, "Вычитание (-)" },
            { LexicalAnalyzer.assign, "Присваивание (:=)" },
            { LexicalAnalyzer.twopoints, "Диапазон (..)" },
            { LexicalAnalyzer.lcomment, "Левый комментарий ( (* )"},
            { LexicalAnalyzer.rcomment, "Правый комментарий ( *) )"},
            
            { LexicalAnalyzer.programsy, "program" },
            { LexicalAnalyzer.varsy, "var" },
            { LexicalAnalyzer.constsy, "const" },
            { LexicalAnalyzer.beginsy, "begin" },
            { LexicalAnalyzer.endsy, "end" },
            { LexicalAnalyzer.ifsy, "if" },
            { LexicalAnalyzer.thensy, "then" },
            { LexicalAnalyzer.elsesy, "else" },
            { LexicalAnalyzer.whilesy, "while" },
            { LexicalAnalyzer.dosy, "do" },
            { LexicalAnalyzer.forsy, "for" },
            { LexicalAnalyzer.tosy, "to" },
            { LexicalAnalyzer.downtosy, "downto" },
            { LexicalAnalyzer.repeatsy, "repeat" },
            { LexicalAnalyzer.untilsy, "until" },
            { LexicalAnalyzer.casesy, "case" },
            { LexicalAnalyzer.ofsy, "of" },
            { LexicalAnalyzer.andsy, "and" },
            { LexicalAnalyzer.orsy, "or" },
            { LexicalAnalyzer.notsy, "not" },
            { LexicalAnalyzer.divsy, "div" },
            { LexicalAnalyzer.modsy, "mod" },
            { LexicalAnalyzer.arraysy, "array" },
            { LexicalAnalyzer.recordsy, "record" },
            { LexicalAnalyzer.setsy, "set" },
            { LexicalAnalyzer.filesy, "file" },
            { LexicalAnalyzer.functionsy, "function" },
            { LexicalAnalyzer.procedurensy, "procedure" },
            { LexicalAnalyzer.typesy, "type" },
            { LexicalAnalyzer.labelsy, "label" },
            { LexicalAnalyzer.gotosy, "goto" },
            { LexicalAnalyzer.withsy, "with" },
            { LexicalAnalyzer.insy, "in" },
            { LexicalAnalyzer.nilsy, "nil" },
            { LexicalAnalyzer.packedsy, "packed" },
            { LexicalAnalyzer.integersy, "integer" },
            { LexicalAnalyzer.realsy, "real" },
            { LexicalAnalyzer.charsy, "char" }
        };
    }

    /// <summary>
    /// Структура для хранения информации о токене
    /// </summary>
    private struct TokenInfo
    {
        public byte Code { get; }
        public string Value { get; }
        public TextPosition Position { get; }

        public TokenInfo(byte code, string value, TextPosition position)
        {
            Code = code;
            Value = value ?? "";
            Position = position;
        }
    }
}