#include "Headers/TaskMenu.h"

#include "Headers/TaskMenu.h"
#include "Headers/Task1.h"
#include "Headers/Task2.h"
#include "Headers/Task3.h"
#include "Headers/Task4.h"
#include "Headers/Task5.h"
#include "Headers/Task6.h"
#include "Headers/Task7.h"

TaskMenu::TaskMenu()
{
    tasks.push_back(std::make_unique<Task1_ParallelMatrixCreation>());
    tasks.push_back(std::make_unique<Task2_MaxAverageRow>());
    tasks.push_back(std::make_unique<Task3_SortColumns>());
    tasks.push_back(std::make_unique<Task4_MatrixDeterminant>());
    tasks.push_back(std::make_unique<Task5_BlockEncryption>());
    tasks.push_back(std::make_unique<Task6_BlockDecryption>());
    tasks.push_back(std::make_unique<Task7_BinaryTreeSum>());
}

void TaskMenu::displayMenu()
{
    std::cout << "\n========== МЕНЮ ЗАДАНИЙ ==========\n";
    for (size_t i = 0; i < tasks.size(); ++i) {
        std::cout << (i + 1) << ". " << tasks[i] -> getName() << "\n";
    }
    std::cout << "0. Выход\n";
    std::cout << "==================================\n";
    std::cout << "Выберите задание: ";
}

int TaskMenu::getUserChoice()
{
    int choice;
    std::cin >> choice;

    if (std::cin.fail()) {
        std::cin.clear();
        std::cin.ignore(10000, '\n');
        return -1;
    }

    return choice;
}

void TaskMenu::run()
{
    while (true) {
        displayMenu();
        int choice = getUserChoice();

        if (choice == 0) {
            std::cout << "До свидания!\n";
            break;
        }

        if (choice < 1 || choice > static_cast<int>(tasks.size())) {
            std::cout << "Неверный выбор! Попробуйте снова.\n";
            continue;
        }

        try {
            tasks[choice - 1] -> execute();
        }
        catch (const std::exception& e) {
            std::cout << "Ошибка при выполнении задания: " << e.what() << "\n";
        }

        std::cout << "\nНажмите Enter для продолжения...";
        std::cin.ignore();
        std::cin.get();
    }
}