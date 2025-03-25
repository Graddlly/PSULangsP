.model small
.stack 100h

.data
    msgFirstOperand db "Enter first number: ", "$"
    msgSecondOperand db "Enter second number: ", "$"
    msgOperators db "Select operator (+, -, *, /): ", "$"
    msgIncorectNumber db "Incorrect number", "$"
    msgDivisionbyZero db "Division by zero", "$"
    msgUnknownOperator db "Unknown operator", "$"
    msgContinueQuestion db "Continue (y/n): ", "$"
    newLine db 0Dh, 0Ah, "$"
    operand1 db 254, 0, 254 dup("$")
    operand2 db 254, 0, 254 dup("$")
    operator db 254, 0, 254 dup("$")
    continueQuestionAnswer db 254, 0, 254 dup("$")

.code

; input-output
; ax - placeholder
; bx - buffer
input proc

    mov dx, ax
    mov ah, 09h
    int 21h

    mov ah, 0Ah
    mov dx, bx
    int 21h

    mov dl, 0Dh
    mov ah, 02h
    int 21h
    mov dl, 0Ah
    mov ah, 02h
    int 21h

    ret
input endp

; ax - address of text
println proc
    mov dx, ax
    mov ah, 09h
    int 21h
    ; new line
    mov dl, 0Dh
    mov ah, 02h
    int 21h
    mov dl, 0Ah
    mov ah, 02h
    int 21h
    ;
    ret
println endp

; al - text
println_text proc
    mov dl, al
    mov ah, 02h
    int 21h
    mov dl, 0Dh
    mov ah, 02h
    int 21h
    mov dl, 0Ah
    mov ah, 02h
    int 21h
    ret
println_text endp

; ax - number
print_int proc
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

    mov dl, 0Dh
    mov ah, 02h
    int 21h
    mov dl, 0Ah
    mov ah, 02h
    int 21h

    ret
print_int endp

; String to number
; input: si - buffer address
; output: ax - number
string_to_number proc
; обрабатываем содержимое буфера
    ; mov si,offset buff+2 ; берем адрес начала строки
    cmp byte ptr [si],"-" ; если первый символ минус
    jnz ii1
    mov di,1  ; устанавливаем флаг
    inc si    ; и пропускаем его
ii1:
    xor ax,ax
    mov bx,10  ; основание сc
ii2:
    mov cl,[si] ; берем символ из буфера
    cmp cl,0dh  ; проверяем не последний ли он
    jz endin

; если символ не последний, то проверяем его на правильность
    cmp cl,'0'  ; если введен неверный символ <0
    jb er
    cmp cl,'9'  ; если введен неверный символ >9
    ja er

    sub cl,'0' ; делаем из символа число
    mul bx     ; умножаем на 10
    add ax,cx  ; прибавляем к остальным
    inc si     ; указатель на следующий символ
    jmp ii2     ; повторяем

er:   ; если была ошибка, то выводим сообщение об этом и выходим
    mov dx, offset msgIncorectNumber
    mov ah,09
    int 21h
    int 20h

; все символы из буфера обработаны число находится в ax

endin:
    cmp di,1 ; если установлен флаг, то
    je do_neg
    ret
string_to_number endp

do_neg proc
    neg ax
    ret
do_neg endp
; end s2n

main proc
    mov ax, @data ;
    mov ds, ax

    mov ax, 0003h
    int 10h

main_loop:
    mov ax, offset msgFirstOperand
    mov bx, offset operand1
    call input

    mov ax, offset msgSecondOperand
    mov bx, offset operand2
    call input

    mov ax, offset msgOperators
    mov bx, offset operator
    call input

    mov si, offset operand1 + 2
    call string_to_number ; result in ax
    push ax

    mov si, offset operand2 + 2
    call string_to_number
    mov bx, ax
    pop ax

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
    call println

continue:
    mov ax, offset msgContinueQuestion
    mov bx, offset continueQuestionAnswer
    call input

    mov cl, continueQuestionAnswer + 2
    cmp cx, "n"
    jne main_loop

    call exit
main endp


; ax - first number
; bx - second number
_add proc
    add ax, bx
    call print_int
    jmp continue
_add endp

; ax - first number
; bx - second number
_sub proc
    sub ax, bx
    call print_int
    jmp continue
_sub endp

; ax - first number
; bx - second number
_mul proc
    imul bx ; ax = ax * bx
    call print_int
    jmp continue
_mul endp

; ax - first number
; bx - second number
_div proc
    cmp bx, 0
    je _div_division_by_zero

    cwd ; заполним DX знаковым битом из AX
    idiv bx
    call print_int

    jmp continue
_div_division_by_zero:
    mov ax, offset msgDivisionbyZero
    call println
    jmp continue
_div endp

exit proc
    mov ax, 4C00h
    int 21h
exit endp

end main