//
// Created by Novik on 14.12.2025.
//

#include "../headers/task2.h"

namespace Task2
{
    char buffer[BUFFER_SIZE];
    int writePos = 0;
    int processPos = 0;  // Позиция для обработки
    int readPos = 0;     // Позиция для чтения
    int charsInBuffer = 0;
    bool inputFinished = false;
    bool encryptionFinished = false;

    HANDLE hSemFull = nullptr;
    HANDLE hSemEmpty = nullptr;
    HANDLE hSemProcessed = nullptr;
    HANDLE hMutexBuffer = nullptr;
    HANDLE hMutexProcess = nullptr;  // Мьютекс для позиции обработки
    HANDLE hMutexFile = nullptr;

    ifstream inputFile;
    ofstream outputFile;

    DWORD WINAPI reader(LPVOID param)
    {
        char ch;
        while(inputFile.get(ch))
        {
            WaitForSingleObject(hSemEmpty, INFINITE);
            WaitForSingleObject(hMutexBuffer, INFINITE);

            buffer[writePos] = ch;
            writePos++;
            if(writePos >= BUFFER_SIZE)
                writePos = 0;
            charsInBuffer++;

            ReleaseMutex(hMutexBuffer);
            ReleaseSemaphore(hSemFull, 1, nullptr);
        }

        inputFinished = true;
        cout << "Чтение завершено\n";
        return 0;
    }

    DWORD WINAPI encryptor(const LPVOID param)
    {
        const int threadId = *static_cast<int *>(param);
        delete static_cast<int *>(param);

        while(true)
        {
            if(WaitForSingleObject(hSemFull, 100) == WAIT_TIMEOUT)
            {
                if(inputFinished)
                {
                    WaitForSingleObject(hMutexBuffer, INFINITE);
                    const bool noMore = (charsInBuffer == 0);
                    ReleaseMutex(hMutexBuffer);
                    if(noMore)
                        break;
                }
                continue;
            }

            // Атомарно получаем позицию для обработки
            WaitForSingleObject(hMutexProcess, INFINITE);
            const int pos = processPos;
            processPos++;
            if(processPos >= BUFFER_SIZE)
                processPos = 0;
            ReleaseMutex(hMutexProcess);

            // Шифруем символ
            char& c = buffer[pos];

            if(c >= 'a' && c <= 'z')
                c = c - 'a' + 'A';

            if(c >= 'A' && c <= 'Z')
            {
                if(c == 'Z')
                    c = 'A';
                else
                    c = c + 1;
            }

            // Сигнализируем о готовности
            ReleaseSemaphore(hSemProcessed, 1, nullptr);
        }

        cout << "Шифровщик " << threadId << " завершён\n";
        return 0;
    }

    DWORD WINAPI writer(LPVOID param)
    {
        while(true)
        {
            if(WaitForSingleObject(hSemProcessed, 100) == WAIT_TIMEOUT)
            {
                if(inputFinished)
                {
                    WaitForSingleObject(hMutexBuffer, INFINITE);
                    const bool noMore = (charsInBuffer == 0);
                    ReleaseMutex(hMutexBuffer);
                    if(noMore)
                        break;
                }
                continue;
            }

            WaitForSingleObject(hMutexBuffer, INFINITE);

            const char ch = buffer[readPos];
            readPos++;
            if(readPos >= BUFFER_SIZE)
                readPos = 0;
            charsInBuffer--;

            ReleaseMutex(hMutexBuffer);

            WaitForSingleObject(hMutexFile, INFINITE);
            outputFile.put(ch);
            ReleaseMutex(hMutexFile);

            ReleaseSemaphore(hSemEmpty, 1, nullptr);
        }

        encryptionFinished = true;
        cout << "Запись завершена\n";
        return 0;
    }
}

void runTask2()
{
    using namespace Task2;

    writePos = 0;
    processPos = 0;
    readPos = 0;
    charsInBuffer = 0;
    inputFinished = false;
    encryptionFinished = false;

    inputFile.open("input.txt");
    outputFile.open("encrypted.txt");

    if(!inputFile.is_open())
    {
        cerr << "Ошибка открытия input.txt\n";
        return;
    }

    HANDLE hEncryptors[NUM_CRYPT];

    hSemFull = CreateSemaphore(nullptr, 0, BUFFER_SIZE, nullptr);
    hSemEmpty = CreateSemaphore(nullptr, BUFFER_SIZE, BUFFER_SIZE, nullptr);
    hSemProcessed = CreateSemaphore(nullptr, 0, BUFFER_SIZE, nullptr);
    hMutexBuffer = CreateMutex(nullptr, FALSE, nullptr);
    hMutexProcess = CreateMutex(nullptr, FALSE, nullptr);
    hMutexFile = CreateMutex(nullptr, FALSE, nullptr);

    const HANDLE hReader = CreateThread(nullptr, 0, reader, nullptr,
        0, nullptr);

    for(int i = 0; i < NUM_CRYPT; i++)
    {
        const auto id = new int(i + 1);
        hEncryptors[i] = CreateThread(nullptr, 0, encryptor, id, 0, nullptr);
    }

    const HANDLE hWriter = CreateThread(nullptr, 0, writer, nullptr,
        0, nullptr);

    WaitForSingleObject(hReader, INFINITE);
    WaitForMultipleObjects(NUM_CRYPT, hEncryptors, TRUE, INFINITE);
    WaitForSingleObject(hWriter, INFINITE);

    inputFile.close();
    outputFile.close();

    cout << "\nШифрование завершено! Результат в encrypted.txt\n";

    CloseHandle(hReader);
    for(int i = 0; i < NUM_CRYPT; i++)
        CloseHandle(hEncryptors[i]);
    CloseHandle(hWriter);

    CloseHandle(hSemFull);
    CloseHandle(hSemEmpty);
    CloseHandle(hSemProcessed);
    CloseHandle(hMutexBuffer);
    CloseHandle(hMutexProcess);
    CloseHandle(hMutexFile);
}