open System
        
let rec readInt() =
    printf "Введите целое число: "
    let input = Console.ReadLine()
    match Int32.TryParse(input) with
    | true, number -> number
    | _ -> 
        printfn "Ошибка! Введите корректное целое число."
        readInt()

let numberToDigitList (num: int) =
    num
    |> abs
    |> string
    |> Seq.map (fun c -> int c - int '0') // ASCII '1' - '0' = 1 (код 49 - 48 = 1)
    |> Seq.toList

[<EntryPoint>]
let main _ =
    let number = readInt()
    let digits = numberToDigitList number
    printfn "Список цифр числа: %A" digits
    0
