open System

// Функция для подсчёта количества строк заданной длины
let rec countStringsOfLength length strings acc =
    match strings with
    | [] -> acc
    | head :: tail -> countStringsOfLength length tail (if String.length head = length then acc + 1 else acc)

// Функция для запроса целочисленного ввода у пользователя
let rec getIntInput prompt =
    printf "%s" prompt
    match Int32.TryParse(Console.ReadLine()) with
    | (true, value) -> value
    | _ ->
        printfn "Ошибка ввода! Введите целое число."
        getIntInput prompt

// Функция для ввода списка строк
let rec getStringList acc =
    printf "Введите строку (или пустую строку для завершения): "
    let input = Console.ReadLine()
    match input with
    | "" -> List.rev acc
    | _ -> getStringList (input :: acc)

[<EntryPoint>]
let main _ =
    printfn "Программа для подсчёта строк заданной длины.\n"
    
    let strings = getStringList []
    let length = getIntInput "\nВведите длину строк для подсчёта: "
    let count = countStringsOfLength length strings 0
    
    printfn "Количество строк длины %d: %d" length count
    0
