#ifndef DOS2_TASK1_H
#define DOS2_TASK1_H

#include <windows.h>
#include <string>
#include "iostream"

using namespace std;

class Task1 {
    CRITICAL_SECTION cs{};
    int checksum;
    std::string text;
    int k; // количество потоков

    struct ThreadParam {
        Task1* task;
        int threadIndex;
    };

    static DWORD WINAPI calculateChecksumThread(LPVOID param);

public:
    Task1();
    ~Task1();
    void run();
};


#endif //DOS2_TASK1_H