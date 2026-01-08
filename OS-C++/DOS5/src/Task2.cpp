//
// Created by graddlly on 08.01.2026.
//

#include "../headers/Task2.h"
#include <pthread.h>
#include <iostream>
#include <cmath>

struct RendezvousData {
    double x;
    int n;
    int currentTerm;

    double factorialResult;
    double powerResult;
    bool factorialReady;
    bool powerReady;
    bool requestSent;
    bool finished;

    pthread_mutex_t mutex;
    pthread_cond_t condFactorial;
    pthread_cond_t condPower;
    pthread_cond_t condMain;
};

// Обслуживающая задача: вычисление факториала
void* calculateFactorial(void* arg) {
    auto* data = static_cast<RendezvousData *>(arg);

    while (true) {
        pthread_mutex_lock(&data->mutex);

        // Ждём запроса от главной задачи
        while (!data->requestSent || data->factorialReady) {
            if (data->finished) {
                pthread_mutex_unlock(&data->mutex);
                return nullptr;
            }
            pthread_cond_wait(&data->condFactorial, &data->mutex);
        }

        if (data->finished) {
            pthread_mutex_unlock(&data->mutex);
            return nullptr;
        }

        int i = data->currentTerm;
        pthread_mutex_unlock(&data->mutex);

        // Вычисляем факториал
        double fact = 1;
        for (int j = 1; j <= i; j++) {
            fact *= j;
        }

        pthread_mutex_lock(&data->mutex);
        data->factorialResult = fact;
        data->factorialReady = true;

        cout << "  [Факториал] " << i << "! = " << fact << endl;

        // Сигнализируем о готовности
        pthread_cond_signal(&data->condMain);
        pthread_mutex_unlock(&data->mutex);
    }
}

// Обслуживающая задача: вычисление степени
void* calculatePower(void* arg) {
    auto* data = static_cast<RendezvousData *>(arg);

    while (true) {
        pthread_mutex_lock(&data->mutex);

        // Ждём запроса от главной задачи
        while (!data->requestSent || data->powerReady) {
            if (data->finished) {
                pthread_mutex_unlock(&data->mutex);
                return nullptr;
            }
            pthread_cond_wait(&data->condPower, &data->mutex);
        }

        if (data->finished) {
            pthread_mutex_unlock(&data->mutex);
            return nullptr;
        }

        int i = data->currentTerm;
        double x = data->x;
        pthread_mutex_unlock(&data->mutex);

        // Вычисляем степень
        double power = 1;
        for (int j = 0; j < i; j++) {
            power *= x;
        }

        pthread_mutex_lock(&data->mutex);
        data->powerResult = power;
        data->powerReady = true;

        cout << "  [Степень] " << x << "^" << i << " = " << power << endl;

        // Сигнализируем о готовности
        pthread_cond_signal(&data->condMain);
        pthread_mutex_unlock(&data->mutex);
    }
}

void runTask2() {
    cout << "=== ЗАДАНИЕ 2: Вычисление e^x через механизм рандеву ===\n";

    RendezvousData data{};

    cout << "Введите значение x: ";
    cin >> data.x;
    cout << "Введите количество членов ряда n: ";
    cin >> data.n;

    if (data.n < 0) {
        cout << "Ошибка: количество членов должно быть неотрицательным!\n";
        return;
    }

    data.currentTerm = 0;
    data.factorialReady = false;
    data.powerReady = false;
    data.requestSent = false;
    data.finished = false;

    pthread_mutex_init(&data.mutex, nullptr);
    pthread_cond_init(&data.condFactorial, nullptr);
    pthread_cond_init(&data.condPower, nullptr);
    pthread_cond_init(&data.condMain, nullptr);

    pthread_t threadFactorial, threadPower;

    // Создаём обслуживающие потоки
    pthread_create(&threadFactorial, nullptr, calculateFactorial, &data);
    pthread_create(&threadPower, nullptr, calculatePower, &data);

    // Главный поток вычисляет сумму
    cout << "\nВычисление e^" << data.x << " по формуле Маклорена:\n\n";

    double sum = 0;
    for (int i = 0; i <= data.n; i++) {
        pthread_mutex_lock(&data.mutex);

        // Устанавливаем текущий термин и запрашиваем вычисления
        data.currentTerm = i;
        data.factorialReady = false;
        data.powerReady = false;
        data.requestSent = true;

        // Разбуживаем обслуживающие потоки
        pthread_cond_broadcast(&data.condFactorial);
        pthread_cond_broadcast(&data.condPower);

        // Ждём результаты от обоих потоков
        while (!data.factorialReady || !data.powerReady) {
            pthread_cond_wait(&data.condMain, &data.mutex);
        }

        // Вычисляем член ряда
        const double term = data.powerResult / data.factorialResult;
        sum += term;

        data.requestSent = false;

        cout << "[Главный] Член " << i << " = " << term << ", текущая сумма = " << sum << "\n\n";

        pthread_mutex_unlock(&data.mutex);
    }

    // Сигнализируем потокам о завершении
    pthread_mutex_lock(&data.mutex);
    data.finished = true;
    pthread_cond_broadcast(&data.condFactorial);
    pthread_cond_broadcast(&data.condPower);
    pthread_mutex_unlock(&data.mutex);

    // Ждём завершения потоков
    pthread_join(threadFactorial, nullptr);
    pthread_join(threadPower, nullptr);

    cout << "====================================\n";
    cout << "Итоговый результат e^" << data.x << " = " << sum << endl;
    cout << "Проверка через exp(): " << exp(data.x) << endl;
    cout << "Погрешность: " << fabs(sum - exp(data.x)) << endl;

    pthread_mutex_destroy(&data.mutex);
    pthread_cond_destroy(&data.condFactorial);
    pthread_cond_destroy(&data.condPower);
    pthread_cond_destroy(&data.condMain);
}
