.MODEL SMALL
.STACK 100H
.DATA

.CODE
MAIN PROC
    ; Инициализация графического режима 13h (320x200, 256 цветов)
                     MOV  AH, 00H
                     MOV  AL, 13H
                     INT  10H

    ; Координаты верхнего левого угла
                     MOV  CX, 50              ; X-координата
                     MOV  DX, 50              ; Y-координата
                     MOV  SI, 100             ; Ширина
                     MOV  DI, 50              ; Высота
                     MOV  AL, 15              ; Цвет (белый)

    DRAW_RECTANGLE:  
                     PUSH CX
                     PUSH DX
                     MOV  BX, SI              ; Счетчик ширины

    ; Рисуем верхнюю и нижнюю границы
                     MOV  DX, 50              ; Верхняя граница
    DRAW_TOP:        
                     MOV  AH, 0CH             ; Функция установки пикселя
                     INT  10H
                     INC  CX
                     DEC  BX
                     JNZ  DRAW_TOP

                     MOV  CX, 50
                     MOV  DX, 99              ; Нижняя граница
                     MOV  BX, SI
    DRAW_BOTTOM:     
                     MOV  AH, 0CH
                     INT  10H
                     INC  CX
                     DEC  BX
                     JNZ  DRAW_BOTTOM

    ; Рисуем боковые границы
                     MOV  CX, 50
                     MOV  DX, 50
                     MOV  BX, DI              ; Счетчик высоты
    DRAW_LEFT:       
                     MOV  AH, 0CH
                     INT  10H
                     INC  DX
                     DEC  BX
                     JNZ  DRAW_LEFT

                     MOV  CX, 149             ; Правая граница
                     MOV  DX, 50
                     MOV  BX, DI
    DRAW_RIGHT:      
                     MOV  AH, 0CH
                     INT  10H
                     INC  DX
                     DEC  BX
                     JNZ  DRAW_RIGHT

    ; Рисуем диагонали из нижнего левого угла
                     MOV  CX, 50              ; Начальная X-координата
                     MOV  DX, 99              ; Начальная Y-координата

    ; Диагональ 15 градусов
                     MOV  BX, 50
    DRAW_DIAGONAL_15:
                     CMP  CX, 149             ; Проверка выхода за границы
                     JGE  END_DIAGONAL_15
                     CMP  DX, 50
                     JLE  END_DIAGONAL_15
                     MOV  AH, 0CH
                     INT  10H
                     INC  CX                  ; X увеличивается медленно
                     INC  CX
                     INC  CX
                     DEC  DX                  ; Y уменьшается быстрее
                     DEC  BX
                     JNZ  DRAW_DIAGONAL_15
    END_DIAGONAL_15: 

    ; Диагональ 30 градусов
                     MOV  CX, 50
                     MOV  DX, 99
                     MOV  BX, 50
    DRAW_DIAGONAL_30:
                     CMP  CX, 149             ; Проверка выхода за границы
                     JGE  END_DIAGONAL_30
                     CMP  DX, 50
                     JLE  END_DIAGONAL_30
                     MOV  AH, 0CH
                     INT  10H
                     INC  CX                  ; Угол 30 градусов - X меняется быстрее
                     INC  CX
                     DEC  DX                  ; Y уменьшается
                     DEC  BX
                     JNZ  DRAW_DIAGONAL_30
    END_DIAGONAL_30: 

    ; Диагональ 45 градусов
                     MOV  CX, 50
                     MOV  DX, 99
                     MOV  BX, 50
    DRAW_DIAGONAL_45:
                     CMP  CX, 149
                     JGE  END_DIAGONAL_45
                     CMP  DX, 50
                     JLE  END_DIAGONAL_45
                     MOV  AH, 0CH
                     INT  10H
                     INC  CX                  ; Угол 45 градусов - X и Y меняются равномерно
                     DEC  DX
                     DEC  BX
                     JNZ  DRAW_DIAGONAL_45
    END_DIAGONAL_45: 

    ; Диагональ 60 градусов
                     MOV  CX, 50
                     MOV  DX, 99
                     MOV  BX, 50
    DRAW_DIAGONAL_60:
                     CMP  CX, 149
                     JGE  END_DIAGONAL_60
                     CMP  DX, 50
                     JLE  END_DIAGONAL_60
                     MOV  AH, 0CH
                     INT  10H
                     INC  CX                  ; Угол 60 градусов - X меняется медленнее
                     DEC  DX
                     DEC  DX
                     DEC  BX
                     JNZ  DRAW_DIAGONAL_60
    END_DIAGONAL_60: 

                     POP  DX
                     POP  CX

    ; Ожидание нажатия клавиши
                     MOV  AH, 00H
                     INT  16H

    ; Возврат в текстовый режим (03h)
                     MOV  AH, 00H
                     MOV  AL, 03H
                     INT  10H

    ; Завершение программы
                     MOV  AH, 4CH
                     INT  21H
MAIN ENDP
END MAIN
