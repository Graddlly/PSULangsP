open System

type Tree =
    | Leaf
    | Node of int * Tree * Tree

// Создание дерева
let rec createTree depth =
    printf "Ввести значение узла? (д/н): "
    match Console.ReadLine().ToLower() with
    | "д" | "да" ->
        let rec getValidIntInput () =
            printf "Введите целое число: "
            let valueStr = Console.ReadLine()
            match Int32.TryParse(valueStr) with
            | (true, value) -> value
            | (false, _) ->
                printfn "Ошибка ввода! Пожалуйста, введите целое число."
                getValidIntInput ()

        let value = getValidIntInput ()
        
        printfn $"Заполнение левого поддерева (глубина {depth + 1})"
        let left = createTree (depth + 1)

        printfn $"Заполнение правого поддерева (глубина {depth + 1})"
        let right = createTree (depth + 1)

        Node(value, left, right)
    | "н" | "нет" -> Leaf
    | _ ->
        printfn "Введено неправильное значение. Введите правильно и повторите попытку!"
        createTree depth

// Вывод дерева
let printTreeVisual tree =
    let rec buildTreeDisplay tree =
        match tree with
        | Leaf -> ([], 0, 0)  // (строки, высота, средняя позиция корня)
        | Node(value, Leaf, Leaf) ->
            let valueStr = "  " + string value
            ([valueStr], 1, valueStr.Length / 2)  // Листовой узел
        | Node(value, left, right) ->
            // Рекурсивно строим левое и правое поддеревья
            let (lStrings, lHeight, lPos) = buildTreeDisplay left
            let (rStrings, rHeight, rPos) = buildTreeDisplay right

            let valueStr = "  " +  string value
            let rootPos = valueStr.Length / 2

            // Минимальное расстояние между поддеревьями
            let minSeparation = 3

            // Вычисляем позиции левого и правого поддеревьев
            let lShift = 0
            let combinedPos = max (lPos + lShift + minSeparation) (rootPos)
            let rShift = combinedPos - rPos

            // Первая строка - значение корня
            let firstLine = String.replicate (combinedPos - rootPos) " " + valueStr

            // Вторая строка - соединительные линии
            let secondLine =
                if left = Leaf && right = Leaf then []
                else
                    let leftConnector =
                        if left = Leaf then ""
                        else String.replicate (combinedPos - 1) " " + "/"
                    let rightConnector =
                        if right = Leaf then ""
                        else String.replicate (rShift - 1) " " + "\\"
                    [leftConnector + rightConnector]

            // Смещаем строки левого и правого поддеревьев
            let leftLines =
                if left = Leaf then []
                else List.map (fun s -> String.replicate lShift " " + s) lStrings
            let rightLines =
                if right = Leaf then []
                else List.map (fun s -> String.replicate rShift " " + s) rStrings

            // Соединяем все строки
            let leftPadded = leftLines @ List.replicate (max 0 (rHeight - lHeight)) ""
            let rightPadded = rightLines @ List.replicate (max 0 (lHeight - rHeight)) ""

            let combinedLines =
                List.map2
                    (fun l r -> l + String.replicate (max 0 (rShift - l.Length)) " " + r)
                    leftPadded
                    rightPadded

            let allLines = [firstLine] @ secondLine @ combinedLines
            (allLines, 1 + max lHeight rHeight + (if secondLine.IsEmpty then 0 else 1), combinedPos)
    match tree with
    | Leaf -> printfn "Пустое дерево"
    | _ ->
        let (lines, _, _) = buildTreeDisplay tree
        List.iter (printfn "%s") lines

// Fold для дерева
let rec foldTree (folder: int -> int -> int -> int) (initial: int) (tree: Tree) =
    match tree with
    | Leaf -> initial
    | Node (value, left, right) ->
        let leftResult = foldTree folder initial left
        let rightResult = foldTree folder initial right
        folder value leftResult rightResult

// Подсчет количества вхождений элемента
let countOccurrences (target: int) tree =
    let folder value leftCount rightCount =
        let currentCount = if value = target then 1 else 0
        currentCount + leftCount + rightCount
    foldTree folder 0 tree

// Безопасный ввод целого числа
let rec getIntInput prompt =
    printf $"%s{prompt}"
    let input = Console.ReadLine()
    match Int32.TryParse(input) with
    | (true, value) -> value
    | (false, _) ->
        printfn "Ошибка: Введено не целое число. Попробуйте снова."
        getIntInput prompt

[<EntryPoint>]
let main _ =
    printfn "=== Программа для работы с бинарным деревом ==="
    printfn "Сейчас вы будете вводить дерево целых чисел."
    printfn "Для каждого узла вам нужно указать, хотите ли вы добавить значение,"
    printfn "и если да - ввести целое число."

    printfn "\nНачало создания дерева:"
    let tree = createTree 0

    printfn "\nДерево построено. Вот как оно выглядит:"
    printTreeVisual tree
    printfn ""

    let targetElement = getIntInput "Введите целое число, количество вхождений которого нужно посчитать:"
    let occurrences = countOccurrences targetElement tree
    printfn $"Элемент %d{targetElement} встречается в дереве %d{occurrences} раз(а)."
    0