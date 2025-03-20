open System

type Tree =
    | Leaf
    | Node of float * Tree * Tree

// Вставка значения в дерево
let rec insertValue value tree =
    match tree with
    | Leaf -> Node(value, Leaf, Leaf)
    | Node(nodeValue, left, right) ->
        if value <= nodeValue then Node(nodeValue, insertValue value left, right)
        else Node(nodeValue, left, insertValue value right)

// Создание дерева из списка значений
let createTreeFromList values = List.fold (fun tree value -> insertValue value tree) Leaf values

// Ввод корректного вещественного числа
let rec readValidFloat prompt =
    printf $"%s{prompt}"
    let success, value = Double.TryParse(Console.ReadLine())
    if success then value
    else
        printfn "Ошибка ввода! Пожалуйста, введите корректное вещественное число."
        readValidFloat prompt

// Ввод списка вещественных чисел с клавиатуры
let inputValuesList count =
    printfn $"Введите %d{count} вещественных чисел:"
    [ for i in 1..count -> readValidFloat $"Число %d{i}: " ]

// Создание дерева
let rec createTree depth =
    printf "Ввести значение узла? (д/н): "
    let response = Console.ReadLine().ToLower()

    match response with
    | "д" | "да" ->
        let value = readValidFloat "Введите вещественное число: "

        printfn $"Заполнение левого поддерева (глубина {depth + 1})"
        let left = createTree (depth + 1)

        printfn $"Заполнение правого поддерева (глубина {depth + 1})"
        let right = createTree (depth + 1)

        Node(value, left, right)
    | "н" | "нет" -> Leaf
    | _ ->
        printfn "Введено неправильное значение. Введите правильно и повторите попытку!"
        createTree depth

// Определение высоты дерева
let rec getTreeHeight tree =
    match tree with
    | Leaf -> 0
    | Node(_, left, right) -> 1 + max (getTreeHeight left) (getTreeHeight right)

// Вывод дерева
let printTreeVisual tree =
    // Ширина значения узла
    let getValueWidth value = (string value).Length

    // Максимальная ширина узла в дереве
    let rec getMaxNodeWidth tree =
        match tree with
        | Leaf -> 0
        | Node(value, left, right) ->
            max (getValueWidth value) (max (getMaxNodeWidth left) (getMaxNodeWidth right))

    // Высота и максимальная ширина узла
    let height = getTreeHeight tree
    let maxNodeWidth = max 4 (getMaxNodeWidth tree)

    // Двумерный массив для представления дерева
    let width = int (Math.Pow(2.0, float height)) * maxNodeWidth
    let lines = Array.init (height * 2) (fun _ -> Array.create width ' ')

    // Заполнение массива значениями и соединительными линиями
    let rec fillArray tree level leftBound rightBound =
        match tree with
        | Leaf -> ()
        | Node(value, left, right) ->
            let mid = (leftBound + rightBound) / 2
            let valueStr = string value
            let strLen = valueStr.Length
            let startPos = mid - strLen / 2

            // Значение узла
            for i = 0 to strLen - 1 do
                if startPos + i < width then
                    lines.[level * 2].[startPos + i] <- valueStr.[i]

            // Если есть потомки, добавление соединительных линий
            if left <> Leaf || right <> Leaf then
                lines.[level * 2 + 1].[mid] <- '|'

                let leftMid = (leftBound + mid) / 2
                let rightMid = (mid + rightBound) / 2

                if left <> Leaf then
                    // Горизонтальная линия влево
                    for i = leftMid + 1 to mid - 1 do
                        lines.[level * 2 + 2].[i] <- '_'
                    lines.[level * 2 + 2].[leftMid] <- '/'

                if right <> Leaf then
                    // Горизонтальная линия вправо
                    for i = mid + 1 to rightMid - 1 do
                        lines.[level * 2 + 2].[i] <- '_'
                    lines.[level * 2 + 2].[rightMid] <- '\\'

                // Рекурсивное заполнение для потомков
                fillArray left (level + 1) leftBound mid
                fillArray right (level + 1) mid rightBound

    // Заполнение массива
    if tree <> Leaf then
        fillArray tree 0 0 width

        // Вывод результата
        for line in lines do
            let trimmedLine = String(line).TrimEnd()
            if trimmedLine.Length > 0 then printfn $"%s{trimmedLine}"
    else printfn "Пустое дерево"

// Функция для выбора способа создания дерева
let rec chooseTreeCreationMethod() =
    printfn "Выберите способ создания дерева:"
    printfn "1 - Ручной ввод элементов"
    printfn "2 - Автоматическое создание и распределение элементов"
    printf "Ваш выбор (1/2): "

    let choice = Console.ReadLine()
    match choice with
    | "1" | "2" -> choice
    | _ ->
        printfn "Неверный выбор. Пожалуйста, выберите 1 или 2."
        chooseTreeCreationMethod()

// Fold - acc (value -> leftRes -> rightRes -> newAcc)
let rec foldTree (folder: float -> float -> float -> float) (initial: float) (tree: Tree) =
    match tree with
    | Leaf -> initial
    | Node (value, left, right) ->
        let leftResult = foldTree folder initial left
        let rightResult = foldTree folder initial right
        folder value leftResult rightResult

// Подсчет количества вхождений элемента
let countOccurrences (target: float) tree =
    let folder value leftCount rightCount =
        let currentCount = if value = target then 1.0 else 0.0
        currentCount + leftCount + rightCount
    foldTree folder 0.0 tree

// Безопасный ввод вещественного числа
let rec getFloatInput prompt =
    printfn $"%s{prompt}"
    let input = Console.ReadLine()
    match Double.TryParse(input) with
    | (true, value) -> value
    | (false, _) ->
        printfn "Ошибка: Введено не вещественное число. Попробуйте снова."
        getFloatInput prompt

[<EntryPoint>]
let runMain _ =
    printfn "=== Программа для работы с деревом вещественных чисел ==="

    let choice = chooseTreeCreationMethod()
    let originalTree =
        match choice with
        | "1" ->
            printfn "\nСейчас вы будете вводить дерево вещественных чисел."
            printfn "Для каждого узла вам нужно указать, хотите ли вы добавить значение,"
            printfn "и если да - ввести вещественное число."
            printfn "\nНачало создания дерева:"
            createTree 0
        | "2" ->
            printfn "\nАвтоматическое создание бинарного дерева поиска."
            let elementCount = int (readValidFloat "Введите количество элементов: ")

            let values = inputValuesList elementCount
            printfn "\nВведенные значения:"
            values |> List.map string |> String.concat ", " |> printfn "%s"

            createTreeFromList values
        | _ ->
            printfn "Ошибка выбора. Завершение программы."
            Leaf

    printfn "\nИсходное дерево:"
    printTreeVisual originalTree

    let targetElement = readValidFloat "\nВведите вещественное число, количество вхождений которого нужно посчитать: "
    let occurrences = countOccurrences targetElement originalTree

    printfn $"Элемент %f{targetElement} встречается в дереве %d{int occurrences} раз(а)."
    0