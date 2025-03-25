.model small
.stack 100h
.data
    x_center equ 160    ; X-координата центра экрана (320/2)
    y_center equ 100    ; Y-координата центра экрана (200/2)
    radius   equ 2      ; Радиус точки
.code
    start:             
                       mov  ax, @data
                       mov  ds, ax
    
    ; Переключение в графический режим 320x200 256 цветов
                       mov  ax, 13h
                       int  10h
    
    ; Рисуем точку
                       mov  cx, x_center
                       mov  dx, y_center
                       call draw_filled_circle
    
    ; Ожидание нажатия клавиши
                       mov  ah, 0
                       int  16h
    
    ; Возвращение в текстовый режим
                       mov  ax, 03h
                       int  10h
    
                       mov  ax, 4C00h
                       int  21h
    
    ; Процедура рисования заполненного круга
    ; Вход: CX - X-координата, DX - Y-координата центра, radius - радиус
    ; Использует пиксельную функцию BIOS

draw_filled_circle proc
                       push bp
                       mov  bp, sp
    
                       mov  si, -radius
    outer_loop:        
                       mov  di, -radius
    inner_loop:        
                       mov  ax, si
                       imul ax
                       mov  bx, di
                       imul bx
                       add  ax, bx
                       cmp  ax, radius * radius
                       jg   skip_pixel
    
    ; Вычисление координат и рисование пикселя
                       mov  ah, 0Ch
                       mov  al, 15                 ; Белый цвет
                       mov  cx, si
                       add  cx, x_center
                       mov  dx, di
                       add  dx, y_center
                       int  10h
    
    skip_pixel:        
                       inc  di
                       cmp  di, radius
                       jle  inner_loop
    
                       inc  si
                       cmp  si, radius
                       jle  outer_loop
    
                       pop  bp
                       ret

draw_filled_circle endp

end start
