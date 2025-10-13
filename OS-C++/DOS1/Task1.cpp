#include "Headers/Task1.h"

const int Task1_ParallelMatrixCreation::m;
const int Task1_ParallelMatrixCreation::n;

DWORD WINAPI Task1_ParallelMatrixCreation::create_row(LPVOID param)
{
    ThreadData* data = static_cast<ThreadData*>(param);
    Task1_ParallelMatrixCreation* task = data -> task;
    int row_num = data -> row_num;

    for (int j = 0; j < n; j++) {
        task -> mtx[row_num][j] = (float)(rand() % 1000) / 10.0f;
    }

    delete data;
    return 0;
}

void Task1_ParallelMatrixCreation::execute()
{
    displayHeader(getName());

    HANDLE hThread[m];
    DWORD dwThreadID[m];

    for (int i = 0; i < m; i++) {
        ThreadData* data = new ThreadData{this, i};
        hThread[i] = CreateThread(NULL, 0, create_row, data, 0, &dwThreadID[i]);

        if (hThread[i] == NULL) {
            std::cout << "Поток для строки № " << i << " не был создан\n";
        }
    }

    WaitForMultipleObjects(m, hThread, TRUE, INFINITE);

    for (int i = 0; i < m; i++) {
        CloseHandle(hThread[i]);
    }

    std::cout << "Матрица создана параллельно\n";
    std::cout << "Первые 5x5 элементов матрицы:\n";
    for (int i = 0; i < std::min(5, m); i++) {
        for (int j = 0; j < std::min(5, n); j++) {
            std::cout << mtx[i][j] << "\t";
        }
        std::cout << "\n";
    }
}