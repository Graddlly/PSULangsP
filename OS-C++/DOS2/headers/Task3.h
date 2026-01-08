#ifndef DOS2_TASK3_H
#define DOS2_TASK3_H

#include "windows.h"
#include <iostream>
#include <iomanip>
#include <cmath>

using namespace std;

class Task3 {
    CRITICAL_SECTION cs{};
    double totalArea;
    double a, b; // границы отрезка [a, b]
    int n;       // количество элементов разбиения
    int m;       // количество подразбиений для каждого элемента

    struct ThreadParam {
        Task3* task;
        int elementIndex;
        double xi;
        double xi1;
    };

    static DWORD WINAPI calculateElementThread(LPVOID param);
    static double f(double x); // функция

public:
    Task3();
    ~Task3();
    void run();
};


#endif //DOS2_TASK3_H