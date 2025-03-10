open System
open System.IO

// Функция для получения ввода пользователя с проверкой
let getInput prompt validator errorMessage =
    let rec loop () =
        printf prompt
        let input = Console.ReadLine()
        match validator input with
        | true -> input
        | false -> 
            printf errorMessage
            loop()
    loop()

// Функция для проверки, существует ли каталог
let directoryExists directory =
    try
        Directory.Exists(directory)
    with
    | _ -> false

// Функция для получения файлов из каталога, начинающихся с заданным символом
let getFilesStartingWith directory startChar =
    try
        let files = seq {
            for filePath in Directory.EnumerateFiles(directory) do
                let fileName = Path.GetFileName(filePath)
                if fileName.Length > 0 && fileName.[0] = startChar then
                    yield fileName
        }
        Ok files
    with
    | ex -> Error ex.Message

[<EntryPoint>]
let main _ =    
    printf "Программа для подсчета файлов, начинающихся с заданного символа\n"
    
    // Получаем путь к каталогу
    let directory = 
        getInput "Введите путь к каталогу: " 
                 directoryExists 
                 "Ошибка: Указанный каталог не существует. Пожалуйста, введите корректный путь."
    
    // Получаем символ для поиска
    let startChar = 
        getInput "Введите символ, с которого должны начинаться имена файлов: " 
                 (fun s -> s.Length = 1) 
                 "Ошибка: Введите ровно один символ."
        |> char
    
    printfn $"\nИдет поиск файлов в каталоге '%s{directory}', начинающихся с символа '%c{startChar}'..."
    
    // Используем отложенные вычисления для получения и обработки файлов
    match getFilesStartingWith directory startChar with
    | Ok files ->
        // Создаем отложенное вычисление для подсчета файлов
        let countComputation = lazy (
            printfn "Выполняется подсчет файлов..."
            Seq.length files
        )
        
        // Вызываем отложенное вычисление для подсчета файлов
        let count = countComputation.Force()
        
        printfn $"\nНайдено %d{count} файлов, начинающихся с символа '%c{startChar}'."
        
        // Спрашиваем, хочет ли пользователь увидеть список файлов
        let showFiles = getInput "Хотите увидеть список найденных файлов? (да/нет): "
                         (fun s -> s.ToLower() = "да" || s.ToLower() = "нет")
                         "Ошибка: Введите 'да' или 'нет'."
        
        if showFiles.ToLower() = "да" then
            printfn "\nСписок найденных файлов:"
            
            // Создаем отложенное вычисление для вывода файлов
            let printFilesComputation = lazy (
                files
                |> Seq.mapi (fun i file -> (i + 1, file))
                |> Seq.iter (fun (i, file) -> printfn $"%d{i}. %s{file}")
            )
            
            // Вызываем отложенное вычисление для вывода файлов
            printFilesComputation.Force()
        
    | Error message ->
        printfn $"Произошла ошибка при поиске файлов: %s{message}"
    
    0