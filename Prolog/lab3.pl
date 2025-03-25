% Предикат для получения множеств от пользователя
get_sets(A, B, C) :-
    write('Пожалуйста, введите элементы множества A (через запятую, натуральные числа): '),
    read_line_to_string(user_input, AString),
    parse_set(AString, A),
    
    write('Пожалуйста, введите элементы множества B (через запятую, натуральные числа): '),
    read_line_to_string(user_input, BString),
    parse_set(BString, B),
    
    write('Пожалуйста, введите элементы множества C (через запятую, натуральные числа): '),
    read_line_to_string(user_input, CString),
    parse_set(CString, C).

% Предикат для разбора строки в множество (список без повторений)
parse_set(String, Set) :-
    split_string(String, ",", " ", ElementStrings),
    maplist(atom_string, Elements, ElementStrings),
    list_to_set(Elements, Set).

% Предикат для демонстрации закона
demonstrate_law(A, B, C) :-
    % Вычисляем левую часть: A ∩ (B ∪ C)
    union(B, C, BUnionC),
    intersection(A, BUnionC, LeftSide),
    
    % Вычисляем правую часть: (A ∩ B) ∪ (A ∩ C)
    intersection(A, B, AIntersectB),
    intersection(A, C, AIntersectC),
    union(AIntersectB, AIntersectC, RightSide),
    
    % Выводим результаты
    writeln('\nРезультаты вычислений:'),
    format('Множество A: ~w~n', [A]),
    format('Множество B: ~w~n', [B]),
    format('Множество C: ~w~n', [C]),
    format('B ∪ C: ~w~n', [BUnionC]),
    format('A ∩ (B ∪ C): ~w~n', [LeftSide]),
    format('A ∩ B: ~w~n', [AIntersectB]),
    format('A ∩ C: ~w~n', [AIntersectC]),
    format('(A ∩ B) ∪ (A ∩ C): ~w~n', [RightSide]),
    
    % Проверяем равенство и выводим заключение
    (LeftSide == RightSide ->
        writeln('\nВывод: A ∩ (B ∪ C) = (A ∩ B) ∪ (A ∩ C)'),
        writeln('Первый дистрибутивный закон доказан для данных множеств!')
    ;
        writeln('\nОшибка в вычислениях: A ∩ (B ∪ C) ≠ (A ∩ B) ∪ (A ∩ C)'),
        writeln('Проверьте правильность реализации операций над множествами.')
    ).

% Реализация операции объединения множеств (без дублирования)
union([], Set, Set).
union([H|T], Set, Result) :-
    member(H, Set),
    !,
    union(T, Set, Result).
union([H|T], Set, [H|Result]) :-
    union(T, Set, Result).

% Реализация операции пересечения множеств
intersection([], _, []).
intersection([H|T], Set, [H|Result]) :-
    member(H, Set),
    !,
    intersection(T, Set, Result).
intersection([_|T], Set, Result) :-
    intersection(T, Set, Result).

% Предикат для чтения числа
read_integer(X) :-
    read_line_to_string(user_input, String),
    number_string(X, String).

main :-
    writeln('Программа доказательства первого дистрибутивного закона'),
    writeln('Этот закон утверждает, что A ∩ (B ∪ C) = (A ∩ B) ∪ (A ∩ C)'), nl,

    get_sets(A, B, C),
    demonstrate_law(A, B, C), nl,

    write('Хотите попробовать еще раз? (1 - да, 0 - нет): '),
    read_integer(Answer),
    (
        Answer = 1 ->
        nl, 
        write(-------------------), nl, nl,
        main
    ;
        writeln('Программа завершена.'), nl
    ).

?- main.