.model tiny
.stack 100h
.186
.data
    old_handler dd ?
    shift_pressed db 0   ; Флаг нажатия Shift
    remap_enabled db 0   ; Флаг включения/выключения подмены (1 - включено, 0 - выключено)
    ctrl_pressed db 0    ; Флаг нажатия Ctrl
    alt_pressed db 0     ; Флаг нажатия Alt

    symbols db "J", "K", "L", "X", "C", "V", "B", "N", "M"
    symbols_lower db "j", "k", "l", "x", "c", "v", "b", "n", "m"
    symbolsCodes db 32h, 31h, 30h, 2Eh, 2Dh, 2Ch, 26h, 25h, 24h

.code
start proc near
    mov ax, @data
    mov ds, ax

    ; Сохранение старого обработчика
    mov ax, 3509h
    int 21h
    mov word ptr [old_handler], bx
    mov word ptr [old_handler+2], es

    ; Установка нового обработчика
    push ds
    mov ax, 2509h
    mov dx, seg kbh
    mov ds, dx
    mov dx, offset kbh
    int 21h
    pop ds
    call exit
start endp

; Функция поиска символа по коду клавиши
findChar proc
    push si
    xor si, si
findChar_loop:
    cmp al, byte ptr [symbolsCodes + si]
    je findChar_exit
    inc si
    cmp si, 10
    je findChar_notFound
    jmp findChar_loop
findChar_exit:
    cmp dl, 1
    je upper_case
    mov bl, byte ptr [symbols_lower + si]
    jmp findChar_done
upper_case:
    mov bl, byte ptr [symbols + si]
findChar_done:
    pop si
    ret
findChar_notFound:
    xor bx, bx
    pop si
    ret
findChar endp

; Новый обработчик клавиатуры
kbh proc far
    pusha
    push es
    push ds
    push cs
    pop ds

    in al, 60h  ; Читаем код клавиши

    ; Обработка Ctrl
    cmp al, 1Dh
    je ctrl_pressed_set
    cmp al, 9Dh
    je ctrl_pressed_clear

    ; Обработка Alt
    cmp al, 38h
    je alt_pressed_set
    cmp al, 0B8h
    je alt_pressed_clear

    ; Обработка Shift
    cmp al, 2Ah
    je shift_pressed_set
    cmp al, 36h
    je shift_pressed_set
    cmp al, 2Ah + 80h
    je shift_pressed_clear
    cmp al, 36h + 80h
    je shift_pressed_clear

    ; Проверка сочетания Ctrl + Alt + Z
    cmp al, 2Ch
    je toggle_remap

    ; Проверка режима подмены 
    cmp byte ptr [remap_enabled], 0
    je pass_to_old_handler  ; Если подмена отключена, передаем старому обработчику

    ; Проверяем, является ли это отпусканием клавиши
    test al, 80h
    jnz pass_to_old_handler ; Отпускания клавиш передаем старому обработчику

    ; Подмена символов
    mov dl, byte ptr [shift_pressed]
    call findChar

    cmp bl, 0
    je pass_to_old_handler  ; Если символ не подменяется, передаем старому обработчику

    mov ah, 05h
    mov ch, al
    mov cl, bl
    int 16h
    jmp kbh_exit

ctrl_pressed_set:
    mov byte ptr [ctrl_pressed], 1
    jmp check_pass
ctrl_pressed_clear:
    mov byte ptr [ctrl_pressed], 0
    jmp check_pass

alt_pressed_set:
    mov byte ptr [alt_pressed], 1
    jmp check_pass
alt_pressed_clear:
    mov byte ptr [alt_pressed], 0
    jmp check_pass

shift_pressed_set:
    mov byte ptr [shift_pressed], 1
    jmp check_pass
shift_pressed_clear:
    mov byte ptr [shift_pressed], 0
    jmp check_pass

toggle_remap:
    cmp byte ptr [ctrl_pressed], 1
    jne pass_to_old_handler
    cmp byte ptr [alt_pressed], 1
    jne pass_to_old_handler
    xor byte ptr [remap_enabled], 1  ; Инвертируем флаг
    jmp kbh_exit

check_pass:
    cmp byte ptr [remap_enabled], 0
    je pass_to_old_handler  ; Если подмена отключена, передаем событие старому обработчику
    jmp kbh_exit

pass_to_old_handler:
    pop ds
    pop es
    popa
    jmp dword ptr cs:old_handler

kbh_exit:
    pop ds
    pop es
    popa
    mov al, 20h
    out 20h, al
    iret
kbh endp

exit proc
    mov ax, 3100h
    int 21h
exit endp

end start