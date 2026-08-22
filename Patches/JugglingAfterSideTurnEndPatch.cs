using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models.Powers;

namespace JugglingCounterMod.Patches;

/// <summary>
/// JugglingPower.AfterSideTurnEnd Postfix：己方回合结束时该实例角标显式置 0，
/// 与游戏内部 attacksPlayedThisTurn 清零同点触发（participants 含 Owner 才清零，条件一致）。
/// AfterSideTurnEnd 方法体为同步执行（无 await），Postfix 时清零已完成。
/// </summary>
[HarmonyPatch(typeof(JugglingPower), nameof(JugglingPower.AfterSideTurnEnd))]
public static class JugglingAfterSideTurnEndPatch
{
    public static void Postfix(JugglingPower __instance, IEnumerable<Creature> participants)
    {
        try
        {
            if (participants == null || __instance.Owner == null || !participants.Contains(__instance.Owner))
            {
                return;
            }
            JugglingCounterRegistry.SetZeroFor(__instance);
        }
        catch (Exception e)
        {
            Log.Error($"{ModEntry.ModId}: JugglingPower.AfterSideTurnEnd postfix failed: {e}");
        }
    }
}
