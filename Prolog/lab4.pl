% Определение боксеров и их имен
boxers([th, jt, fj, hf]).
name(th, 'Томас Герберт').
name(jt, 'Джеймс Томас').
name(fj, 'Френсис Джеймс').
name(hf, 'Герберт Френсис').

% Отношения силы
stronger(hf, th).  % Герберт Френсис > Томас Герберт
stronger(fj, jt).  % Френсис Джеймс > Джеймс Томас
stronger(fj, th).  % Френсис Джеймс > Томас Герберт
stronger(jt, th).  % Джеймс Томас > Томас Герберт
stronger(hf, fj).  % Герберт Френсис > Френсис Джеймс

% Удаление элемента из списка
remove(X, [X|T], T).
remove(X, [H|T], [H|R]) :- remove(X, T, R).

% Генерация перестановок
permutation([], []).
permutation(L, [H|T]) :- remove(H, L, R), permutation(R, T).

% Нахождение позиции элемента в списке
position(X, [X|_], 0).
position(X, [_|T], N) :- position(X, T, M), N is M + 1.

% Проверка всех отношений stronger
check_stronger(Order) :-
    \+ (stronger(A, B), position(B, Order, IdxB), position(A, Order, IdxA), IdxB >= IdxA).

% Нахождение порядка от слабейшего к сильнейшему
ordering(Order) :-
    boxers(Boxers),
    permutation(Boxers, Order),
    check_stronger(Order).

% Преобразование списка инициалов в имена
names([], []).
names([H|T], [Name|Names]) :- name(H, Name), names(T, Names).

% Вывод порядка
print_order(Order) :-
    names(Order, Names),
    atomic_list_concat(Names, ', ', Str),
    write('От слабейшего к сильнейшему: '), write(Str), nl.

main :-
    write('Программа для определения порядка боксеров от слабейшего к сильнейшему.'), nl,

    write('Анализирую условие задачи...'), nl,
    write('В задаче участвуют четыре боксера: Томас Герберт, Герберт Френсис, Френсис Джеймс и Джеймс Томас.'), nl,
    write('Условия о силе боксеров:'), nl,
    write('- Герберт намного сильнее Томаса'), nl,
    write('- Френсис сильнее и Томаса, и Герберта'), nl,
    write('- Герберт слабее Джеймса, но сильнее Френсиса'), nl, nl,

    write('На основе заданных условий...'), nl,
    once(ordering(Order)),
    print_order(Order), nl,
    writeln('Программа завершена.'), nl.

?- main.