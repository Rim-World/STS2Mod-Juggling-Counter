using System;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace JugglingCounterMod.Patches;

/// <summary>
/// NPower.RefreshAmount Postfix：杂耍 buff 图标创建/刷新时，同步右上角"本回合攻击牌计数"角标并注册节点。
/// RefreshAmount 为 private 方法，Harmony 用字符串名定位（先例：BaseLib TwoAmountPowers）。
/// </summary>
[HarmonyPatch(typeof(NPower), "RefreshAmount")]
public static class NPowerRefreshAmountPatch
{
    public static void Postfix(NPower __instance)
    {
        try
        {
            if (!__instance.IsNodeReady())
            {
                return;
            }
            JugglingCounterRegistry.Sync(__instance);
        }
        catch (Exception e)
        {
            Log.Error($"{ModEntry.ModId}: NPower.RefreshAmount postfix failed: {e}");
        }
    }
}
