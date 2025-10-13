#include "Headers/Task5.h"

DWORD WINAPI Task5_BlockEncryption::encrypt_block(LPVOID param)
{
    EncryptParam* p = static_cast<EncryptParam*>(param);
    std::string& block = *(p -> block);

    std::reverse(block.begin(), block.end());

    for (size_t i = 0; i < block.length(); i++) {
        block[i] ^= p -> key[i % p -> key.length()];
    }

    delete p;
    return 0;
}

void Task5_BlockEncryption::execute()
{
    displayHeader(getName());

    std::string text = "Novitskiy Dmitriy Vitalevich.";
    std::string key = "505";

    std::cout << "Исходный текст: " << text << "\n";
    std::cout << "Ключ: " << key << "\n";

    std::vector<std::string> blocks;
    for (size_t i = 0; i < text.length(); i += key.length()) {
        blocks.push_back(text.substr(i, key.length()));
    }

    std::cout << "Количество блоков: " << blocks.size() << "\n";

    std::vector<HANDLE> hThread(blocks.size());
    std::vector<DWORD> dwThreadID(blocks.size());

    for (size_t i = 0; i < blocks.size(); i++) {
        EncryptParam* param = new EncryptParam;
        param -> block = &blocks[i];
        param -> key = key;
        param -> block_id = i;

        hThread[i] = CreateThread(NULL, 0, encrypt_block, param, 0, &dwThreadID[i]);
    }

    WaitForMultipleObjects(blocks.size(), hThread.data(), TRUE, INFINITE);

    std::string encrypted_text;
    for (const auto& block : blocks) {
        encrypted_text += block;
    }

    std::cout << "Зашифрованный текст (ASCII коды): ";
    for (char c : encrypted_text) {
        std::cout << (int)(unsigned char)c << " ";
    }
    std::cout << "\n";

    for (size_t i = 0; i < hThread.size(); i++) {
        CloseHandle(hThread[i]);
    }
}