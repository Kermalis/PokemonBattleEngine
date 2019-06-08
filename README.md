# PokemonBattleEngine

Pokémon Battle Engine is a C# library that can emulate Pokémon battles.
This repository includes the engine library, a Windows/Linux/MacOS/Android/iOS client, a server, a Discord bot, and some extra code to rip data from the official games.
The engine only emulates Pokémon B2W2 versions, so nothing introduced after generation 5 is included and neither are generation 1/2 exclusive items, event Pokémon, and move compatibility.

![Client Preview](Client Preview.gif)

The engine does not send information a player should not know; information only gets sent to each player/spectator when it is revealed.
For example, a client has no way of knowing if the opponent has sent out a Pokémon with Illusion, the opponent's nature, stats, Hidden Power damage/type, unused item/moves, etc.
Therefore, a custom/modified client cannot do anything more than an ordinary player unless it is hosting the battle. A lot of work has been done to prevent any cheating, so if there are any exploits, create an issue please!

There is also [a settings file](PokemonBattleEngine/Data/Settings.cs) that you can change. Have you ever wanted to have level 200 Pokémon or 8 moves? You can do that!

Currently, you need to change the settings to be identical for the server and all connected clients for the server to accept parties/actions and for the clients to communicate without crashing.
This will change in the future as the server will send the settings to all connected clients.

----
# To Do:
* Triple-battle shifting and Rotation-battles
* Complete [the Pokémon data dumper](PokemonBattleEngineTesting/PokemonDataDumper.cs)
* Separate forme-specific moves in Pokémon data and instead have legality checker do some work
* Add lots of items, moves, and volatile statuses (taunt, torment, etc. are volatile statuses)
* Finish adding all event Pokémon
* Add previews of Discord and Android/iOS

----
# PokemonBattleEngine Uses:
* [My fork of Ether.Network](https://github.com/Kermalis/Ether.Network)
* [Microsoft.Data.Sqlite](https://docs.microsoft.com/en-us/ef/core)
* [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
* [SQLitePCLRaw](https://github.com/ericsink/SQLitePCL.raw)

# PokemonBattleEngineClient Uses:
* [Avalonia](https://github.com/AvaloniaUI/Avalonia)
* [AvaloniaGif](https://github.com/jmacato/AvaloniaGif)

# PokemonBattleEngineDiscord Uses:
* [Discord.Net](https://github.com/RogueException/Discord.Net)

# PokemonBattleEngineExtras Uses:
* [EndianBinaryIO](https://github.com/Kermalis/EndianBinaryIO)