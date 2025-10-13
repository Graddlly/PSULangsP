#ifndef LW1_TASK2_H
#define LW1_TASK2_H

#pragma once
#include "BaseTask.h"

class Task2_MaxAverageRow : public BaseTask {
    static const int m = 10, n = 20;
    float mtx[m][n];
    float row_averages[m];

    struct ThreadData {
        Task2_MaxAverageRow* task;
        int row_num;
    };

    static DWORD WINAPI calculate_row_average(LPVOID param);

public:
    void execute() override;
    std::string getName() const override { return "Поиск строки с максимальным средним значением"; }
};

#endif