//
// Created by Novik on 23.11.2025.
//

#include "../main.h"
#include <windows.h>
#include <iostream>
#include <list>
#include <ctime>

namespace Task4
{
    constexpr int hashBase = 10;
    int numbers;
    HANDLE mutexes[hashBase];
    HANDLE hMutex;
    list<int> hashTable[hashBase];

    int Hash(const int x)
    {
        return x % hashBase;
    }

    DWORD WINAPI InsertHashTable(LPVOID lpParam)
    {
        for (int i = 0; i < numbers; ++i)
        {
            srand(static_cast<int>(time(nullptr)) + i * 100 + GetCurrentThreadId());
            int num = (rand() % 100000);
            const int numLine = Hash(num);

            WaitForSingleObject(mutexes[numLine], INFINITE);

            hashTable[numLine].push_back(num);

            WaitForSingleObject(hMutex, INFINITE);

            cout << "Поток " << GetCurrentThreadId() << " вставил число "
                      << num << " в строку " << numLine << endl;

            ReleaseMutex(hMutex);
            ReleaseMutex(mutexes[numLine]);

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

void task4_hashtable()
{
    using namespace Task4;

    cout << "=== ЗАДАЧА 4: ХЕШ-ТАБЛИЦА ===\n\n";

    const int n = DataEntry("Введите число потоков (от 1 до 20): ", 1, 20);
    Task4::numbers = DataEntry("Введите количество чисел для каждого потока (от 1 до 100): ", 1, 100);

    for (int i = 0; i < hashBase; ++i)
    {
        hashTable[i].clear();
        mutexes[i] = CreateMutex(nullptr, false, nullptr);
    }
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
    for (int i = 0; i < hashBase; ++i)
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
    CloseHandle(hMutex);

    for (const auto &mut : mutexes)
    {
        CloseHandle(mut);
    }

    delete[] threads;

    cout << "\n>>> Задача завершена! <<<\n";
    cin.get();
}