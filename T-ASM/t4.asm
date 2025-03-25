.model small
.stack 100h

.data
    COLOR_BROWN equ 6
    COLOR_BLACK equ 0
    COLOR_WHITE equ 15

    ; Глобальные переменные
    step_x      dw  ?
    step_y      dw  ?
    error       dw  ?
    delta       dw  ?
    threshold   dw  ?
    steep       db  ?

.code
main proc
                     mov      ax, @data
                     mov      ds, ax
                     mov      es, ax
    
    ; Установка графического режима
                     mov      ax, 13h
                     int      10h

    ; === Рисуем тело ===
                     mov      cx, 100
                     mov      dx, 100
                     mov      si, 80
                     mov      di, 50
                     mov      al, COLOR_BROWN
                     call     draw_rect

    ; === Рисуем голову ===
                     mov      cx, 140
                     mov      dx, 80
                     mov      si, 20
                     mov      al, COLOR_BROWN
                     call     draw_circle

    ; === Левое ухо ===
                     push     130
                     push     60
                     push     120
                     push     80
                     push     130
                     push     80
                     mov      al, COLOR_BROWN
                     call     draw_triangle
                     add      sp, 12

    ; === Правое ухо ===
                     push     150
                     push     60
                     push     160
                     push     80
                     push     150
                     push     80
                     mov      al, COLOR_BROWN
                     call     draw_triangle
                     add      sp, 12

    ; === Глаза ===
                     mov      cx, 130
                     mov      dx, 85
                     mov      si, 3
                     mov      al, COLOR_WHITE
                     call     draw_circle

                     mov      cx, 150
                     mov      dx, 85
                     mov      si, 3
                     mov      al, COLOR_WHITE
                     call     draw_circle

                     mov      cx, 130
                     mov      dx, 85
                     mov      al, COLOR_BLACK
                     call     put_pixel

                     mov      cx, 150
                     mov      dx, 85
                     mov      al, COLOR_BLACK
                     call     put_pixel

    ; === Нос ===
                     push     138
                     push     95
                     push     142
                     push     95
                     push     140
                     push     100
                     mov      al, COLOR_BLACK
                     call     draw_triangle
                     add      sp, 12

    ; Ожидание клавиши
                     mov      ah, 0
                     int      16h

    ; Возврат в текстовый режим
                     mov      ax, 3
                     int      10h

                     mov      ax, 4C00h
                     int      21h
main endp

    ; ==============================================
put_pixel proc
                     push     es
                     push     ax
                     push     di
                     push     dx
                     push     cx
    
                     mov      di, 0A000h
                     mov      es, di
    
                     mov      di, dx
                     shl      di, 6
                     shl      dx, 8
                     add      di, dx
                     add      di, cx
    
                     mov      es:[di], al
    
                     pop      cx
                     pop      dx
                     pop      di
                     pop      ax
                     pop      es
                     ret
put_pixel endp

    ; ==============================================
draw_rect proc
                     pusha
                     mov      bx, cx
    
    rect_y_loop:     
                     mov      cx, bx
                     mov      di, si
    
    rect_x_loop:     
                     call     put_pixel
                     inc      cx
                     dec      di
                     jnz      rect_x_loop
    
                     inc      dx
                     dec      bp
                     jnz      rect_y_loop
    
                     popa
                     ret
draw_rect endp

    ; ==============================================
draw_circle proc
                     pusha
                     mov      byte ptr [color], al
                     mov      center_x, cx
                     mov      center_y, dx
    
                     xor      ax, ax
                     mov      x, ax
                     mov      ax, si
                     mov      y, ax
                     mov      ax, 1
                     sub      ax, si
                     mov      err, ax
    
    circle_main_loop:
                     mov      ax, x
                     cmp      ax, y
                     jg       circle_end
    
                     call     draw_8_points
    
                     mov      ax, err
                     cmp      ax, 0
                     jg       err_positive
    
                     inc      x
                     mov      ax, x
                     shl      ax, 1
                     inc      ax
                     add      err, ax
                     jmp      circle_main_loop
    
    err_positive:    
                     dec      y
                     mov      ax, x
                     sub      ax, y
                     shl      ax, 1
                     inc      ax
                     add      err, ax
                     jmp      circle_main_loop
    
    circle_end:      
                     popa
                     ret
    
    ; Локальные переменные
    color            db       ?
    x                dw       0
    y                dw       0
    err              dw       0
    center_x         dw       ?
                     center_y dw
    
    draw_8_points:   
                     mov      ax, x
                     mov      bx, y
    
    ; Октант 1
                     mov      cx, center_x
                     add      cx, ax
                     mov      dx, center_y
                     sub      dx, bx
                     mov      al, color
                     call     put_pixel
    
    ; Октант 2
                     mov      cx, center_x
                     add      cx, bx
                     mov      dx, center_y
                     sub      dx, ax
                     call     put_pixel
    
    ; Октант 3
                     mov      cx, center_x
                     sub      cx, ax
                     mov      dx, center_y
                     sub      dx, bx
                     call     put_pixel
    
    ; Октант 4
                     mov      cx, center_x
                     sub      cx, bx
                     mov      dx, center_y
                     sub      dx, ax
                     call     put_pixel
    
    ; Октант 5
                     mov      cx, center_x
                     sub      cx, ax
                     mov      dx, center_y
                     add      dx, bx
                     call     put_pixel
    
    ; Октант 6
                     mov      cx, center_x
                     sub      cx, bx
                     mov      dx, center_y
                     add      dx, ax
                     call     put_pixel
    
    ; Октант 7
                     mov      cx, center_x
                     add      cx, ax
                     mov      dx, center_y
                     add      dx, bx
                     call     put_pixel
    
    ; Октант 8
                     mov      cx, center_x
                     add      cx, bx
                     mov      dx, center_y
                     add      dx, ax
                     call     put_pixel
                     ret
draw_circle endp

    ; ==============================================
draw_triangle proc
                     push     bp
                     mov      bp, sp
                     pusha
    
                     mov      tr_color, al
    
    ; Линия 1-2
                     mov      ax, [bp+14]
                     mov      bx, [bp+12]
                     mov      cx, [bp+10]
                     mov      dx, [bp+8]
                     call     draw_line
    
    ; Линия 2-3
                     mov      ax, [bp+10]
                     mov      bx, [bp+8]
                     mov      cx, [bp+6]
                     mov      dx, [bp+4]
                     call     draw_line
    
    ; Линия 3-1
                     mov      ax, [bp+6]
                     mov      bx, [bp+4]
                     mov      cx, [bp+14]
                     mov      dx, [bp+12]
                     call     draw_line
    
                     popa
                     pop      bp
                     ret
    tr_color         db       ?
draw_triangle endp

    ; ==============================================
draw_line proc
                     pusha
                     mov      x1, ax
                     mov      y1, bx
                     mov      x2, cx
                     mov      y2, dx
    
                     mov      cx, x2
                     sub      cx, x1
                     mov      si, cx
    
                     mov      dx, y2
                     sub      dx, y1
                     mov      di, dx
    
                     mov      ax, 1
                     mov      bx, 1
                     cmp      cx, 0
                     jge      dx_ok
                     neg      ax
                     neg      cx
    dx_ok:           
                     cmp      dx, 0
                     jge      dy_ok
                     neg      bx
                     neg      dx
    dy_ok:           
    
                     mov      step_x, ax
                     mov      step_y, bx
    
                     cmp      cx, dx
                     jge      main_loop
                     xchg     cx, dx
                     mov      steep, 1
                     jmp      cont
    main_loop:       
                     mov      steep, 0
    cont:            
    
                     mov      error, 0
                     mov      ax, dx
                     shl      ax, 1
                     mov      delta, ax
    
                     mov      ax, cx
                     shl      ax, 1
                     mov      threshold, ax
    
                     mov      cx, x1
                     mov      dx, y1
                     mov      bx, si
    
    line_loop:       
                     call     plot_pixel
                     mov      ax, error
                     add      ax, delta
                     mov      error, ax
    
                     cmp      ax, threshold
                     jl       no_corr
                     sub      error, threshold
                     add      dx, step_y
    
    no_corr:         
                     add      cx, step_x
                     dec      bx
                     jnz      line_loop
    
                     popa
                     ret
    
    plot_pixel:      
                     cmp      steep, 1
                     je       plot_steep
                     push     cx
                     push     dx
                     mov      cx, x1
                     mov      dx, y1
                     mov      al, tr_color
                     call     put_pixel
                     pop      dx
                     pop      cx
                     ret
    
    plot_steep:      
                     push     cx
                     push     dx
                     mov      cx, dx
                     mov      dx, x1
                     mov      al, tr_color
                     call     put_pixel
                     pop      dx
                     pop      cx
                     ret
    
    ; Локальные переменные
    x1               dw       ?
    y1               dw       ?
    x2               dw       ?
    y2               dw       ?
draw_line endp

end main