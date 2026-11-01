using System;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace JugglingCounterMod.Patches;

/// <summary>
/// NPower._ExitTree Postfix：节点退出场景树时注销注册表条目，防止泄漏/悬挂回调。
/// </summary>
[HarmonyPatch(typeof(NPower), nameof(NPower._ExitTree))]
public static class NPowerExitTreePatch
{
    public static void Postfix(NPower __instance)
    {
        try
        {
            JugglingCounterRegistry.Unregister(__instance);
        }
        catch (Exception e)
        {
            Log.Error($"{ModEntry.ModId}: NPower._ExitTree postfix failed: {e}");
        }
    }
}
