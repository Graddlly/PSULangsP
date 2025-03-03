open System

// Функция для ввода одного символа
let rec getPrefix () =
    printf "Введите один символ, который нужно добавить в начало строк: "
    match Console.ReadLine() with
    | null | "" ->
        printfn "Ошибка: ввод не должен быть пустым. Попробуйте снова.\n"
        getPrefix ()
    | input when input.Length > 1 ->
        printfn "Ошибка: введите только один символ. Попробуйте снова.\n"
        getPrefix ()
    | input -> input.[0]

// Функция для ввода списка строк
let rec getStringList () =
    printf "Введите строки через запятую: "
    let inputStrings = Console.ReadLine()
    match inputStrings with
    | null | "" ->
        printfn "Ошибка: ввод не должен быть пустым. Попробуйте снова.\n"
        getStringList ()
    | _ ->
        let stringList = inputStrings.Split(',') |> Array.toList |> List.map (_.Trim()) // (fun s -> s.Trim())
        stringList

// Функция обработки списка строк
let addPrefixToList (prefix: char) (strings: string list) =
    strings |> List.map (sprintf "%c%s" prefix) // (fun str -> sprintf "%c%s" prefix str)

[<EntryPoint>]
let main _ =
    printfn "Эта программа добавляет указанный символ в начало каждой строки."

    let prefix = getPrefix()
    let stringList = getStringList()
    let modifiedList = addPrefixToList prefix stringList

    printfn "\nРезультат:"
    modifiedList |> List.iter (printfn "%s")

    0
