open System

// Функция для добавления элемента в список
let addElement lst elem = elem :: lst

// Функция для удаления элемента из списка
let removeElement lst elem = List.filter (fun x -> x <> elem) lst

// Функция для поиска элемента в списке
let findElement lst elem = List.tryFind (fun x -> x = elem) lst

// Функция для сцепки двух списков
let concatenateLists lst1 lst2 = lst1 @ lst2

// Функция для получения элемента по номеру
let getElementByIndex lst index =
    if index < 0 || index >= List.length lst then None
    else Some (List.item index lst)

// Функция для запроса числа у пользователя
let rec getIntInput prompt =
    printf "%s" prompt
    match Int32.TryParse(Console.ReadLine()) with
    | (true, value) -> value
    | _ ->
        printfn "Ошибка ввода! Введите целое число."
        getIntInput prompt
        
// Функция для запроса списка целых чисел у пользователя
let rec getIntListInput prompt =
    printf "%s" prompt
    let input = Console.ReadLine()
    let elements = input.Split(' ')
    let parsedElements = elements |> Array.choose (fun s ->
        match Int32.TryParse(s) with | (true, v) -> Some v | _ -> None)
    if parsedElements.Length = elements.Length then List.ofArray parsedElements
    else
        printfn "Ошибка ввода! Введите только целые числа через пробел."
        getIntListInput prompt

// Функция для запроса строки у пользователя
let getStringInput prompt =
    printf "%s" prompt
    Console.ReadLine()

// Основное меню
let rec menu lst =
    printfn "\nТекущий список: %A" lst
    printfn "Выберите действие:"
    printfn "1. Добавить элемент"
    printfn "2. Удалить элемент"
    printfn "3. Найти элемент"
    printfn "4. Сцепить с другим списком"
    printfn "5. Получить элемент по индексу"
    printfn "6. Выйти"

    match getIntInput "Введите номер действия: " with
    | 1 ->
        let elem = getStringInput "Введите элемент: "
        menu (addElement lst elem)
    | 2 ->
        let elem = getStringInput "Введите элемент для удаления: "
        menu (removeElement lst elem)
    | 3 ->
        let elem = getStringInput "Введите элемент для поиска: "
        match findElement lst elem with
        | Some _ -> printfn "Элемент найден."
        | None -> printfn "Элемент не найден."
        menu lst
    | 4 ->
        let newLst = getIntListInput "Введите элементы второго списка через пробел: " |> List.map string
        menu (concatenateLists lst newLst)
    | 5 ->
        let index = getIntInput "Введите индекс элемента: "
        match getElementByIndex lst index with
        | Some value -> printfn "Элемент на позиции %d: %s" index value
        | None -> printfn "Некорректный индекс."
        menu lst
    | 6 -> printfn "Выход из программы."
    | _ ->
        printfn "Некорректный выбор, попробуйте снова."
        menu lst

// Запуск программы с пустым списком
[<EntryPoint>]
let main _ =
    menu []
    0
