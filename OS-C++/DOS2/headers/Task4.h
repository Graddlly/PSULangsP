#ifndef DOS2_TASK4_H
#define DOS2_TASK4_H

#include <windows.h>
#include <deque>
#include <string>
#include <iostream>
#include <fstream>

using namespace std;

class Task4 {
    CRITICAL_SECTION cs{};
    std::deque<int> myDeque;
    std::deque<int> fileData;
    int currentIndex;
    int totalElements;

    struct ThreadParam {
        Task4* task;
        int threadId;
    };

    static DWORD WINAPI addToDequeThread(LPVOID param);
    bool loadDataFromFile(const std::string& filename);
    void printDeque() const;

public:
    Task4();
    ~Task4();
    void run();
};


#endif //DOS2_TASK4_H