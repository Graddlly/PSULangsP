#include <iostream>
#include "headers/Task1.h"
#include "headers/Task2.h"
#include "headers/Task3.h"

using namespace std;

void printMenu() {
    cout << "\n=== МЕНЮ ===" << endl;
    cout << "1. Задание 1: Сортировка массива обменом с потоками" << endl;
    cout << "2. Задание 2: Вычисление e^x через рандеву" << endl;
    cout << "3. Задание 3: Слияние отсортированных файлов" << endl;
    cout << "0. Выход" << endl;
    cout << "Выберите задание: ";
}

int main() {
    int choice;

    while (true) {
        printMenu();
        cin >> choice;

        if (cin.fail()) {
            cin.clear();
            cin.ignore(10000, '\n');
            cout << "Ошибка ввода! Попробуйте снова." << endl;
            continue;
        }

        cout << "\n";

        switch (choice) {
            case 1:
                runTask1();
                break;
            case 2:
                runTask2();
                break;
            case 3:
                runTask3();
                break;
            case 0:
                cout << "Выход из программы." << endl;
                return 0;
            default:
                cout << "Неверный выбор! Попробуйте снова." << endl;
        }

        cout << "\nНажмите Enter для продолжения...";
        cin.ignore(10000, '\n');
        cin.get();
    }

    return 0;
}