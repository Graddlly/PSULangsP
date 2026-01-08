cmake_minimum_required(VERSION 4.1)
project(DOS5)

set(CMAKE_CXX_STANDARD 23)

add_executable(DOS5 main.cpp
        src/Task1.cpp
        headers/Task1.h
        src/Task2.cpp
        headers/Task2.h
        src/Task3.cpp
        headers/Task3.h)
