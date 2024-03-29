﻿namespace Kermalis.PokemonBattleEngine.Data;

public interface IPBEItemData
{
	/// <summary>The power <see cref="PBEMoveEffect.Fling"/> has when the user is holding this item. 0 will cause the move to fail.</summary>
	byte FlingPower { get; }
}
public interface IPBEBerryData
{
	byte Bitterness { get; }
	byte Dryness { get; }
	byte Sourness { get; }
	byte Spicyness { get; }
	byte Sweetness { get; }
	/// <summary>The power <see cref="PBEMoveEffect.NaturalGift"/> has when the user is holding this item.</summary>
	byte NaturalGiftPower { get; }
	/// <summary>The type <see cref="PBEMoveEffect.NaturalGift"/> becomes when the user is holding this item.</summary>
	PBEType NaturalGiftType { get; }
}
