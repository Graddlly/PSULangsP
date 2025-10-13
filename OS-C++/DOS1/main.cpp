#include "Headers/TaskMenu.h"
#include <ctime>
#include <locale>

int main()
{
    SetConsoleOutputCP(CP_UTF8);
    srand(static_cast<unsigned int>(time(nullptr)));

    TaskMenu menu;
    menu.run();

    return 0;
}