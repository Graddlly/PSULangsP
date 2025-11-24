//
// Created by Novik on 24.11.2025.
//

#include "../main.h"
#include <windows.h>
#include <stdio.h>
#include <iostream>
#include <fstream>
#include <string>

namespace Task3
{
    constexpr int BUFFER_SIZE = 256;

    // Два буфера для конвейера
    struct Buffer {
        char data[BUFFER_SIZE];
        int length;
        bool hasData;
        bool isFinished;
    };

    Buffer buffer1; // Между читателем и дешифровщиком
    Buffer buffer2; // Между дешифровщиком и писателем

    HANDLE hMutex1; // Защита buffer1
    HANDLE hMutex2; // Защита buffer2

    string decryptionKey;

    // Функции дешифрования
    void reverseString(char* str, const int len) {
        for (int i = 0; i < len / 2; i++) {
            const char temp = str[i];
            str[i] = str[len - 1 - i];
            str[len - 1 - i] = temp;
        }
    }

    void xorDecrypt(char* data, const int len, const string& key) {
        const int keyLen = key.length();
        for (int i = 0; i < len; i++) {
            data[i] ^= key[i % keyLen];
        }
    }

    // Поток 1: Чтение из зашифрованного файла
    DWORD WINAPI readerThread(const LPVOID param) {
        const auto filename = static_cast<const char *>(param);
        ifstream inFile(filename, ios::binary);

        if (!inFile.is_open()) {
            cout << "Ошибка открытия входного файла!\n";
            WaitForSingleObject(hMutex1, INFINITE);
            buffer1.isFinished = true;
            ReleaseMutex(hMutex1);
            return 1;
        }

        while (!inFile.eof()) {
            char tempBuffer[BUFFER_SIZE];
            inFile.read(tempBuffer, BUFFER_SIZE);

            if (const int bytesRead = inFile.gcount(); bytesRead > 0) {
                // Ждем, пока buffer1 освободится
                while (true) {
                    WaitForSingleObject(hMutex1, INFINITE);
                    if (!buffer1.hasData) {
                        // Буфер свободен, записываем данные
                        memcpy(buffer1.data, tempBuffer, bytesRead);
                        buffer1.length = bytesRead;
                        buffer1.hasData = true;
                        cout << "[Читатель] Прочитано: " << bytesRead << " байт\n";
                        ReleaseMutex(hMutex1);
                        break;
                    }
                    ReleaseMutex(hMutex1);
                    Sleep(10); // Небольшая задержка
                }
            }
        }

        inFile.close();

        // Сигнализируем о завершении
        WaitForSingleObject(hMutex1, INFINITE);
        buffer1.isFinished = true;
        ReleaseMutex(hMutex1);

        cout << "[Читатель] Чтение завершено\n";
        return 0;
    }

    // Поток 2: Дешифрование
    DWORD WINAPI decryptThread(LPVOID param) {
        while (true) {
            char tempBuffer[BUFFER_SIZE];
            int len = 0;
            bool finished = false;

            // Читаем из buffer1
            while (true) {
                WaitForSingleObject(hMutex1, INFINITE);
                if (buffer1.hasData) {
                    memcpy(tempBuffer, buffer1.data, buffer1.length);
                    len = buffer1.length;
                    buffer1.hasData = false; // Освобождаем буфер
                    ReleaseMutex(hMutex1);
                    break;
                }
                if (buffer1.isFinished) {
                    finished = true;
                    ReleaseMutex(hMutex1);
                    break;
                }
                ReleaseMutex(hMutex1);
                Sleep(10);
            }

            if (finished && len == 0) break;

            if (len > 0) {
                // Дешифруем: обратный порядок операций
                // Сначала XOR (отменяет XOR из шифрования)
                xorDecrypt(tempBuffer, len, decryptionKey);
                // Потом реверс (отменяет реверс из шифрования)
                reverseString(tempBuffer, len);

                cout << "[Дешифровщик] Расшифровано: " << len << " байт\n";

                // Записываем в buffer2
                while (true) {
                    WaitForSingleObject(hMutex2, INFINITE);
                    if (!buffer2.hasData) {
                        memcpy(buffer2.data, tempBuffer, len);
                        buffer2.length = len;
                        buffer2.hasData = true;
                        ReleaseMutex(hMutex2);
                        break;
                    }
                    ReleaseMutex(hMutex2);
                    Sleep(10);
                }
            }
        }

        // Сигнализируем о завершении
        WaitForSingleObject(hMutex2, INFINITE);
        buffer2.isFinished = true;
        ReleaseMutex(hMutex2);

        cout << "[Дешифровщик] Дешифрование завершено\n";
        return 0;
    }

    // Поток 3: Запись в файл
    DWORD WINAPI writerThread(const LPVOID param) {
        const auto filename = static_cast<const char *>(param);
        ofstream outFile(filename, ios::binary);

        if (!outFile.is_open()) {
            cout << "Ошибка открытия выходного файла!\n";
            return 1;
        }

        while (true) {
            char tempBuffer[BUFFER_SIZE];
            int len = 0;
            bool finished = false;

            // Читаем из buffer2
            while (true) {
                WaitForSingleObject(hMutex2, INFINITE);
                if (buffer2.hasData) {
                    memcpy(tempBuffer, buffer2.data, buffer2.length);
                    len = buffer2.length;
                    buffer2.hasData = false; // Освобождаем буфер
                    ReleaseMutex(hMutex2);
                    break;
                }
                if (buffer2.isFinished) {
                    finished = true;
                    ReleaseMutex(hMutex2);
                    break;
                }
                ReleaseMutex(hMutex2);
                Sleep(10);
            }

            if (finished && len == 0) break;

            if (len > 0) {
                outFile.write(tempBuffer, len);
                cout << "[Писатель] Записано: " << len << " байт\n";
            }
        }

        outFile.close();
        cout << "[Писатель] Запись завершена\n";
        return 0;
    }
}

void task3_decryption()
{
    using namespace Task3;

    cout << "=== ЗАДАЧА 3: ДЕШИФРОВАНИЕ ФАЙЛА ===\n\n";

    string inputFile, outputFile;

    cout << "Введите имя зашифрованного файла: ";
    cin >> inputFile;

    cout << "Введите имя выходного файла: ";
    cin >> outputFile;

    cout << "Введите ключ дешифрования: ";
    cin >> decryptionKey;

    if (decryptionKey.empty()) {
        cout << "Ошибка: ключ не может быть пустым!\n";
        cin.get();
        exit(1);
    }

    // Инициализация буферов
    buffer1.hasData = false;
    buffer1.isFinished = false;
    buffer1.length = 0;

    buffer2.hasData = false;
    buffer2.isFinished = false;
    buffer2.length = 0;

    // Создаем мьютексы
    hMutex1 = CreateMutex(nullptr, FALSE, nullptr);
    hMutex2 = CreateMutex(nullptr, FALSE, nullptr);

    cout << "\n--- Начало дешифрования ---\n";

    // Создаем потоки
    const HANDLE hReader = CreateThread(nullptr, 0, readerThread,
        (LPVOID)inputFile.c_str(), 0, nullptr);
    const HANDLE hDecrypt = CreateThread(nullptr, 0, decryptThread,
        nullptr, 0, nullptr);
    const HANDLE hWriter = CreateThread(nullptr, 0, writerThread,
        (LPVOID)outputFile.c_str(), 0, nullptr);

    const HANDLE hThreads[3] = { hReader, hDecrypt, hWriter };

    // Ожидаем завершения всех потоков
    WaitForMultipleObjects(3, hThreads, TRUE, INFINITE);

    // Закрываем дескрипторы
    CloseHandle(hReader);
    CloseHandle(hDecrypt);
    CloseHandle(hWriter);
    CloseHandle(hMutex1);
    CloseHandle(hMutex2);

    cout << "\n=== ДЕШИФРОВАНИЕ ЗАВЕРШЕНО УСПЕШНО! ===\n";
    cin.get();
}