//
// Created by Novik on 14.12.2025.
//

#include "../headers/task4.h"

namespace Task4
{
    char printPool[POOL_SIZE];
    int poolUsed = 0;
    bool allPrintersFinished = false;
    int activePrinters = NUM_THREADS_PRINT;

    HANDLE hSemSpace = nullptr;
    HANDLE hSemData = nullptr;
    HANDLE hMutexPool = nullptr;
    HANDLE hMutexPrinter = nullptr;
    HANDLE hMutexOutput = nullptr;

    ofstream outputFile;

    DWORD WINAPI printer(const LPVOID param)
    {
        const auto p = static_cast<PrinterParam *>(param);
        const int printerId = p->printerId;
        const string filename = p->filename;
        delete p;

        ifstream file(filename);
        if(!file.is_open())
        {
            cerr << "Принтер " << printerId << ": не могу открыть " << filename << "\n";
            InterlockedDecrement(reinterpret_cast<LONG *>(&activePrinters));
            return 1;
        }

        cout << "Принтер " << printerId << " начал работу с файлом " << filename << "\n";

        string text;
        string line;
        while(getline(file, line))
        {
            text += line + "\n";
        }
        file.close();

        // Захватываем мьютекс принтера на ВСЁ время печати
        WaitForSingleObject(hMutexPrinter, INFINITE);

        int textPos = 0;
        const int textLen = text.length();

        while(textPos < textLen)
        {
            WaitForSingleObject(hMutexPool, INFINITE);

            int canWrite = POOL_SIZE - poolUsed;

            if(canWrite == 0)
            {
                // Места нет - освобождаем мьютекс пула и ждем
                ReleaseMutex(hMutexPool);
                cout << "Принтер " << printerId << " ждёт места в пуле...\n";

                // Сигнализируем менеджеру
                ReleaseSemaphore(hSemData, 1, nullptr);

                Sleep(100);
                continue;
            }

            // Записываем столько, сколько можем
            const int toWrite = min(canWrite, textLen - textPos);

            for(int i = 0; i < toWrite; i++)
            {
                printPool[poolUsed + i] = text[textPos + i];
            }

            poolUsed += toWrite;
            textPos += toWrite;

            cout << "Принтер " << printerId << " записал " << toWrite
                      << " байт в пул (занято: " << poolUsed << "/" << POOL_SIZE
                      << ", осталось записать: " << (textLen - textPos) << ")\n";

            ReleaseMutex(hMutexPool);

            // Сигнализируем о новых данных
            ReleaseSemaphore(hSemData, 1, nullptr);

            if(textPos < textLen)
                Sleep(50);
        }

        // Только теперь освобождаем мьютекс принтера - весь текст записан
        ReleaseMutex(hMutexPrinter);

        cout << "Принтер " << printerId << " ПОЛНОСТЬЮ завершил печать файла " << filename << "\n";

        InterlockedDecrement(reinterpret_cast<LONG *>(&activePrinters));
        ReleaseSemaphore(hSemData, 1, nullptr);

        return 0;
    }

    DWORD WINAPI poolManager(LPVOID param)
    {
        int totalProcessed = 0;

        while(true)
        {
            // Проверяем условие завершения
            WaitForSingleObject(hMutexPool, INFINITE);
            const bool shouldExit = (activePrinters == 0 && poolUsed == 0);
            ReleaseMutex(hMutexPool);

            if(shouldExit)
            {
                cout << "Менеджер пула: все данные обработаны (всего: " << totalProcessed << " байт)\n";
                break;
            }

            // Ждём данные в пуле
            if(WaitForSingleObject(hSemData, 200) == WAIT_TIMEOUT)
                continue;

            WaitForSingleObject(hMutexPool, INFINITE);

            if(poolUsed > 0)
            {
                constexpr int BATCH_SIZE = 20;
                const int toRead = min(BATCH_SIZE, poolUsed);

                char buffer[BATCH_SIZE + 1];
                for(int i = 0; i < toRead; i++)
                {
                    buffer[i] = printPool[i];
                }
                buffer[toRead] = '\0';

                // Сдвигаем содержимое пула
                for(int i = 0; i < poolUsed - toRead; i++)
                {
                    printPool[i] = printPool[i + toRead];
                }

                poolUsed -= toRead;
                totalProcessed += toRead;

                cout << "Менеджер пула: прочитано " << toRead
                          << " байт (осталось в пуле: " << poolUsed << ", всего обработано: " << totalProcessed << ")\n";

                ReleaseMutex(hMutexPool);

                // Записываем в выходной файл
                WaitForSingleObject(hMutexOutput, INFINITE);
                outputFile.write(buffer, toRead);
                outputFile.flush();
                ReleaseMutex(hMutexOutput);
            }
            else
            {
                ReleaseMutex(hMutexPool);
            }

            Sleep(80);
        }

        return 0;
    }
}

void runTask4()
{
    using namespace Task4;

    poolUsed = 0;
    allPrintersFinished = false;
    activePrinters = NUM_THREADS_PRINT;

    outputFile.open("printed_output.txt");

    HANDLE hPrinters[NUM_THREADS_PRINT];

    hSemSpace = CreateSemaphore(nullptr, POOL_SIZE, POOL_SIZE, nullptr);
    hSemData = CreateSemaphore(nullptr, 0, POOL_SIZE * 10, nullptr);
    hMutexPool = CreateMutex(nullptr, FALSE, nullptr);
    hMutexPrinter = CreateMutex(nullptr, FALSE, nullptr);
    hMutexOutput = CreateMutex(nullptr, FALSE, nullptr);

    const HANDLE hManager = CreateThread(nullptr, 0, poolManager, nullptr,
        0, nullptr);

    const vector<string> files = {"file1.txt", "file2.txt", "file3.txt"};

    // Запускаем принтеры последовательно для гарантии порядка
    for(int i = 0; i < NUM_THREADS_PRINT; i++)
    {
        const auto param = new PrinterParam;
        param->printerId = i + 1;
        param->filename = files[i];
        hPrinters[i] = CreateThread(nullptr, 0, printer, param,
            0, nullptr);

        // Ждем завершения текущего принтера перед запуском следующего
        cout << "\n=== Ожидание завершения принтера " << (i+1) << " ===\n\n";
        WaitForSingleObject(hPrinters[i], INFINITE);
    }
    cout << "\n=== Все принтеры завершили работу ===\n";

    // Даем менеджеру время на обработку остатков
    Sleep(1000);

    WaitForSingleObject(hManager, INFINITE);

    outputFile.close();

    cout << "\nРабота завершена! Результат в printed_output.txt\n";

    for(int i = 0; i < NUM_THREADS_PRINT; i++)
        CloseHandle(hPrinters[i]);
    CloseHandle(hManager);

    CloseHandle(hSemSpace);
    CloseHandle(hSemData);
    CloseHandle(hMutexPool);
    CloseHandle(hMutexPrinter);
    CloseHandle(hMutexOutput);
}