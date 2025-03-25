.model small
.stack 100h
.data
.code

main proc
    ; Инициализация сегмента данных
    mov ax, @data
    mov ds, ax

    ; Установить графический режим 13h (320x200, 256 цветов)
    mov ax, 13h
    int 10h

    ; Рисуем тело собаки
    mov cx, 160 ; центр по X
    mov dx, 130 ; центр по Y
    mov al, 4   ; цвет (темно-красный)
    mov bx, 25  ; радиус
    call draw_circle

    ; Рисуем голову собаки
    mov cx, 160 ; центр по X
    mov dx, 100 ; центр по Y
    mov al, 4   ; цвет (темно-красный)
    mov bx, 20  ; радиус
    call draw_circle

    ; Рисуем левое ухо
    mov cx, 140
    mov dx, 90
    mov al, 4
    mov bx, 10
    call draw_circle

    ; Рисуем правое ухо
    mov cx, 180
    mov dx, 90
    mov al, 4
    mov bx, 10
    call draw_circle

    ; Рисуем левый глаз
    mov cx, 155
    mov dx, 95
    mov al, 15 ; белый
    mov bx, 2
    call draw_circle

    ; Рисуем правый глаз
    mov cx, 165
    mov dx, 95
    mov al, 15
    mov bx, 2
    call draw_circle

    ; Рисуем нос
    mov cx, 160
    mov dx, 105
    mov al, 1 ; синий
    mov bx, 3
    call draw_circle

    ; Рисуем левую переднюю лапу
    mov cx, 145
    mov dx, 155
    mov al, 4
    mov bx, 8
    call draw_circle

    ; Рисуем правую переднюю лапу
    mov cx, 175
    mov dx, 155
    mov al, 4
    mov bx, 8
    call draw_circle

    ; Рисуем левую заднюю лапу
    mov cx, 155
    mov dx, 165
    mov al, 4
    mov bx, 8
    call draw_circle

    ; Рисуем правую заднюю лапу
    mov cx, 165
    mov dx, 165
    mov al, 4
    mov bx, 8
    call draw_circle

    ; Рисуем хвост
    mov cx, 185
    mov dx, 130
    mov al, 4
    mov bx, 10
    call draw_circle

    ; Ожидание нажатия клавиши
    mov ah, 0
    int 16h

    ; Возвращение в текстовый режим
    mov ax, 3h
    int 10h

    ; Завершение программы
    mov ax, 4C00h
    int 21h

main endp

; Процедура для рисования круга
; Вход: CX - центр по X, DX - центр по Y, AL - цвет, BX - радиус
draw_circle proc
    push ax
    push bx
    push cx
    push dx
    push si
    push di

    ; Инициализация переменных
    mov si, 0       ; x = 0
    mov di, bx      ; y = радиус
    mov bp, 1
    sub bp, bx      ; d = 1 - радиус

draw_circle_loop:
    ; Рисуем 8 точек
    call plot_points

    ; Обновляем d
    mov ax, bp
    add ax, si
    shl ax, 1
    add ax, 1
    jl skip_inc_y

    dec di          ; y--
    add ax, di
    shl ax, 1

skip_inc_y:
    mov bp, ax
    inc si          ; x++
    cmp si, di
    jle draw_circle_loop

    pop di
    pop si
    pop dx
    pop cx
    pop bx
    pop ax
    ret
draw_circle endp

; Подпрограмма для рисования 8 точек
plot_points proc
    push ax
    push bx
    push cx
    push dx

    ; (cx + si, dx + di)
    mov ax, si
    add ax, cx
    mov bx, di
    add bx, dx
    call plot_pixel

    ; (cx - si, dx + di)
    mov ax, cx
    sub ax, si
    mov bx, di
    add bx, dx
    call plot_pixel

    ; (cx + si, dx - di)
    mov ax, si
    add ax, cx
    mov bx, dx
    sub bx, di
    call plot_pixel

    ; (cx - si, dx - di)
    mov ax, cx
    sub ax, si
    mov bx, dx
    sub bx, di
    call plot_pixel

    ; (cx + di, dx + si)
    mov ax, di
    add ax, cx
    mov bx, si
    add bx, dx
    call plot_pixel

    ; (cx - di, dx + si)
    mov ax, cx
    sub ax, di
    mov bx, si
    add bx, dx
    call plot_pixel

    ; (cx + di, dx - si)
    mov ax, di
    add ax, cx
    mov bx, dx
    sub bx, si
    call plot_pixel

    ; (cx - di, dx - si)
    mov ax, cx
    sub ax, di
    mov bx, dx
    sub bx, si
    call plot_pixel

    pop dx
    pop cx
    pop bx
    pop ax
    ret
plot_points endp

; Подпрограмма для рисования пикселя
; Вход: AX - X, BX - Y, AL - цвет
plot_pixel proc
    push ax
    push bx
    push cx
    push dx

    mov cx, ax
    mov dx, bx
    mov ah, 0Ch
    int 10h

    pop dx
    pop cx
    pop bx
    pop ax
    ret
plot_pixel endp

end main
