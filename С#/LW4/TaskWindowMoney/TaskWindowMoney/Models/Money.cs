using System;

namespace TaskWindowMoney.Models;

/// <summary>
/// Класс для работы с денежными суммами в рублях и копейках.
/// </summary>
public partial class Money
{
    private uint _rubles;
    private byte _kopeks;
    
    /// <summary>
    /// Создает экземпляр класса Money с нулевыми значениями.
    /// </summary>
    public Money()
    {
        _rubles = 0;
        _kopeks = 0;
    }

    /// <summary>
    /// Создает экземпляр класса Money с указанными значениями.
    /// </summary>
    /// <param name="rubles">Количество рублей.</param>
    /// <param name="kopeks">Количество копеек (от 0 до 99).</param>
    /// <exception cref="ArgumentException">Возникает, если копейки вне диапазона [0, 99].</exception>
    public Money(uint rubles, byte kopeks)
    {
        if (kopeks < 100)
        {
            _rubles = rubles;
            _kopeks = kopeks;
        }
        else 
            throw new ArgumentException("Копейки должны быть в диапазоне от 0 до 99.");
    }
    
    /// <summary>
    /// Целое количество рублей.
    /// </summary>
    public uint Rubles
    {
        get
        {
            return _rubles;
        }
        set
        {
            _rubles = value;
        }
    }

    /// <summary>
    /// Целое количество копеек (от 0 до 99).
    /// </summary>
    public byte Kopeks
    {
        get
        {
            return _kopeks;
        }
        set 
        {
            if (value < 100) 
                _kopeks = value;
            else 
                throw new ArgumentException("Копейки должны быть в диапазоне от 0 до 99.");
        }
    }
    
    /// <summary>
    /// Вычитает указанную денежную сумму из текущей.
    /// </summary>
    /// <param name="other">Вычитаемая сумма.</param>
    /// <returns>Результат вычитания.</returns>
    /// <exception cref="InvalidOperationException">Возникает, если результат отрицательный.</exception>
    public Money Subtract(Money other)
    {
        // Преобразуем значения в копейки для упрощения вычислений
        var thisTotalKopeks = (long)_rubles * 100 + _kopeks;
        var otherTotalKopeks = (long)other._rubles * 100 + other._kopeks;
        
        if (thisTotalKopeks < otherTotalKopeks) 
            throw new InvalidOperationException("Результат вычитания не может быть отрицательным.");
        
        var resultKopeks = thisTotalKopeks - otherTotalKopeks;
        // Преобразуем обратно в рубли и копейки
        var resultRubles = (uint)(resultKopeks / 100);
        var resultKop = (byte)(resultKopeks % 100);

        return new Money(resultRubles, resultKop);
    }
    
    /// <summary>
    /// Возвращает строковое представление денежной суммы.
    /// </summary>
    /// <returns>Строка в формате "X руб. Y коп."</returns>
    public override string ToString()
    {
        return $"{_rubles} руб. {_kopeks} коп.";
    }
}