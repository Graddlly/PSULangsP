//
// Created by Novik on 14.12.2025.
//

#ifndef DOS4_TASK1_H
#define DOS4_TASK1_H

using namespace std;

#include <windows.h>
#include <iostream>
#include <string>
#include <map>
#include <vector>
#include <ctime>

namespace Task1
{
    constexpr int NUM_FACTORIES = 5;  // Количество цехов
    constexpr int MAX_QUERIES_FROM_ONE = 3;  // Максимум заявок от одного цеха
    constexpr int BUFFER_SIZE = 20;

    // Структура заявки
    struct Request
    {
        string material;
        int quantity;
    };

    // Глобальные переменные
    extern Request buffer[BUFFER_SIZE];
    extern int cur_pos;
    extern map<string, int> totalRequest;

    extern HANDLE hSemFull;
    extern HANDLE hSemEmpty;
    extern HANDLE hMutex;
    extern HANDLE hMutexTotal;

    // Структура параметров
    struct ShopParam
    {
        int shopId;
        vector<Request> requests;
    };

    // Функции потоков
    DWORD WINAPI shop(LPVOID param);
    DWORD WINAPI processingCenter(LPVOID param);
}

// Главная функция запуска
void runTask1();

#endif //DOS4_TASK1_H