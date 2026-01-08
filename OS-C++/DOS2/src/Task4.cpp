#include "../headers/Task4.h"

Task4::Task4() : currentIndex(0), totalElements(0) {
    InitializeCriticalSection(&cs);
}

Task4::~Task4() {
    DeleteCriticalSection(&cs);
}

bool Task4::loadDataFromFile(const string& filename) {
    fileData.clear();

    ifstream file(filename);
    if (!file.is_open()) {
        cout << "Ошибка открытия файла! Будут использованы тестовые данные." << endl;
        cout << "Тестовые данные: " << endl;
        // Генерация тестовых данных
        for (int i = 1; i <= 20; i++) {
            int kk = i * 34;
            cout << kk << " ";
            fileData.push_back(kk);
        }
        cout << endl;
        return false;
    }

    int value;
    while (file >> value) {
        fileData.push_back(value);
    }
    file.close();

    cout << "Прочитано элементов из файла: " << fileData.size() << endl;
    return true;
}

void Task4::printDeque() const {
    cout << "\n=== Результирующий дек ===" << endl;
    cout << "Размер дека: " << myDeque.size() << endl;
    cout << "Содержимое (от начала к концу): ";
    for (size_t i = 0; i < myDeque.size(); i++) {
        cout << myDeque[i];
        if (i < myDeque.size() - 1) cout << " -> ";
    }
    cout << endl;
}

DWORD WINAPI Task4::addToDequeThread(const LPVOID param) {
    const auto* tp = static_cast<ThreadParam *>(param);
    Task4* task = tp->task;
    const int threadId = tp->threadId;

    while (true) {
        EnterCriticalSection(&task->cs);

        // Проверка, есть ли еще элементы для добавления
        if (task->currentIndex >= task->totalElements) {
            LeaveCriticalSection(&task->cs);
            break;
        }

        // Получаем следующий элемент
        int element = task->fileData[task->currentIndex];
        task->currentIndex++;

        // Случайное решение: вставить в начало (0) или конец (1)
        if (const int position = rand() % 2; position == 0) {
            task->myDeque.push_front(element);
            // cout << "Поток " << threadId << ": добавлен элемент "
            //          << element << " в НАЧАЛО дека" << endl;
        } else {
            task->myDeque.push_back(element);
            // cout << "Поток " << threadId << ": добавлен элемент "
            //          << element << " в КОНЕЦ дека" << endl;
        }

        LeaveCriticalSection(&task->cs);
    }

    // cout << "Поток " << threadId << " завершил работу" << endl;
    delete tp;
    return 0;
}

void Task4::run() {
    cout << "\n=== ЗАДАНИЕ 4: ФОРМИРОВАНИЕ ДЕКА ===" << endl;

    string filename;
    int n;

    cout << "Введите имя файла с числами: ";
    cin >> filename;

    myDeque.clear();
    loadDataFromFile(filename);

    totalElements = fileData.size();

    if (totalElements == 0) {
        cout << "Нет данных для обработки!" << endl;
        return;
    }

    cout << "Введите количество потоков: ";
    cin >> n;

    if (n <= 0) {
        cout << "Некорректное количество потоков!" << endl;
        return;
    }

    auto* hThread = new HANDLE[n];
    auto* dwThreadID = new DWORD[n];

    currentIndex = 0;

    cout << "\n=== Запуск " << n << " потоков ===" << endl;
    for (int i = 0; i < n; i++) {
        auto* tp = new ThreadParam;
        tp->task = this;
        tp->threadId = i + 1;

        hThread[i] = CreateThread(nullptr, 0, addToDequeThread,
                                   tp, 0, &(dwThreadID[i]));

        if (hThread[i] == nullptr) {
            cout << "Ошибка создания потока " << GetLastError() << endl;
            delete[] hThread;
            delete[] dwThreadID;
            return;
        }
    }

    WaitForMultipleObjects(n, hThread, TRUE, INFINITE);

    printDeque();

    for (int i = 0; i < n; i++) {
        CloseHandle(hThread[i]);
    }

    delete[] hThread;
    delete[] dwThreadID;
}