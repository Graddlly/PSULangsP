% Предикат для получения первой цифры числа
first_digit(N, D) :-
    N >= 10,
    N1 is N // 10,
    first_digit(N1, D).
first_digit(N, N) :-
    N < 10.

% Предикат для получения последней цифры числа
last_digit(N, D) :- D is N mod 10.

% Предикат для чтения числа
read_integer(X) :-
    read_line_to_string(user_input, String),
    number_string(X, String).

main :-
    writeln('Программа нахождения суммы первой и последней цифры числа'),
    write('Пожалуйста, введите натуральное число: '),
    read_integer(Number), nl,

    first_digit(Number, First),
    last_digit(Number, Last),
    Sum is First + Last,

    format('Первая цифра числа ~w: ~w~n', [Number, First]),
    format('Последняя цифра числа ~w: ~w~n', [Number, Last]),
    format('Сумма первой и последней цифры: ~w~n', [Sum]), nl,

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
