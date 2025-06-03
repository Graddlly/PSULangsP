using PascalCompiler.IOModule;
using PascalCompiler.IOModule.Models;
using PascalCompiler.LexicalAnalyzer.Assets;

// ReSharper disable InconsistentNaming
namespace PascalCompiler.LexicalAnalyzer;

public class LexicalAnalyzer
{
    public const byte
        star = 21, // *
        slash = 60, // /
        equal = 16, // =
        comma = 20, // ,
        semicolon = 14, // ;
        colon = 5, // :
        point = 61,	// .
        arrow = 62,	// ^
        leftpar = 9,	// (
        rightpar = 4,	// )
        lbracket = 11,	// [
        rbracket = 12,	// ]
        flpar = 63,	// {
        frpar = 64,	// }
        later = 65,	// <
        greater = 66,	// >
        laterequal = 67,	//  <=
        greaterequal = 68,	//  >=
        latergreater = 69,	//  <>
        plus = 70,	// +
        minus = 71,	// –
        lcomment = 72,	//  (*
        rcomment = 73,	//  *)
        assign = 51,	//  :=
        twopoints = 74,	//  ..
        ident = 2,	// идентификатор
        floatc = 82,	// вещественная константа
        intc = 15,	// целая константа
        charc = 83, // символьная константа
        casesy = 31,
        elsesy = 32,
        filesy = 57,
        gotosy = 33,
        thensy = 52,
        typesy = 34,
        untilsy = 53,
        dosy = 54,
        withsy = 37,
        ifsy = 56,
        insy = 100,
        ofsy = 101,
        orsy = 102,
        tosy = 103,
        endsy = 104,
        varsy = 105,
        divsy = 106,
        andsy = 107,
        notsy = 108,
        forsy = 109,
        modsy = 110,
        nilsy = 111,
        setsy = 112,
        beginsy = 113,
        whilesy = 114,
        arraysy = 115,
        constsy = 116,
        labelsy = 117,
        downtosy = 118,
        packedsy = 119,
        recordsy = 120,
        repeatsy = 121,
        programsy = 122,
        functionsy = 123,
        procedurensy = 124,
        integersy = 125,
        realsy = 126,
        charsy = 127;
    
    private const byte ERR_NUMBER_OVERFLOW = 203;
    private const byte ERR_INVALID_CHAR = 1;
    private const byte ERR_UNCLOSED_STRING = 7;
    private const byte ERR_UNCLOSED_COMMENT = 12;

    private TextPosition _token; // позиция символа
    private readonly Keywords _keywords; // таблица ключевых слов

    public byte Symbol { get; private set; }
    public TextPosition Token { get; private set; }
    public string AddrName { get; private set; }
    public int NmbInt { get; private set; }
    public float NmbFloat { get; private set; }
    public char OneSymbol { get; private set; }

    public LexicalAnalyzer()
    {
        _keywords = new Keywords();
        Symbol = 0;
        _token = new TextPosition();
        AddrName = "";
        NmbInt = 0;
        NmbFloat = 0;
        OneSymbol = '\0';
    }

    /// <summary>
    /// Получение следующего символа из входного потока
    /// </summary>
    /// <returns>Код символа</returns>
    public byte NextSym()
    {
        while (true)
        {
            while (InputOutput.Ch == ' ' || InputOutput.Ch == '\t') InputOutput.NextCh();

            _token.lineNumber = InputOutput.PositionNow.lineNumber;
            _token.charNumber = InputOutput.PositionNow.charNumber;

            if (InputOutput.IsEndOfFile())
            {
                return 0;
            }

            switch (InputOutput.Ch)
            {
                case >= '0' and <= '9':
                    ScanNumber();
                    break;

                case >= 'a' and <= 'z':
                case >= 'A' and <= 'Z':
                    ScanIdentifierOrKeyword();
                    break;

                case '\'':
                    ScanCharacterConstant();
                    break;

                case '<':
                    ScanLessOperator();
                    break;

                case '>':
                    ScanGreaterOperator();
                    break;

                case ':':
                    ScanColonOperator();
                    break;

                case '.':
                    ScanPointOperator();
                    break;

                case '(':
                    ScanLeftParenthesis();
                    break;

                case ')':
                    Symbol = rightpar;
                    InputOutput.NextCh();
                    break;

                case '[':
                    Symbol = lbracket;
                    InputOutput.NextCh();
                    break;

                case ']':
                    Symbol = rbracket;
                    InputOutput.NextCh();
                    break;

                case '{':
                    ScanCurlyBraceComment();
                    break;

                case '}':
                    Symbol = frpar;
                    InputOutput.NextCh();
                    break;

                case ';':
                    Symbol = semicolon;
                    InputOutput.NextCh();
                    break;

                case ',':
                    Symbol = comma;
                    InputOutput.NextCh();
                    break;

                case '=':
                    Symbol = equal;
                    InputOutput.NextCh();
                    break;

                case '+':
                    Symbol = plus;
                    InputOutput.NextCh();
                    break;

                case '-':
                    Symbol = minus;
                    InputOutput.NextCh();
                    break;

                case '*':
                    Symbol = star;
                    InputOutput.NextCh();
                    break;

                case '/':
                    Symbol = slash;
                    InputOutput.NextCh();
                    break;

                case '^':
                    Symbol = arrow;
                    InputOutput.NextCh();
                    break;

                case '\n':
                case '\r':
                    InputOutput.NextCh();
                    continue;

                default:
                    InputOutput.Error(ERR_INVALID_CHAR);
                    InputOutput.NextCh();
                    continue;
            }
            
            return Symbol;
        }
    }

    /// <summary>
    /// Сканирование числовой константы
    /// </summary>
    private void ScanNumber()
    {
        const int MAX_INT = short.MaxValue;
        NmbInt = 0;
        var overflow = false;
        
        while (InputOutput.Ch >= '0' && InputOutput.Ch <= '9')
        {
            var digit = (byte)(InputOutput.Ch - '0');
            
            if (NmbInt <= MAX_INT / 10 && 
                (NmbInt < MAX_INT / 10 || digit <= MAX_INT % 10))
            {
                NmbInt = 10 * NmbInt + digit;
            }
            else
            {
                overflow = true;
            }
            
            InputOutput.NextCh();
        }
        
        if (InputOutput.Ch == '.')
        {
            InputOutput.NextCh();
            if (InputOutput.Ch >= '0' && InputOutput.Ch <= '9')
            {
                ScanFloatNumber();
                return;
            }
            else
            {
                Symbol = intc;
            }
        }
        else
        {
            Symbol = intc;
        }

        if (overflow)
        {
            InputOutput.Error(ERR_NUMBER_OVERFLOW);
            NmbInt = 0;
        }
    }

    /// <summary>
    /// Сканирование вещественного числа
    /// </summary>
    private void ScanFloatNumber()
    {
        NmbFloat = NmbInt;
        var fraction = 0.1f;
        
        while (InputOutput.Ch >= '0' && InputOutput.Ch <= '9')
        {
            NmbFloat += (InputOutput.Ch - '0') * fraction;
            fraction /= 10;
            InputOutput.NextCh();
        }

        Symbol = floatc;
    }

    /// <summary>
    /// Сканирование идентификатора или ключевого слова
    /// </summary>
    private void ScanIdentifierOrKeyword()
    {
        AddrName = "";
        
        while ((InputOutput.Ch >= 'a' && InputOutput.Ch <= 'z') ||
               (InputOutput.Ch >= 'A' && InputOutput.Ch <= 'Z') ||
               (InputOutput.Ch >= '0' && InputOutput.Ch <= '9') ||
               InputOutput.Ch == '_')
        {
            AddrName += InputOutput.Ch;
            InputOutput.NextCh();
        }
        
        var length = (byte)AddrName.Length;
        if (_keywords.Keyword.TryGetValue(length, out var value) && 
            value.ContainsKey(AddrName.ToLower()))
        {
            Symbol = value[AddrName.ToLower()];
        }
        else
        {
            Symbol = ident;
        }
    }

    /// <summary>
    /// Сканирование символьной константы
    /// </summary>
    private void ScanCharacterConstant()
    {
        InputOutput.NextCh();
        
        if (InputOutput.Ch == '\'')
        {
            OneSymbol = '\0';
            InputOutput.NextCh();
        }
        else if (InputOutput.IsEndOfFile() || InputOutput.Ch == '\n')
        {
            InputOutput.Error(ERR_UNCLOSED_STRING);
            OneSymbol = '\0';
        }
        else
        {
            OneSymbol = InputOutput.Ch;
            InputOutput.NextCh();
            
            if (InputOutput.Ch == '\'')
            {
                InputOutput.NextCh();
            }
            else
            {
                InputOutput.Error(ERR_UNCLOSED_STRING);
            }
        }
        
        Symbol = charc;
    }

    /// <summary>
    /// Сканирование операторов сравнения, начинающихся с '<'
    /// </summary>
    private void ScanLessOperator()
    {
        InputOutput.NextCh();
        switch (InputOutput.Ch)
        {
            case '=':
                Symbol = laterequal;
                InputOutput.NextCh();
                break;
            case '>':
                Symbol = latergreater;
                InputOutput.NextCh();
                break;
            default:
                Symbol = later;
                break;
        }
    }

    /// <summary>
    /// Сканирование операторов сравнения, начинающихся с '>'
    /// </summary>
    private void ScanGreaterOperator()
    {
        InputOutput.NextCh();
        if (InputOutput.Ch == '=')
        {
            Symbol = greaterequal;
            InputOutput.NextCh();
        }
        else
        {
            Symbol = greater;
        }
    }

    /// <summary>
    /// Сканирование операторов, начинающихся с ':'
    /// </summary>
    private void ScanColonOperator()
    {
        InputOutput.NextCh();
        if (InputOutput.Ch == '=')
        {
            Symbol = assign;
            InputOutput.NextCh();
        }
        else
        {
            Symbol = colon;
        }
    }

    /// <summary>
    /// Сканирование операторов, начинающихся с '.'
    /// </summary>
    private void ScanPointOperator()
    {
        InputOutput.NextCh();
        if (InputOutput.Ch == '.')
        {
            Symbol = twopoints;
            InputOutput.NextCh();
        }
        else
        {
            Symbol = point;
        }
    }

    /// <summary>
    /// Сканирование левой скобки и комментариев
    /// </summary>
    private void ScanLeftParenthesis()
    {
        InputOutput.NextCh();
        if (InputOutput.Ch == '*')
        {
            ScanParenthesesComment();
        }
        else
        {
            Symbol = leftpar;
        }
    }

    /// <summary>
    /// Сканирование комментария в фигурных скобках
    /// </summary>
    private void ScanCurlyBraceComment()
    {
        InputOutput.NextCh();
        
        while (!InputOutput.IsEndOfFile() && InputOutput.Ch != '}')
        {
            InputOutput.NextCh();
        }
        
        if (InputOutput.Ch == '}')
        {
            InputOutput.NextCh();
            Symbol = NextSym();
        }
        else
        {
            InputOutput.Error(ERR_UNCLOSED_COMMENT);
            Symbol = 0;
        }
    }

    /// <summary>
    /// Сканирование комментария в скобках (* *)
    /// </summary>
    private void ScanParenthesesComment()
    {
        InputOutput.NextCh();
        
        while (!InputOutput.IsEndOfFile())
        {
            if (InputOutput.Ch == '*')
            {
                InputOutput.NextCh();
                if (InputOutput.Ch == ')')
                {
                    InputOutput.NextCh();
                    Symbol = NextSym();
                    return;
                }
            }
            else
            {
                InputOutput.NextCh();
            }
        }
        
        InputOutput.Error(ERR_UNCLOSED_COMMENT);
        Symbol = 0;
    }
    
    /// <summary>
    /// Получение значения токена для отображения
    /// </summary>
    /// <param name="lexer">Лексический анализатор</param>
    /// <param name="symbolCode">Код символа</param>
    /// <returns>Строковое представление значения</returns>
    private string GetTokenValueInt()
    {
        return Symbol switch
        {
            ident => AddrName,
            intc => NmbInt.ToString(),
            floatc => NmbFloat.ToString(),
            charc => $"'{OneSymbol}'",
            _ => "" 
        };
    }
    
    /// <summary>
    /// Основной анализ
    /// </summary>
    /// <param name="tokenAnalyzer">Главный tokenAnalyzer</param>
    /// <param name="processedTokenCount">Кол-во проанализированных токенов</param>
    /// <returns></returns>
    public List<byte> AnalyzeProgram(TokenAnalyzer tokenAnalyzer, out int processedTokenCount)
    {
        var symbolCodesList = new List<byte>();
        processedTokenCount = 0;
        var programBlockDepth = 0;
        var foundProgramEndKeyword = false; 

        byte currentSymbolCode;
        do
        {
            currentSymbolCode = NextSym(); 

            if (currentSymbolCode != 0) 
            {
                processedTokenCount++;
                symbolCodesList.Add(currentSymbolCode);

                var tokenValue = GetTokenValueInt();
                tokenAnalyzer.AddToken(currentSymbolCode, tokenValue, Token);

                switch (currentSymbolCode)
                {
                    case beginsy:
                        programBlockDepth++;
                        break;
                    
                    case endsy:
                    {
                        programBlockDepth--;
                        if (programBlockDepth == 0) 
                        {
                            foundProgramEndKeyword = true;
                        }

                        break;
                    }
                    
                    default:
                    {
                        if (foundProgramEndKeyword) 
                        {
                            if (currentSymbolCode == point)
                            {
                                foundProgramEndKeyword = false; 
                            }
                            else
                            {
                                InputOutput.Error(2, Token); 
                                foundProgramEndKeyword = false; 
                            }
                        }

                        break;
                    }
                }
            }
        } while (currentSymbolCode != 0 && !InputOutput.IsEndOfFile()); 

        if (foundProgramEndKeyword) 
        {
            InputOutput.Error(2, Token); 
        }
        
        return symbolCodesList;
    }

    /// <summary>
    /// Вывод таблицы соответствий символов и кодов
    /// </summary>
    public void PrintSymbolTable()
    {
        Console.WriteLine("=== ТАБЛИЦА СИМВОЛОВ ===");
        Console.WriteLine("Операторы:");
        Console.WriteLine($"*  -> {star}");
        Console.WriteLine($"/  -> {slash}");
        Console.WriteLine($"=  -> {equal}");
        Console.WriteLine($",  -> {comma}");
        Console.WriteLine($";  -> {semicolon}");
        Console.WriteLine($":  -> {colon}");
        Console.WriteLine($".  -> {point}");
        Console.WriteLine($"^  -> {arrow}");
        Console.WriteLine($"(  -> {leftpar}");
        Console.WriteLine($")  -> {rightpar}");
        Console.WriteLine($"[  -> {lbracket}");
        Console.WriteLine($"]  -> {rbracket}");
        Console.WriteLine($"{{  -> {flpar}");
        Console.WriteLine($"}}  -> {frpar}");
        Console.WriteLine($"<  -> {later}");
        Console.WriteLine($">  -> {greater}");
        Console.WriteLine($"<= -> {laterequal}");
        Console.WriteLine($">= -> {greaterequal}");
        Console.WriteLine($"<> -> {latergreater}");
        Console.WriteLine($"+  -> {plus}");
        Console.WriteLine($"-  -> {minus}");
        Console.WriteLine($":= -> {assign}");
        Console.WriteLine($".. -> {twopoints}");
        Console.WriteLine($"(* -> {lcomment}");
        Console.WriteLine($"*) -> {rcomment}");
        Console.WriteLine();
        
        Console.WriteLine("Типы токенов:");
        Console.WriteLine($"Идентификатор -> {ident}");
        Console.WriteLine($"Целая константа -> {intc}");
        Console.WriteLine($"Вещественная константа -> {floatc}");
        Console.WriteLine($"Символьная константа -> {charc}");
        Console.WriteLine();
        
        Console.WriteLine("Ключевые слова:");
        foreach (var keyword in _keywords.Keyword.SelectMany(lengthGroup => lengthGroup.Value))
        {
            Console.WriteLine($"{keyword.Key} -> {keyword.Value}");
        }
        Console.WriteLine("========================\n");
    }
}