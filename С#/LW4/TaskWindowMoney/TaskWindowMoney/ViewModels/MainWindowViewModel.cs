using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Avalonia.Data;
using System.Text;
using Avalonia;
using TaskWindowMoney.Models;

namespace TaskWindowMoney.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private uint _firstRubles;
    private byte _firstKopeks;
    private uint _secondRubles;
    private byte _secondKopeks;
    private ObservableCollection<string>? _resultHistory;
    private string? _errorMessage;
    
    public MainWindowViewModel()
    {
        ResultHistory = [];
        
        SubtractCommand = new RelayCommand(Subtract);
        IncrementCommand = new RelayCommand(Increment);
        DecrementCommand = new RelayCommand(Decrement);
        ToUintCommand = new RelayCommand(ConvertToUint);
        ToDoubleCommand = new RelayCommand(ConvertToDouble);
        ClearResultsCommand = new RelayCommand(ClearResults);
    }

    public uint FirstRubles
    {
        get
        {
            return _firstRubles;
        }
        set
        {
            SetProperty(ref _firstRubles, value);
        }
    }

    public byte FirstKopeks
    {
        get
        {
            return _firstKopeks;
        }
        set
        {
            if (value >= 100)
                throw new DataValidationException("Копейки должны быть в диапазоне от 0 до 99.");
            SetProperty(ref _firstKopeks, value);
        }
    }

    public uint SecondRubles
    {
        get
        {
            return _secondRubles;
        }
        set
        {
            SetProperty(ref _secondRubles, value);
        }
    }

    public byte SecondKopeks
    {
        get
        {
            return _secondKopeks;
        }
        set
        {
            if (value >= 100)
                throw new DataValidationException("Копейки должны быть в диапазоне от 0 до 99.");
            SetProperty(ref _secondKopeks, value);
        }
    }

    public ObservableCollection<string>? ResultHistory
    {
        get
        {
            return _resultHistory;
        }
        set
        {
            SetProperty(ref _resultHistory, value);
        }
    }

    public string? ErrorMessage
    {
        get
        {
            return _errorMessage;
        }
        set
        {
            SetProperty(ref _errorMessage, value);
        }
    }

    public ICommand SubtractCommand { get; }
    public ICommand IncrementCommand { get; }
    public ICommand DecrementCommand { get; }
    public ICommand ToUintCommand { get; }
    public ICommand ToDoubleCommand { get; }
    public ICommand ClearResultsCommand { get; }

    private Money GetFirstMoney()
    {
        return new Money(FirstRubles, FirstKopeks);
    }

    private Money GetSecondMoney()
    {
        return new Money(SecondRubles, SecondKopeks);
    }

    private void AppendResult(string text)
    {
        ResultHistory?.Add(text);
    }

    private void Subtract()
    {
        try
        {
            ErrorMessage = string.Empty;
            var money1 = GetFirstMoney();
            var money2 = GetSecondMoney();
            var result = money1 - money2;
            
            var sb = new StringBuilder();
            sb.AppendLine("Вычитание денежных сумм:");
            sb.AppendLine($"Первая сумма: {money1}");
            sb.AppendLine($"Вторая сумма: {money2}");
            sb.AppendLine($"Результат: {result}");

            AppendResult(sb.ToString());
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            AppendResult($"Ошибка при вычитании: {ex.Message}");
        }
    }

    private void Increment()
    {
        try
        {
            ErrorMessage = string.Empty;
            var money = GetFirstMoney();
            var result = money++;
            
            var sb = new StringBuilder();
            sb.AppendLine("Увеличение денежной суммы на 1 копейку:");
            sb.AppendLine($"Исходная сумма: {result}");
            sb.AppendLine($"Результат: {money}");
            
            FirstRubles = result.Rubles;
            FirstKopeks = result.Kopeks;

            AppendResult(sb.ToString());
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            AppendResult($"Ошибка при увеличении: {ex.Message}");
        }
    }

    private void Decrement()
    {
        try
        {
            ErrorMessage = string.Empty;
            var money = GetFirstMoney();
            var result = money--;
            
            var sb = new StringBuilder();
            sb.AppendLine("Уменьшение денежной суммы на 1 копейку:");
            sb.AppendLine($"Исходная сумма: {result}");
            sb.AppendLine($"Результат: {money}");
            
            FirstRubles = result.Rubles;
            FirstKopeks = result.Kopeks;

            AppendResult(sb.ToString());
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            AppendResult($"Ошибка при уменьшении: {ex.Message}");
        }
    }

    private void ConvertToUint()
    {
        try
        {
            ErrorMessage = string.Empty;
            var money = GetFirstMoney();
            uint rubles = money;
            
            var sb = new StringBuilder();
            sb.AppendLine("Неявное преобразование денежной суммы в рубли (без копеек):");
            sb.AppendLine($"Исходная сумма: {money}");
            sb.AppendLine($"Результат преобразования: {rubles} рублей");

            AppendResult(sb.ToString());
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            AppendResult($"Ошибка при преобразовании: {ex.Message}");
        }
    }

    private void ConvertToDouble()
    {
        try
        {
            ErrorMessage = string.Empty;
            var money = GetFirstMoney();
            var kopeksAsRubles = (double)money;
            
            var sb = new StringBuilder();
            sb.AppendLine("Явное преобразование копеек в десятичную дробь (часть рубля):");
            sb.AppendLine($"Исходная сумма: {money}");
            sb.AppendLine($"Результат преобразования: {kopeksAsRubles:F2} рубля");

            AppendResult(sb.ToString());
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            AppendResult($"Ошибка при преобразовании: {ex.Message}");
        }
    }

    private void ClearResults()
    {
        ResultHistory?.Clear();
    }
}