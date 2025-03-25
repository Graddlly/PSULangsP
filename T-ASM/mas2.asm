.model small
.stack 100h
.data
    array db 100 dup (?)         ; Массив для хранения до 100 байт
    len db ?                     ; Длина массива
    min db ?                     ; Минимальное значение
    max db ?                     ; Максимальное значение
    sum dw ?                     ; Сумма элементов
    average db ?                 ; Среднее значение
    input_buf db 4, ?, 4 dup (?) ; Буфер для ввода строк
    prompt_len db 'Enter number of elements (1-100): $'
    prompt_element db 'Enter element ', 0
    msg_min db 'Min: $'
    msg_max db 'Max: $'
    msg_avg db 'Average: $'
    msg_array db 'Array: $'
    newline db 13, 10, '$'       ; Символы новой строки

.code
main proc
    mov ax, @data
    mov ds, ax

    ; Запрос количества элементов
    mov dx, offset prompt_len
    mov ah, 09h
    int 21h

    ; Ввод строки
    mov dx, offset input_buf
    mov ah, 0Ah
    int 21h

    ; Преобразование строки в число (len)
    mov si, offset input_buf + 2
    mov bx, 0
convert_len:
    mov al, [si]
    cmp al, 13                  ; Проверка на Enter (CR)
    je convert_len_done
    cmp al, '0'
    jb convert_len_done
    cmp al, '9'
    ja convert_len_done
    sub al, '0'
    mov ah, 0
    push ax
    mov ax, bx
    mov cx, 10
    mul cx
    pop dx
    add ax, dx
    mov bx, ax
    inc si
    jmp convert_len
convert_len_done:
    mov [len], bl                ; Сохранение длины
    mov dx, offset newline
    mov ah, 09h
    int 21h

    ; Ввод элементов массива
    mov cl, [len]                ; Установка счетчика цикла
    mov ch, 0
    mov si, offset array         ; si указывает на начало массива
    mov di, 1                    ; Счетчик элементов для отображения
input_loop:
    ; Вывод "Enter element X: "
    mov dx, offset prompt_element
    mov ah, 09h
    int 21h
    mov ax, di                   ; Загрузка номера элемента в ax
    call print_number            ; Вывод номера элемента
    mov dl, ':'
    mov ah, 02h
    int 21h
    mov dl, ' '
    int 21h

    ; Очистка буфера ввода
    mov byte ptr [input_buf + 1], 0

    ; Ввод элемента
    mov dx, offset input_buf
    mov ah, 0Ah
    int 21h

    ; Переход на новую строку после ввода
    mov dx, offset newline
    mov ah, 09h
    int 21h

    ; Преобразование введенной строки в число
    push di                      ; Сохраняем di
    mov di, offset input_buf + 2 ; di для строки ввода
    xor bx, bx                   ; Обнуляем bx перед преобразованием
convert_element:
    mov al, [di]
    cmp al, 13                  ; Проверка на Enter (CR)
    je convert_element_done
    cmp al, '0'
    jb convert_element_done
    cmp al, '9'
    ja convert_element_done
    sub al, '0'
    mov ah, 0
    push ax
    mov ax, bx
    mov bx, 10
    mul bx
    pop bx
    add ax, bx
    mov bx, ax
    inc di
    jmp convert_element
convert_element_done:
    mov [si], bl                 ; Сохранение элемента в массив
    inc si                       ; Переход к следующей ячейке массива
    pop di                       ; Восстановление di
    inc di                       ; Увеличение номера элемента
    loop input_loop              ; Уменьшение cx и переход, если не ноль

    ; Вывод массива
    mov dx, offset msg_array
    mov ah, 09h
    int 21h
    mov cl, [len]
    mov ch, 0
    mov si, offset array
print_array_loop:
    mov al, [si]
    call print_byte
    mov dl, ' '
    mov ah, 02h
    int 21h
    inc si
    loop print_array_loop
    mov dx, offset newline
    mov ah, 09h
    int 21h

    ; Поиск минимума, максимума и суммы
    mov cl, [len]
    mov ch, 0
    mov si, offset array
    mov byte ptr [min], 255
    mov byte ptr [max], 0
    mov word ptr [sum], 0
find_loop:
    mov al, [si]
    cmp al, [min]
    jae not_less
    mov [min], al
not_less:
    cmp al, [max]
    jbe not_greater
    mov [max], al
not_greater:
    mov ah, 0
    add [sum], ax
    inc si
    loop find_loop

    ; Вычисление среднего
    mov ax, [sum]
    mov bl, [len]
    div bl
    mov [average], al

    ; Вывод результатов
    mov dx, offset msg_min
    mov ah, 09h
    int 21h
    mov al, [min]
    call print_byte
    mov dx, offset newline
    mov ah, 09h
    int 21h

    mov dx, offset msg_max
    mov ah, 09h
    int 21h
    mov al, [max]
    call print_byte
    mov dx, offset newline
    mov ah, 09h
    int 21h

    mov dx, offset msg_avg
    mov ah, 09h
    int 21h
    mov al, [average]
    call print_byte
    mov dx, offset newline
    mov ah, 09h
    int 21h

    mov ah, 4Ch
    int 21h
main endp

; Процедура для вывода байта как десятичного числа
print_byte proc
    push ax
    push bx
    push cx
    push dx
    mov ah, 0
    cmp al, 0
    jne not_zero
    mov dl, '0'
    mov ah, 02h
    int 21h
    jmp print_done
not_zero:
    mov bl, 100
    div bl
    cmp al, 0
    je no_hundreds
    add al, '0'
    mov dl, al
    mov ah, 02h
    int 21h
no_hundreds:
    mov al, ah
    mov ah, 0
    mov bl, 10
    div bl
    cmp al, 0
    jne print_tens
    cmp byte ptr [min], 100
    jae print_tens
    jmp no_tens
print_tens:
    add al, '0'
    mov dl, al
    mov ah, 02h
    int 21h
no_tens:
    add ah, '0'
    mov dl, ah
    mov ah, 02h
    int 21h
print_done:
    pop dx
    pop cx
    pop bx
    pop ax
    ret
print_byte endp

; Процедура для вывода номера элемента ("Enter element X")
print_number proc
    push ax
    push bx
    push cx
    push dx
    mov bx, 10
    mov cx, 0
print_loop:
    xor dx, dx
    div bx
    push dx
    inc cx
    cmp ax, 0
    jne print_loop
print_digits:
    pop dx
    add dl, '0'
    mov ah, 02h
    int 21h
    loop print_digits
    pop dx
    pop cx
    pop bx
    pop ax
    ret
print_number endp

end main