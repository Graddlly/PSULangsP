using Avalonia;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TaskWindowMoney;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            LogError("Критическая ошибка при запуске приложения ", e);
            throw;
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    
    private static void LogError(string context, Exception ex)
    {
        var errorMessage = $"{DateTime.Now}: [{context}]\nСообщение: {ex.Message}\nСтек вызовов:\n{ex.StackTrace}\n";
        if (ex.InnerException != null)
        {
            errorMessage += $"\nВнутреннее исключение:\nСообщение: {ex.InnerException.Message}\nСтек вызовов:\n{ex.InnerException.StackTrace}\n";
        }
        errorMessage += "\n-------------------------------------------------\n";

        Console.WriteLine(errorMessage);

        try
        {
            var logFilePath = Path.Combine(AppContext.BaseDirectory, "critical_error_log.txt");
            File.AppendAllText(logFilePath, errorMessage, Encoding.UTF8);
            Console.WriteLine($"Информация об ошибке также записана в: {logFilePath}");
        }
        catch (Exception logEx)
        {
            Console.WriteLine($"Не удалось записать ошибку в файл лога: {logEx.Message}");
        }
    }
    
    private static void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        LogError("Необработанное исключение в задаче (TaskScheduler_UnobservedTaskException)", e.Exception);
        e.SetObserved();
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        LogError("Необработанное доменное исключение (CurrentDomain_UnhandledException)", (Exception)e.ExceptionObject);
    }
}