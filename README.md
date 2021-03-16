# GravityBalls

Balls spawn every 0,25 seconds.
Every ball has gravity field.
On collision balls merges into one preserving surface area, mass and gravity field sum.
If a ball has mass of 50 initial balls - it splits into smaller balls and these balls goes with disabled collision for 0,5 second in random directions.
Balls stop spawning when balls count exceeds 250..
Also after exceeding 250 count value gravity inverts. (Balls still receive collisions)
Balls are not affected by any gravity field.
Balls have non zero drag.

Controlls - simple camera controller from URP sample scene - wasd(sides) + qe(up-down) + shift(speedUp)
