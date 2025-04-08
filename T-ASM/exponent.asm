.model small
.286
.stack 100h

.data
    prompt_base db 'Enter base: $'
    prompt_exponent db 13, 10, 'Enter exponent: $'
    result_msg db 13, 10, 'Result: $'
    error_msg db 13, 10, 'Invalid input! $'
    continue_msg db 13, 10, 'Continue? (y/n): $'
    msgIncorectNumber db 13, 10, 'Incorrect number! $'
    newline db 13, 10, '$'
    input_buffer db 20, ?, 20 dup('$')
    output_buffer db 20 dup('$')
    base dq 0.0
    exponent dq 0.0
    result dq 0.0
    ten dw 10
    temp dw 0

.code
main proc
    mov ax, @data
    mov ds, ax

program_start:
    finit               ; Инициализация сопроцессора FPU

    ; Запрос и ввод основания
    mov ah, 09h
    lea dx, prompt_base
    int 21h             ; Вывод строки "Enter base: "
    mov ah, 0Ah
    lea dx, input_buffer
    int 21h             ; Ввод строки от пользователя
    lea si, input_buffer + 2
    call string_to_float ; Преобразование строки в число с плавающей точкой
    fstp qword ptr [base] ; Сохранение основания в переменную base

    ; Запрос и ввод показателя
    mov ah, 09h
    lea dx, prompt_exponent
    int 21h             ; Вывод строки "Enter exponent: "
    mov ah, 0Ah
    lea dx, input_buffer
    int 21h             ; Ввод строки от пользователя
    lea si, input_buffer + 2
    call string_to_float ; Преобразование строки в число с плавающей точкой
    fstp qword ptr [exponent] ; Сохранение показателя в переменную exponent

    ; Проверка основания на 0
    fld qword ptr [base]
    fldz
    fcompp              ; Сравнение base с 0
    fstsw ax
    sahf                ; Перенос флагов FPU в процессорные флаги
    jz zero_check       ; Если base = 0, переход к проверке exponent

    ; Проверка, является ли показатель целым числом
    fld qword ptr [exponent]
    fld st(0)
    frndint             ; Округление exponent до целого
    fsub st(1), st(0)   ; Вычитание целой части из исходного значения
    fstp st(0)          ; Удаление целой части из стека
    fldz
    fcompp              ; Сравнение разницы с 0
    fstsw ax
    sahf
    je compute_integer_power  ; Если exponent целое, используем целочисленное вычисление
    jmp compute_float_power   ; Иначе вычисление для дробного показателя

compute_integer_power:  ; Вычисление степени для целого показателя
    fld qword ptr [exponent]
    fistp word ptr [temp]  ; Преобразование exponent в целое число
    mov cx, [temp]         ; Загрузка показателя в счетчик
    fld1                   ; Начальное значение результата = 1
    fld qword ptr [base]   ; Загрузка основания
    cmp cx, 0
    jge positive_exp       ; Если показатель >= 0, переход к умножению
    fchs                   ; Инверсия основания для отрицательного показателя
    neg cx                 ; Абсолютное значение показателя

positive_exp:
    jcxz store_result      ; Если показатель = 0, результат = 1
multiply_loop:
    fmul st(1), st(0)      ; Умножение результата на основание
    loop multiply_loop     ; Повторение cx раз
    fstp st(0)             ; Удаление лишней копии основания
    jmp store_result       ; Сохранение результата

compute_float_power:    ; Вычисление степени для дробного показателя
    fld qword ptr [exponent]  ; Загрузка показателя
    fld qword ptr [base]      ; Загрузка основания
    fyl2x                     ; Вычисление exponent * log2(base)
    f2xm1                     ; Вычисление 2^x - 1
    fld1
    faddp                     ; Прибавление 1, получение 2^x
    jmp store_result          ; Сохранение результата

zero_check:             ; Обработка случаев с основанием 0
    fld qword ptr [exponent]
    fldz
    fcompp              ; Сравнение exponent с 0
    fstsw ax
    sahf
    jz error_exit       ; 0^0 - ошибка
    jg zero_result      ; 0^положительное = 0
    jmp error_exit      ; 0^отрицательное - ошибка

zero_result:
    fldz                ; Результат = 0 для 0^положительное
    jmp store_result

store_result:
    fstp qword ptr [result]  ; Сохранение результата в переменную result

show_result:
    mov ah, 09h
    lea dx, result_msg
    int 21h             ; Вывод "Result: "
    fld qword ptr [result]
    call print_float    ; Вывод результата в виде строки
    mov ah, 02h
    mov dl, 13
    int 21h             ; Перевод строки
    mov dl, 10
    int 21h

    ; Запрос на продолжение
    mov ah, 09h
    lea dx, continue_msg
    int 21h             ; Вывод "Continue? (y/n): "
    mov ah, 01h
    int 21h             ; Считывание ответа пользователя
    cmp al, 'y'
    jne check_upper_y
    ; Добавление двух пустых строк перед перезапуском
    mov ah, 09h
    lea dx, newline
    int 21h
    mov ah, 09h
    lea dx, newline
    int 21h
    jmp program_start   ; Перезапуск программы
check_upper_y:
    cmp al, 'Y'
    jne exit
    ; Добавление двух пустых строк перед перезапуском
    mov ah, 09h
    lea dx, newline
    int 21h
    mov ah, 09h
    lea dx, newline
    int 21h
    jmp program_start

error_exit:             ; Вывод сообщения об ошибке и выход
    mov ah, 09h
    lea dx, error_msg
    int 21h

exit:
    mov ax, 4C00h
    int 21h             ; Завершение программы

main endp

string_to_float proc    ; Преобразование строки в число с плавающей точкой
    push ax
    push dx
    push si
    push bp
    mov bp, sp
    push 10             ; Константа 10 для вычислений
    push 0              ; Временная переменная

    fldz                ; Начальное значение = 0

    xor di, di          ; Флаг отрицательного числа
    cmp byte ptr [si], "-"
    jnz s2f_1
    mov di, 1           ; Установка флага для отрицательного числа
    inc si

s2f_1:                  ; Парсинг целой части
    mov cl, [si]
    cmp cl, 0Dh         ; Проверка на конец строки (Enter)
    jz s2f_end
    cmp cl, '.'
    je s2f_2            ; Переход к дробной части
    cmp cl, '0'
    jb s2f_error        ; Проверка на допустимый символ
    cmp cl, '9'
    ja s2f_error
    sub cl, '0'         ; Преобразование символа в цифру
    mov [bp - 4], cl
    fimul word ptr [bp - 2]  ; Умножение текущего результата на 10
    fiadd word ptr [bp - 4]  ; Прибавление новой цифры
    inc si
    jmp s2f_1

s2f_2:                  ; Парсинг дробной части
    inc si
    fld1                ; Делитель для дробной части
s2f_3:
    mov cl, [si]
    cmp cl, 0Dh
    jz s2f_4            ; Конец строки
    cmp cl, '0'
    jb s2f_error
    cmp cl, '9'
    ja s2f_error
    sub cl, '0'
    mov [bp - 4], cl
    fidiv word ptr [bp - 2]  ; Деление делителя на 10
    fld st(0)
    fimul word ptr [bp - 4]  ; Умножение цифры на текущий делитель
    faddp st(2), st     ; Добавление к результату
    inc si
    jmp s2f_3

s2f_error:              ; Обработка ошибки ввода
    mov dx, offset msgIncorectNumber
    mov ah, 09h
    int 21h
    mov ax, 4C01h
    int 21h

s2f_4:
    fstp st(0)          ; Удаление делителя из стека

s2f_end:
    cmp di, 1
    jne s2f_exit
    fchs                ; Инверсия знака для отрицательного числа

s2f_exit:
    leave
    pop si
    pop dx
    pop ax
    ret
string_to_float endp

print_float proc        ; Вывод числа с плавающей точкой
    enter 4, 0
    mov ten, 10         ; Константа 10 для преобразования

    ftst                ; Проверка знака числа
    fstsw ax
    sahf
    jnc print_float_positive
    mov al, '-'
    int 29h             ; Вывод минуса
    fchs                ; Инверсия числа

print_float_positive:   ; Разделение на целую и дробную части
    fld1
    fld st(1)
    fprem               ; Выделение дробной части
    fsub st(2), st      ; Вычитание дробной части из исходного числа
    fxch st(2)          ; Целая часть в ST(0)
    xor cx, cx          ; Счетчик цифр

print_float_1:          ; Преобразование целой части в стек цифр
    fidiv ten           ; Деление на 10
    fxch st(1)
    fld st(1)
    fprem               ; Остаток от деления
    fsub st(2), st
    fimul ten           ; Преобразование остатка в цифру
    fistp temp
    push temp           ; Сохранение цифры в стеке
    inc cx
    fxch st(1)
    ftst
    fstsw ax
    sahf
    jnz print_float_1   ; Продолжение, пока целая часть не 0

print_float_2:          ; Вывод целой части
    pop ax
    add al, '0'
    int 29h             ; Вывод цифры
    loop print_float_2

    fstp st             ; Удаление остатка
    fxch st(1)
    ftst
    fstsw ax
    sahf
    jz print_float_quit ; Если нет дробной части, выход
    mov al, '.'
    int 29h             ; Вывод точки
    mov cx, 6           ; Максимум 6 знаков после точки

print_float_3:          ; Вывод дробной части
    fimul ten           ; Умножение на 10
    fxch st(1)
    fld st(1)
    fprem               ; Выделение очередной цифры
    fsub st(2), st
    fxch st(2)
    fistp temp
    mov ax, temp
    or al, 30h          ; Преобразование в символ
    int 29h             ; Вывод цифры
    fxch st(1)
    ftst
    fstsw ax
    sahf
    loopne print_float_3  ; Продолжение до 6 цифр или нуля

print_float_quit:
    fstp st             ; Очистка стека
    fstp st
    leave
    ret
print_float endp

end main