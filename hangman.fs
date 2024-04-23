\ Hangman game for Tali Forth 2
\ benchapman@fastmail.com 
\ Releassed into the Public Domain CC0
\ Example of beginner Forth coding
\ v 0.2 20240423

\ Graphics and wordlist from Github 
\ https://gist.github.com/chrishorton/ hangmanwordbank.py

marker -hangman

3 constant x
3 constant y

: h1
x y      at-xy ." -={ H A N G M A N }=-"
x y 1 +  at-xy ."         +---+" 
x y 2 +  at-xy ."         |   |"
x y 3 +  at-xy ."             |"
x y 4 +  at-xy ."             |"
x y 5 +  at-xy ."             |"
x y 6 +  at-xy ."             |"
x y 7 +  at-xy ."             |"
x y 8 +  at-xy ."             |"
x y 9 +  at-xy ."      ==========";
: h2 x 8 + y 3 + at-xy 'O' emit 0 15 at-xy ; 
: h3 x 8 + y 4 + at-xy '|' emit 0 15 at-xy ;
: h4 x 7 + y 4 + at-xy '/' emit 0 15 at-xy ;
: h5 x 9 + y 4 + at-xy '\' emit 0 15 at-xy ;
: h6 x 8 + y 5 + at-xy '|' emit 0 15 at-xy ;
: h7 x 7 + y 6 + at-xy '/' emit 0 15 at-xy ;
: h8 x 9 + y 6 + at-xy '\' emit 0 15 at-xy ;

: draw_man ( n --)
    case  
        1 of h1 endof
        2 of h2 endof
        3 of h3 endof
        4 of h4 endof
        5 of h5 endof
        6 of h6 endof
        7 of h7 endof
        8 of h8 endof
    endcase ;

: wait 16000 0 do loop ;

: animate ( --)
   page
   9 1 do i draw_man wait loop 
   page 1 draw_man ;

\ What hangman to display?
variable man
: man+ 1 man +! ;
: man1 1 man ! ;
: man@ man @ ;

\ Pipe-delimited array of animals
\ animal names cannot be longer than 10 chars. Final pipe char is required
: "animals" s" 
ant|baboon|badger|bat|bear|beaver|camel|cat|clam|cobra|cougar|coyote|crow|deer|dog|donkey|duck|eagle|ferret|fox|frog|goat|goose|hawk|lion|lizard|llama|mole|monkey|moose|mouse|mule|newt|otter|owl|panda|parrot|pigeon|python|ram|rat|raven|rhino|salmon|seal|shark|sheep|skunk|sloth|snake|spider|stork|swan|tiger|toad|trout|turkey|turtle|weasel|whale|wolf|wombat|zebra|" 
;

0 value chunk
0 value start
0 value end 
'|' constant delim

\ count chunks of text 
: how_many ( addr u -- u) 
    0 -rot bounds do i c@ delim = if 1+ then loop ;

\ find addr and len for specific chunk in source string
\ source string and chunk number to find on stack; chunk is 0-indexed 
: ?animal ( addr u u -- addr u)
    -rot bounds dup to start
    0 to chunk
    do i c@ delim = 
        if 
        i to end
        dup chunk =
            if
                start end start -
                leave \ exit on match
            then
            end 1+ to start
            chunk 1+ to chunk
        then
    loop 
    rot drop
    ;

\ buffer for chosen animal
create animal 10 chars allot 
: animal0 ( --) animal 10 bl fill ;
: animal@ ( -- addr u) animal 10 -trailing ;
: animal_len ( -- u) animal@ swap drop ;

\ buffer for answer
create answer 10 chars allot
: answer0 ( --) answer 10 bl fill ;
: answer@ ( -- addr u) answer 10 -trailing ;


0 value len
\ show all the animals
: review_animals ( --)
  0 to len
  0 to chunk
  cr cr 5 x + spaces
  "animals" bounds do
  i c@ dup delim =
  if
    10 len - spaces drop
    0 to len
    chunk 1+ to chunk
    chunk 3 mod 0= if cr 5 x + spaces then
  else 
    emit 
    len 1+ to len
  then
  loop  
  cr 7 x + spaces ." Hit a key to continue"
  key drop
  ;

\ store string in the animal variable
: set_animal ( addr u --) animal swap move ;

\ Pseudorandom number routine Starting Forth, p. 265
variable rnd here rnd !
: random rnd @ 31421 * 6927 + dup rnd ! ;
: choose ( u -- random 0..u-1 )
    random um* swap drop ; 

\ get lower-case alpha key 
: get_key ( -- k)
    begin
        key dup
        'a' '{' within invert
    while
        drop
    repeat
    ;

create played 30 chars allot
: played0 played 30 bl fill ;

variable played_ptr
: played_ptr0 0 played_ptr ! ;

: add_played ( c -- )
    played played_ptr c@ + c!
    2 played_ptr +!
    ;

: draw_played ( -- )
    x 16 + y 4 + at-xy
    ." Letters played: " 
    x 16 + y 5 + at-xy
    played 30 -trailing type
    x y 13 + at-xy
    ;
    
variable right

\ process character from key
: get_answer ( -- )
    false right !
    get_key ( -- k)
    animal_len 0 
    do dup dup animal i + c@ =
        if 
          answer i + c!
          true right !
        else
          drop
       then 
    loop 
    right @ 0= if man+ then 
    add_played
    ;

\ If answer currently is " oo  ", 
\ and animal is "goose", result is "_ o o _ _"
: draw_answer ( -- )
    x 5 + y 11 + at-xy
    animal_len 0 
    do answer i + c@ dup bl = 
        if '_' emit drop
        else
          emit
        then 
        bl emit
    loop 
    x y 13 + at-xy 
    ;

: init 
    man1 answer0 animal0 played0 played_ptr0 
    "animals" 2dup how_many choose ?animal set_animal 
    page 1 draw_man 
    draw_answer
    ;

: win ( -- f) answer 10 animal 10 compare 0= ;

: lose ( -- f) man@ 8 = ;

0 value games

: play_game
    page
    games 0= if animate else 1 draw_man then
    x 5 + y 13 + at-xy ." Welcome to Hangman!"
    x 5 + y 14 + at-xy ." Guess the animal before you run out of chances."
    x 5 + y 15 + at-xy ." Do you want to see the list of possible animals (y/n)? "
    key 'y' = if review_animals then 
    init
    begin 
        get_answer
        man@ draw_man
        draw_answer 
        draw_played
        lose if
            x 5 + y 13 + at-xy ." Better luck next time!" cr
            x 5 + y 14 + at-xy ." The correct answer was: " animal@ type
            then
        win if
            x 5 + y 13 + at-xy ." YOU WON!!!"
            then
        win lose or
    until
    ;

: again?
  x 5 + y 15 + at-xy ." Play again? "
  key 'y' = 
  ;

: play
    begin
        play_game
        games 1+ to games
        again? 0= 
    until
        x 5 + y 14 + at-xy ." You played " games . ." games during this session."
        x 5 + y 15 + at-xy ." Goodbye! Type 'play' to play again."
        x 5 + y 16 + at-xy ."          or '-hangman' to remove from memory." cr cr

    ;

