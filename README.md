# PokemonBattleEngine

A C# library that can emulate Pokémon battles.

![Preview](Preview.gif)

The library comes with a desktop client, server and Discord bot.
The engine only emulates as if it were Pokémon B2W2 versions, so there will not be features introduced after generation 5.

The library does not send information a player should not know; information slowly gets revealed to each player over time.
For example, your opponent will not know your ability until your ability does something. He/she will also be unaware of your moves until you use them.
The opponent will never know your nature or hidden power damage/type unless they use context clues as a human.
Therefore, a modified client cannot do anything more than an ordinary player (unless my code has exploits... in that case... create an issue!)

There is a settings file that you can change for custom battles. Ever wanted to have level 200 Pokémon? Or 8 moves? You can do that!
Check [Settings.cs](PokemonBattleEngine/Data/Settings.cs)

----
# To Do:
* Struggle
* Rotation battles, triple battle shifting, triple battle auto-center
* Add most moves, items, Pokémon, secondary statuses (underwater, cursed, mud sport, etc.)
* Add timeouts for waiting for a client. A modified client can remove response packets to troll and the server will currently wait infinitely
* Spectators
* Specific place for event Pokémon shells
* Better validation for Pokémon (programmatically deduce how the Pokémon was obtained, make MinLevel relevant)
* Find out how locked moves behave when PP runs out
* Damage, effectiveness and critical packets should include a list of hit Pokémon
* Switch-in effects and turn ended effects should go by speed (with speed ties and everything like turn order)
* Winning
* Watchable replays
* Change all ability/item boosts to settings instead of constants.

----
# PokemonBattleEngine Uses:
* [My fork of Ether.Network](https://github.com/Kermalis/Ether.Network)

# PokemonBattleEngineClient Uses:
* [Avalonia](https://github.com/AvaloniaUI/Avalonia)
* [AvaloniaGif](https://github.com/jmacato/AvaloniaGif)
* [pokecheck.org ripped sprites](http://sprites.pokecheck.org)

# PokemonBattleEngineDiscord Uses:
* [Discord.Net](https://github.com/RogueException/Discord.Net)