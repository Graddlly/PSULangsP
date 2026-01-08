//
// Created by Novik on 14.12.2025.
//

#include "../headers/task1.h"

namespace Task1
{
    Request buffer[BUFFER_SIZE];
    int cur_pos = 0;
    map<string, int> totalRequest;

    HANDLE hSemFull = nullptr;
    HANDLE hSemEmpty = nullptr;
    HANDLE hMutex = nullptr;
    HANDLE hMutexTotal = nullptr;

    DWORD WINAPI shop(const LPVOID param)
    {
        const auto p = static_cast<ShopParam *>(param);
        const int shopId = p->shopId;
        const vector<Request> requests = p->requests;

        for(int i = 0; i < requests.size(); i++)
        {
            WaitForSingleObject(hSemEmpty, INFINITE);
            WaitForSingleObject(hMutex, INFINITE);

            buffer[cur_pos] = requests[i];
            cout << "Цех " << shopId << " отправил заявку " << (i+1)
                      << ": " << requests[i].material << " - "
                      << requests[i].quantity << " ед.\n";
            cur_pos++;

            ReleaseMutex(hMutex);
            ReleaseSemaphore(hSemFull, 1, nullptr);

            Sleep(50 + rand() % 100);
        }

        delete p;
        return 0;
    }

    DWORD WINAPI processingCenter(LPVOID param)
    {
        constexpr int totalRequests = NUM_FACTORIES * MAX_QUERIES_FROM_ONE;

        for(int i = 0; i < totalRequests; i++)
        {
            WaitForSingleObject(hSemFull, INFINITE);
            WaitForSingleObject(hMutex, INFINITE);

            cur_pos--;
            auto [material, quantity] = buffer[cur_pos];

            ReleaseMutex(hMutex);

            WaitForSingleObject(hMutexTotal, INFINITE);

            if(totalRequest.contains(material))
            {
                totalRequest[material] += quantity;
            }
            else
            {
                totalRequest[material] = quantity;
            }

            cout << "  -> Обработана заявка: " << material
                      << " - " << quantity << " ед. (итого: "
                      << totalRequest[material] << ")\n";

            ReleaseMutex(hMutexTotal);
            ReleaseSemaphore(hSemEmpty, 1, nullptr);

            Sleep(30);
        }

        return 0;
    }
}

void runTask1()
{
    using namespace Task1;

    srand(time(nullptr));

    HANDLE hShops[NUM_FACTORIES];

    cur_pos = 0;
    totalRequest.clear();

    hSemFull = CreateSemaphore(nullptr, 0, BUFFER_SIZE, nullptr);
    hSemEmpty = CreateSemaphore(nullptr, BUFFER_SIZE, BUFFER_SIZE, nullptr);
    hMutex = CreateMutex(nullptr, FALSE, nullptr);
    hMutexTotal = CreateMutex(nullptr, FALSE, nullptr);

    // Генерируем тестовые данные для цехов

    cout << "Генерация заявок для цехов...\n\n";

    // Или можно сделать ручной ввод:
    char choice;
    cout << "Ввести заявки вручную? (y/n): ";
    cin >> choice;
    cin.ignore();

    // Запускаем обрабатывающий центр
    const HANDLE hCenter = CreateThread(nullptr, 0, processingCenter, nullptr,
        0, nullptr);

    if(choice == 'y' || choice == 'Y')
    {
        // Ручной ввод - сначала собираем все данные
        vector<ShopParam*> params;

        for(int i = 0; i < NUM_FACTORIES; i++)
        {
            auto param = new ShopParam;
            param->shopId = i + 1;

            cout << "\n--- Цех " << (i+1) << " ---\n";
            for(int j = 0; j < MAX_QUERIES_FROM_ONE; j++)
            {
                Request req;
                cout << "Заявка " << (j+1) << ":\n";
                cout << "  Материал: ";
                getline(cin, req.material);
                cout << "  Количество: ";
                cin >> req.quantity;
                cin.ignore();
                param->requests.push_back(req);
            }

            params.push_back(param);
        }

        // Теперь создаем потоки с уже готовыми данными
        cout << "\n=== Начало обработки заявок ===\n\n";
        for(int i = 0; i < NUM_FACTORIES; i++)
        {
            hShops[i] = CreateThread(nullptr, 0, shop, params[i], 0, nullptr);
        }
    }
    else
    {
        // Автоматическая генерация
        cout << "\n=== Начало обработки заявок ===\n\n";
        for(int i = 0; i < NUM_FACTORIES; i++)
        {
            const auto param = new ShopParam;
            param->shopId = i + 1;

            for(int j = 0; j < MAX_QUERIES_FROM_ONE; j++)
            {
                const string materials[] = {"Сталь", "Медь", "Алюминий", "Пластик", "Стекло",
                    "Дерево", "Резина", "Бетон", "Кирпич", "Цемент"};
                constexpr int materialsCount = 10;
                Request req;
                req.material = materials[rand() % materialsCount];
                req.quantity = 10 + rand() % 90;
                param->requests.push_back(req);
            }

            hShops[i] = CreateThread(nullptr, 0, shop, param, 0, nullptr);
        }
    }

    WaitForMultipleObjects(NUM_FACTORIES, hShops, TRUE, INFINITE);
    WaitForSingleObject(hCenter, INFINITE);

    cout << "\n========================================\n";
    cout << "       ИТОГОВАЯ СВОДНАЯ ЗАЯВКА\n";
    cout << "========================================\n";
    for(auto&[fst, snd] : totalRequest)
    {
        cout << fst << ": " << snd << " ед.\n";
    }
    cout << "========================================\n";

    for(int i = 0; i < NUM_FACTORIES; i++)
        CloseHandle(hShops[i]);
    CloseHandle(hCenter);

    CloseHandle(hSemFull);
    CloseHandle(hSemEmpty);
    CloseHandle(hMutex);
    CloseHandle(hMutexTotal);
}