cmake_minimum_required(VERSION 4.1)
project(DOS4)

set(CMAKE_CXX_STANDARD 23)

add_executable(DOS4 main.cpp main.h
        src/task1.cpp headers/task1.h
        src/task2.cpp headers/task2.h
        src/task3.cpp headers/task3.h
        src/task4.cpp headers/task4.h)
