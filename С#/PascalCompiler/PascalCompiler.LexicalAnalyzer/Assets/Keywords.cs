namespace PascalCompiler.LexicalAnalyzer.Assets;

public class Keywords
{
    public Dictionary<byte, Dictionary<string, byte>> Keyword { get; } = new();

    public Keywords()
    {
        InitializeKeywords();
    }

    /// <summary>
    /// Инициализация таблицы ключевых слов
    /// </summary>
    private void InitializeKeywords()
    {
        AddKeywordGroup(2, new Dictionary<string, byte>
        {
            ["do"] = LexicalAnalyzer.dosy,
            ["if"] = LexicalAnalyzer.ifsy,
            ["in"] = LexicalAnalyzer.insy,
            ["of"] = LexicalAnalyzer.ofsy,
            ["or"] = LexicalAnalyzer.orsy,
            ["to"] = LexicalAnalyzer.tosy
        });
        
        AddKeywordGroup(3, new Dictionary<string, byte>
        {
            ["end"] = LexicalAnalyzer.endsy,
            ["var"] = LexicalAnalyzer.varsy,
            ["div"] = LexicalAnalyzer.divsy,
            ["and"] = LexicalAnalyzer.andsy,
            ["not"] = LexicalAnalyzer.notsy,
            ["for"] = LexicalAnalyzer.forsy,
            ["mod"] = LexicalAnalyzer.modsy,
            ["nil"] = LexicalAnalyzer.nilsy,
            ["set"] = LexicalAnalyzer.setsy
        });
        
        AddKeywordGroup(4, new Dictionary<string, byte>
        {
            ["then"] = LexicalAnalyzer.thensy,
            ["else"] = LexicalAnalyzer.elsesy,
            ["case"] = LexicalAnalyzer.casesy,
            ["file"] = LexicalAnalyzer.filesy,
            ["goto"] = LexicalAnalyzer.gotosy,
            ["type"] = LexicalAnalyzer.typesy,
            ["with"] = LexicalAnalyzer.withsy,
            ["real"] = LexicalAnalyzer.realsy,
            ["char"] = LexicalAnalyzer.charsy,
            ["true"] = LexicalAnalyzer.truesy
        });
        
        AddKeywordGroup(5, new Dictionary<string, byte>
        {
            ["begin"] = LexicalAnalyzer.beginsy,
            ["while"] = LexicalAnalyzer.whilesy,
            ["array"] = LexicalAnalyzer.arraysy,
            ["const"] = LexicalAnalyzer.constsy,
            ["label"] = LexicalAnalyzer.labelsy,
            ["until"] = LexicalAnalyzer.untilsy,
            ["false"] = LexicalAnalyzer.falsesy
        });

        AddKeywordGroup(6, new Dictionary<string, byte>
        {
            ["downto"] = LexicalAnalyzer.downtosy,
            ["packed"] = LexicalAnalyzer.packedsy,
            ["record"] = LexicalAnalyzer.recordsy,
            ["repeat"] = LexicalAnalyzer.repeatsy
        });

        AddKeywordGroup(7, new Dictionary<string, byte>
        {
            ["program"] = LexicalAnalyzer.programsy,
            ["integer"] = LexicalAnalyzer.integersy,
            ["boolean"] = LexicalAnalyzer.booleansy
        });

        AddKeywordGroup(8, new Dictionary<string, byte>
        {
            ["function"] = LexicalAnalyzer.functionsy
        });

        AddKeywordGroup(9, new Dictionary<string, byte>
        {
            ["procedure"] = LexicalAnalyzer.procedurensy
        });
    }

    /// <summary>
    /// Добавление группы ключевых слов определенной длины
    /// </summary>
    /// <param name="length">Длина ключевых слов</param>
    /// <param name="keywords">Словарь ключевых слов и их кодов</param>
    private void AddKeywordGroup(byte length, Dictionary<string, byte> keywords)
    {
        Keyword[length] = new Dictionary<string, byte>(keywords);
    }

    /// <summary>
    /// Поиск ключевого слова
    /// </summary>
    /// <param name="word">Слово для поиска</param>
    /// <returns>Код ключевого слова или 0, если не найдено</returns>
    private byte FindKeyword(string word)
    {
        if (string.IsNullOrEmpty(word))
            return 0;

        var length = (byte)word.Length;
        var lowerWord = word.ToLower();

        if (Keyword.TryGetValue(length, out var value) && value.TryGetValue(lowerWord, out var keyword))
        {
            return keyword;
        }

        return 0;
    }

    /// <summary>
    /// Проверка, является ли слово ключевым
    /// </summary>
    /// <param name="word">Слово для проверки</param>
    /// <returns>true, если слово является ключевым</returns>
    public bool IsKeyword(string word)
    {
        return FindKeyword(word) != 0;
    }
}