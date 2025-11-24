//
// Created by Novik on 23.11.2025.
//

#include "../main.h"
#include <windows.h>
#include <iostream>
#include <fstream>

namespace Task1
{
    char* buffer = nullptr;
    int bufferSize;
    int bytesRead = 0;
    bool finished = false;
    unsigned char sum = 0;
    HANDLE hMutex, hMutexSum, hMutexRead;

    DWORD WINAPI ReadFileBuffer(const LPVOID param)
    {
        const auto filename = static_cast<const char *>(param);
        ifstream file(filename, ios::binary);

        if (!file)
        {
            cout << "Ошибка открытия файла!\n";
            finished = true;
            ReleaseMutex(hMutexSum);
            return 1;
        }

        cout << "Читатель: начинаю чтение файла...\n";

        while (true)
        {
            WaitForSingleObject(hMutexRead, INFINITE);
            WaitForSingleObject(hMutex, INFINITE);

            file.read(buffer, bufferSize);
            bytesRead = static_cast<int>(file.gcount());

            if (bytesRead == 0)
            {
                finished = true;
                cout << "Читатель: файл прочитан полностью\n";
                ReleaseMutex(hMutex);
                ReleaseMutex(hMutexSum);
                break;
            }

            cout << "Читатель: прочитано " << bytesRead << " байт\n";
            ReleaseMutex(hMutex);
            ReleaseMutex(hMutexSum);
        }

        file.close();
        return 0;
    }

    DWORD WINAPI SumFile(LPVOID param)
    {
        cout << "Вычислитель: готов к работе...\n";

        while (true)
        {
            WaitForSingleObject(hMutexSum, INFINITE);
            WaitForSingleObject(hMutex, INFINITE);

            if (finished)
            {
                ReleaseMutex(hMutex);
                ReleaseMutex(hMutexRead);
                break;
            }

            for (int i = 0; i < bytesRead; i++)
            {
                sum = (sum + static_cast<unsigned char>(buffer[i])) % 256;
            }

            cout << "Вычислитель: обработано " << bytesRead << " байт, "
                      << "текущая сумма = " << static_cast<unsigned int>(sum) << "\n";

            ReleaseMutex(hMutex);
            ReleaseMutex(hMutexRead);
        }

        cout << "\n>>> Конечная контрольная сумма по модулю 256 = "
                  << static_cast<unsigned int>(sum) << " <<<\n";
        return 0;
    }
}

void task1_checksum()
{
    using namespace Task1;

    cout << "=== ЗАДАЧА 1: КОНТРОЛЬНАЯ СУММА ФАЙЛА ===\n\n";

    char filename[256];
    cout << "Введите имя файла: ";
    cin.getline(filename, 256);

    cout << "Введите размер буфера (1-1024): ";
    cin >> bufferSize;
    cin.ignore();

    if (bufferSize < 1 || bufferSize > 1024)
    {
        cout << "Ошибка: размер буфера должен быть от 1 до 1024!\n";
        return;
    }

    finished = false;
    sum = 0;
    bytesRead = 0;
    buffer = new char[bufferSize];

    hMutexSum = CreateMutex(nullptr, false, nullptr);
    hMutexRead = CreateMutex(nullptr, false, nullptr);
    hMutex = CreateMutex(nullptr, false, nullptr);

    // Освобождаем hMutexRead для начала работы читателя
    ReleaseMutex(hMutexRead);

    cout << "\n--- Начало обработки ---\n\n";

    const HANDLE hReader = CreateThread(nullptr, 0, ReadFileBuffer, (LPVOID)filename, 0, nullptr);
    const HANDLE hCalculator = CreateThread(nullptr, 0, SumFile, nullptr, 0, nullptr);

    const HANDLE hThreads[2] = {hReader, hCalculator};

    WaitForMultipleObjects(2, hThreads, true, INFINITE);

    cout << "\n--- Конец обработки ---\n";

    CloseHandle(hReader);
    CloseHandle(hCalculator);
    CloseHandle(hMutexSum);
    CloseHandle(hMutexRead);
    CloseHandle(hMutex);

    delete[] buffer;
}