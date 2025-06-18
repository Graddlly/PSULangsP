using PascalCompiler.IOModule;
using PascalCompiler.SemanticAnalyzer.Assets;
using PascalCompiler.SyntaxAnalyzer.Assets;

namespace PascalCompiler.SemanticAnalyzer;
// ReSharper disable InconsistentNaming

/// <summary>
/// Семантический анализатор для языка Pascal
/// Поддерживает:
/// - Проверку типов при присваивании
/// - Проверку границ массивов
/// - Проверку совместимости типов в выражениях
/// - Анализ использования переменных
/// </summary>
public class SemanticAnalyzer
{
    private readonly Dictionary<string, VariableInfo> _symbolTable;
    private readonly List<SemanticError> _semanticErrors;
    
    private const byte ERR_TYPE_MISMATCH = 30;
    private const byte ERR_ARRAY_INDEX_TYPE = 31;
    private const byte ERR_NON_ARRAY_INDEXING = 32;
    private const byte ERR_ARRAY_WITHOUT_INDEX = 33;
    private const byte ERR_INCOMPATIBLE_TYPES = 34;
    private const byte ERR_UNINITIALIZED_VARIABLE = 35;

    public SemanticAnalyzer(Dictionary<string, VariableInfo> symbolTable)
    {
        _symbolTable = symbolTable;
        _semanticErrors = [];
    }

    /// <summary>
    /// Проверка семантической корректности присваивания
    /// </summary>
    public void CheckAssignment(string variableName, VariableType expressionType, bool isArrayElement = false)
    {
        if (!_symbolTable.TryGetValue(variableName, out var variableInfo))
        {
            return;
        }
        
        switch (variableInfo.IsArray)
        {
            case true when !isArrayElement:
                AddSemanticError(ERR_ARRAY_WITHOUT_INDEX, 
                    $"Массив '{variableName}' должен использоваться с индексом");
                return;
            
            case false when isArrayElement:
                AddSemanticError(ERR_NON_ARRAY_INDEXING, 
                    $"Переменная '{variableName}' не является массивом");
                return;
        }

        if (!AreTypesCompatible(variableInfo.Type, expressionType))
        {
            AddSemanticError(ERR_TYPE_MISMATCH, 
                $"Несовместимые типы: нельзя присвоить {GetTypeDescription(expressionType)} переменной " + 
                $"типа {GetTypeDescription(variableInfo.Type)}");
        }
        
        variableInfo.IsInitialized = true;
    }

    /// <summary>
    /// Проверка типа индекса массива
    /// </summary>
    public void CheckArrayIndex(VariableType indexType)
    {
        if (indexType != VariableType.Integer)
        {
            AddSemanticError(ERR_ARRAY_INDEX_TYPE, 
                $"Индекс массива должен быть целочисленным типом, получен: {GetTypeDescription(indexType)}");
        }
    }

    /// <summary>
    /// Проверка использования переменной в выражении
    /// </summary>
    public VariableType CheckVariableUsage(string variableName, bool isArrayElement = false)
    {
        if (!_symbolTable.TryGetValue(variableName, out var variableInfo))
        {
            return VariableType.Integer;
        }
        
        switch (variableInfo.IsArray)
        {
            case true when !isArrayElement:
                AddSemanticError(ERR_ARRAY_WITHOUT_INDEX, 
                    $"Массив '{variableName}' должен использоваться с индексом");
                return variableInfo.Type;
            
            case false when isArrayElement:
                AddSemanticError(ERR_NON_ARRAY_INDEXING, 
                    $"Переменная '{variableName}' не является массивом");
                return variableInfo.Type;
        }

        if (!variableInfo.IsInitialized)
        {
            AddSemanticError(ERR_UNINITIALIZED_VARIABLE, 
                $"Переменная '{variableName}' используется до инициализации");
        }

        return variableInfo.Type;
    }

    /// <summary>
    /// Проверка совместимости типов в бинарной операции
    /// </summary>
    public VariableType CheckBinaryOperation(VariableType leftType, VariableType rightType, string operation)
    {
        return operation switch
        {
            "+" or "-" or "*" => CheckArithmeticOperation(leftType, rightType),
            "/" => CheckDivisionOperation(leftType, rightType),
            "div" or "mod" => CheckIntegerOperation(leftType, rightType),
            _ => leftType
        };
    }

    /// <summary>
    /// Проверка арифметических операций
    /// </summary>
    private VariableType CheckArithmeticOperation(VariableType leftType, VariableType rightType)
    {
        if (IsNumericType(leftType) && IsNumericType(rightType))
        {
            if (leftType == VariableType.Real || rightType == VariableType.Real)
                return VariableType.Real;
            
            return VariableType.Integer;
        }

        AddSemanticError(ERR_INCOMPATIBLE_TYPES, 
            $"Арифметическая операция требует числовые типы, получены: " + 
            $"{GetTypeDescription(leftType)} и {GetTypeDescription(rightType)}");
        
        return VariableType.Integer;
    }

    /// <summary>
    /// Проверка операции деления
    /// </summary>
    private VariableType CheckDivisionOperation(VariableType leftType, VariableType rightType)
    {
        if (IsNumericType(leftType) && IsNumericType(rightType))
        {
            return VariableType.Real;
        }

        AddSemanticError(ERR_INCOMPATIBLE_TYPES, 
            $"Операция деления требует числовые типы, получены: {GetTypeDescription(leftType)} и {GetTypeDescription(rightType)}");
        
        return VariableType.Real;
    }

    /// <summary>
    /// Проверка целочисленных операций (div, mod)
    /// </summary>
    private VariableType CheckIntegerOperation(VariableType leftType, VariableType rightType)
    {
        if (leftType == VariableType.Integer && rightType == VariableType.Integer)
        {
            return VariableType.Integer;
        }

        AddSemanticError(ERR_INCOMPATIBLE_TYPES, 
            $"Целочисленная операция требует тип Integer, получены: {GetTypeDescription(leftType)} и {GetTypeDescription(rightType)}");
        
        return VariableType.Integer;
    }

    /// <summary>
    /// Проверка совместимости типов
    /// </summary>
    private static bool AreTypesCompatible(VariableType targetType, VariableType sourceType)
    {
        if (targetType == sourceType)
            return true;
        
        return targetType == VariableType.Real && sourceType == VariableType.Integer;
    }

    /// <summary>
    /// Проверка, является ли тип числовым
    /// </summary>
    private static bool IsNumericType(VariableType type)
    {
        return type is VariableType.Integer or VariableType.Real;
    }

    /// <summary>
    /// Получение описания типа
    /// </summary>
    private static string GetTypeDescription(VariableType type)
    {
        return type switch
        {
            VariableType.Integer => "Integer",
            VariableType.Real => "Real",
            VariableType.Char => "Char",
            VariableType.Boolean => "Boolean",
            _ => "Unknown"
        };
    }

    /// <summary>
    /// Добавление семантической ошибки
    /// </summary>
    private void AddSemanticError(byte errorCode, string message)
    {
        var error = new SemanticError(errorCode, message);
        _semanticErrors.Add(error);
        
        InputOutput.Error(errorCode);
        
#if DEBUG
        Console.WriteLine($"[СЕМАНТИЧЕСКАЯ ОШИБКА {errorCode}] {message}");
#endif
    }

    /// <summary>
    /// Получение списка семантических ошибок
    /// </summary>
    public List<SemanticError> GetSemanticErrors()
    {
        return [.._semanticErrors];
    }

    /// <summary>
    /// Проверка, есть ли семантические ошибки
    /// </summary>
    public bool HasSemanticErrors()
    {
        return _semanticErrors.Count > 0;
    }

    /// <summary>
    /// Вывод отчета о семантическом анализе
    /// </summary>
    public void PrintSemanticReport()
    {
        Console.WriteLine("\n=== ОТЧЕТ СЕМАНТИЧЕСКОГО АНАЛИЗА ===");
        
        if (_semanticErrors.Count == 0)
        {
            Console.WriteLine("Семантических ошибок не обнаружено");
        }
        else
        {
            Console.WriteLine($"Обнаружено семантических ошибок: {_semanticErrors.Count}");
            Console.WriteLine();
            
            for (var i = 0; i < _semanticErrors.Count; i++)
            {
                var error = _semanticErrors[i];
                Console.WriteLine($"{i + 1}. Ошибка {error.ErrorCode}: {error.Message}");
            }
        }
        
        Console.WriteLine("===================================\n");
    }

    /// <summary>
    /// Анализ использования переменных (неиспользуемые переменные)
    /// </summary>
    public void AnalyzeVariableUsage()
    {
        Console.WriteLine("\n=== АНАЛИЗ ИСПОЛЬЗОВАНИЯ ПЕРЕМЕННЫХ ===");
        
        var unusedVariables = _symbolTable
            .Where(kvp => !kvp.Value.IsInitialized)
            .Select(kvp => kvp.Key)
            .ToList();
        
        if (unusedVariables.Count == 0)
        {
            Console.WriteLine("Все объявленные переменные используются");
        }
        else
        {
            Console.WriteLine("Неиспользуемые переменные:");
            foreach (var variable in unusedVariables)
            {
                Console.WriteLine($"- {variable}");
            }
        }
        
        Console.WriteLine("======================================\n");
    }
}