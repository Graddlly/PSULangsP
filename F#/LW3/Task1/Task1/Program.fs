open System

// Функция для запроса ввода строки с проверкой на пустоту
let rec readNonEmptyString prompt =
    printf "%s" prompt
    match Console.ReadLine() with
    | null | "" ->
        printfn "Ошибка: ввод не должен быть пустым. Попробуйте снова."
        readNonEmptyString prompt
    | input -> input

// Функция для запроса ввода одиночного символа
let rec readChar prompt =
    match readNonEmptyString prompt with
    | input when input.Length = 1 -> input.[0]
    | _ ->
        printfn "Ошибка: нужно ввести ровно один символ. Попробуйте снова."
        readChar prompt

// Функция для запроса количества строк
let rec readInt prompt =
    printf "%s" prompt
    match Int32.TryParse(Console.ReadLine()) with
    | (true, value) when value > 0 -> value
    | _ ->
        printfn "Ошибка: введите положительное число. Попробуйте снова."
        readInt prompt

// Функция для получения строк с использованием отложенных вычислений
let readAllLines count =
    seq {
        for i in 1 .. count do
            yield readNonEmptyString (sprintf "Введите строку #%d: " i)
    }

[<EntryPoint>]
let main _ =
    printfn "Программа для добавления префиксов ко всем данным строкам.\n"
    let prefixChar = readChar "Введите символ, который нужно добавить к строкам: "
    let count = readInt "Введите количество строк, которые вы хотите ввести: "
    
    let lines = readAllLines count |> Seq.cache
    let result = lines |> Seq.map (sprintf "%c%s" prefixChar)
    
    printfn "\nРезультат:"
    result |> Seq.iter (printfn "Обработанная строка: %s\n")
    0
