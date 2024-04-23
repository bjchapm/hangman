## Hangman for Tali Forth 2

Date: 4/18/24

License: Public Domain CC0

Author: Ben Chapman benchapman@fastmail.com

This is a **basic** Hangman game created by a novice Forth programmer (me).

The wordlist and ASCII-art graphics are from [this
gist](https://gist.github.com/chrishorton/8510732aa9a80a03c829b09f12e20d9c).
Thanks to Chris Horton for these.

The rest is mine, for better or worse. I tried to create a version of Hangman
without looking at any other implementations. This is an attempt to force
myself to learn more Forth. I'm sure that it's not very "Forthy" yet, but it
was an enjoyable challenge.

It's written for [Tali Forth 2](https://github.com/SamCoVT/TaliForth2) for the
65C02 processor and I have successfully run it on my [Planck 65C02
system](https://planck6502.com/ "An open-hardware, extensible 65c02-based
computer"), as well as in the [py65](https://github.com/mnaberez/py65)
simulator on my Mac.  It also runs in [gforth](https://gforth.org/) on my Mac.

### Play notes

There is a hard-coded delay value that controls the speed of the introductory
animation.  See the `wait` word. The current value of 16000 means that the
animation zooms by on the gforth system, but crawls on the py65 one. Adjust as
you see fit (or comment out the `animate` word inside of `play_game`).

If you guess 'o' and there are multiple 'o' letters, each one will be filled in.
In other words, don't guess 'o' again. However, if you repeat a guess of a letter that's
in the answer, the guess won't count against you. A repeated incorrect guess
will count against you.

Thanks to all of the people who create these amazing projects and share them
with the rest of us!


