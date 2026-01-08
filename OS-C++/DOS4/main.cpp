#include "main.h"

void showMenu()
{
    cout << "\n========================================\n";
    cout << "  МНОГОПОТОЧНЫЕ ПРОГРАММЫ С СЕМАФОРАМИ\n";
    cout << "========================================\n";
    cout << "1. Формирование общей заявки от цехов\n";
    cout << "2. Шифрование текста\n";
    cout << "3. Дешифрование текста\n";
    cout << "4. Управление пулом печати\n";
    cout << "0. Выход\n";
    cout << "========================================\n";
    cout << "Выберите задание: ";
}

int main()
{
    SetConsoleOutputCP(CP_UTF8);

    int choice;

    while(true)
    {
        showMenu();
        cin >> choice;

        system("cls");

        switch(choice)
        {
            case 1:
                cout << "=== ЗАДАНИЕ 1: Формирование общей заявки ===\n\n";
                runTask1();
                break;

            case 2:
                cout << "=== ЗАДАНИЕ 2: Шифрование текста ===\n\n";
                runTask2();
                break;

            case 3:
                cout << "=== ЗАДАНИЕ 3: Дешифрование текста ===\n\n";
                runTask3();
                break;

            case 4:
                cout << "=== ЗАДАНИЕ 4: Управление пулом печати ===\n\n";
                runTask4();
                break;

            case 0:
                cout << "Программа завершена.\n";
                return 0;

            default:
                cout << "Неверный выбор! Попробуйте снова.\n";
        }

        cout << "\nНажмите Enter для продолжения...";
        cin.ignore();
        cin.get();
        system("cls");
    }
}