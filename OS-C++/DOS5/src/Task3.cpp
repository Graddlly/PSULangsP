//
// Created by graddlly on 08.01.2026.
//

#include "../headers/Task3.h"
#include <pthread.h>
#include <iostream>
#include <fstream>
#include <vector>
#include <string>
#include <climits>

struct MergeData {
    vector<string> fileNames;
    string outputFile;

    vector<int> values;          // Текущие значения из каждого файла
    vector<bool> dataReady;      // Флаги готовности данных
    vector<bool> fileEnded;      // Флаги окончания файлов
    bool allDataReady{};                // Все потоки предоставили данные
    bool continueWork{};                // Флаг продолжения работы

    pthread_mutex_t mutex{};
    pthread_cond_t condReaders{};       // Для потоков чтения
    pthread_cond_t condWriter{};        // Для потока записи
};

struct ReaderArgs {
    MergeData* data;
    int fileIndex;
};

// Поток чтения из файла
void* readerThread(void* arg) {
    const auto* args = static_cast<ReaderArgs *>(arg);
    MergeData* data = args->data;
    const int idx = args->fileIndex;

    ifstream file(data->fileNames[idx]);
    if (!file.is_open()) {
        cerr << "Ошибка открытия файла: " << data->fileNames[idx] << endl;
        pthread_mutex_lock(&data->mutex);
        data->fileEnded[idx] = true;
        pthread_cond_signal(&data->condWriter);
        pthread_mutex_unlock(&data->mutex);
        delete args;
        return nullptr;
    }

    cout << "Поток чтения " << idx + 1 << " начал работу с файлом " << data->fileNames[idx] << endl;

    while (data->continueWork) {
        // Читаем следующее значение
        if (int value; file >> value) {
            pthread_mutex_lock(&data->mutex);

            // Ждём, пока поток записи не обработает предыдущее значение
            while (data->dataReady[idx] /*&& data->continueWork*/) {
                pthread_cond_wait(&data->condReaders, &data->mutex);
            }

            /*if (!data->continueWork) {
                pthread_mutex_unlock(&data->mutex);
                break;
            }*/

            data->values[idx] = value;
            data->dataReady[idx] = true;
            cout << "  [Поток " << idx + 1 << "] прочитал: " << value << endl;

            // Проверяем, все ли потоки предоставили данные
            bool allReady = true;
            for (size_t i = 0; i < data->dataReady.size(); i++) {
                if (!data->dataReady[i] && !data->fileEnded[i]) {
                    allReady = false;
                    break;
                }
            }
            if (allReady) {
                data->allDataReady = true;
                pthread_cond_signal(&data->condWriter);
            }

            pthread_mutex_unlock(&data->mutex);
        } else {
            // Файл закончился
            pthread_mutex_lock(&data->mutex);
            data->fileEnded[idx] = true;
            cout << "  [Поток " << idx + 1 << "] завершил чтение файла" << endl;

            // Сигнализируем потоку записи
            bool allReady = true;
            for (size_t i = 0; i < data->dataReady.size(); i++) {
                if (!data->dataReady[i] && !data->fileEnded[i]) {
                    allReady = false;
                    break;
                }
            }
            if (allReady) {
                data->allDataReady = true;
                pthread_cond_signal(&data->condWriter);
            }

            pthread_mutex_unlock(&data->mutex);
            break;
        }
    }

    file.close();
    delete args;
    return nullptr;
}

// Поток записи результата
void* writerThread(void* arg) {
    auto* data = static_cast<MergeData *>(arg);
    ofstream outFile(data->outputFile);

    if (!outFile.is_open()) {
        cerr << "Ошибка создания выходного файла!" << endl;
        pthread_mutex_lock(&data->mutex);
        data->continueWork = false;
        pthread_cond_broadcast(&data->condReaders);
        pthread_mutex_unlock(&data->mutex);
        return nullptr;
    }

    cout << "Поток записи начал работу\n\n";

    while (true) {
        pthread_mutex_lock(&data->mutex);

        // Ждём, пока все потоки чтения предоставят данные
        while (!data->allDataReady && data->continueWork) {
            pthread_cond_wait(&data->condWriter, &data->mutex);
        }

        if (!data->continueWork) {
            pthread_mutex_unlock(&data->mutex);
            break;
        }

        // Проверяем, остались ли ещё данные для обработки
        bool hasData = false;
        for (size_t i = 0; i < data->dataReady.size(); i++) {
            if (data->dataReady[i] || !data->fileEnded[i]) {
                hasData = true;
                break;
            }
        }

        if (!hasData) {
            pthread_mutex_unlock(&data->mutex);
            break;
        }

        // Находим минимальное значение среди доступных
        int minValue = INT_MAX;
        int minIndex = -1;

        for (size_t i = 0; i < data->values.size(); i++) {
            if (data->dataReady[i] && data->values[i] < minValue) {
                minValue = data->values[i];
                minIndex = i;
            }
        }

        if (minIndex != -1) {
            // Записываем минимальное значение
            outFile << minValue << " ";
            cout << "[Запись] Записано: " << minValue << " (из файла " << minIndex + 1 << ")" << endl;

            // Сбрасываем флаг для этого потока
            data->dataReady[minIndex] = false;
        }

        // Проверяем, все ли активные потоки предоставили данные
        bool allReady = true;
        for (size_t i = 0; i < data->dataReady.size(); i++) {
            if (!data->dataReady[i] && !data->fileEnded[i]) {
                allReady = false;
                break;
            }
        }

        if (!allReady) {
            data->allDataReady = false;
        }

        // Сигнализируем потокам чтения
        pthread_cond_broadcast(&data->condReaders);

        pthread_mutex_unlock(&data->mutex);
    }

    outFile.close();
    cout << "\n[Запись] Слияние завершено. Результат в файле " << data->outputFile << endl;

    pthread_mutex_lock(&data->mutex);
    data->continueWork = false;
    pthread_cond_broadcast(&data->condReaders);
    pthread_mutex_unlock(&data->mutex);

    return nullptr;
}

void runTask3() {
    cout << "=== ЗАДАНИЕ 3: Слияние отсортированных файлов ===\n";

    int n;
    cout << "Введите количество файлов для слияния: ";
    cin >> n;

    if (n <= 0) {
        cout << "Ошибка: количество файлов должно быть положительным!\n";
        return;
    }

    MergeData data;
    data.fileNames.resize(n);
    data.values.resize(n);
    data.dataReady.resize(n, false);
    data.fileEnded.resize(n, false);
    data.allDataReady = false;
    data.continueWork = true;

    cout << "Введите имена входных файлов:\n";
    for (int i = 0; i < n; i++) {
        cout << "Файл " << i + 1 << ": ";
        cin >> data.fileNames[i];
    }

    cout << "Введите имя выходного файла: ";
    cin >> data.outputFile;

    pthread_mutex_init(&data.mutex, nullptr);
    pthread_cond_init(&data.condReaders, nullptr);
    pthread_cond_init(&data.condWriter, nullptr);

    cout << "\nНачало слияния...\n";

    // Создаём потоки чтения
    vector<pthread_t> readers(n);
    for (int i = 0; i < n; i++) {
        auto* args = new ReaderArgs{&data, i};
        pthread_create(&readers[i], nullptr, readerThread, args);
    }

    // Создаём поток записи
    pthread_t writer;
    pthread_create(&writer, nullptr, writerThread, &data);

    // Ждём завершения всех потоков
    for (int i = 0; i < n; i++) {
        pthread_join(readers[i], nullptr);
    }
    pthread_join(writer, nullptr);

    pthread_mutex_destroy(&data.mutex);
    pthread_cond_destroy(&data.condReaders);
    pthread_cond_destroy(&data.condWriter);

    cout << "\nОперация слияния успешно завершена!" << endl;
}