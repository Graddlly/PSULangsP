#include "Headers/Task3.h"

const int Task3_SortColumns::m;
const int Task3_SortColumns::n;

DWORD WINAPI Task3_SortColumns::sort_column(LPVOID param)
{
    SortParam* p = static_cast<SortParam*>(param);
    int col = p->col_num;

    for (int i = 0; i < m - 1; i++) {
        for (int j = 0; j < m - i - 1; j++) {
            if (p->task->mtx[j][col] > p->task->mtx[j + 1][col]) {
                std::swap(p->task->mtx[j][col], p->task->mtx[j + 1][col]);
            }
        }
    }

    delete p;
    return 0;
}

void Task3_SortColumns::execute()
{
    displayHeader(getName());

    for (int i = 0; i < m; i++) {
        for (int j = 0; j < n; j++) {
            mtx[i][j] = (float)(rand() % 100);
        }
    }

    std::cout << "Матрица до сортировки (первые 5x5):\n";
    for (int i = 0; i < std::min(5, m); i++) {
        for (int j = 0; j < std::min(5, n); j++) {
            std::cout << mtx[i][j] << "\t";
        }
        std::cout << "\n";
    }

    HANDLE hThread[n];
    DWORD dwThreadID[n];

    for (int j = 0; j < n; j++) {
        SortParam* param = new SortParam{this, j};
        hThread[j] = CreateThread(NULL, 0, sort_column, param, 0, &dwThreadID[j]);
    }

    WaitForMultipleObjects(n, hThread, TRUE, INFINITE);

    std::cout << "\nМатрица после сортировки (первые 5x5):\n";
    for (int i = 0; i < std::min(5, m); i++) {
        for (int j = 0; j < std::min(5, n); j++) {
            std::cout << mtx[i][j] << "\t";
        }
        std::cout << "\n";
    }

    for (int j = 0; j < n; j++) {
        CloseHandle(hThread[j]);
    }
}