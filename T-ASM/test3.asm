.model small
.stack 100h

.data
    ; Массив точек (X, Y) в экранных координатах
    points       dw  205,50, 190,70, 175,50, 190,40, 205,50, 250,50, 250,60
                 dw  280,60, 280,80, 250,90, 205,110, 220,160, 250,170, 205,170
                 dw  175,115, 100,120, 85,150, 70,160, 100,170, 40,170, 32,100
                 dw  55,80, 40,70, 48,50, 70,40, 115,40, 130,50, 130,60, 115,50
                 dw  70,50, 70,70, 175,80, 190,70, 250,90
    points_count equ 34                                                            ; Количество точек

    ; Переменные для алгоритма Брезенхема
    x0           dw  ?
    y0           dw  ?
    x1           dw  ?
    y1           dw  ?
    delta_x      dw  ?
    delta_y      dw  ?
    error        dw  ?
    xstep        dw  ?
    ystep        dw  ?
    steep        dw  ?
    current_x    dw  ?
    current_y    dw  ?

.code
main proc
                   mov  ax, @data
                   mov  ds, ax

    ; Инициализация графического режима 13h
                   mov  ax, 0013h
                   int  10h

    ; Соединение точек линиями
                   mov  si, offset points
                   mov  cx, points_count
                   dec  cx                   ; Количество линий на 1 меньше точек

    draw_lines:    
    ; Загрузка (x0, y0) и (x1, y1)
                   mov  ax, [si]             ; x0
                   mov  [x0], ax
                   mov  ax, [si + 2]         ; y0
                   mov  [y0], ax
                   mov  ax, [si + 4]         ; x1
                   mov  [x1], ax
                   mov  ax, [si + 6]         ; y1
                   mov  [y1], ax

                   call draw_line

                   add  si, 4                ; Переход к следующей паре точек
                   loop draw_lines

    ; Ожидание нажатия клавиши
                   mov  ah, 00h
                   int  16h

    ; Возврат в текстовый режим
                   mov  ax, 0003h
                   int  10h

    ; Завершение программы
                   mov  ax, 4C00h
                   int  21h
main endp

    ; Процедура рисования линии алгоритмом Брезенхема
draw_line proc near
                   push ax bx cx dx si di    ; Замена pusha на явные push

                   mov  [steep], 0

    ; Вычисление дельт
                   mov  ax, [x1]
                   sub  ax, [x0]
                   mov  [delta_x], ax        ; delta_x = x1 - x0
                   mov  ax, [y1]
                   sub  ax, [y0]
                   mov  [delta_y], ax        ; delta_y = y1 - y0

    ; Проверка крутизны
                   mov  ax, [delta_x]
                   cwd
                   xor  ax, dx
                   sub  ax, dx               ; |delta_x|
                   mov  bx, ax
                   mov  ax, [delta_y]
                   cwd
                   xor  ax, dx
                   sub  ax, dx               ; |delta_y|
                   cmp  ax, bx
                   jle  no_steep
                   mov  [steep], 1
    ; Обмен координат
                   mov  ax, [x0]
                   xchg ax, [y0]
                   mov  [x0], ax
                   mov  ax, [x1]
                   xchg ax, [y1]
                   mov  [x1], ax
    no_steep:      

    ; Пересчет delta_x и delta_y
                   mov  ax, [x1]
                   sub  ax, [x0]
                   mov  [delta_x], ax
                   mov  ax, [y1]
                   sub  ax, [y0]
                   mov  [delta_y], ax

    ; Определение шага по X
                   mov  ax, [delta_x]
                   cmp  ax, 0
                   jge  xstep_positive
                   neg  ax
                   mov  [xstep], -1
                   jmp  xstep_set
    xstep_positive:
                   mov  [xstep], 1
    xstep_set:     
                   mov  [delta_x], ax        ; |delta_x|

    ; Определение шага по Y
                   mov  ax, [delta_y]
                   cmp  ax, 0
                   jge  ystep_positive
                   neg  ax
                   mov  [ystep], -1
                   jmp  ystep_set
    ystep_positive:
                   mov  [ystep], 1
    ystep_set:     
                   mov  [delta_y], ax        ; |delta_y|

    ; Инициализация ошибки
                   mov  ax, [delta_x]
                   shr  ax, 1
                   mov  [error], ax

    ; Начальные координаты
                   mov  ax, [x0]
                   mov  [current_x], ax
                   mov  ax, [y0]
                   mov  [current_y], ax

    line_loop:     
                   cmp  [steep], 1
                   jne  no_steep_draw
                   mov  cx, [current_y]
                   mov  dx, [current_x]
                   jmp  draw_pixel
    no_steep_draw: 
                   mov  cx, [current_x]
                   mov  dx, [current_y]

    draw_pixel:    
    ; Проверка границ
                   cmp  cx, 0
                   jl   skip_pixel
                   cmp  cx, 319
                   jg   skip_pixel
                   cmp  dx, 0
                   jl   skip_pixel
                   cmp  dx, 199
                   jg   skip_pixel

                   mov  ah, 0Ch
                   mov  al, 14
                   mov  bh, 0
                   int  10h

    skip_pixel:    
    ; Проверка окончания
                   mov  ax, [current_x]
                   cmp  ax, [x1]
                   jne  continue
                   mov  ax, [current_y]
                   cmp  ax, [y1]
                   je   end_line

    continue:      
                   mov  ax, [error]
                   sub  ax, [delta_y]
                   mov  [error], ax

                   jns  no_y_step
                   mov  ax, [current_y]
                   add  ax, [ystep]
                   mov  [current_y], ax
                   mov  ax, [error]
                   add  ax, [delta_x]
                   mov  [error], ax

    no_y_step:     
                   mov  ax, [current_x]
                   add  ax, [xstep]
                   mov  [current_x], ax
                   jmp  line_loop

    end_line:      
                   pop  di si dx cx bx ax    ; Замена popa на явные pop
                   ret
draw_line endp

end main