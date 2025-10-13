#ifndef LW1_TASK3_H
#define LW1_TASK3_H

#pragma once
#include "BaseTask.h"
#include <algorithm>

class Task3_SortColumns : public BaseTask {
    static const int m = 10, n = 20;
    float mtx[m][n];

    struct SortParam {
        Task3_SortColumns* task;
        int col_num;
    };

    static DWORD WINAPI sort_column(LPVOID param);

public:
    void execute() override;
    std::string getName() const override { return "Сортировка столбцов по возрастанию"; }
};

#endif