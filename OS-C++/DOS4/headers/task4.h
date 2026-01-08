//
// Created by Novik on 14.12.2025.
//

#ifndef DOS4_TASK4_H
#define DOS4_TASK4_H

using namespace std;

#include <windows.h>
#include <iostream>
#include <fstream>
#include <string>
#include <vector>

namespace Task4
{
    constexpr int POOL_SIZE = 100;
    constexpr int NUM_THREADS_PRINT = 3; // Количество потоков печати

    extern char printPool[POOL_SIZE];
    extern int poolUsed; // Сколько байт занято в пуле
    extern bool allPrintersFinished;
    extern int activePrinters;

    extern HANDLE hSemSpace; // Семафор свободного места
    extern HANDLE hSemData; // Семафор данных в пуле
    extern HANDLE hMutexPool; // Мьютекс для доступа к пулу
    extern HANDLE hMutexPrinter; // Мьютекс для атомарной записи принтера
    extern HANDLE hMutexOutput; // Мьютекс для выходного файла

    extern ofstream outputFile;

    struct PrinterParam
    {
        int printerId;
        string filename;
    };

    DWORD WINAPI printer(LPVOID param);
    DWORD WINAPI poolManager(LPVOID param);
}

void runTask4();

#endif //DOS4_TASK4_H