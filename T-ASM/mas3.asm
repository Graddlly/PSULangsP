.model small
.stack 100h
.data
    array db 100 dup (?)         ; Массив на 100 байт (максимальная длина)
    len db ?                     ; Длина массива (вводится пользователем)
    seed dw ?                    ; Начальное значение для генерации случайных чисел (16 бит)
    rng_state dw ?               ; Состояние генератора
    min db ?                     ; Минимальное значение
    max db ?                     ; Максимальное значение
    sum dw ?                     ; Сумма элементов (word, так как сумма может превысить 255)
    average db ?                 ; Среднее значение
    printed db ?                 ; Флаг для процедуры печати числа
    input_buf db 4, ?, 4 dup (?) ; Буфер для ввода строки (макс. 4 символа)
    prompt_len db 'Enter array length (1-100): $' ; Приглашение для ввода длины
    msg_min db 'Min: $'          ; Сообщение для минимального значения
    msg_max db 'Max: $'          ; Сообщение для максимального значения
    msg_avg db 'Average: $'      ; Сообщение для среднего значения
    msg_array db 'Generated array: $' ; Сообщение для вывода массива
    newline db 13, 10, '$'       ; Символы перевода строки (CRLF)

.code
main proc
    mov ax, @data                ; Инициализация сегмента данных
    mov ds, ax

    ; Инициализация генератора случайных чисел
    call init_random

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

    ; Генерация массива случайных чисел
    mov cl, [len]                ; Счетчик = длина массива
    mov ch, 0
    mov si, offset array         ; Указатель на начало массива

generate_loop:
    call random_byte              ; Вызываем улучшенный генератор
    mov [si], al                  ; Записываем случайный байт в массив
    
    ; Ограничение диапазона 0-99
    and byte ptr [si], 01111111b  ; Убираем старший бит для диапазона 0-127
    
    inc si                        ; Следующий элемент массива
    loop generate_loop

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

; Улучшенный генератор случайных чисел на основе алгоритма xorshift
random_byte proc
    push bx
    mov ax, [rng_state]
    
    ; Алгоритм xorshift 16-bit
    mov bx, ax
    shl bx, 7
    xor ax, bx
    
    mov bx, ax
    shr bx, 9
    xor ax, bx
    
    mov bx, ax
    shl bx, 8
    xor ax, bx
    
    mov [rng_state], ax          ; Обновляем состояние
    
    ; Берем младший байт как случайное число
    pop bx
    ret
random_byte endp

; Процедура инициализации генератора случайных чисел
init_random proc
    push ax
    push dx
    
    ; Используем системный таймер для начального seed
    mov ah, 00h
    int 1Ah                      ; Получаем тики таймера в DX
    mov [seed], dx
    mov [rng_state], dx          ; Инициализируем состояние генератора
    
    pop dx
    pop ax
    ret
init_random endp

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