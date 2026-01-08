#ifndef DOS2_TASK2_H
#define DOS2_TASK2_H

#include <random>
#include <iostream>
#include <iomanip>
#include "windows.h"

using namespace std;

class Task2 {
    CRITICAL_SECTION cs{};
    double** matrix;
    double totalSum;
    int n, m; // размеры матрицы

    struct FillThreadParam {
        Task2* task;
        int row;
    };

    struct SumThreadParam {
        Task2* task;
        int row;
        HANDLE fillThreadHandle;
    };

    static DWORD WINAPI fillRowThread(LPVOID param);
    static DWORD WINAPI sumRowThread(LPVOID param);

    void allocateMatrix();
    void deallocateMatrix();
    void printMatrix() const;

public:
    Task2();
    ~Task2();
    void run();
};


#endif //DOS2_TASK2_H