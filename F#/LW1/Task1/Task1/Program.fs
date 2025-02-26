open System

let rec readInt() =
    printf "Введите целое число (или 'q' для выхода): "
    match Console.ReadLine() with
    | null | "" -> 
        printfn "Пустой ввод. Попробуйте снова."
        readInt()
    | "q" -> None
    | input -> 
        match Int32.TryParse(input) with
        | true, value -> Some value
        | false, _ ->
            printfn "Некорректный ввод. Введите целое число."
            readInt()

let rec getBoolList acc =
    match readInt() with
    | Some n -> getBoolList ((n % 2 <> 0) :: acc)
    | None -> List.rev acc

[<EntryPoint>]
let main _ =
    printfn "Эта программа формирует список значений true/false в зависимости от четности чисел."
    let boolList = getBoolList []
    printfn "Результат: %A" boolList
    0