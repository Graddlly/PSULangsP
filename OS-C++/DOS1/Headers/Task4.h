#ifndef LW1_TASK4_H
#define LW1_TASK4_H

#pragma once
#include "BaseTask.h"

class Task4_MatrixDeterminant : public BaseTask {
    static const int SIZE = 4;
    float matrix[SIZE][SIZE];
    float determinants[SIZE];

    struct DetParam {
        Task4_MatrixDeterminant* task;
        int col;
        float minor[3][3];
        float sign;
    };

    static float calculate_3x3_determinant(float m[3][3]);
    static DWORD WINAPI calculate_minor_determinant(LPVOID param);

public:
    void execute() override;
    std::string getName() const override { return "Вычисление определителя матрицы 4x4"; }
};

#endif