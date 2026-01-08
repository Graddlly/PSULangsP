#include <iostream>
#include <windows.h>
#include <locale>
#include "headers/Task1.h"
#include "headers/Task2.h"
#include "headers/Task3.h"
#include "headers/Task4.h"

using namespace std;

void showMenu() {
    cout << "\n========================================" << endl;
    cout << "         МНОГОПОТОЧНЫЕ ЗАДАЧИ" << endl;
    cout << "========================================" << endl;
    cout << "1. Контрольная сумма текста" << endl;
    cout << "2. Формирование и суммирование матрицы" << endl;
    cout << "3. Площадь криволинейной трапеции" << endl;
    cout << "4. Формирование дека из файла" << endl;
    cout << "0. Выход" << endl;
    cout << "========================================" << endl;
    cout << "Выберите задание: ";
}

int main() {
    SetConsoleOutputCP(CP_UTF8);
    srand(static_cast<unsigned int>(time(nullptr)));

    int choice;

    while (true) {
        showMenu();
        cin >> choice;

        if (cin.fail()) {
            cin.clear();
            cin.ignore(10000, '\n');
            cout << "Некорректный ввод! Попробуйте снова." << endl;
            continue;
        }

        switch (choice) {
            case 1: {
                Task1 task1;
                task1.run();
                break;
            }
            case 2: {
                Task2 task2;
                task2.run();
                break;
            }
            case 3: {
                Task3 task3;
                task3.run();
                break;
            }
            case 4: {
                Task4 task4;
                task4.run();
                break;
            }
            case 0:
                cout << "\nВыход из программы." << endl;
                return 0;
            default:
                cout << "Неверный выбор! Попробуйте снова." << endl;
        }

        cout << "\nНажмите Enter для продолжения...";
        cin.ignore();
        cin.get();
    }
}