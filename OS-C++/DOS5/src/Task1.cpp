//
// Created by graddlly on 08.01.2026.
//

#include "../headers/Task1.h"
#include <pthread.h>
#include <iostream>
#include <vector>

struct ThreadData {
    vector<float>* arr;
    int passNumber;
    int n;
    pthread_mutex_t* mutex;
    pthread_cond_t* cond;
    vector<bool>* halfDone;
};

void* sortPass(void* arg) {
    const auto* data = static_cast<ThreadData *>(arg);
    vector<float>& arr = *(data->arr);
    const int n = data->n;
    const int passNum = data->passNumber;

    // Ждём, пока предыдущий поток обработает половину
    if (passNum > 0) {
        pthread_mutex_lock(data->mutex);
        while (!(*(data->halfDone))[passNum - 1]) {
            pthread_cond_wait(data->cond, data->mutex);
        }
        pthread_mutex_unlock(data->mutex);
    }

    cout << "Поток прохода " << passNum + 1 << " начал работу\n";

    const int limit = n - passNum - 1;
    const int halfPoint = limit / 2;

    for (int i = 0; i < limit; i++) {
        pthread_mutex_lock(data->mutex);
        if (arr[i] > arr[i + 1]) {
            swap(arr[i], arr[i + 1]);
        }
        pthread_mutex_unlock(data->mutex);

        // Сигнализируем о достижении половины
        if (i == halfPoint && !(*(data->halfDone))[passNum]) {
            pthread_mutex_lock(data->mutex);
            (*(data->halfDone))[passNum] = true;
            pthread_cond_broadcast(data->cond);
            pthread_mutex_unlock(data->mutex);
            cout << "Поток прохода " << passNum + 1 << " обработал половину\n";
        }
    }

    // Если ещё не установили флаг (для маленьких массивов)
    pthread_mutex_lock(data->mutex);
    if (!(*(data->halfDone))[passNum]) {
        (*(data->halfDone))[passNum] = true;
        pthread_cond_broadcast(data->cond);
    }
    pthread_mutex_unlock(data->mutex);

    cout << "Поток прохода " << passNum + 1 << " завершён\n";
    return nullptr;
}

void runTask1() {
    cout << "=== ЗАДАНИЕ 1: Сортировка обменом с потоками ===\n";

    int n;
    cout << "Введите размер массива: ";
    cin >> n;

    if (n <= 0) {
        cout << "Ошибка: размер массива должен быть положительным!\n";
        return;
    }

    vector<float> arr(n);
    cout << "Введите элементы массива:\n";
    for (int i = 0; i < n; i++) {
        cout << "arr[" << i << "] = ";
        cin >> arr[i];
    }

    cout << "\nИсходный массив:\n";
    for (int i = 0; i < n; i++) {
        cout << arr[i] << " ";
    }
    cout << "\n\n";

    pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;
    pthread_cond_t cond = PTHREAD_COND_INITIALIZER;

    vector<pthread_t> threads(n - 1);
    vector<ThreadData> threadData(n - 1);
    vector halfDone(n - 1, false);

    // Создание потоков для каждого прохода
    for (int i = 0; i < n - 1; i++) {
        threadData[i].arr = &arr;
        threadData[i].passNumber = i;
        threadData[i].n = n;
        threadData[i].mutex = &mutex;
        threadData[i].cond = &cond;
        threadData[i].halfDone = &halfDone;

        pthread_create(&threads[i], nullptr, sortPass, &threadData[i]);
    }

    // Ожидание завершения всех потоков
    for (int i = 0; i < n - 1; i++) {
        pthread_join(threads[i], nullptr);
    }

    cout << "\nОтсортированный массив:\n";
    for (int i = 0; i < n; i++) {
        cout << arr[i] << " ";
    }
    cout << endl;

    pthread_mutex_destroy(&mutex);
    pthread_cond_destroy(&cond);
}