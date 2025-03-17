open System

type Tree =
    | Leaf
    | Node of float * Tree * Tree

// Создание дарева
let rec createTree depth =
    printf "Ввести значение узла? (д/н): "
    let response = Console.ReadLine().ToLower()
    
    if response = "д" || response = "да" then
        printf "Введите вещественное число: "
        let valueStr = Console.ReadLine()
        let success, value = Double.TryParse(valueStr)
        
        let rec finalValue =
            if success then value
            else
                printfn "Ошибка ввода! Используется значение 0.0"
                0.0
            
        printfn $"Заполнение левого поддерева (глубина %d{depth + 1})"
        let left = createTree (depth + 1)
        
        printfn $"Заполнение правого поддерева (глубина %d{depth + 1})"
        let right = createTree (depth + 1)
        
        Node(finalValue, left, right)
    elif response = "н" || response = "нет" then Leaf
    else
        printf "Введено неправильное значение. Введите правильно и повторите попытку!"
        createTree depth

// Функция преобразования дерева (округление значений)
let rec mapTree mapper tree =
    match tree with
    | Leaf -> Leaf
    | Node(value, left, right) ->
        Node(mapper value, mapTree mapper left, mapTree mapper right)

// Получение высоты дерева
let rec getHeight tree =
    match tree with
    | Leaf -> 0
    | Node(_, left, right) ->
        1 + max (getHeight left) (getHeight right)

// Создание строкового представления значения
let formatValue value isOriginal =
    if isOriginal then $"%.6g{value}"
    else $"%.0f{value}"

// Функция вывода дерева
let printTreeVisual tree isOriginal =
    let rec buildTreeDisplay tree =
        match tree with
        | Leaf -> ([], 0, 0)  // (строки, высота, средняя позиция корня)
        | Node(value, Leaf, Leaf) ->
            let valueStr = "  " + formatValue value isOriginal
            ([valueStr], 1, valueStr.Length / 2)  // Листовой узел
        | Node(value, left, right) ->
            // Рекурсивно строим левое и правое поддеревья
            let (lStrings, lHeight, lPos) = buildTreeDisplay left
            let (rStrings, rHeight, rPos) = buildTreeDisplay right
            
            let valueStr = "  " + formatValue value isOriginal
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
                    (fun l r -> 
                        l + String.replicate (max 0 (rShift - l.Length)) " " + r) 
                    leftPadded 
                    rightPadded
            
            let allLines = [firstLine] @ secondLine @ combinedLines
            (allLines, 1 + max lHeight rHeight + (if secondLine.IsEmpty then 0 else 1), combinedPos)
    
    match tree with
    | Leaf -> printfn "Пустое дерево"
    | _ ->
        let (lines, _, _) = buildTreeDisplay tree
        List.iter (printfn "%s") lines

[<EntryPoint>]
let runMain _ =
    printfn "=== Программа для работы с бинарным деревом ==="
    printfn "Сейчас вы будете вводить дерево вещественных чисел."
    printfn "Для каждого узла вам нужно указать, хотите ли вы добавить значение,"
    printfn "и если да - ввести вещественное число."
    
    printfn "\nНачало создания дерева:"
    let originalTree = createTree 0
    
    printfn "\nИсходное дерево:"
    printTreeVisual originalTree true
    
    // Создание нового дерева с округленными значениями
    let roundedTree = mapTree Math.Round originalTree
    printfn "\nДерево с округленными значениями:"
    printTreeVisual roundedTree false
    0