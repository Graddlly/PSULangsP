open System
open System.IO

// Функция для получения ввода с проверкой
let getInput prompt validator errorMessage =
    let rec loop () =
        printf prompt
        let input = Console.ReadLine().Trim()
        if validator input then input
        else 
            printfn errorMessage
            loop()
    loop()

// Функция для проверки существования каталога
let directoryExists dir = Directory.Exists dir

// Функция для получения файлов, начинающихся с заданного символа
let getFilesStartingWith dir prefix =
    lazy (
        seq {
            for filePath in Directory.EnumerateFiles(dir) do
                let fileName = Path.GetFileName(filePath)
                if fileName.StartsWith(prefix.ToString(), StringComparison.OrdinalIgnoreCase) then
                    yield fileName
        }
    )

[<EntryPoint>]
let main _ =
    printfn "Программа для подсчета файлов, начинающихся с заданного символа\n"

    // Запрос пути к каталогу с проверкой существования
    let directory =
        getInput "Введите путь к каталогу: " 
                 directoryExists
                 "Ошибка: Указанный каталог не существует. Попробуйте снова."

    // Запрос символа для поиска с проверкой
    let prefix =
        getInput "Введите начальный символ имени файла: " 
                 (fun s -> s.Length = 1) 
                 "Ошибка: Введите ровно один символ."
        |> char

    printfn $"\nИдет поиск файлов в каталоге '%s{directory}', начинающихся с символа '%c{prefix}'..."

    // Ленивый поиск файлов
    let files = getFilesStartingWith directory prefix

    // Подсчет количества файлов
    let totalLazy = lazy (Seq.length files.Value)
    let total = totalLazy.Force()
    printfn $"Найдено %d{total} файлов, начинающихся с '%c{prefix}'."

    // Если файлов нет, программа завершает работу
    if total = 0 then
        printfn "Файлы не найдены. Завершение программы."
    else
        // Запрос у пользователя, хочет ли он увидеть список файлов
        let showFiles =
            getInput "Хотите увидеть список найденных файлов? (да/нет): "
                     (fun s -> s.ToLower() = "да" || s.ToLower() = "нет")
                     "Ошибка: Введите 'да' или 'нет'."

        // Ленивый вывод списка файлов
        let printFilesLazy = lazy (
            printfn "\nСписок найденных файлов:"
            files.Value
            |> Seq.mapi (fun i -> sprintf "%d. %s" (i + 1))
            |> Seq.iter (printfn "%s")
        )

        // Если пользователь хочет увидеть файлы, вызываем `printFilesLazy`
        if showFiles.ToLower() = "да" then printFilesLazy.Force()
        else printfn "Завершение программы."

    0
