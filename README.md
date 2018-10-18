# PokemonBattleEngine

A C# library that can emulate Pokémon battles.

The library comes with a client and server, so it is possible to host a battle server and have clients matchmake into it and battle each other.
The engine only emulates as if it were Pokémon B2W2 versions, so there will not be features included after generation 5.

Includes some "anti-cheating" code that protects information a player should not know.
Information slowly gets revealed to each player over time.
For example, your opponent will not know your ability until your ability does something. He/she will also be unaware of your moves until you use them.
Therefore, a hacked client cannot do anything more than an ordinary player (unless my code has exploits... in that case... create an issue!)

----
# To Do:
* Switching out and multiple pokemon per team
* Accuracy/Evasion math
* Critical hits
* Turn order
* Move targetting
* Double/triple/rotation battles
* Add most moves, items, pokemon, secondary statuses (underwater, cursed, mud sport, etc.)

----
# PokemonBattleEngine Uses:
* [Ether.Network](https://github.com/Eastrall/Ether.Network)