#ifndef LW1_TASKMENU_H
#define LW1_TASKMENU_H

#pragma once
#include "BaseTask.h"
#include <vector>
#include <memory>

class TaskMenu {
    std::vector<std::unique_ptr<BaseTask>> tasks;

    void displayMenu();
    int getUserChoice();

public:
    TaskMenu();
    void run();
};

#endif