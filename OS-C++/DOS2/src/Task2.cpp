#include "../headers/Task2.h"

Task2::Task2() : matrix(nullptr), totalSum(0), n(0), m(0) {
    InitializeCriticalSection(&cs);
}

Task2::~Task2() {
    if (matrix != nullptr) {
        deallocateMatrix();
    }
    DeleteCriticalSection(&cs);
}

void Task2::allocateMatrix() {
    matrix = new double*[n];
    for (int i = 0; i < n; i++) {
        matrix[i] = new double[m];
    }
}

void Task2::deallocateMatrix() {
    for (int i = 0; i < n; i++) {
        delete[] matrix[i];
    }
    delete[] matrix;
    matrix = nullptr;
}

void Task2::printMatrix() const {
    cout << "\n=== Сформированная матрица ===" << endl;
    for (int i = 0; i < n; i++) {
        for (int j = 0; j < m; j++) {
            cout << fixed << setprecision(1)
                      << setw(6) << matrix[i][j] << " ";
        }
        cout << endl;
    }
}

DWORD WINAPI Task2::fillRowThread(LPVOID param) {
    random_device rd;
    mt19937_64 gen(rd());
    uniform_real_distribution<> dist(0.0, 100.0);

    const auto* ftp = static_cast<FillThreadParam *>(param);
    const Task2* task = ftp->task;
    const int row = ftp->row;

    // cout << "Поток формирования строки " << row << " запущен" << endl;

    for (int col = 0; col < task->m; col++) {
        // task->matrix[row][col] = (rand() % 1000) / 10.0;
        task->matrix[row][col] = dist(gen);
    }

    // cout << "Строка " << row << " сформирована" << endl;
    delete ftp;
    return 0;
}

DWORD WINAPI Task2::sumRowThread(LPVOID param) {
    const auto* stp = static_cast<SumThreadParam *>(param);
    Task2* task = stp->task;
    const int row = stp->row;

    // Ожидание завершения формирования соответствующей строки
    WaitForSingleObject(stp->fillThreadHandle, INFINITE);

    // cout << "Поток суммирования строки " << row << " начал работу" << endl;

    double rowSum = 0;
    for (int col = 0; col < task->m; col++) {
        rowSum += task->matrix[row][col];
    }

    // Добавление суммы строки к общей сумме
    EnterCriticalSection(&task->cs);
    task->totalSum += rowSum;
    LeaveCriticalSection(&task->cs);

    //cout << "Сумма строки " << row << ": " << fixed
    //          << setprecision(2) << rowSum << endl;

    delete stp;
    return 0;
}

void Task2::run() {
    cout << "\n=== ЗАДАНИЕ 2: ФОРМИРОВАНИЕ И СУММИРОВАНИЕ МАТРИЦЫ ===" << endl;

    cout << "Введите количество строк (n): ";
    cin >> n;
    cout << "Введите количество столбцов (m): ";
    cin >> m;

    if (n <= 0 || m <= 0) {
        cout << "Некорректные размеры матрицы!" << endl;
        return;
    }

    allocateMatrix();

    auto* fillThreads = new HANDLE[n];
    auto* sumThreads = new HANDLE[n];
    auto* fillThreadIDs = new DWORD[n];
    auto* sumThreadIDs = new DWORD[n];

    totalSum = 0;

    cout << "\n=== Запуск потоков ===" << endl;
    for (int i = 0; i < n; i++) {
        // Запуск потока формирования строки
        auto* ftp = new FillThreadParam;
        ftp->task = this;
        ftp->row = i;
        fillThreads[i] = CreateThread(nullptr, 0, fillRowThread,
            ftp, 0, &(fillThreadIDs[i]));

        if (fillThreads[i] == nullptr) {
            cout << "Ошибка создания потока формирования " << GetLastError() << endl;
            delete[] fillThreads;
            delete[] sumThreads;
            delete[] fillThreadIDs;
            delete[] sumThreadIDs;
            return;
        }

        // Запуск потока суммирования в приостановленном состоянии
        auto* stp = new SumThreadParam;
        stp->task = this;
        stp->row = i;
        stp->fillThreadHandle = fillThreads[i];
        sumThreads[i] = CreateThread(nullptr, 0, sumRowThread,
            stp, CREATE_SUSPENDED, &(sumThreadIDs[i]));

        if (sumThreads[i] == nullptr) {
            cout << "Ошибка создания потока суммирования " << GetLastError() << endl;
            delete[] fillThreads;
            delete[] sumThreads;
            delete[] fillThreadIDs;
            delete[] sumThreadIDs;
            return;
        }

        ResumeThread(sumThreads[i]);
    }

    // Ожидание завершения всех потоков
    WaitForMultipleObjects(n, fillThreads, TRUE, INFINITE);
    WaitForMultipleObjects(n, sumThreads, TRUE, INFINITE);

    printMatrix();

    cout << "\n=== Общая сумма элементов матрицы: " << fixed
              << setprecision(2) << totalSum << " ===" << endl;

    // Освобождение ресурсов
    for (int i = 0; i < n; i++) {
        CloseHandle(fillThreads[i]);
        CloseHandle(sumThreads[i]);
    }

    delete[] fillThreads;
    delete[] sumThreads;
    delete[] fillThreadIDs;
    delete[] sumThreadIDs;

    deallocateMatrix();
}