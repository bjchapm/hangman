\ Hangman game for Tali Forth 2
\ benchapman@fastmail.com 20240416
\ Releassed into the Public Domain CC0
\ Example of beginner Forth coding

\ Graphics and wordlist from Github 
\ https://gist.github.com/chrishorton/ hangmanwordbank.py

marker **hangman**

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

: wait 32000 0 do loop ;

: animate ( --)
   page
   9 1 do i draw_man wait loop 
   page 1 draw_man ;

\ What hangman to display?
variable man
: man+ 1 man +! ;
: man1 1 man ! ;
: man@ man @ ;

\ An array of 10-character wide fields
\ No animals longer than 10 chars!
: "animals" s" ant       baboon    badger    bat       bear      beaver    camel     cat       clam      cobra     cougar    coyote    crow      deer      dog       donkey    duck      eagle     ferret    fox       frog      goat      goose     hawk      lion      lizard    llama     mole      monkey    moose     mouse     mule      newt      otter     owl       panda     parrot    pigeon    python    ram       rat       raven     rhino     salmon    seal      shark     sheep     skunk     sloth     snake     spider    stork     swan      tiger     toad      trout     turkey    turtle    weasel    whale     wolf      wombat    zebra     " ;

\ Get number of animals: length / 10
: how_many ( addr u -- addr u u1) dup 10 / ;

\ buffer for chosen animal
create animal 10 chars allot 
: animal0 ( --) animal 10 bl fill ;
: animal@ ( -- addr u) animal 10 -trailing ;
: animal_len ( -- u) animal@ swap drop ;

\ buffer for answer
create answer 10 chars allot
: answer0 ( --) answer 10 bl fill ;
: answer@ ( -- addr u) answer 10 -trailing ;

\ show all the animals
: review_animals ( --)
  cr
  "animals" bounds do
  cr 15 spaces i 30 type 
  30 +loop 
  cr cr
  15 spaces ." Hit a key to continue"
  key drop
  ;

\ pick a particular animal by index from the super string
\ store it in the animal variable
: set_animal ( addr u u --)
    swap drop 
    10 * + 10 
    animal swap move ;

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
    "animals" how_many choose set_animal 
    page 1 draw_man 
    draw_answer
    ;

: win ( -- f) answer 10 animal 10 compare 0= ;

: lose ( -- f) man@ 8 = ;

: play_game
    page
    animate
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
        again? 0= 
    until
        x 5 + y 15 + at-xy ." Goodbye! Type 'play' to play again."
    ;

