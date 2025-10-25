using ECommons.Automation;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RotationSolver.RebornRotations.Magical;

[Rotation("LeliaRotation(TESTING)", CombatType.PvE, GameVersion = "7.35")]
[SourceCode(Path = "main/BasicRotations/Magical/BLM_RP.cs")]

public class BLM_RPLeliaRotation : BlackMageRotation
{
    public static unsafe int LimitBreakValue => LimitBreakController.Instance()->CurrentUnits;
    public static unsafe int MyLimitBreakLevel => LimitBreakController.Instance()->BarUnits == 0 ? 0 : LimitBreakValue / LimitBreakController.Instance()->BarUnits;
    public static unsafe int LimitBreakMax => LimitBreakController.Instance()->BarCount;

    #region Config Options
    [RotationConfig(CombatType.PvE, Name = "Use Leylines in combat when standing still")]
    public bool LeylineMadness { get; set; } = false;

    [RotationConfig(CombatType.PvE, Name = "Use both stacks of Leylines automatically")]
    public bool Leyline2Madness { get; set; } = false;

    [RotationConfig(CombatType.PvE, Name = "Use Retrace when out of Leylines in combat and standing still")]
    public bool UseRetrace { get; set; } = false;

    [RotationConfig(CombatType.PvE, Name = "Use Gemdraught/Tincture/pot")]
    public bool UseMedicine { get; set; } = false;

    [RotationConfig(CombatType.PvE, Name = "LBを使用します。")]
    public bool UseLB2 { get; set; } = false;

    [Range(1, 3, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvE, Name = "使用するLBレベルは？")]
    public int UseLBValue2 { get; set; } = 1;

    [RotationConfig(CombatType.PvE, Name = "LB(MAX)を使用します。\nその時の最大LBレベル時に使用します。")]
    public bool UseLBMax2 { get; set; } = false;

    #endregion


    protected override IAction? CountDownAction(float remainTime)
    {
        if (remainTime < BlizzardIiiPvE.Info.CastTime + CountDownAhead)
        {
            if (BlizzardIiiPvE.CanUse(out IAction act))
            {
                return act;
            }
        }
        return base.CountDownAction(remainTime);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {

        if (InCombat && HasHostilesInRange)
        {
            if (UseMedicine && UseBurstMedicine(out act))
            {
                return true;
            }

            if (LeylineMadness && LeyLinesPvE.CanUse(out act, usedUp: Leyline2Madness))
            {
                return true;
            }

            if (!IsLastAbility(ActionID.LeyLinesPvE) && UseRetrace && RetracePvE.CanUse(out act))
            {
                return true;
            }

            if (InAstralFire && CurrentMp < 800)
            {
                if (!ManafontPvE.Cooldown.IsCoolingDown)
                {
                    if (ManafontPvE.CanUse(out act))
                    {
                        return true;
                    }
                }
            }

            if (!IsPolyglotStacksMaxed)
            {
                if (AmplifierPvE.CanUse(out act))
                {
                    return true;
                }
            }

            if (CanMakeInstant && InUmbralIce && !IsParadoxActive)
            {
                if (SwiftcastPvE.CanUse(out act))
                {
                    return true;
                }

                if (TriplecastPvE.CanUse(out act, usedUp: true))
                {
                    return true;
                }
            }

        }

        if (HasBuffs && UseBurstMedicine(out act))
        {
            return true;
        }

        return base.AttackAbility(nextGCD, out act);
    }

    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {

        return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool GeneralGCD(out IAction? act)
    {
        if (InCombat && UseLB2 && !UseLBMax2 && MyLimitBreakLevel >= UseLBValue2 && !IsDead && Target.DistanceToPlayer() < 20)
        {
            Chat.ExecuteCommand("/action リミットブレイク <t>");
        }

        if (InCombat && UseLBMax2 && MyLimitBreakLevel == LimitBreakMax && !IsDead && Target.DistanceToPlayer() < 20)
        {
            Chat.ExecuteCommand("/action リミットブレイク <t>");
        }

        //2 target thunder
        if (HighThunderIiPvE.CanUse(out act)
            && (HighThunderIiPvE.Target.Target?.WillStatusEndGCD(HighThunderIiPvE.Config.StatusGcdCount, 0, true, HighThunderIiPvE.Setting.TargetStatusProvide ?? []) ?? false))
        {
            return true;
        }

        if (ThunderIvPvE.CanUse(out act)
            && (ThunderIvPvE.Target.Target?.WillStatusEndGCD(ThunderIvPvE.Config.StatusGcdCount, 0, true, ThunderIvPvE.Setting.TargetStatusNeed ?? []) ?? false))
        {
            return true;
        }

        if (ThunderIiPvE.CanUse(out act)
            && (ThunderIiPvE.Target.Target?.WillStatusEndGCD(ThunderIiPvE.Config.StatusGcdCount, 0, true, ThunderIiPvE.Setting.TargetStatusNeed ?? []) ?? false))
        {
            return true;
        }

        //1 target thunder
        if (HighThunderPvE.CanUse(out act)
            && (HighThunderPvE.Target.Target?.WillStatusEndGCD(HighThunderPvE.Config.StatusGcdCount, 0, true, HighThunderPvE.Setting.TargetStatusProvide ?? []) ?? false))
        {
            return true;
        }

        if (ThunderIiiPvE.CanUse(out act)
            && (ThunderIiiPvE.Target.Target?.WillStatusEndGCD(ThunderIiiPvE.Config.StatusGcdCount, 0, true, ThunderIiiPvE.Setting.TargetStatusNeed ?? []) ?? false))
        {
            return true;
        }

        if (ThunderPvE.CanUse(out act)
            && (ThunderPvE.Target.Target?.WillStatusEndGCD(ThunderPvE.Config.StatusGcdCount, 0, true, ThunderPvE.Setting.TargetStatusNeed ?? []) ?? false))
        {
            return true;
        }

        if ((IsPolyglotStacksMaxed && (EnochianEndAfterGCD(0) || AmplifierPvE.Cooldown.WillHaveOneChargeGCD(0))) || Player.HasStatus(true, StatusID.LeyLines))
        {
            if (FoulPvE.CanUse(out act, skipAoeCheck: !XenoglossyPvE.EnoughLevel))
            {
                return true;
            }

            if (XenoglossyPvE.CanUse(out act))
            {
                return true;
            }
        }

        if (ParadoxPvE.CanUse(out act))
        {
            return true;
        }

        if (NextGCDisInstant && InUmbralIce)
        {
            if (IsSoulStacksMaxed)
            {
                if (BlizzardIiiPvE.CanUse(out act))
                {
                    return true;
                }
            }
            if (UmbralHearts < 3)
            {
                if (BlizzardIvPvE.CanUse(out act))
                {
                    return true;
                }
            }
        }

        if (Player.HasStatus(true, StatusID.Firestarter))
        {
            if (FireIiiPvE.CanUse(out act))
            {
                return true;
            }
        }

        if (DespairPvE.CanUse(out act))
        {
            return true;
        }

        if (IsSoulStacksMaxed)
        {
            if (TransposePvE.CanUse(out act))
            {
                return true;
            }
        }

        if (UmbralSoulPvE.CanUse(out act))
        {
            return true;
        }

        return base.GeneralGCD(out act);
    }
}