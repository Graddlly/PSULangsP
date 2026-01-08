#include "Headers/Task2.h"

const int Task2_MaxAverageRow::m;
const int Task2_MaxAverageRow::n;

DWORD WINAPI Task2_MaxAverageRow::calculate_row_average(LPVOID param)
{
    ThreadData* data = static_cast<ThreadData*>(param);
    Task2_MaxAverageRow* task = data->task;
    int row_num = data->row_num;

    float sum = 0;
    for (int j = 0; j < n; j++) {
        sum += task->mtx[row_num][j];
    }
    task->row_averages[row_num] = sum / n;

    delete data;
    return 0;
}

void Task2_MaxAverageRow::execute()
{
    displayHeader(getName());

    for (int i = 0; i < m; i++) {
        for (int j = 0; j < n; j++) {
            mtx[i][j] = (float)(rand() % 100);
        }
    }

    HANDLE hThread[m];
    DWORD dwThreadID[m];

    for (int i = 0; i < m; i++) {
        ThreadData* data = new ThreadData{this, i};
        hThread[i] = CreateThread(NULL, 0, calculate_row_average, data, 0, &dwThreadID[i]);
    }

    WaitForMultipleObjects(m, hThread, TRUE, INFINITE);

    int max_row = 0;
    float max_avg = row_averages[0];

    for (int i = 1; i < m; i++) {
        if (row_averages[i] > max_avg) {
            max_avg = row_averages[i];
            max_row = i;
        }
    }

    displayResult("Строка № " + std::to_string(max_row) +
                 " (среднее = " + std::to_string(max_avg) + ")");

    for (int i = 0; i < m; i++) {
        CloseHandle(hThread[i]);
    }
}