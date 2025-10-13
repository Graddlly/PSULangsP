#ifndef LW1_TASK5_H
#define LW1_TASK5_H

#pragma once
#include "BaseTask.h"
#include <vector>
#include <algorithm>

class Task5_BlockEncryption : public BaseTask {
    struct EncryptParam {
        std::string* block;
        std::string key;
        int block_id;
    };

    static DWORD WINAPI encrypt_block(LPVOID param);

public:
    void execute() override;
    std::string getName() const override { return "Блочное шифрование"; }
};

#endif