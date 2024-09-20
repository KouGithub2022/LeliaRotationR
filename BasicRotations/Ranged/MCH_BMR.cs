using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using System.Text;
using Lumina.Data.Structs.Excel;

namespace DefaultRotations.Ranged;

[Rotation("BMR", CombatType.PvE, GameVersion = "7.00", Description = "")]
[SourceCode(Path = "main/DefaultRotations/Ranged/MCH_Default.cs")]
[Api(4)]
public sealed class MCH_BMR : MachinistRotation
{
    #region Countdown logic
    // Defines logic for actions to take during the countdown before combat starts.
    protected override IAction? CountDownAction(float remainTime)
    {
        // ReassemblePvE's duration is 5s, need to fire the first GCD before it ends
        if (remainTime < 5 && ReassemblePvE.CanUse(out var act)) return act;
        // tincture needs to be used on -2s exactly
        if (remainTime <= 2 && UseBurstMedicine(out act)) return act;
        return base.CountDownAction(remainTime);
    }
    #endregion

    #region oGCD Logic
    // Determines emergency actions to take based on the next planned GCD action.
    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        return base.EmergencyAbility(nextGCD, out act);
    }

    // Logic for using attack abilities outside of GCD, focusing on burst windows and cooldown management.
    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {
        return base.AttackAbility(nextGCD, out act);
    }
    #endregion

    #region GCD Logic
    protected override bool GeneralGCD(out IAction? act)
    {
        return base.GeneralGCD(out act);
    }
    #endregion

}