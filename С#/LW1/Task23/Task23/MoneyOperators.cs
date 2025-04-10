namespace Task23;

public partial class Money
{
    /// <summary>
    /// Увеличивает денежную сумму на 1 копейку.
    /// </summary>
    /// <param name="money">Исходная денежная сумма.</param>
    /// <returns>Новая денежная сумма.</returns>
    public static Money operator ++(Money money)
    {
        var result = new Money(money._rubles, money._kopeks);
        
        if (result._kopeks == 99)
        {
            result._kopeks = 0;
            result._rubles++;
        }
        else 
            result._kopeks++;
        
        return result;
    }
    
    /// <summary>
    /// Уменьшает денежную сумму на 1 копейку.
    /// </summary>
    /// <param name="money">Исходная денежная сумма.</param>
    /// <returns>Новая денежная сумма.</returns>
    /// <exception cref="InvalidOperationException">Возникает, если результат отрицательный.</exception>
    public static Money operator --(Money money)
    {
        if (money._rubles == 0 && money._kopeks == 0) 
            throw new InvalidOperationException("Результат вычитания не может быть отрицательным.");

        var result = new Money(money._rubles, money._kopeks);
        
        if (result._kopeks == 0)
        {
            result._kopeks = 99;
            result._rubles--;
        }
        else 
            result._kopeks--;
        
        return result;
    }
    
    /// <summary>
    /// Неявно преобразует денежную сумму в количество рублей (копейки отбрасываются).
    /// </summary>
    /// <param name="money">Денежная сумма.</param>
    public static implicit operator uint(Money money)
    {
        return money._rubles;
    }
    
    /// <summary>
    /// Явно преобразует денежную сумму в десятичное число,
    /// где целая часть отбрасывается, а дробная часть представляет копейки в рублях.
    /// </summary>
    /// <param name="money">Денежная сумма.</param>
    public static explicit operator double(Money money)
    {
        return (double)money._kopeks / 100;
    }
    
    /// <summary>
    /// Вычитает одну денежную сумму из другой.
    /// </summary>
    /// <param name="left">Уменьшаемое.</param>
    /// <param name="right">Вычитаемое.</param>
    /// <returns>Результат вычитания.</returns>
    public static Money operator -(Money left, Money right)
    {
        return left.Subtract(right);
    }
    
    /// <summary>
    /// Вычитает указанное количество рублей из денежной суммы.
    /// </summary>
    /// <param name="left">Денежная сумма.</param>
    /// <param name="right">Количество вычитаемых рублей.</param>
    /// <returns>Результат вычитания.</returns>
    public static Money operator -(Money left, uint right)
    {
        var rightMoney = new Money(right, 0);
        return left.Subtract(rightMoney);
    }
    
    /// <summary>
    /// Вычитает денежную сумму из указанного количества рублей.
    /// </summary>
    /// <param name="left">Количество рублей.</param>
    /// <param name="right">Вычитаемая денежная сумма.</param>
    /// <returns>Результат вычитания.</returns>
    public static Money operator -(uint left, Money right)
    {
        var leftMoney = new Money(left, 0);
        return leftMoney.Subtract(right);
    }
}