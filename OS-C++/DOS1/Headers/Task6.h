#ifndef LW1_TASK6_H
#define LW1_TASK6_H

#pragma once
#include "BaseTask.h"
#include <vector>
#include <algorithm>

class Task6_BlockDecryption : public BaseTask {
    struct DecryptParam {
        std::string* block;
        std::string key;
        int block_id;
    };

    static DWORD WINAPI decrypt_block(LPVOID param);

public:
    void execute() override;
    std::string getName() const override { return "Блочное расшифрование"; }
};

#endif