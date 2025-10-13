#include "Headers/Task6.h"

DWORD WINAPI Task6_BlockDecryption::decrypt_block(LPVOID param)
{
    DecryptParam* p = static_cast<DecryptParam*>(param);
    std::string& block = *(p -> block);

    for (size_t i = 0; i < block.length(); i++) {
        block[i] ^= p -> key[i % p -> key.length()];
    }

    std::reverse(block.begin(), block.end());

    delete p;
    return 0;
}

void Task6_BlockDecryption::execute()
{
    displayHeader(getName());

    std::string original_text = "Novitskiy Dmitriy Vitalevich.";
    std::string key = "505";

    std::vector<std::string> blocks;
    for (size_t i = 0; i < original_text.length(); i += key.length()) {
        blocks.push_back(original_text.substr(i, key.length()));
    }

    for (auto& block : blocks) {
        std::reverse(block.begin(), block.end());
        for (size_t j = 0; j < block.length(); j++) {
            block[j] ^= key[j % key.length()];
        }
    }

    std::cout << "Получен зашифрованный текст\n";

    std::vector<HANDLE> hThread(blocks.size());
    std::vector<DWORD> dwThreadID(blocks.size());

    for (size_t i = 0; i < blocks.size(); i++) {
        DecryptParam* param = new DecryptParam;
        param -> block = &blocks[i];
        param -> key = key;
        param -> block_id = i;

        hThread[i] = CreateThread(NULL, 0, decrypt_block, param, 0, &dwThreadID[i]);
    }

    WaitForMultipleObjects(blocks.size(), hThread.data(), TRUE, INFINITE);

    std::string decrypted_text;
    for (const auto& block : blocks) {
        decrypted_text += block;
    }

    displayResult("Расшифрованный текст: " + decrypted_text);

    for (size_t i = 0; i < hThread.size(); i++) {
        CloseHandle(hThread[i]);
    }
}