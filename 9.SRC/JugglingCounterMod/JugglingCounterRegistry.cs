using System;
using System.Collections.Generic;
using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace JugglingCounterMod;

/// <summary>
/// 管理杂耍 buff 图标（NPower）上的"本回合攻击牌计数"角标：创建、更新、清理。
/// 设计借鉴（均为 MIT 开源）：BaseLib TwoAmountPowers（Duplicate %AmountLabel 创建第二标签、
/// 注册表触发刷新）、RitsuLib ExtraCornerAmountLabels（按节点实例 id 的状态字典 + _ExitTree 清理、
/// Power TopRight 布局常量）。
/// </summary>
public static class JugglingCounterRegistry
{
    public const string LabelName = "JugglingAttackCountLabel";

    // 右上角布局（power.tscn 实测 + RitsuLib ExtraCornerHostLayout 的 Power TopRight 常量）：
    // 与原层数标签同宽(100)、右对齐，y 移到顶部行（原标签 y21..44 → 0..23）
    private const float CornerLeft = -56f;
    private const float CornerTop = 0f;

    private static readonly Dictionary<ulong, NPower> Entries = new();
    private static readonly Lock SyncRoot = new();

    /// <summary>
    /// NPower.RefreshAmount 时调用：确保角标存在、注册节点、更新文本。
    /// 覆盖时机：buff 图标创建（_Ready → Reload → RefreshAmount）、层数变化等。
    /// </summary>
    public static void Sync(NPower node)
    {
        if (node.Model is not JugglingPower power)
        {
            return;
        }
        EnsureLabel(node);
        Register(node);
        UpdateText(node, JugglingCounter.CountAttacksPlayedThisTurn(power));
    }

    /// <summary>
    /// 攻击牌写入战斗历史后调用（CombatHistory.CardPlayStarted Postfix）：刷新所有已注册角标。
    /// 此时当前攻击牌已计入 History，统计值即为"含本张"的本回合攻击牌数。
    /// 每个角标按自己的 power.Owner 统计，多玩家/多实例天然正确。
    /// </summary>
    public static void RefreshAll()
    {
        List<ulong> dead = new();
        lock (SyncRoot)
        {
            foreach (KeyValuePair<ulong, NPower> pair in Entries)
            {
                NPower node = pair.Value;
                if (!GodotObject.IsInstanceValid(node))
                {
                    dead.Add(pair.Key);
                    continue;
                }
                try
                {
                    if (node.Model is JugglingPower power)
                    {
                        UpdateText(node, JugglingCounter.CountAttacksPlayedThisTurn(power));
                    }
                }
                catch (Exception e)
                {
                    MegaCrit.Sts2.Core.Logging.Log.Error(
                        $"{ModEntry.ModId}: refresh corner label failed: {e}");
                }
            }
            foreach (ulong id in dead)
            {
                Entries.Remove(id);
            }
        }
    }

    /// <summary>
    /// 己方回合结束时调用（JugglingPower.AfterSideTurnEnd Postfix）：该实例的角标显式置 0，
    /// 与游戏内部 attacksPlayedThisTurn 清零同点触发，不依赖 History 的 side 切换时序。
    /// </summary>
    public static void SetZeroFor(JugglingPower power)
    {
        lock (SyncRoot)
        {
            foreach (KeyValuePair<ulong, NPower> pair in Entries)
            {
                NPower node = pair.Value;
                if (GodotObject.IsInstanceValid(node) && node.Model == power)
                {
                    UpdateText(node, 0);
                }
            }
        }
    }

    /// <summary>节点退出场景树时调用（NPower._ExitTree Postfix）：注销，防止泄漏/悬挂回调。</summary>
    public static void Unregister(NPower node)
    {
        lock (SyncRoot)
        {
            Entries.Remove(node.GetInstanceId());
        }
    }

    private static void Register(NPower node)
    {
        lock (SyncRoot)
        {
            Entries[node.GetInstanceId()] = node;
        }
    }

    private static void EnsureLabel(NPower node)
    {
        if (node.HasNode(LabelName))
        {
            return;
        }
        MegaLabel amountLabel = node.GetNode<MegaLabel>("%AmountLabel");
        MegaLabel label = (MegaLabel)amountLabel.Duplicate(); // 继承字体/字号18/颜色/描边/阴影/右对齐
        label.Name = LabelName;
        label.UniqueNameInOwner = false; // 避免 unique 名冲突
        label.Visible = false;           // 配置完成前先隐藏
        node.AddChild(label);
        node.MoveChild(label, amountLabel.GetIndex());
        label.Position = new Vector2(CornerLeft, CornerTop);
        label.Visible = true;
    }

    private static void UpdateText(NPower node, int count)
    {
        MegaLabel? corner = node.GetNodeOrNull<MegaLabel>(LabelName);
        corner?.SetTextAutoSize(count.ToString());
    }
}
