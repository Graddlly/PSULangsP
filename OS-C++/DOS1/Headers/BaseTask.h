#ifndef LW1_BASETASK_H
#define LW1_BASETASK_H

#pragma once
#include <windows.h>
#include <iostream>
#include <string>

class BaseTask {
public:
    virtual ~BaseTask() = default;
    virtual void execute() = 0;
    virtual std::string getName() const = 0;

protected:
    void displayHeader(const std::string& taskName) {
        std::cout << "\n=== " << taskName << " ===\n";
    }

    void displayResult(const std::string& result) {
        std::cout << "Результат: " << result << "\n";
    }
};

#endif