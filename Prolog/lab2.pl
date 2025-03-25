% Предикат для поиска минимального нечётного числа в списке
min_odd_in_list(L, Min) :-
    findall(X, (member(X, L), X mod 2 =\= 0), OddNumbers),
    OddNumbers \= [],
    min_list(OddNumbers, Min).

% Предикат для проверки, есть ли нечётные числа в списке
has_odd_numbers(L) :-
    member(X, L),
    X mod 2 =\= 0.

% Предикат для чтения числа
read_integer(X) :-
    read_line_to_string(user_input, String),
    number_string(X, String).

% Предикат для чтения списка чисел
read_list(L) :-
    write('Введите количество элементов в списке (натуральным числом): '),
    read_integer(N), nl,
    write('Введите '), write(N), write(' наутальных чисел, по одному в строке:'), nl,
    read_list_elements(N, L).

% Предикат для чтения элементов списка
read_list_elements(0, []) :- !.
read_list_elements(N, [X|Xs]) :-
    N > 0,
    read_integer(X),
    N1 is N - 1,
    read_list_elements(N1, Xs).

main :-
    write('Программа поиска минимального нечётного числа в списке'), nl,
    read_list(L),

    (has_odd_numbers(L) ->
        min_odd_in_list(L, Min), nl,
        write('Минимальное нечётное число в списке: '), write(Min), nl, nl
    ;
        nl, write('В списке нет нечётных чисел!'), nl, nl
    ),

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