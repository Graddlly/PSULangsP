using PascalCompiler.SyntaxAnalyzer.Assets;

namespace PascalCompiler.SemanticAnalyzer;

/// <summary>
/// Расширенный синтаксический анализатор с интегрированным семантическим анализом
/// </summary>
public class EnhancedSyntaxAnalyzer : SyntaxAnalyzer.SyntaxAnalyzer
{
    private readonly SemanticAnalyzer _semanticAnalyzer;

    public EnhancedSyntaxAnalyzer()
    {
        _semanticAnalyzer = new SemanticAnalyzer(_symbolTable);
    }

    /// <summary>
    /// Основной метод анализа с семантической проверкой
    /// </summary>
    public new void AnalyzeProgram()
    {
        try
        {
            base.AnalyzeProgram();
#if DEBUG
            Console.WriteLine("\n=== СЕМАНТИЧЕСКИЙ АНАЛИЗ ===");
            _semanticAnalyzer.PrintSemanticReport();
            _semanticAnalyzer.AnalyzeVariableUsage();

            Console.WriteLine(!_semanticAnalyzer.HasSemanticErrors()
                ? "=== АНАЛИЗ ЗАВЕРШЕН УСПЕШНО ==="
                : "=== АНАЛИЗ ЗАВЕРШЕН С СЕМАНТИЧЕСКИМИ ОШИБКАМИ ===");
#endif
        }
        catch (SyntaxException ex)
        {
            Console.WriteLine($"\n=== КРИТИЧЕСКАЯ СИНТАКСИЧЕСКАЯ ОШИБКА ===");
            Console.WriteLine($"Ошибка: {ex.Message}");
            Console.WriteLine("Семантический анализ не выполнен из-за синтаксических ошибок");
        }
    }

    /// <summary>
    /// Переопределенный разбор оператора присваивания с семантической проверкой
    /// </summary>
    protected override void ParseAssignmentStatement()
    {
        var variableName = _lexer.AddrName;

        if (!_symbolTable.TryGetValue(variableName, out var variableInfo))
        {
            SyntaxError(ERR_UNDECLARED_IDENTIFIER, $"Необъявленная переменная '{variableName}'");
            return;
        }

        NextSymbol();
        var isArrayElement = false;
        
        if (_currentSymbol == LexicalAnalyzer.LexicalAnalyzer.lbracket)
        {
            if (!variableInfo.IsArray)
            {
                SyntaxError(ERR_INVALID_ARRAY_INDEX, $"Переменная '{variableName}' не является массивом");
            }

            isArrayElement = true;
            NextSymbol();
            
            var indexType = ParseExpression();
            _semanticAnalyzer.CheckArrayIndex(indexType);

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
            var expressionType = ParseExpression();
            
            _semanticAnalyzer.CheckAssignment(variableName, expressionType, isArrayElement);

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
    /// Переопределенный разбор выражения с возвращением типа
    /// </summary>
    private VariableType ParseExpression()
    {
        var leftType = ParseTerm();

        while (_currentSymbol is LexicalAnalyzer.LexicalAnalyzer.plus or LexicalAnalyzer.LexicalAnalyzer.minus)
        {
            var operation = _currentSymbol == LexicalAnalyzer.LexicalAnalyzer.plus ? "+" : "-";
            NextSymbol();
            var rightType = ParseTerm();
            
            leftType = _semanticAnalyzer.CheckBinaryOperation(leftType, rightType, operation);
        }

        return leftType;
    }

    /// <summary>
    /// Переопределенный разбор терма с возвращением типа
    /// </summary>
    private VariableType ParseTerm()
    {
        var leftType = ParseFactor();

        while (_currentSymbol is LexicalAnalyzer.LexicalAnalyzer.star or LexicalAnalyzer.LexicalAnalyzer.slash)
        {
            var operation = _currentSymbol == LexicalAnalyzer.LexicalAnalyzer.star ? "*" : "/";
            NextSymbol();
            var rightType = ParseFactor();
            
            leftType = _semanticAnalyzer.CheckBinaryOperation(leftType, rightType, operation);
        }

        return leftType;
    }

    /// <summary>
    /// Переопределенный разбор фактора с возвращением типа
    /// </summary>
    private VariableType ParseFactor()
    {
        switch (_currentSymbol)
        {
            case LexicalAnalyzer.LexicalAnalyzer.intc:
                NextSymbol();
                return VariableType.Integer;

            case LexicalAnalyzer.LexicalAnalyzer.floatc:
                NextSymbol();
                return VariableType.Real;

            case LexicalAnalyzer.LexicalAnalyzer.charc:
                NextSymbol();
                return VariableType.Char;
            
            case LexicalAnalyzer.LexicalAnalyzer.truesy:
            case LexicalAnalyzer.LexicalAnalyzer.falsesy:
                NextSymbol();
                return VariableType.Boolean;

            case LexicalAnalyzer.LexicalAnalyzer.ident:
            {
                var variableName = _lexer.AddrName;
                NextSymbol();
                var isArrayElement = false;

                if (_currentSymbol == LexicalAnalyzer.LexicalAnalyzer.lbracket)
                {
                    isArrayElement = true;
                    NextSymbol();
                    var indexType = ParseExpression();
                    _semanticAnalyzer.CheckArrayIndex(indexType);

                    if (_currentSymbol == LexicalAnalyzer.LexicalAnalyzer.rbracket)
                    {
                        NextSymbol();
                    }
                    else
                    {
                        SyntaxError(ERR_EXPECTED_RBRACKET, "Ожидается закрывающая квадратная скобка");
                    }
                }

                return _semanticAnalyzer.CheckVariableUsage(variableName, isArrayElement);
            }

            case LexicalAnalyzer.LexicalAnalyzer.leftpar:
            {
                NextSymbol();
                var expressionType = ParseExpression();

                if (_currentSymbol == LexicalAnalyzer.LexicalAnalyzer.rightpar)
                {
                    NextSymbol();
                }
                else
                {
                    SyntaxError(9, "Ожидается закрывающая скобка");
                }

                return expressionType;
            }

            default:
                SyntaxError(ERR_EXPECTED_IDENTIFIER, "Ожидается число, переменная или выражение в скобках");
                return VariableType.Integer;
        }
    }

    /// <summary>
    /// Получить ссылку на семантический анализатор
    /// </summary>
    public SemanticAnalyzer GetSemanticAnalyzer()
    {
        return _semanticAnalyzer;
    }
}