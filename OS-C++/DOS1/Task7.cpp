#include "Headers/Task7.h"

DWORD WINAPI Task7_BinaryTreeSum::calculate_tree_sum(LPVOID param)
{
    TreeSumParam* p = static_cast<TreeSumParam*>(param);

    if (p -> node == nullptr) {
        *(p -> result) = 0;
        return 0;
    }

    int left_sum = 0, right_sum = 0;

    HANDLE left_thread = NULL;
    HANDLE right_thread = NULL;
    DWORD thread_id;

    if (p -> node -> left) {
        TreeSumParam* left_param = new TreeSumParam;
        left_param -> node = p -> node -> left;
        left_param -> result = &left_sum;
        left_thread = CreateThread(NULL, 0, calculate_tree_sum, left_param, 0, &thread_id);
    }

    if (p -> node -> right) {
        TreeSumParam* right_param = new TreeSumParam;
        right_param -> node = p -> node -> right;
        right_param -> result = &right_sum;
        right_thread = CreateThread(NULL, 0, calculate_tree_sum, right_param, 0, &thread_id);
    }

    if (left_thread) {
        WaitForSingleObject(left_thread, INFINITE);
        CloseHandle(left_thread);
    }

    if (right_thread) {
        WaitForSingleObject(right_thread, INFINITE);
        CloseHandle(right_thread);
    }

    *(p -> result) = p -> node -> value + left_sum + right_sum;
    return 0;
}

Task7_BinaryTreeSum::TreeNode* Task7_BinaryTreeSum::generateRandomTree(int depth, int maxValue)
{
    if (depth <= 0 || rand() % 3 == 0) return nullptr;

    TreeNode* node = new TreeNode(rand() % maxValue + 1);
    node -> left = generateRandomTree(depth - 1, maxValue);
    node -> right = generateRandomTree(depth - 1, maxValue);

    return node;
}

Task7_BinaryTreeSum::TreeNode* Task7_BinaryTreeSum::createTreeFromInput()
{
    std::cout << "Введите корневое значение (или -1 для пустого дерева): ";
    int value;
    std::cin >> value;

    if (value == -1) return nullptr;

    TreeNode* root = new TreeNode(value);
    std::vector<TreeNode*> queue;
    queue.push_back(root);

    while (!queue.empty()) {
        TreeNode* current = queue.front();
        queue.erase(queue.begin());

        std::cout << "Введите левый потомок для " << current -> value << " (-1 для пустого): ";
        std::cin >> value;
        if (value != -1) {
            current -> left = new TreeNode(value);
            queue.push_back(current -> left);
        }

        std::cout << "Введите правый потомок для " << current -> value << " (-1 для пустого): ";
        std::cin >> value;
        if (value != -1) {
            current -> right = new TreeNode(value);
            queue.push_back(current -> right);
        }
    }

    return root;
}

void Task7_BinaryTreeSum::printTree(TreeNode* node, int level)
{
    if (node == nullptr) return;

    printTree(node -> right, level++);

    for (int i = 0; i < level; i++) {
        std::cout << "    ";
    }
    std::cout << node -> value << "\n";

    printTree(node -> left, level++);
}

void Task7_BinaryTreeSum::execute()
{
    displayHeader(getName());

    std::cout << "Выберите способ создания дерева:\n";
    std::cout << "1. Автоматическая генерация\n";
    std::cout << "2. Ввод с клавиатуры\n";
    std::cout << "Ваш выбор: ";

    int choice;
    std::cin >> choice;

    delete root;

    if (choice == 1) {
        std::cout << "Введите максимальную глубину дерева (рекомендуется 3-5): ";
        int depth;
        std::cin >> depth;

        std::cout << "Введите максимальное значение узла: ";
        int maxValue;
        std::cin >> maxValue;

        root = generateRandomTree(depth, maxValue);
    }
    else if (choice == 2) {
        root = createTreeFromInput();
    }
    else {
        std::cout << "Неверный выбор. Создается дерево по умолчанию.\n";
        root = new TreeNode(10);
        root -> left = new TreeNode(5);
        root -> right = new TreeNode(15);
        root -> left -> left = new TreeNode(3);
        root -> left -> right = new TreeNode(7);
    }

    if (root == nullptr) {
        displayResult("Дерево пустое, сумма = 0");
        return;
    }

    std::cout << "\nСозданное дерево:\n";
    printTree(root);

    int total_sum = 0;
    TreeSumParam* param = new TreeSumParam;
    param -> node = root;
    param -> result = &total_sum;

    HANDLE main_thread = CreateThread(NULL, 0, calculate_tree_sum, param, 0, nullptr);
    WaitForSingleObject(main_thread, INFINITE);
    CloseHandle(main_thread);

    displayResult("Сумма всех элементов дерева: " + std::to_string(total_sum));
}