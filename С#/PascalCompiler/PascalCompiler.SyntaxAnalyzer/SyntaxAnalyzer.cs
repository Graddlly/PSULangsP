using PascalCompiler.IOModule;
using PascalCompiler.SyntaxAnalyzer.Assets;

namespace PascalCompiler.SyntaxAnalyzer;
// ReSharper disable InvalidXmlDocComment
// ReSharper disable InconsistentNaming

/// <summary>
/// Синтаксический анализатор для языка Pascal
/// Поддерживает:
/// - Описание переменных простых типов
/// - Описание массивов
/// - Операторы присваивания и составной (простые и индексированные переменные)
/// </summary>
public class SyntaxAnalyzer
{
    protected readonly LexicalAnalyzer.LexicalAnalyzer _lexer;
    protected byte _currentSymbol;
    protected readonly Dictionary<string, VariableInfo> _symbolTable;
    
    private const byte ERR_EXPECTED_SEMICOLON = 2;
    protected const byte ERR_EXPECTED_IDENTIFIER = 3;
    private const byte ERR_EXPECTED_EQUALS = 4;
    private const byte ERR_EXPECTED_BEGIN = 10;
    private const byte ERR_EXPECTED_END = 11;
    private const byte ERR_INVALID_TYPE = 13;
    private const byte ERR_DUPLICATE_IDENTIFIER = 14;
    protected const byte ERR_UNDECLARED_IDENTIFIER = 15;
    private const byte ERR_EXPECTED_COLON = 16;
    private const byte ERR_EXPECTED_OF = 17;
    private const byte ERR_EXPECTED_ARRAY = 18;
    private const byte ERR_EXPECTED_LBRACKET = 19;
    protected const byte ERR_EXPECTED_RBRACKET = 20;
    protected const byte ERR_EXPECTED_ASSIGN = 21;
    private const byte ERR_EXPECTED_DOT = 22;
    protected const byte ERR_INVALID_ARRAY_INDEX = 23;

    public SyntaxAnalyzer()
    {
        _lexer = new LexicalAnalyzer.LexicalAnalyzer();
        _symbolTable = new Dictionary<string, VariableInfo>();
    }

    /// <summary>
    /// Основной метод синтаксического анализа
    /// </summary>
    public void AnalyzeProgram()
    {
        _currentSymbol = _lexer.NextSym();
        
        try
        {
            ParseProgram();
#if DEBUG
            Console.WriteLine("\n=== СИНТАКСИЧЕСКИЙ АНАЛИЗ ЗАВЕРШЕН УСПЕШНО ===");
#endif
        }
        catch (SyntaxException ex)
        {
            Console.WriteLine($"\n=== КРИТИЧЕСКАЯ СИНТАКСИЧЕСКАЯ ОШИБКА ===");
            Console.WriteLine($"Ошибка: {ex.Message}");
        }

#if DEBUG
        PrintSymbolTable();
#endif
    }

    /// <summary>
    /// Разбор программы: program <идентификатор>; <блок>.
    /// </summary>
    private void ParseProgram()
    {
        if (_currentSymbol == LexicalAnalyzer.LexicalAnalyzer.programsy)
        {
#if DEBUG
            Console.WriteLine("Найдено ключевое слово 'program'");
#endif
            NextSymbol();

            if (_currentSymbol == LexicalAnalyzer.LexicalAnalyzer.ident)
            {
#if DEBUG
                Console.WriteLine($"Имя программы: {_lexer.AddrName}");
#endif
                NextSymbol();

                if (_currentSymbol == LexicalAnalyzer.LexicalAnalyzer.semicolon)
                {
                    NextSymbol();
                    ParseBlock();

                    if (_currentSymbol == LexicalAnalyzer.LexicalAnalyzer.point)
                    {
                        Console.WriteLine("Программа завершена корректно");
                    }
                    else
                    {
                        SyntaxError(ERR_EXPECTED_DOT, "Ожидается точка в конце программы");
                    }
                }
                else
                {
                    SyntaxError(ERR_EXPECTED_SEMICOLON, "Ожидается точка с запятой после имени программы");
                }
            }
            else
            {
                SyntaxError(ERR_EXPECTED_IDENTIFIER, "Ожидается имя программы");
            }
        }
        else
        {
            ParseBlock();
        }
    }

    /// <summary>
    /// Разбор блока программы
    /// </summary>
    private void ParseBlock()
    {
        if (_currentSymbol == LexicalAnalyzer.LexicalAnalyzer.varsy)
        {
            ParseVariableDeclarations();
        }
        
        ParseCompoundStatement();
    }

    /// <summary>
    /// Разбор объявлений переменных
    /// </summary>
    private void ParseVariableDeclarations()
    {
#if DEBUG
        Console.WriteLine("\n=== АНАЛИЗ ОБЪЯВЛЕНИЙ ПЕРЕМЕННЫХ ===");
#endif
        NextSymbol();
        
        do
        {
            ParseVariableDeclaration();
            
            if (_currentSymbol == LexicalAnalyzer.LexicalAnalyzer.semicolon)
            {
                NextSymbol();
            }
            else
            {
                SyntaxError(ERR_EXPECTED_SEMICOLON, "Ожидается точка с запятой после объявления переменной");
                break;
            }
        } 
        while (_currentSymbol == LexicalAnalyzer.LexicalAnalyzer.ident);
    }

    /// <summary>
    /// Разбор одного объявления переменной
    /// </summary>
    private void ParseVariableDeclaration()
    {
        var identifiers = new List<string>();

        if (_currentSymbol == LexicalAnalyzer.LexicalAnalyzer.ident)
        {
            identifiers.Add(_lexer.AddrName);
            NextSymbol();
            
            while (_currentSymbol == LexicalAnalyzer.LexicalAnalyzer.comma)
            {
                NextSymbol();
                if (_currentSymbol == LexicalAnalyzer.LexicalAnalyzer.ident)
                {
                    identifiers.Add(_lexer.AddrName);
                    NextSymbol();
                }
                else
                {
                    SyntaxError(ERR_EXPECTED_IDENTIFIER, "Ожидается идентификатор после запятой");
                    return;
                }
            }
        }
        else
        {
            SyntaxError(ERR_EXPECTED_IDENTIFIER, "Ожидается идентификатор переменной");
            return;
        }

        switch (_currentSymbol)
        {
            case LexicalAnalyzer.LexicalAnalyzer.equal:
            {
                SyntaxError(ERR_EXPECTED_EQUALS, "Для объявления переменных используется двоеточие, а не знак равенства");
                NextSymbol();
                var typeInfo = ParseType();
            
                foreach (var identifier in identifiers)
                {
                    if (!_symbolTable.TryAdd(identifier, typeInfo))
                    {
                        SyntaxError(ERR_DUPLICATE_IDENTIFIER, $"Повторное объявление идентификатора '{identifier}'");
                    }
                    else
                    {
#if DEBUG
                        Console.WriteLine($"Объявлена константная переменная: {identifier} : {typeInfo}");
#endif
                    }
                }

                break;
            }
            
            case LexicalAnalyzer.LexicalAnalyzer.colon:
            {
                NextSymbol();
                var typeInfo = ParseType();

                foreach (var identifier in identifiers)
                {
                    if (!_symbolTable.TryAdd(identifier, typeInfo))
                    {
                        SyntaxError(ERR_DUPLICATE_IDENTIFIER, $"Повторное объявление идентификатора '{identifier}'");
                    }
                    else
                    {
#if DEBUG
                        Console.WriteLine($"Объявлена переменная: {identifier} : {typeInfo}");
#endif
                    }
                }

                break;
            }
            default:
                SyntaxError(ERR_EXPECTED_COLON, "Ожидается двоеточие после списка переменных");
                break;
        }
    }

    /// <summary>
    /// Разбор типа данных
    /// </summary>
    private VariableInfo ParseType()
    {
        if (_currentSymbol == LexicalAnalyzer.LexicalAnalyzer.lbracket)
        {
            SyntaxError(ERR_EXPECTED_ARRAY, "Ожидается ключевое слово 'array' перед объявлением массива");
            return ParseArrayType();
        }
        
        return _currentSymbol == LexicalAnalyzer.LexicalAnalyzer.arraysy ? ParseArrayType() : ParseSimpleType();
    }

    /// <summary>
    /// Разбор простого типа
    /// </summary>
    private VariableInfo ParseSimpleType()
    {
        var typeInfo = _currentSymbol switch
        {
            LexicalAnalyzer.LexicalAnalyzer.integersy => new VariableInfo(VariableType.Integer, false),
            LexicalAnalyzer.LexicalAnalyzer.realsy => new VariableInfo(VariableType.Real, false),
            LexicalAnalyzer.LexicalAnalyzer.charsy => new VariableInfo(VariableType.Char, false),
            LexicalAnalyzer.LexicalAnalyzer.booleansy => new VariableInfo(VariableType.Boolean, false),
            _ => throw new SyntaxException("Неизвестный тип данных")
        };
        
        if (typeInfo.Type is VariableType.Integer or VariableType.Real or VariableType.Char or VariableType.Boolean)
        {
            NextSymbol();
            return typeInfo;
        }
        
        SyntaxError(ERR_INVALID_TYPE, "Недопустимый тип данных");
        return new VariableInfo(VariableType.Integer, false);
    }

    /// <summary>
    /// Разбор типа массива
    /// </summary>
    private VariableInfo ParseArrayType()
    {
        NextSymbol();

        if (_currentSymbol == LexicalAnalyzer.LexicalAnalyzer.lbracket)
        {
            NextSymbol();
            ParseArrayRange();
            
            if (_currentSymbol == LexicalAnalyzer.LexicalAnalyzer.rbracket)
            {
                NextSymbol();
                
                if (_currentSymbol == LexicalAnalyzer.LexicalAnalyzer.ofsy)
                {
                    NextSymbol();
                    var elementType = ParseSimpleType();
                    return new VariableInfo(elementType.Type, true);
                }
                else
                {
                    SyntaxError(ERR_EXPECTED_OF, "Ожидается ключевое слово 'of' в объявлении массива");
                }
            }
            else
            {
                SyntaxError(ERR_EXPECTED_RBRACKET, "Ожидается закрывающая квадратная скобка");
            }
        }
        else
        {
            SyntaxError(ERR_EXPECTED_LBRACKET, "Ожидается открывающая квадратная скобка в объявлении массива");
        }
        
        return new VariableInfo(VariableType.Integer, true);
    }

    /// <summary>
    /// Разбор диапазона массива
    /// </summary>
    private void ParseArrayRange()
    {
        if (_currentSymbol is LexicalAnalyzer.LexicalAnalyzer.intc or LexicalAnalyzer.LexicalAnalyzer.ident)
        {
#if DEBUG
            Console.WriteLine($"[DEBUG][ParseArrayRange] Начало. Кандидат на нижнюю границу: {_currentSymbol}");
#endif
            
            NextSymbol();

            if (_currentSymbol == LexicalAnalyzer.LexicalAnalyzer.twopoints)
            {
#if DEBUG
                Console.WriteLine($"[DEBUG][ParseArrayRange] Найдено '..'. Текущий символ: {_currentSymbol}");
#endif
                NextSymbol();

                if (_currentSymbol is LexicalAnalyzer.LexicalAnalyzer.intc or LexicalAnalyzer.LexicalAnalyzer.ident)
                {
#if DEBUG
                    Console.WriteLine($"[DEBUG][ParseArrayRange] Кандидат на верхнюю границу: {_currentSymbol}");
#endif
                    NextSymbol();
                    
#if DEBUG
                    Console.WriteLine($"[DEBUG][ParseArrayRange] После использования верхней границы. _currentSymbol ДОЛЖЕН БЫТЬ ']': {_currentSymbol}");
#endif
                }
                else
                {
                    SyntaxError(ERR_EXPECTED_IDENTIFIER, "Ожидается верхняя граница диапазона");
                }
            }
            else
            {
#if DEBUG
                Console.WriteLine($"[DEBUG][ParseArrayRange] Ожидалось '..' (две точки, код 74), а получилось: {_currentSymbol}. Lexer.AddrName: '{_lexer.AddrName}'");
#endif
            }
        }
        else
        {
            SyntaxError(ERR_EXPECTED_IDENTIFIER, "Ожидается нижняя граница диапазона массива");
        }
    }

    /// <summary>
    /// Разбор составного оператора
    /// </summary>
    private void ParseCompoundStatement()
    {
        if (_currentSymbol == LexicalAnalyzer.LexicalAnalyzer.beginsy)
        {
#if DEBUG
            Console.WriteLine("\n=== АНАЛИЗ СОСТАВНОГО ОПЕРАТОРА ===");
#endif
            NextSymbol();
            
            ParseStatement();
            
            while (_currentSymbol == LexicalAnalyzer.LexicalAnalyzer.semicolon)
            {
                NextSymbol();
                if (_currentSymbol != LexicalAnalyzer.LexicalAnalyzer.endsy)
                {
                    ParseStatement();
                }
            }
            
            if (_currentSymbol == LexicalAnalyzer.LexicalAnalyzer.endsy)
            {
#if DEBUG
                Console.WriteLine("Составной оператор завершен корректно");
#endif
                NextSymbol();
            }
            else
            {
                SyntaxError(ERR_EXPECTED_END, "Ожидается ключевое слово 'end'");
            }
        }
        else
        {
            SyntaxError(ERR_EXPECTED_BEGIN, "Ожидается ключевое слово 'begin'");
        }
    }

    /// <summary>
    /// Разбор оператора
    /// </summary>
    private void ParseStatement()
    {
        switch (_currentSymbol)
        {
            case LexicalAnalyzer.LexicalAnalyzer.ident:
                ParseAssignmentStatement();
                break;
            
            case LexicalAnalyzer.LexicalAnalyzer.beginsy:
                ParseCompoundStatement();
                break;
        }
    }

    /// <summary>
    /// Разбор оператора присваивания
    /// </summary>
    private void ParseAssignmentStatement()
    {
        var variableName = _lexer.AddrName;

        if (!_symbolTable.TryGetValue(variableName, out var value))
        {
            SyntaxError(ERR_UNDECLARED_IDENTIFIER, $"Необъявленная переменная '{variableName}'");
            return;
        }

        NextSymbol();

        if (_currentSymbol == LexicalAnalyzer.LexicalAnalyzer.lbracket)
        {
            if (!value.IsArray)
            {
                SyntaxError(ERR_INVALID_ARRAY_INDEX, $"Переменная '{variableName}' не является массивом");
            }
            
            NextSymbol();
            ParseExpression();
            
            if (_currentSymbol == LexicalAnalyzer.LexicalAnalyzer.rbracket)
            {
                NextSymbol();
            }
            else
            {
                SyntaxError(ERR_EXPECTED_RBRACKET, "Ожидается закрывающая квадратная скобка");
            }
        }

        if (_currentSymbol == LexicalAnalyzer.LexicalAnalyzer.assign)
        {
            NextSymbol();
            ParseExpression();
#if DEBUG
            Console.WriteLine($"Присваивание переменной '{variableName}' выполнено корректно");
#endif
        }
        else
        {
            SyntaxError(ERR_EXPECTED_ASSIGN, "Ожидается оператор присваивания ':='");
        }
    }

    /// <summary>
    /// Упрощенный разбор выражения
    /// </summary>
    private void ParseExpression()
    {
        ParseTerm();
        
        while (_currentSymbol is LexicalAnalyzer.LexicalAnalyzer.plus or LexicalAnalyzer.LexicalAnalyzer.minus)
        {
            NextSymbol();
            ParseTerm();
        }
    }

    /// <summary>
    /// Разбор терма (множители и деление)
    /// </summary>
    private void ParseTerm()
    {
        ParseFactor();
        
        while (_currentSymbol is LexicalAnalyzer.LexicalAnalyzer.star or LexicalAnalyzer.LexicalAnalyzer.slash)
        {
            NextSymbol();
            ParseFactor();
        }
    }

    /// <summary>
    /// Разбор фактора (число, переменная, выражение в скобках)
    /// </summary>
    private void ParseFactor()
    {
        switch (_currentSymbol)
        {
            case LexicalAnalyzer.LexicalAnalyzer.intc or LexicalAnalyzer.LexicalAnalyzer.floatc 
                 or LexicalAnalyzer.LexicalAnalyzer.charc:
                NextSymbol();
                break;
            
            case LexicalAnalyzer.LexicalAnalyzer.ident:
            {
                var variableName = _lexer.AddrName;
                if (!_symbolTable.ContainsKey(variableName))
                {
                    SyntaxError(ERR_UNDECLARED_IDENTIFIER, $"Необъявленная переменная '{variableName}'");
                }
            
                NextSymbol();

                if (_currentSymbol == LexicalAnalyzer.LexicalAnalyzer.lbracket)
                {
                    if (_symbolTable.TryGetValue(variableName, out var value) && !value.IsArray)
                    {
                        SyntaxError(ERR_INVALID_ARRAY_INDEX, $"Переменная '{variableName}' не является массивом");
                    }
                
                    NextSymbol();
                    ParseExpression();
                
                    if (_currentSymbol == LexicalAnalyzer.LexicalAnalyzer.rbracket)
                    {
                        NextSymbol();
                    }
                    else
                    {
                        SyntaxError(ERR_EXPECTED_RBRACKET, "Ожидается закрывающая квадратная скобка");
                    }
                }

                break;
            }
            case LexicalAnalyzer.LexicalAnalyzer.leftpar:
            {
                NextSymbol();
                ParseExpression();
            
                if (_currentSymbol == LexicalAnalyzer.LexicalAnalyzer.rightpar)
                {
                    NextSymbol();
                }
                else
                {
                    SyntaxError(9, "Ожидается закрывающая скобка");
                }

                break;
            }
            default:
                SyntaxError(ERR_EXPECTED_IDENTIFIER, "Ожидается число, переменная или выражение в скобках");
                break;
        }
    }

    /// <summary>
    /// Переход к следующему символу
    /// </summary>
    protected void NextSymbol()
    {
        _currentSymbol = _lexer.NextSym();
    }

    /// <summary>
    /// Обработка синтаксической ошибки
    /// </summary>
    protected void SyntaxError(byte errorCode, string message)
    {
        InputOutput.Error(errorCode);
#if DEBUG
        Console.WriteLine($"[СИНТАКСИЧЕСКАЯ ОШИБКА {errorCode}] {message}");
#endif

        while (_currentSymbol != 0 && 
               _currentSymbol != LexicalAnalyzer.LexicalAnalyzer.semicolon &&
               _currentSymbol != LexicalAnalyzer.LexicalAnalyzer.endsy &&
               _currentSymbol != LexicalAnalyzer.LexicalAnalyzer.beginsy)
        {
            NextSymbol();
        }
    }

    /// <summary>
    /// Вывод таблицы символов
    /// </summary>
    private void PrintSymbolTable()
    {
        Console.WriteLine("\n=== ТАБЛИЦА СИМВОЛОВ ===");
        if (_symbolTable.Count == 0)
        {
            Console.WriteLine("Таблица символов пуста");
        }
        else
        {
            Console.WriteLine($"{"Имя",-15} {"Тип",-10} {"Массив",-8}");
            Console.WriteLine(new string('-', 35));
            
            foreach (var kvp in _symbolTable)
            {
                var arrayStr = kvp.Value.IsArray ? "Да" : "Нет";
                Console.WriteLine($"{kvp.Key,-15} {kvp.Value.Type,-10} {arrayStr,-8}");
            }
        }
        Console.WriteLine("========================\n");
    }
}