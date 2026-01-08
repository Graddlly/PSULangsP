//
// Created by Novik on 23.11.2025.
//

#include "../main.h"
#include <windows.h>
#include <iostream>
#include <list>
#include <ctime>

namespace Task5
{
    int tableSize;
    int hashBase;
    int numbers;
    list<int>* hashTable;
    HANDLE hMutex;

    class CMonitor {
        HANDLE* mutexes;
        int size;

    public:
        explicit CMonitor(const int n) : size(n)
        {
            mutexes = new HANDLE[size];
            for (int i = 0; i < size; ++i)
            {
                mutexes[i] = CreateMutex(nullptr, false, nullptr);
            }
        }

        void OccupateRow(const int rowNumber) const
        {
            WaitForSingleObject(mutexes[rowNumber], INFINITE);
        }

        void FreeRow(const int rowNumber) const
        {
            ReleaseMutex(mutexes[rowNumber]);
        }

        void Close() const
        {
            for (int i = 0; i < size; ++i)
            {
                CloseHandle(mutexes[i]);
            }
            delete[] mutexes;
        }
    };

    CMonitor* monitor;

    int Hash(const int x)
    {
        return x % hashBase % tableSize;
    }

    DWORD WINAPI InsertHashTable(LPVOID lpParam)
    {
        for (int i = 0; i < numbers; ++i)
        {
            srand(static_cast<int>(time(nullptr)) ^ (i + 1) * 100 ^ GetCurrentThreadId());
            int num = (rand() % 100000);
            const int numLine = Hash(num);

            monitor->OccupateRow(numLine);

            hashTable[numLine].push_back(num);

            WaitForSingleObject(hMutex, INFINITE);

            cout << "Поток " << GetCurrentThreadId() << " вставил число "
                      << num << " в строку " << numLine << endl;

            ReleaseMutex(hMutex);
            monitor->FreeRow(numLine);

            Sleep(10);
        }
        return 0;
    }

    int DataEntry(const char* text, const int minVal, const int maxVal)
    {
        int val;
        while (true)
        {
            cout << text;
            cin >> val;

            if (cin.fail())
            {
                cin.clear();
                cin.ignore(10000, '\n');
                cout << "Ошибка! Введите число.\n";
                continue;
            }

            if (val < minVal || val > maxVal)
            {
                cout << "Ошибка! Число должно быть от " << minVal
                          << " до " << maxVal << ".\n";
                continue;
            }

            break;
        }
        return val;
    }
}

void task5_monitor()
{
    using namespace Task5;

    cout << "=== ЗАДАЧА 5: ХЕШ-ТАБЛИЦА С МОНИТОРОМ ===\n\n";

    int n = DataEntry("Введите число потоков (от 1 до 20): ", 1, 20);
    Task5::numbers = DataEntry("Введите количество чисел для каждого потока (от 1 до 100): ", 1, 100);
    tableSize = DataEntry("Введите размер хеш-таблицы (строк) (от 1 до 50): ", 1, 50);
    hashBase = DataEntry("Введите основание хеш-функции (от 1 до 100000): ", 1, 100000);

    hashTable = new list<int>[tableSize];

    monitor = new CMonitor(tableSize);
    hMutex = CreateMutex(nullptr, false, nullptr);

    auto* threads = new HANDLE[n];

    cout << "\n--- Генерация чисел ---\n\n";

    for (int i = 0; i < n; ++i)
    {
        threads[i] = CreateThread(nullptr, 0, InsertHashTable, nullptr, 0, nullptr);
    }

    WaitForMultipleObjects(n, threads, true, INFINITE);

    cout << "\n--- Результат ---\n\n";
    cout << "Хеш-таблица:\n";
    for (int i = 0; i < tableSize; ++i)
    {
        cout << i << ": ";
        for (const int &it : hashTable[i])
        {
            cout << it << " ";
        }
        cout << "\n";
    }

    for (int i = 0; i < n; ++i)
    {
        CloseHandle(threads[i]);
    }

    monitor->Close();
    delete monitor;

    delete[] hashTable;
    delete[] threads;
    CloseHandle(hMutex);

    cout << "\n>>> Задача завершена! <<<\n";
    cin.get();
}