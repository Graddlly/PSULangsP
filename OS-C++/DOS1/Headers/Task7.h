#ifndef LW1_TASK7_H
#define LW1_TASK7_H

#pragma once
#include "BaseTask.h"
#include <vector>

class Task7_BinaryTreeSum : public BaseTask {
    struct TreeNode {
        int value;
        TreeNode* left;
        TreeNode* right;

        TreeNode(int val) : value(val), left(nullptr), right(nullptr) {}
        ~TreeNode() {
            delete left;
            delete right;
        }
    };

    struct TreeSumParam {
        TreeNode* node;
        int* result;
    };

    TreeNode* root;

    static DWORD WINAPI calculate_tree_sum(LPVOID param);
    TreeNode* generateRandomTree(int depth, int maxValue);
    TreeNode* createTreeFromInput();
    void printTree(TreeNode* node, int level = 0);

public:
    Task7_BinaryTreeSum() : root(nullptr) {}
    ~Task7_BinaryTreeSum() { delete root; }

    void execute() override;
    std::string getName() const override { return "Сумма элементов бинарного дерева"; }
};

#endif