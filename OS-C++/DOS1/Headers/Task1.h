#ifndef LW1_TASK1_H
#define LW1_TASK1_H

#pragma once
#include "BaseTask.h"
#include <algorithm>

class Task1_ParallelMatrixCreation : public BaseTask {
    static const int m = 10, n = 20;
    float mtx[m][n];

    struct ThreadData {
        Task1_ParallelMatrixCreation* task;
        int row_num;
    };

    static DWORD WINAPI create_row(LPVOID param);

public:
    void execute() override;
    std::string getName() const override { return "Параллельное создание строк матрицы"; }
};

#endif