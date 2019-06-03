# PokemonBattleEngine

A C# library that can emulate Pokémon battles.

![Preview](Preview.gif)

The library comes with a desktop client, mobile app, server, and Discord bot.
The engine only emulates as if it were Pokémon B2W2 versions, so there will not be features introduced after generation 5.

The library does not send information a player should not know; information slowly gets revealed to each player over time.
For example, your opponent will not know your ability until your ability does something. The client has no way of telling if the opponent has a Pokémon out with Illusion.
The opponent will never know your nature or hidden power damage/type unless they use context clues as a human.
Therefore, a modified client cannot do anything more than an ordinary player (unless my code has exploits... in that case... create an issue!)

There is a settings file that you can change for custom battles. Ever wanted to have level 200 Pokémon? Or 8 moves? You can do that!
Check [Settings.cs](PokemonBattleEngine/Data/Settings.cs)

----
# To Do:
* Rotation battles and triple battle shifting
* Complete [Pokémon data dumper](PokemonBattleEngineTesting/PokemonDataDumper.cs)
* Separate forme-specific moves in PokemonData and instead have legality checker do some work
* Add lots of items, moves, and secondary statuses (taunt, torment, etc.)
* Protect from corrupt packets (if they are even a problem)
* Mobile HP bars

----
# PokemonBattleEngine Uses:
* [My fork of Ether.Network](https://github.com/Kermalis/Ether.Network)
* [Microsoft.Data.Sqlite](https://docs.microsoft.com/en-us/ef/core)
* [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
* [SQLitePCLRaw](https://github.com/ericsink/SQLitePCL.raw)

# PokemonBattleEngineClient Uses:
* [Avalonia](https://github.com/AvaloniaUI/Avalonia)
* [AvaloniaGif](https://github.com/jmacato/AvaloniaGif)
* [pokecheck.org sprites](http://sprites.pokecheck.org)

# PokemonBattleEngineDiscord Uses:
* [Discord.Net](https://github.com/RogueException/Discord.Net)

# PokemonBattleEngineMobile Uses:
* [Xamarin.Forms](https://github.com/xamarin/Xamarin.Forms)
* [FFImageLoading](https://github.com/luberda-molinet/FFImageLoading)

# PokemonBattleEngineTesting Uses:
* [EndianBinaryIO](https://github.com/Kermalis/EndianBinaryIO)