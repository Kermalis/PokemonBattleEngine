using Discord;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngineDiscord;

internal sealed partial class BattleContext
{
	private const string Separator = "**--------------------**";
	private static readonly Emoji _shinyEmoji = new("✨");
	private static readonly Emoji _switchEmoji = new("😼");
	private static readonly Emoji _confirmationEmoji = new("👍");
	private static readonly Dictionary<PBEType, Emote>[] _moveEmotes = new Dictionary<PBEType, Emote>[PBESettings.DefaultNumMoves]
	{
		new Dictionary<PBEType, Emote>
		{
			{ PBEType.None, Emote.Parse("<:Normal1:708768399538520095>") },
			{ PBEType.Bug, Emote.Parse("<:Bug1:708768297889300503>") },
			{ PBEType.Dark, Emote.Parse("<:Dark1:708768298732355667>") },
			{ PBEType.Dragon, Emote.Parse("<:Dragon1:708768299235672084>") },
			{ PBEType.Electric, Emote.Parse("<:Electric1:708768298103341073>") },
			{ PBEType.Fighting, Emote.Parse("<:Fighting1:708768297457549472>") },
			{ PBEType.Fire, Emote.Parse("<:Fire1:708768298782818394>") },
			{ PBEType.Flying, Emote.Parse("<:Flying1:708768298841669713>") },
			{ PBEType.Ghost, Emote.Parse("<:Ghost1:708768298598400081>") },
			{ PBEType.Grass, Emote.Parse("<:Grass1:708768299219025950>") },
			{ PBEType.Ground, Emote.Parse("<:Ground1:708768298317119548>") },
			{ PBEType.Ice, Emote.Parse("<:Ice1:708768397214744627>") },
			{ PBEType.Normal, Emote.Parse("<:Normal1:708768399538520095>") },
			{ PBEType.Poison, Emote.Parse("<:Poison1:708768399450308680>") },
			{ PBEType.Psychic, Emote.Parse("<:Psychic1:708768399915876462>") },
			{ PBEType.Rock, Emote.Parse("<:Rock1:708768399836315670>") },
			{ PBEType.Steel, Emote.Parse("<:Steel1:708768399400108083>") },
			{ PBEType.Water, Emote.Parse("<:Water1:708768400389963877>") }
		},
		new Dictionary<PBEType, Emote>
		{
			{ PBEType.None, Emote.Parse("<:Normal2:708768399496314880>") },
			{ PBEType.Bug, Emote.Parse("<:Bug2:708768298665246791>") },
			{ PBEType.Dark, Emote.Parse("<:Dark2:708768298992533586>") },
			{ PBEType.Dragon, Emote.Parse("<:Dragon2:708768298883350570>") },
			{ PBEType.Electric, Emote.Parse("<:Electric2:708768297960603708>") },
			{ PBEType.Fighting, Emote.Parse("<:Fighting2:708768297654681620>") },
			{ PBEType.Fire, Emote.Parse("<:Fire2:708768298870767616>") },
			{ PBEType.Flying, Emote.Parse("<:Flying2:708768298921099325>") },
			{ PBEType.Ghost, Emote.Parse("<:Ghost2:708768299072356402>") },
			{ PBEType.Grass, Emote.Parse("<:Grass2:708768298900127845>") },
			{ PBEType.Ground, Emote.Parse("<:Ground2:708768298338353232>") },
			{ PBEType.Ice, Emote.Parse("<:Ice2:708768397336510534>") },
			{ PBEType.Normal, Emote.Parse("<:Normal2:708768399496314880>") },
			{ PBEType.Poison, Emote.Parse("<:Poison2:708768399647440907>") },
			{ PBEType.Psychic, Emote.Parse("<:Psychic2:708768399441788938>") },
			{ PBEType.Rock, Emote.Parse("<:Rock2:708768399442051114>") },
			{ PBEType.Steel, Emote.Parse("<:Steel2:708768399873933325>") },
			{ PBEType.Water, Emote.Parse("<:Water2:708768398829682759>") }
		},
		new Dictionary<PBEType, Emote>
		{
			{ PBEType.None, Emote.Parse("<:Normal3:708768399404302426>") },
			{ PBEType.Bug, Emote.Parse("<:Bug3:708768298958979233>") },
			{ PBEType.Dark, Emote.Parse("<:Dark3:708768298690674739>") },
			{ PBEType.Dragon, Emote.Parse("<:Dragon3:708768300045434911>") },
			{ PBEType.Electric, Emote.Parse("<:Electric3:708768298057203823>") },
			{ PBEType.Fighting, Emote.Parse("<:Fighting3:708768297679847474>") },
			{ PBEType.Fire, Emote.Parse("<:Fire3:708768298744938546>") },
			{ PBEType.Flying, Emote.Parse("<:Flying3:708768298480828557>") },
			{ PBEType.Ghost, Emote.Parse("<:Ghost3:708768298845601822>") },
			{ PBEType.Grass, Emote.Parse("<:Grass3:708768298858184785>") },
			{ PBEType.Ground, Emote.Parse("<:Ground3:708768298312925244>") },
			{ PBEType.Ice, Emote.Parse("<:Ice3:708768396644450355>") },
			{ PBEType.Normal, Emote.Parse("<:Normal3:708768399404302426>") },
			{ PBEType.Poison, Emote.Parse("<:Poison3:708768399681126420>") },
			{ PBEType.Psychic, Emote.Parse("<:Psychic3:708768401123836015>") },
			{ PBEType.Rock, Emote.Parse("<:Rock3:708768399274016838>") },
			{ PBEType.Steel, Emote.Parse("<:Steel3:708768399383330836>") },
			{ PBEType.Water, Emote.Parse("<:Water3:708768399936978977>") }
		},
		new Dictionary<PBEType, Emote>
		{
			{ PBEType.None, Emote.Parse("<:Normal4:708768399332999240>") },
			{ PBEType.Bug, Emote.Parse("<:Bug4:708768298883612792>") },
			{ PBEType.Dark, Emote.Parse("<:Dark4:708768298665508906>") },
			{ PBEType.Dragon, Emote.Parse("<:Dragon4:708768298627498066>") },
			{ PBEType.Electric, Emote.Parse("<:Electric4:708768297918660698>") },
			{ PBEType.Fighting, Emote.Parse("<:Fighting4:708768297532915785>") },
			{ PBEType.Fire, Emote.Parse("<:Fire4:708768298380034152>") },
			{ PBEType.Flying, Emote.Parse("<:Flying4:708768298795270245>") },
			{ PBEType.Ghost, Emote.Parse("<:Ghost4:708768298841669672>") },
			{ PBEType.Grass, Emote.Parse("<:Grass4:708768298875093084>") },
			{ PBEType.Ground, Emote.Parse("<:Ground4:708768298468114512>") },
			{ PBEType.Ice, Emote.Parse("<:Ice4:708768398821163009>") },
			{ PBEType.Normal, Emote.Parse("<:Normal4:708768399332999240>") },
			{ PBEType.Poison, Emote.Parse("<:Poison4:708768400020602910>") },
			{ PBEType.Psychic, Emote.Parse("<:Psychic4:708768399328673803>") },
			{ PBEType.Rock, Emote.Parse("<:Rock4:708768399345451009>") },
			{ PBEType.Steel, Emote.Parse("<:Steel4:708768399161032735>") },
			{ PBEType.Water, Emote.Parse("<:Water4:708768398691139656>") }
		}
	};
}
