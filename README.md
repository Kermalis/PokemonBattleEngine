# PokemonBattleEngine

Pokémon Battle Engine is a C# library that can emulate Pokémon battles.
This repository includes the engine library, a Windows/Linux/MacOS/Android/iOS client, a server, a Discord bot, and some extra code to rip data from the official games.
The engine only emulates Pokémon B2W2 versions, so nothing introduced after generation 5 is included and neither are generation 1/2 exclusive items, event Pokémon, and move compatibility.

![ClientPreview](Client%20Preview.gif)

The engine does not send information a player should not know; information only gets sent to each player/spectator when it is revealed.
For example, a client has no way of knowing if the opponent has sent out a Pokémon with Illusion, the opponent's nature, stats, Hidden Power damage/type, unused item/moves, etc.
Therefore, a custom/modified client cannot do anything more than an ordinary player unless it is hosting the battle. A lot of work has been done to prevent any cheating.

Join our _(new-ish)_ [Discord server](https://discord.gg/Z4Mn9qX) to talk or try out the battle bot!

![DiscordPreview](Discord%20Preview.png)

## Other Features:
* There are [settings](PokemonBattleEngine/Data/Settings.cs) that you can change, such as having more moves, a higher maximum level, or weaker poison.
* There are helper classes to build legal Pokémon, as well as require legality for a battle.
* Alternatively, you can represent the Pokémon in any way you wish, as long as you have the basic info to start the battle. This works well with custom games.
* You can save battle replays to watch them back in the client or to train a neural network.
* There is a work-in-progress random team generator inspired by [Pokémon Showdown](https://github.com/smogon/pokemon-showdown)'s which aims to work well with custom settings and moves.
* The library has classes which automatically use the correct language from the games, meaning you can see the Pokémon's names, items, etc in your language, as long as your language was one supported by Pokémon B2W2.
* Multi-Battles exist, but they can be customized to work in 2v1, 2v2, 3v1, or 3v3 battles.

----
## To Do:
* Triple-battle shifting, Wild-battles, Rotation-battles, and "Bag" items
* Add the remaining vanilla [abilities](To%20Do%20Abilities.txt), [held items](To%20Do%20Items.txt), and [moves](To%20Do%20Moves.txt)
* Finish adding all event Pokémon
* Add previews of the Android/iOS apps

----
## PokemonBattleEngine Is Used By:
* [PokemonGameEngine](https://github.com/Kermalis/PokemonGameEngine)

----
## PokemonBattleEngine Uses:
* [EndianBinaryIO](https://github.com/Kermalis/EndianBinaryIO)
* [Microsoft.Data.Sqlite](https://docs.microsoft.com/en-us/ef/core)
* [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
* [SQLitePCLRaw](https://github.com/ericsink/SQLitePCL.raw)
* [TCP networking insprired by EasyTcp](https://github.com/Job79/EasyTcp)

## PokemonBattleEngineClient Uses:
* [Avalonia](https://github.com/AvaloniaUI/Avalonia)
* [AvaloniaGif](https://github.com/jmacato/AvaloniaGif)

## PokemonBattleEngineDiscord Uses:
* [Discord.Net](https://github.com/RogueException/Discord.Net)

## PokemonBattleEngineExtras Uses:
* [SimpleNARC](https://github.com/Kermalis/SimpleNARC)

## PokemonBattleEngineTests Uses:
* [xUnit.net](https://github.com/xunit/xunit)