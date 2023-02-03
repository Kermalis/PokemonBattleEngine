using Avalonia.Media;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.DefaultData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;

namespace Kermalis.PokemonBattleEngineClient.Models;

public sealed class MoveInfo
{
	private static Dictionary<PBEType, (SolidColorBrush Brush, SolidColorBrush BorderBrush)> _typeToBrush = null!;
	internal static void CreateBrushes()
	{
		_typeToBrush = new Dictionary<PBEType, (SolidColorBrush Brush, SolidColorBrush BorderBrush)>
		{
			{ PBEType.None, (new SolidColorBrush(Color.FromRgb(146, 154, 156)), new SolidColorBrush(Color.FromRgb(83, 89, 88))) },
			{ PBEType.Bug, (new SolidColorBrush(Color.FromRgb(162, 212, 56)), new SolidColorBrush(Color.FromRgb(87, 127, 12))) },
			{ PBEType.Dark, (new SolidColorBrush(Color.FromRgb(106, 122, 156)), new SolidColorBrush(Color.FromRgb(64, 79, 109))) },
			{ PBEType.Dragon, (new SolidColorBrush(Color.FromRgb(80, 136, 188)), new SolidColorBrush(Color.FromRgb(6, 83, 137))) },
			{ PBEType.Electric, (new SolidColorBrush(Color.FromRgb(246, 216, 48)), new SolidColorBrush(Color.FromRgb(173, 148, 24))) },
			{ PBEType.Fighting, (new SolidColorBrush(Color.FromRgb(244, 100, 138)), new SolidColorBrush(Color.FromRgb(153, 62, 86))) },
			{ PBEType.Fire, (new SolidColorBrush(Color.FromRgb(255, 152, 56)), new SolidColorBrush(Color.FromRgb(196, 86, 13))) },
			{ PBEType.Flying, (new SolidColorBrush(Color.FromRgb(80, 124, 212)), new SolidColorBrush(Color.FromRgb(36, 75, 153))) },
			{ PBEType.Ghost, (new SolidColorBrush(Color.FromRgb(94, 100, 208)), new SolidColorBrush(Color.FromRgb(49, 56, 137))) },
			{ PBEType.Grass, (new SolidColorBrush(Color.FromRgb(64, 208, 112)), new SolidColorBrush(Color.FromRgb(27, 135, 63))) },
			{ PBEType.Ground, (new SolidColorBrush(Color.FromRgb(232, 130, 68)), new SolidColorBrush(Color.FromRgb(150, 83, 45))) },
			{ PBEType.Ice, (new SolidColorBrush(Color.FromRgb(98, 204, 212)), new SolidColorBrush(Color.FromRgb(52, 128, 145))) },
			{ PBEType.Normal, (new SolidColorBrush(Color.FromRgb(146, 154, 156)), new SolidColorBrush(Color.FromRgb(83, 89, 88))) },
			{ PBEType.Poison, (new SolidColorBrush(Color.FromRgb(188, 82, 232)), new SolidColorBrush(Color.FromRgb(117, 52, 145))) },
			{ PBEType.Psychic, (new SolidColorBrush(Color.FromRgb(255, 136, 130)), new SolidColorBrush(Color.FromRgb(173, 81, 89))) },
			{ PBEType.Rock, (new SolidColorBrush(Color.FromRgb(196, 174, 112)), new SolidColorBrush(Color.FromRgb(114, 101, 66))) },
			{ PBEType.Steel, (new SolidColorBrush(Color.FromRgb(94, 160, 178)), new SolidColorBrush(Color.FromRgb(66, 105, 114))) },
			{ PBEType.Water, (new SolidColorBrush(Color.FromRgb(58, 176, 232)), new SolidColorBrush(Color.FromRgb(34, 106, 137))) }
		};
	}

	public PBEMove Move { get; }
	public IBrush Brush { get; }
	public IBrush BorderBrush { get; }
	public string Description { get; }
	public ReactiveCommand<Unit, Unit> SelectMoveCommand { get; }

	internal MoveInfo(PBEBattlePokemon pkmn, PBEMove move, Action<PBEMove> clickAction)
	{
		Move = move;
		PBEType moveType = pkmn.GetMoveType(move);
		(Brush, BorderBrush) = _typeToBrush[moveType];
		if (move == PBEMove.None)
		{
			Description = string.Empty;
		}
		else
		{
			IPBEMoveData mData = PBEDataProvider.Instance.GetMoveData(move);
			string s = $"Type: {PBEDataProvider.Instance.GetTypeName(mData.Type).FromGlobalLanguage()}";
			if (mData.Type != moveType)
			{
				s += $" → {PBEDataProvider.Instance.GetTypeName(moveType).FromGlobalLanguage()}";
			}
			var sb = new StringBuilder();
			sb.AppendLine(s);
			sb.AppendLine($"Category: {mData.Category}");
			PBEBattleMoveset.PBEBattleMovesetSlot? slot = pkmn.Moves[move];
			if (slot is not null) // TempLocked move you do not own (like Struggle)
			{
				sb.AppendLine($"PP: {slot.PP}/{slot.MaxPP}");
			}
			sb.AppendLine($"Priority: {mData.Priority}");
			sb.AppendLine($"Power: {(mData.Power == 0 ? "―" : mData.Power.ToString())}");
			sb.AppendLine($"Accuracy: {(mData.Accuracy == 0 ? "―" : mData.Accuracy.ToString())}");
			s = $"Targets: {mData.Targets}";
			PBEMoveTarget moveTargets = pkmn.GetMoveTargets(move);
			if (mData.Targets != moveTargets)
			{
				s += $" → {moveTargets}";
			}
			sb.AppendLine(s);
			sb.AppendLine($"Flags: {mData.Flags}");
			switch (mData.Effect)
			{
				case PBEMoveEffect.Recoil: sb.AppendLine($"Recoil: 1/{mData.EffectParam} damage dealt"); break;
				case PBEMoveEffect.Recoil__10PercentBurn: sb.AppendLine($"Recoil: 1/{mData.EffectParam} damage dealt"); break; // TODO: Burn chance
				case PBEMoveEffect.Recoil__10PercentParalyze: sb.AppendLine($"Recoil: 1/{mData.EffectParam} damage dealt"); break; // TODO: Paralyze chance
				case PBEMoveEffect.Struggle: sb.AppendLine("Recoil: 1/4 user's max HP"); break;
			}
			sb.AppendLine();
			sb.Append(PBEDefaultDataProvider.Instance.GetMoveDescription(move).FromGlobalLanguage().Replace('\n', ' '));
			Description = sb.ToString();
		}
		SelectMoveCommand = ReactiveCommand.Create(() => clickAction(move));
	}
}
