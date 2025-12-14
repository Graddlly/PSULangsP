//
// Created by Novik on 14.12.2025.
//

#ifndef DOS4_TASK2_H
#define DOS4_TASK2_H

using namespace std;

#include <windows.h>
#include <iostream>
#include <fstream>

namespace Task2
{
    constexpr int BUFFER_SIZE = 50; // Размер буфера
    constexpr int NUM_CRYPT = 3; // Количество потоков-шифровщиков

    extern char buffer[BUFFER_SIZE];
    extern int writePos;
    extern int processPos;
    extern int readPos;
    extern int charsInBuffer;
    extern bool inputFinished;
    extern bool encryptionFinished;

    extern HANDLE hSemFull;
    extern HANDLE hSemEmpty;
    extern HANDLE hSemProcessed;
    extern HANDLE hMutexBuffer;
    extern HANDLE hMutexProcess;
    extern HANDLE hMutexFile;

    extern ifstream inputFile;
    extern ofstream outputFile;

    DWORD WINAPI reader(LPVOID param);
    DWORD WINAPI encryptor(LPVOID param);
    DWORD WINAPI writer(LPVOID param);
}

void runTask2();

#endif //DOS4_TASK2_H