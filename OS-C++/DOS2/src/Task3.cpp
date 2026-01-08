#include "../headers/Task3.h"

Task3::Task3() : totalArea(0), a(0), b(0), n(0), m(0) {
    InitializeCriticalSection(&cs);
}

Task3::~Task3() {
    DeleteCriticalSection(&cs);
}

double Task3::f(double x) {
    return x * x + 1; // y = x^2 + 1
    // Другие примеры:
    // return sin(x) + 2;
    // return exp(x);
}

DWORD WINAPI Task3::calculateElementThread(LPVOID param) {
    const auto* tp = static_cast<ThreadParam *>(param);
    Task3* task = tp->task;
    const int i = tp->elementIndex;
    const double xi = tp->xi;
    const double xi1 = tp->xi1;

    // Вычисление площади методом прямоугольников
    const double h = (xi1 - xi) / task->m;
    double localArea = 0;

    // Метод средних прямоугольников
    for (int j = 0; j < task->m; j++) {
        const double x_mid = xi + h * (j + 0.5);
        localArea += f(x_mid) * h;
    }

    // Добавление локальной площади к общей
    EnterCriticalSection(&task->cs);
    task->totalArea += localArea;
    cout << "Элемент " << i << " [" << fixed << setprecision(3)
              << xi << ", " << xi1 << "]: площадь = " << localArea << endl;
    LeaveCriticalSection(&task->cs);

    delete tp;
    return 0;
}

void Task3::run() {
    cout << "\n=== ЗАДАНИЕ 3: ПЛОЩАДЬ КРИВОЛИНЕЙНОЙ ТРАПЕЦИИ ===" << endl;
    cout << "Функция: y = x^2 + 1" << endl;

    cout << "Введите левую границу a: ";
    cin >> a;
    cout << "Введите правую границу b: ";
    cin >> b;

    if (a >= b) {
        cout << "Ошибка: a должно быть меньше b!" << endl;
        return;
    }

    cout << "Введите количество элементов разбиения n: ";
    cin >> n;
    cout << "Введите количество подразбиений m для каждого элемента: ";
    cin >> m;

    if (n <= 0 || m <= 0) {
        cout << "Ошибка: n и m должны быть положительными!" << endl;
        return;
    }

    auto* hThread = new HANDLE[n];
    auto* dwThreadID = new DWORD[n];

    totalArea = 0;

    const double h_element = (b - a) / n;

    cout << "\n=== Вычисление площадей элементов ===" << endl;
    for (int i = 0; i < n; i++) {
        auto* tp = new ThreadParam;
        tp->task = this;
        tp->elementIndex = i;
        tp->xi = a + i * h_element;
        tp->xi1 = a + (i + 1) * h_element;

        hThread[i] = CreateThread(nullptr, 0, calculateElementThread,
                                   tp, 0, &(dwThreadID[i]));

        if (hThread[i] == nullptr) {
            cout << "Ошибка создания потока " << GetLastError() << endl;
            delete[] hThread;
            delete[] dwThreadID;
            return;
        }
    }

    WaitForMultipleObjects(n, hThread, TRUE, INFINITE);

    cout << "\n=== Результат ===" << endl;
    cout << "Приближенная площадь криволинейной трапеции: "
              << fixed << setprecision(6) << totalArea << endl;

    // Аналитическое решение для проверки (для f(x) = x^2 + 1)
    const double analytical = (b*b*b/3.0 + b) - (a*a*a/3.0 + a);
    cout << "Аналитическое решение (для проверки): " << analytical << endl;
    cout << "Погрешность: " << abs(totalArea - analytical) << endl;

    for (int i = 0; i < n; i++) {
        CloseHandle(hThread[i]);
    }

    delete[] hThread;
    delete[] dwThreadID;
}