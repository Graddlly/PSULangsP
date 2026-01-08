#include <windows.h>
#include <iostream>
#include "main.h"

void printMenu()
{
    system("cls");
    cout << "========================================\n";
    cout << "  МНОГОПОТОЧНЫЕ ЗАДАЧИ С МЬЮТЕКСАМИ\n";
    cout << "========================================\n\n";
    cout << "1. Задача 1: Контрольная сумма файла\n";
    cout << "2. Задача 2: Шифрование файла\n";
    cout << "3. Задача 3: Дешифрование файла\n";
    cout << "4. Задача 4: Хеш-таблица\n";
    cout << "5. Задача 5: Хеш-таблица с монитором\n";
    cout << "0. Выход\n\n";
    cout << "Выберите задачу: ";
}

int main()
{
    SetConsoleOutputCP(CP_UTF8);

    int choice;

    while (true)
    {
        printMenu();
        cin >> choice;

        if (cin.fail())
        {
            cin.clear();
            cin.ignore(10000, '\n');
            cout << "\nНеверный ввод! Нажмите Enter...";
            cin.get();
            continue;
        }

        cin.ignore();
        system("cls");

        switch (choice)
        {
            case 1:
                task1_checksum();
                break;
            case 2:
                task2_encryption();
                break;
            case 3:
                task3_decryption();
                break;
            case 4:
                task4_hashtable();
                break;
            case 5:
                task5_monitor();
                break;
            case 0:
                cout << "Выход из программы...\n";
                return 0;
            default:
                cout << "Неверный выбор!\n";
        }

        cout << "\n\nНажмите Enter для возврата в меню...";
        cin.get();
    }
}