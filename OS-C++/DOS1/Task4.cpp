#include "Headers/Task4.h"

const int Task4_MatrixDeterminant::SIZE;

float Task4_MatrixDeterminant::calculate_3x3_determinant(float m[3][3])
{
    return m[0][0] * (m[1][1] * m[2][2] - m[1][2] * m[2][1]) -
           m[0][1] * (m[1][0] * m[2][2] - m[1][2] * m[2][0]) +
           m[0][2] * (m[1][0] * m[2][1] - m[1][1] * m[2][0]);
}

DWORD WINAPI Task4_MatrixDeterminant::calculate_minor_determinant(LPVOID param)
{
    DetParam* p = static_cast<DetParam*>(param);
    p->task->determinants[p->col] = p->sign * calculate_3x3_determinant(p->minor);
    delete p;
    return 0;
}

void Task4_MatrixDeterminant::execute()
{
    displayHeader(getName());

    for (int i = 0; i < SIZE; i++) {
        for (int j = 0; j < SIZE; j++) {
            matrix[i][j] = (float)(rand() % 10 + 1);
        }
    }

    std::cout << "Матрица 4x4:\n";
    for (int i = 0; i < SIZE; i++) {
        for (int j = 0; j < SIZE; j++) {
            std::cout << matrix[i][j] << "\t";
        }
        std::cout << "\n";
    }

    HANDLE hThread[SIZE];
    DWORD dwThreadID[SIZE];

    for (int col = 0; col < SIZE; col++) {
        DetParam* param = new DetParam;
        param->task = this;
        param->col = col;
        param->sign = (col % 2 == 0) ? 1.0f : -1.0f;

        int minor_row = 0;
        for (int i = 1; i < SIZE; i++) {
            int minor_col = 0;
            for (int j = 0; j < SIZE; j++) {
                if (j != col) {
                    param->minor[minor_row][minor_col] = matrix[i][j];
                    minor_col++;
                }
            }
            minor_row++;
        }

        hThread[col] = CreateThread(NULL, 0, calculate_minor_determinant, param, 0, &dwThreadID[col]);
    }

    WaitForMultipleObjects(SIZE, hThread, TRUE, INFINITE);

    float det = 0;
    for (int col = 0; col < SIZE; col++) {
        det += matrix[0][col] * determinants[col];
    }

    displayResult("Определитель = " + std::to_string(det));

    for (int col = 0; col < SIZE; col++) {
        CloseHandle(hThread[col]);
    }
}