#include "../headers/Task1.h"

Task1::Task1() : checksum(0), k(0) {
    InitializeCriticalSection(&cs);
}

Task1::~Task1() {
    DeleteCriticalSection(&cs);
}

DWORD WINAPI Task1::calculateChecksumThread(LPVOID param) {
    auto* tp = static_cast<ThreadParam *>(param);
    Task1* task = tp->task;
    int i = tp->threadIndex;
    int n = task->text.length();
    int localSum = 0;

    // Каждый поток обрабатывает символы с номерами i + k*s
    for (int pos = i; pos < n; pos += task->k) {
        localSum += static_cast<unsigned char>(task->text[pos]);
    }

    // Добавляем локальную сумму к общей
    EnterCriticalSection(&task->cs);
    task->checksum = (task->checksum + localSum) % 256;
    LeaveCriticalSection(&task->cs);

    delete tp;
    return 0;
}

void Task1::run() {
    cout << "\n=== ЗАДАНИЕ 1: КОНТРОЛЬНАЯ СУММА ТЕКСТА ===" << endl;
    cin.ignore();

    cout << "Введите текст: ";
    getline(cin, text);

    int n = text.length();
    cout << "Длина текста: " << n << " символов" << endl;

    cout << "Введите количество потоков (k < " << n << "): ";
    cin >> k;

    if (k <= 0 || k >= n) {
        cout << "Некорректное количество потоков!" << endl;
        return;
    }

    auto* hThread = new HANDLE[k];
    auto* dwThreadID = new DWORD[k];

    checksum = 0;

    cout << "Запуск " << k << " потоков..." << endl;
    for (int i = 0; i < k; i++) {
        auto* tp = new ThreadParam;
        tp->task = this;
        tp->threadIndex = i;

        hThread[i] = CreateThread(nullptr, 0, calculateChecksumThread,
                                   tp, 0, &(dwThreadID[i]));

        if (hThread[i] == nullptr) {
            cout << "Ошибка создания потока " << GetLastError() << endl;
            delete[] hThread;
            delete[] dwThreadID;
            return;
        }
    }

    WaitForMultipleObjects(k, hThread, TRUE, INFINITE);

    cout << "Контрольная сумма (по модулю 256): " << checksum << endl;

    for (int i = 0; i < k; i++) {
        CloseHandle(hThread[i]);
    }

    delete[] hThread;
    delete[] dwThreadID;
}