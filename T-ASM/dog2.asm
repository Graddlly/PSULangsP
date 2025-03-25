.MODEL SMALL
.STACK 100h
.DATA
.CODE
START:
    ; Инициализация сегментов
    MOV AX, @DATA
    MOV DS, AX

    ; Переход в графический режим 13h
    MOV AH, 0
    MOV AL, 13h
    INT 10h

    ; Рисуем голову (круг)
    MOV CX, 160
    MOV DX, 100
    MOV SI, 50     ; Радиус
    MOV AL, 14     ; Цвет (желтый)
    CALL DrawCircle

    ; Рисуем глаза (маленькие круги)
    ; Левый глаз
    MOV CX, 140
    MOV DX, 80
    MOV SI, 5
    MOV AL, 0      ; Цвет (черный)
    CALL DrawCircle

    ; Правый глаз
    MOV CX, 180
    MOV DX, 80
    MOV SI, 5
    MOV AL, 0
    CALL DrawCircle

    ; Ожидание нажатия клавиши
    MOV AH, 0
    INT 16h

    ; Возврат в текстовый режим
    MOV AH, 0
    MOV AL, 03h
    INT 10h

    ; Выход из программы
    MOV AH, 4Ch
    INT 21h

; Процедуры рисования

; PutPixel
PutPixel PROC
    ; Входные данные: CX - X, DX - Y, AL - цвет
    MOV AH, 0Ch
    MOV BH, 0
    INT 10h
    RET
PutPixel ENDP

; DrawLine
; DrawCircle (используем упрощённую версию)
DrawCircle PROC
    ; Входные данные: CX - X центра, DX - Y центра, SI - радиус, AL - цвет
    PUSH BP
    PUSH DI
    PUSH BX

    MOV BP, SI    ; r
    MOV AX, 0     ; x = 0
    MOV DI, BP    ; y = r
    MOV D, 3 - (BP * 2)  ; d = 3 - 2r

CircleLoop:
    ; Рисуем точки в 8 октантах
    MOV BX, CX
    ADD BX, AX
    MOV DX, DX
    ADD DX, DI
    CALL PutPixel

    MOV BX, CX
    SUB BX, AX
    MOV DX, DX
    ADD DX, DI
    CALL PutPixel

    MOV BX, CX
    ADD BX, AX
    MOV DX, DX
    SUB DX, DI
    CALL PutPixel

    MOV BX, CX
    SUB BX, AX
    MOV DX, DX
    SUB DX, DI
    CALL PutPixel

    MOV BX, CX
    ADD BX, DI
    MOV DX, DX
    ADD DX, AX
    CALL PutPixel

    MOV BX, CX
    SUB BX, DI
    MOV DX, DX
    ADD DX, AX
    CALL PutPixel

    MOV BX, CX
    ADD BX, DI
    MOV DX, DX
    SUB DX, AX
    CALL PutPixel

    MOV BX, CX
    SUB BX, DI
    MOV DX, DX
    SUB DX, AX
    CALL PutPixel

    ; Проверяем и обновляем d, x, y
    INC AX
    CMP D, 0
    JL Skip
        DEC DI
        ADD D, 4 * (AX - DI) + 10
        JMP ContinueLoop
Skip:
    ADD D, 4 * AX + 6
ContinueLoop:
    CMP AX, DI
    JLE CircleLoop

    POP BX
    POP DI
    POP BP
    RET
DrawCircle ENDP

END START
