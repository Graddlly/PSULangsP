.model small
.stack 100h
.data
    array db 100 dup (?)         ; Массив на 100 байт (максимальная длина)
    len db ?                     ; Длина массива (вводится пользователем)
    seed dw ?                    ; Начальное значение для генерации случайных чисел (16 бит)
    min db ?                     ; Минимальное значение
    max db ?                     ; Максимальное значение
    sum dw ?                     ; Сумма элементов (word, так как сумма может превысить 255)
    average db ?                 ; Среднее значение
    printed db ?                 ; Флаг для процедуры печати числа
    input_buf db 4, ?, 4 dup (?) ; Буфер для ввода строки (макс. 4 символа)
    prompt_len db 'Enter array length (1-100): $' ; Приглашение для ввода длины
    prompt_seed db 'Press any key 5 times to generate seed: $' ; Приглашение для генерации seed
    msg_min db 'Min: $'          ; Сообщение для минимального значения
    msg_max db 'Max: $'          ; Сообщение для максимального значения
    msg_avg db 'Average: $'      ; Сообщение для среднего значения
    msg_array db 'Generated array: $' ; Сообщение для вывода массива
    newline db 13, 10, '$'       ; Символы перевода строки (CRLF)

.code
main proc
    mov ax, @data                ; Инициализация сегмента данных
    mov ds, ax

    ; Получение seed на основе нажатий клавиш
    call get_seed

    ; Вывод приглашения для ввода длины массива
    mov dx, offset prompt_len
    mov ah, 09h
    int 21h

    ; Чтение ввода пользователя
    mov dx, offset input_buf
    mov ah, 0Ah
    int 21h

    ; Преобразование введенной строки в число
    mov si, offset input_buf + 2 ; Начало символов ввода
    mov bx, 0                    ; Инициализация результата
convert_loop:
    mov al, [si]                 ; Чтение символа
    cmp al, '0'                  ; Проверка, является ли символ цифрой
    jb convert_done
    cmp al, '9'
    ja convert_done
    sub al, '0'                  ; Преобразование символа в число
    mov ah, 0                    ; Очистка старшего байта
    push ax                      ; Сохранение текущей цифры
    mov ax, bx                   ; Текущее значение bx
    mov cx, 10
    mul cx                       ; ax = bx * 10
    pop dx                       ; Восстановление цифры
    add ax, dx                   ; Добавление новой цифры
    mov bx, ax                   ; Сохранение результата
    inc si                       ; Переход к следующему символу
    jmp convert_loop
convert_done:
    mov [len], bl                ; Сохранение длины (предполагаем, что <= 100)

    ; Вывод новой строки после ввода
    mov dx, offset newline
    mov ah, 09h
    int 21h

    ; Генерация массива случайных чисел с использованием двух LCG
    mov cl, [len]                ; Счетчик = длина массива
    mov ch, 0
    mov si, offset array         ; Указатель на начало массива
    mov ax, [seed]               ; Начальное значение первого генератора
    mov bx, 12345                ; Начальное значение второго генератора

generate_loop:
    ; Первый генератор (LCG)
    mov dx, 31
    mul dx                       ; ax = ax * 31
    add ax, 17
    xor ax, 0A5h
    add ax, 43
    mov [seed], ax               ; Обновляем seed

    ; Второй генератор (LCG)
    mov dx, 23
    xchg ax, bx                  ; Меняем ax и bx
    mul dx                       ; ax = ax * 23
    add ax, 37
    xor ax, 0B4h
    add ax, 59
    xchg ax, bx                  ; Возвращаем ax и bx

    ; Комбинируем результаты (сложение младших байтов)
    add al, bl                   ; Складываем младшие байты
    mov [si], al                 ; Записываем в массив

    inc si                       ; Переходим к следующему элементу
    loop generate_loop           ; Повторяем для всех элементов

    ; Вывод сгенерированного массива
    mov dx, offset msg_array     ; "Generated array: "
    mov ah, 09h
    int 21h
    mov cl, [len]                ; Счетчик = длина массива
    mov ch, 0
    mov si, offset array         ; Указатель на начало массива
print_array_loop:
    mov al, [si]                 ; Получение текущего элемента
    call print_byte              ; Печать числа
    mov dl, ' '                  ; Вывод пробела после числа
    mov ah, 02h
    int 21h
    inc si                       ; Переход к следующему элементу
    loop print_array_loop
    mov dx, offset newline       ; Перевод строки после вывода массива
    mov ah, 09h
    int 21h

    ; Нахождение минимума, максимума и суммы
    mov cl, [len]                ; Счетчик = длина массива
    mov ch, 0
    mov si, offset array         ; Указатель на начало массива
    mov byte ptr [min], 255      ; Начальное значение минимума
    mov byte ptr [max], 0        ; Начальное значение максимума
    mov word ptr [sum], 0        ; Начальная сумма
find_loop:
    mov al, [si]                 ; Чтение текущего элемента
    cmp al, [min]                ; Сравнение с минимумом
    jae not_less
    mov [min], al                ; Обновление минимума
not_less:
    cmp al, [max]                ; Сравнение с максимумом
    jbe not_greater
    mov [max], al                ; Обновление максимума
not_greater:
    mov ah, 0                    ; Расширение байта до слова
    add [sum], ax                ; Добавление к сумме
    inc si                       ; Переход к следующему элементу
    loop find_loop

    ; Вычисление среднего значения
    mov ax, [sum]                ; Сумма в ax
    mov bl, [len]                ; Длина в bl
    div bl                       ; Деление: al = ax / bl
    mov [average], al            ; Сохранение среднего

    ; Вывод результатов
    mov dx, offset msg_min       ; Вывод "Min: "
    mov ah, 09h
    int 21h
    mov al, [min]                ; Минимальное значение
    call print_byte              ; Печать числа
    mov dx, offset newline       ; Перевод строки
    mov ah, 09h
    int 21h

    mov dx, offset msg_max       ; Вывод "Max: "
    mov ah, 09h
    int 21h
    mov al, [max]                ; Максимальное значение
    call print_byte              ; Печать числа
    mov dx, offset newline       ; Перевод строки
    mov ah, 09h
    int 21h

    mov dx, offset msg_avg       ; Вывод "Average: "
    mov ah, 09h
    int 21h
    mov al, [average]            ; Среднее значение
    call print_byte              ; Печать числа
    mov dx, offset newline       ; Перевод строки
    mov ah, 09h
    int 21h

    ; Завершение программы
    mov ah, 4Ch
    int 21h
main endp

; Процедура для получения seed на основе нажатий клавиш
get_seed proc
    push ax
    push bx
    push cx
    push dx

    ; Выводим приглашение для генерации seed
    mov dx, offset prompt_seed
    mov ah, 09h
    int 21h

    mov cx, 5                    ; Количество нажатий
    mov word ptr [seed], 0       ; Инициализация seed

get_seed_loop:
    push cx                      ; Сохраняем счетчик перед вызовами прерываний

    mov ah, 00h
    int 1Ah                      ; Получаем текущее время (тики таймера в DX)
    mov bx, dx                   ; Сохраняем текущее время

    mov ah, 00h
    int 16h                      ; Ждем нажатия любой клавиши

    mov ah, 00h
    int 1Ah                      ; Получаем новое время
    sub dx, bx                   ; Вычисляем разницу во времени

    add [seed], dx               ; Накапливаем разницу в seed

    mov dl, '*'                  ; Выводим '*' для индикации нажатия
    mov ah, 02h
    int 21h

    pop cx                       ; Восстанавливаем счетчик
    loop get_seed_loop           ; Уменьшаем cx и повторяем

    mov dx, offset newline       ; Перевод строки после завершения
    mov ah, 09h
    int 21h

    pop dx
    pop cx
    pop bx
    pop ax
    ret
get_seed endp

; Процедура для печати байта как десятичного числа
print_byte proc
    push ax
    push bx
    push cx
    push dx
    mov byte ptr [printed], 0    ; Сброс флага печати
    cmp al, 0                    ; Проверка на ноль
    jne not_zero
    mov dl, '0'                  ; Если ноль, печатаем '0'
    mov ah, 02h
    int 21h
    jmp done
not_zero:
    mov ah, 0
    mov bl, 100
    div bl                       ; al = quotient, ah = remainder
    cmp al, 0
    je no_hundreds
    add al, '0'                  ; Преобразование в символ
    mov dl, al
    mov ah, 02h
    int 21h
    mov byte ptr [printed], 1    ; Установка флага
no_hundreds:
    mov al, ah                   ; Остаток в al
    mov ah, 0
    mov bl, 10
    div bl                       ; al = quotient, ah = remainder
    mov cl, al                   ; Сохранение десятков
    cmp cl, 0
    jne print_tens
    cmp byte ptr [printed], 1    ; Проверка, печатались ли цифры
    jne no_tens
print_tens:
    add cl, '0'                  ; Преобразование в символ
    mov dl, cl
    mov ah, 02h
    int 21h
    mov byte ptr [printed], 1    ; Установка флага
no_tens:
    add ah, '0'                  ; Преобразование единиц в символ
    mov dl, ah
    mov ah, 02h
    int 21h
done:
    pop dx
    pop cx
    pop bx
    pop ax
    ret
print_byte endp

end main