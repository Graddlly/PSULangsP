open System

// Функция для запроса у пользователя символа
let rec getCharInput () =
    printf "Введите один символ: "
    match Console.ReadLine() with
    | null | "" ->
        printfn "Ошибка: нужно ввести хотя-бы один символ. Попробуйте снова."
        getCharInput()
    | input when input.Length = 1 -> input.[0]
    | _ ->
        printfn "Ошибка: нужно ввести ровно один символ. Попробуйте снова."
        getCharInput()

// Функция для ввода списка строк
let rec getStringList () =
    printfn "\nВведите строки (оставьте пустую строку для завершения ввода):"
    let rec readLines acc =
        let input = Console.ReadLine()
        match input with
        | null | "" -> List.rev acc
        | _ -> readLines (input :: acc)
    readLines []

// Функция для добавления символа к каждой строке
let prependCharToStrings (c: char) (strings: string list) : string list =
    strings |> List.map (sprintf "%c%s" c) // (fun s -> sprintf "%c%s" c s)

[<EntryPoint>]
let main _ =
    printfn "Программа для добавления префиксов ко всем данным строкам.\n"
    
    let charInput = getCharInput()
    let stringList = getStringList()
    let modifiedList = prependCharToStrings charInput stringList
    
    printfn "\nРезультат:" 
    modifiedList |> List.iter (printfn "%s")
    0
