cmake_minimum_required(VERSION 4.0)
project(DOS3)

set(CMAKE_CXX_STANDARD 23)

add_executable(DOS3 main.cpp main.h
        src/task1.cpp
        src/task2.cpp
        src/task3.cpp
        src/task4.cpp
        src/task5.cpp)
