.model tiny
.386
.stack 100h

.data
    commonMsg db 'Calculator with FPU', '$'
    exitMsg db 'To continue press Enter', '$'
    msgUnknownOperator db "Unknown operator", "$"
    msgDivisionbyZero db "Division by zero!", "$"
    msgOperators db "Select operator (+, -, *, /): ", "$"
    msgIncorectNumber db "Incorrect number!", "$"
    msgContinueQuestion db "Continue? (y/n): ", "$"

    firstOperandMsg db 'Enter first number: ', '$' ; сообщение для ввода а
    secondOperandMsg db 'Enter second number: ', '$' ; сообщение для ввода b
    resultMsg db '', '$' ;
    operand1 db 254, 0, 254 dup("$")
    operand2 db 254, 0, 254 dup("$")
    operator db 254, 0, 254 dup("$")
    continueQuestionAnswer db 254, 0, 254 dup("$")

    firstOperand dq ?
    secondOperand dq ?
    result dq ?

    ten equ word ptr [bp-2] ; регистр для вывода
    temp equ word ptr [bp-4] ; +1 регистр для вывода

.code

; input-output
; ax - placeholder
; bx - buffer
print_newline proc
    ; new line
    mov dl, 0Dh
    mov ah, 02h
    int 21h
    mov dl, 0Ah
    mov ah, 02h
    int 21h
    ret
print_newline endp
; ax - placeholder
; bx - buffer
input proc

    call print

    mov ah, 0Ah
    mov dx, bx
    int 21h

    call print_newline

    ret
input endp

; ax - address of text
println proc
    mov dx, ax
    mov ah, 09h
    int 21h
    call print_newline
    ret
println endp


; ax - address of text
print proc
    mov dx, ax
    mov ah, 09h
    int 21h
    ;
    ret
print endp

; al - text
println_text proc near
    mov dl, al
    mov ah, 02h
    int 21h
    call print_newline
    ret
println_text endp

; ax - number
print_int proc
    pusha
    test    ax, ax
    jns     print_int_oi1

    ; Если оно отрицательное, выведем минус и оставим его модуль.
    mov  cx, ax
    mov     ah, 02h
    mov     dl, '-'
    int     21h
    mov  ax, cx
    neg     ax

; Количество цифр будем держать в CX.
print_int_oi1:
    xor     cx, cx
    mov     bx, 10 ; основание сс. 10 для десятеричной и т.п.
print_int_oi2:
    xor     dx,dx
    div     bx
; Делим число на основание сс. В остатке получается последняя цифра.
; Сразу выводить её нельзя, поэтому сохраним её в стэке.
    push    dx
    inc     cx
; А с частным повторяем то же самое, отделяя от него очередную
; цифру справа, пока не останется ноль, что значит, что дальше
; слева только нули.
    test    ax, ax
    jnz     print_int_oi2
; Теперь приступим к выводу.
    mov     ah, 02h
print_int_oi3:
    pop     dx
; Извлекаем очередную цифру, переводим её в символ и выводим.
; раскоментировать если основание сс > 10, т.е. для вывода требуются буквы
;   cmp     dl,9
;   jbe     oi4
;   add     dl,7
;oi4:
    add     dl, '0'
    int     21h
; Повторим ровно столько раз, сколько цифр насчитали.
    loop    print_int_oi3

    call print_newline
    popa
    ret
print_int endp

; input-output for FPU
; ax - placeholder
; bx - buffer
print_float proc
    enter 4, 0       ; Пролог: выделяем 4 байта в стеке под локальные переменные
    mov ten, 10
    ftst             ; Проверяем знак числа
    fstsw ax
    sahf
    jnc print_float_positive

    mov al, '-'      ; Если число отрицательное, выводим минус
    int 29h
    fchs             ; Берем модуль числа

print_float_positive:
    fld1             ; Загружаем 1 в FPU стек
    fld st(1)        ; Копируем число на вершину стека
    fprem            ; Выделяем дробную часть
    fsub st(2), st   ; Получаем целую часть
    fxch st(2)       ; Меняем местами целую и дробную части
    xor cx, cx       ; Обнуляем счетчик цифр

    ; Выводим целую часть числа
print_float_1:
    fidiv ten        ; Делим целую часть на 10
    fxch st(1)       ; Меняем местами st и st(1) для корректного fprem
    fld st(1)        ; Копируем частное
    fprem            ; Получаем остаток (последняя цифра справа)
    fsub st(2), st   ; Вычисляем новую целую часть
    fimul ten        ; Восстанавливаем цифру
    fistp temp       ; Записываем её во временную переменную
    push temp        ; Кладем в стек
    inc cx           ; Увеличиваем счетчик цифр
    fxch st(1)       ; Подготовка к следующему шагу
    ftst             ; Проверяем, достигли ли нуля
    fstsw ax
    sahf
    jnz print_float_1

    ; Извлекаем цифры из стека и выводим
print_float_2:
    pop ax
    add al, '0'      ; Конвертируем в ASCII
    int 29h          ; Выводим на экран
    loop print_float_2

    ; Обрабатываем дробную часть
    fstp st          ; Убираем остаток из стека
    fxch st(1)
    ftst
    fstsw ax
    sahf
    jz print_float_quit  ; Если дробной части нет, выходим

    mov al, '.'
    int 29h          ; Выводим точку
    mov cx, 10        ; Ограничиваем точность 10 знаками после точки

print_float_3:
    fimul ten        ; Умножаем на 10
    fxch st(1)
    fld st(1)        ; Копируем число
    fprem            ; Получаем дробную часть
    fsub st(2), st   ; Оставляем только дробную
    fxch st(2)
    fistp temp       ; Записываем очередную цифру
    mov ax, temp
    or al, '0'       ; Преобразуем в ASCII
    int 29h          ; Выводим
    fxch st(1)       ; Готовим стек для следующей итерации
    ftst
    fstsw ax
    sahf
    loopne print_float_3

print_float_quit:
    fstp st          ; Очищаем стек FPU
    fstp st
    leave            ; Эпилог
    ret
print_float endp


; String to float
; input: si - buffer address
; output: On top in FPU Stack
string_to_float proc
; обрабатываем содержимое буфера
    push ax
    push dx
    push si
    push bp ; формируем кадр стэка, чтобы хранить десятку и ещё какую-нибудь цифру.
    mov bp, sp
    push 10
    push 0

    fldz

    xor di, di
    cmp byte ptr [si],"-" ; если первый символ минус
    jnz s2f_1
    mov di,1  ; устанавливаем флаг
    inc si    ; и пропускаем его

s2f_1:
    mov cl,[si] ; берем символ из буфера
    cmp cl,0dh  ; проверяем не последний ли он
    jz s2f_end

; если символ не последний, то проверяем его на правильность
    cmp cl, '.'
    je s2f_2
    cmp cl,'0'  ; если введен неверный символ <0
    jb s2f_error
    cmp cl,'9'  ; если введен неверный символ >9
    ja s2f_error

    sub cl,'0' ; делаем из символа число
    mov [bp - 4], cl ; сохраним её во временной ячейке и допишем к текущему результату справа,
    fimul word ptr [bp - 2] ; то есть умножим уже имеющееся число на десять
    fiadd word ptr [bp - 4] ; и прибавим только что обретённую цифру.

    inc si     ; указатель на следующий символ
    jmp s2f_1     ; повторяем

s2f_2:
    inc si
    fld1
s2f_3:
    mov cl,[si] ; берем символ из буфера
    cmp cl,0dh  ; проверяем не последний ли он
    jz s2f_4

    cmp cl,'0'  ; если введен неверный символ <0
    jb s2f_error
    cmp cl,'9'  ; если введен неверный символ >9
    ja s2f_error

    sub cl,'0' ; делаем из символа число
    mov [bp - 4], cl
    fidiv word ptr [bp - 2] ; получаем очередную отрицательную степень десятки,
    fld st(0) ; дублируем её,
    fimul word ptr [bp - 4] ; помножаем на введённую цифру, тем самым получая её на нужном месте,
    faddp st(2), st ; и добавляем к текущему результату.
    inc si
    jmp s2f_3

s2f_error:   ; если была ошибка, то выводим сообщение об этом и выходим
    mov dx, offset msgIncorectNumber
    mov ah,09
    int 21h
    ret

s2f_4:
    fstp st(0)

s2f_end:
    cmp di,1 ; если установлен флаг, то
    jne s2f_exit
    fchs

s2f_exit:
    leave
    pop si
    pop dx
    pop ax
    ret
string_to_float endp

main proc
    mov ax, @data
    mov ds, ax

    finit
    xor eax,eax ; обнуление регистров
    xor edx,edx

main_loop:
    call clear_screen

    mov ax, offset commonMsg
    call println

    mov ax, offset firstOperandMsg
    mov bx, offset operand1
    call input

    mov si, offset operand1 + 2
    call string_to_float
    FSTP firstOperand

    mov ax, offset secondOperandMsg
    mov bx, offset operand2
    call input

    mov si, offset operand2 + 2
    call string_to_float
    FSTP secondOperand

    ; Now we have values in firstOperand and secondOperand variables

    mov ax, offset msgOperators
    mov bx, offset operator
    call input

    mov cl, operator + 2
    cmp cx, "+"
    je _add
    mov cl, operator + 2
    cmp cx, "-"
    je _sub
    mov cl, operator + 2
    cmp cx, "*"
    je _mul
    mov cl, operator + 2
    cmp cx, "/"
    je _div

    ; unknown operator
    mov ax, offset msgUnknownOperator
    call print

continue:
    call print_newline
    mov ax, offset msgContinueQuestion
    mov bx, offset continueQuestionAnswer
    call input

    mov cl, continueQuestionAnswer + 2
    cmp cx, "n"
    jne main_loop

    mov ax, 4C00h
    int 21h
main endp


_add proc
    fld [secondOperand]
    fld [firstOperand]
    fadd st(0), st(1)
    call print_float
    jmp continue
_add endp

_sub proc
    fld [secondOperand]
    fld [firstOperand]
    fsub st(0), st(1)
    call print_float
    jmp continue
_sub endp

_mul proc
    fld [secondOperand]
    fld [firstOperand]
    fmul st(0), st(1)
    call print_float
    jmp continue
_mul endp


_div proc
    ; Check if second operand is zero
    push ax
    fld [secondOperand]
    ftst
    fstsw ax
    sahf
    sete al
    cmp al, 1
    je _div_division_by_zero
    pop ax

    fld [firstOperand]
    fdiv st(0), st(1)
    call print_float

    jmp continue
_div_division_by_zero:
    pop ax
    mov ax, offset msgDivisionbyZero
    call print
    jmp continue
_div endp

clear_screen proc
    mov ax, 0003h
    int 10h
    ret
clear_screen endp

end main