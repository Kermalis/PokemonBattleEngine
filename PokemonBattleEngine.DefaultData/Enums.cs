using System;

namespace Kermalis.PokemonBattleEngine.DefaultData;

[Flags]
public enum PBEDDMoveObtainMethod : ulong
{
	/// <summary>There is no way to learn this move.</summary>
	None,
	/// <summary>The move can be learned by levelling up a Pokémon in Pokémon Ruby Version and Pokémon Sapphire Version.</summary>
	LevelUp_RSColoXD = 1uL << 0,
	/// <summary>The move can be learned by levelling up a Pokémon in Pokémon Fire Red Version.</summary>
	LevelUp_FR = 1uL << 1,
	/// <summary>The move can be learned by levelling up a Pokémon in Pokémon Leaf Green Version.</summary>
	LevelUp_LG = 1uL << 2,
	/// <summary>The move can be learned by levelling up a Pokémon in Pokémon Emerald Version.</summary>
	LevelUp_E = 1uL << 3,
	/// <summary>The move can be learned by levelling up a Pokémon in Pokémon Diamond Version and Pokémon Pearl Version.</summary>
	LevelUp_DP = 1uL << 4,
	/// <summary>The move can be learned by levelling up a Pokémon in Pokémon Platinum Version.</summary>
	LevelUp_Pt = 1uL << 5,
	/// <summary>The move can be learned by levelling up a Pokémon in Pokémon HeartGold Version and Pokémon SoulSilver Version.</summary>
	LevelUp_HGSS = 1uL << 6,
	/// <summary>The move can be learned by levelling up a Pokémon in Pokémon Black Version and Pokémon White Version.</summary>
	LevelUp_BW = 1uL << 7,
	/// <summary>The move can be learned by levelling up a Pokémon in Pokémon Black Version 2 and Pokémon White Version 2.</summary>
	LevelUp_B2W2 = 1uL << 8,
	/// <summary>The move can be learned by using a technical machine on a Pokémon in Pokémon Ruby Version, Pokémon Sapphire Version, Pokémon Fire Red Version, Pokémon Leaf Green Version, Pokémon Emerald Version, Pokémon Colosseum, and Pokémon XD: Gale of Darkness.</summary>
	TM_RSFRLGEColoXD = 1uL << 9,
	/// <summary>The move can be learned by using a technical machine on a Pokémon in Pokémon Diamond Version, Pokémon Pearl Version, and Pokémon Platinum Version.</summary>
	TM_DPPt = 1uL << 10,
	/// <summary>The move can be learned by using a technical machine on a Pokémon in Pokémon HeartGold Version and Pokémon SoulSilver Version.</summary>
	TM_HGSS = 1uL << 11,
	/// <summary>The move can be learned by using a technical machine on a Pokémon in Pokémon Black Version and Pokémon White Version.</summary>
	TM_BW = 1uL << 12,
	/// <summary>The move can be learned by using a technical machine on a Pokémon in Pokémon Black Version 2 and Pokémon White Version 2.</summary>
	TM_B2W2 = 1uL << 13,
	/// <summary>The move can be learned by using a hidden machine on a Pokémon in Pokémon Ruby Version, Pokémon Sapphire Version, Pokémon Fire Red Version, Pokémon Leaf Green Version, Pokémon Emerald Version, Pokémon Colosseum, and Pokémon XD: Gale of Darkness.</summary>
	HM_RSFRLGEColoXD = 1uL << 14,
	/// <summary>The move can be learned by using a hidden machine on a Pokémon in Pokémon Diamond Version, Pokémon Pearl Version, and Pokémon Platinum Version.</summary>
	HM_DPPt = 1uL << 15,
	/// <summary>The move can be learned by using a hidden machine on a Pokémon in Pokémon HeartGold Version and Pokémon SoulSilver Version.</summary>
	HM_HGSS = 1uL << 16,
	/// <summary>The move can be learned by using a hidden machine on a Pokémon in Pokémon Black Version, Pokémon White Version, Pokémon Black Version 2, and Pokémon White Version 2.</summary>
	HM_BWB2W2 = 1uL << 17,
	/// <summary>The move can be taught to a Pokémon by a move tutor in Pokémon Fire Red Version and Pokémon Leaf Green Version.</summary>
	MoveTutor_FRLG = 1uL << 18,
	/// <summary>The move can be taught to a Pokémon by a move tutor in Pokémon Emerald Version.</summary>
	MoveTutor_E = 1uL << 19,
	/// <summary>The move can be taught to a Pokémon by a move tutor in Pokémon XD: Gale of Darkness.</summary>
	MoveTutor_XD = 1uL << 20,
	/// <summary>The move can be taught to a Pokémon by a move tutor in Pokémon Diamond Version and Pokémon Pearl Version.</summary>
	MoveTutor_DP = 1uL << 21,
	/// <summary>The move can be taught to a Pokémon by a move tutor in Pokémon Platinum Version.</summary>
	MoveTutor_Pt = 1uL << 22,
	/// <summary>The move can be taught to a Pokémon by a move tutor in Pokémon HeartGold Version and Pokémon SoulSilver Version.</summary>
	MoveTutor_HGSS = 1uL << 23,
	/// <summary>The move can be taught to a Pokémon by a move tutor in Pokémon Black Version and Pokémon White Version.</summary>
	MoveTutor_BW = 1uL << 24,
	/// <summary>The move can be taught to a Pokémon by a move tutor in Pokémon Black Version 2 and Pokémon White Version 2.</summary>
	MoveTutor_B2W2 = 1uL << 25,
	/// <summary>The move can be learned by hatching a Pokémon egg in Pokémon Ruby Version, Pokémon Sapphire Version, Pokémon Fire Red Version, Pokémon Leaf Green Version, and Pokémon Emerald Version.</summary>
	EggMove_RSFRLGE = 1uL << 26,
	/// <summary>The move can be learned by hatching a Pokémon egg in Pokémon Diamond Version, Pokémon Pearl Version, and Pokémon Platinum Version.</summary>
	EggMove_DPPt = 1uL << 27,
	/// <summary>The move can be learned by hatching a Pokémon egg in Pokémon HeartGold Version and Pokémon SoulSilver Version.</summary>
	EggMove_HGSS = 1uL << 28,
	/// <summary>The move can be learned by hatching a Pokémon egg in Pokémon Black Version, Pokémon White Version, Pokémon Black Version 2, and Pokémon White Version 2.</summary>
	EggMove_BWB2W2 = 1uL << 29,
	/// <summary>The move is known by a Pokémon when found in the Dream World with Pokémon Black Version and Pokémon White Version.</summary>
	DreamWorld_BW = 1uL << 30,
	/// <summary>The move is known by a Pokémon when found in the Dream World with Pokémon Black Version 2 and Pokémon White Version 2.</summary>
	DreamWorld_B2W2 = 1uL << 31,
	/// <summary>The move can be learned by hatching a Pokémon egg under special conditions.</summary>
	EggMove_Special = 1uL << 32,
	/// <summary>The move is learned by a Pokémon when changing forms. The move cannot be used by other forms if this is the only flag or if the species cannot change forms.</summary>
	Form = 1uL << 33
}
