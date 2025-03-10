open System

// Функция для запроса списка строк у пользователя
let readLines count =
    seq {
        for i in 1 .. count do
            printf $"Введите строку %d{i}: "
            yield Console.ReadLine()
    }

// Функция для запроса целого числа у пользователя
let rec readInt prompt =
    printf $"%s{prompt}"
    match Int32.TryParse(Console.ReadLine()) with
    | (true, value) when value > 0 -> value
    | _ ->
        printfn "Ошибка ввода! Введите положительное целое число."
        readInt prompt

[<EntryPoint>]
let main _ =
    printfn "Программа для подсчета строк заданной длины.\n"
    let countLines = readInt "Введите количество строк: "
    let lines = readLines countLines
    let targetLength = readInt "Введите желаемую длину строки: "
    printf "\n"
    
    let count =
        lines
        |> Seq.map (fun s -> if String.length s = targetLength then 1 else 0)
        |> Seq.sum
    
    printfn $"\nКоличество строк длиной %d{targetLength}: %d{count}"
    0
