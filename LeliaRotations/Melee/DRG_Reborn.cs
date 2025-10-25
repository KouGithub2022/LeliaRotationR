using ECommons.Automation;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RotationSolver.RebornRotations.Melee;

[Rotation("LeliaRotation", CombatType.PvE, GameVersion = "7.35")]
[SourceCode(Path = "main/RebornRotations/Melee/DRG_Reborn.cs")]


public sealed class DRG_LeliaRotation : DragoonRotation
{
    #region Config Options
    [RotationConfig(CombatType.PvE, Name = "Use Doom Spike for damage uptime if out of melee range even if it breaks combo")]
    public bool DoomSpikeWhenever { get; set; } = true;

    [Range(1, 20, ConfigUnitType.Yalms)]
    [RotationConfig(CombatType.PvE, Name = "Max distance you need to be from the target for Stardiver useage")]
    public float StardiverDistance { get; set; } = 20;

    [Range(1, 20, ConfigUnitType.Yalms)]
    [RotationConfig(CombatType.PvE, Name = "Max distance you need to be from the target for Dragonfire Dive useage")]
    public float DragonfireDiveDistance { get; set; } = 20;
    public static unsafe int LimitBreakValue => LimitBreakController.Instance()->CurrentUnits;
    public static unsafe int MyLimitBreakLevel => LimitBreakController.Instance()->BarUnits == 0 ? 0 : LimitBreakValue / LimitBreakController.Instance()->BarUnits;
    public static unsafe int LimitBreakMax => LimitBreakController.Instance()->BarCount;

    [RotationConfig(CombatType.PvE, Name = "LB���g�p���܂��B")]
    public bool UseLBPvE { get; set; } = false;

    [Range(1, 3, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvE, Name = "�g�p����LB���x���́H")]
    public int UseLBValuePvE { get; set; } = 1;

    [RotationConfig(CombatType.PvE, Name = "LB(MAX)���g�p���܂��B\n���̎��̍ő�LB���x�����Ɏg�p���܂��B")]
    public bool UseLBMaxPvE { get; set; } = false;
    #endregion

    #region Additional oGCD Logic

    [RotationDesc(ActionID.WingedGlidePvE)]
    protected override bool MoveForwardAbility(IAction nextGCD, out IAction? act)
    {
        if (IsLastAction(false, StardiverPvE))
        {
            return base.MoveForwardAbility(nextGCD, out act);
        }

        if (WingedGlidePvE.CanUse(out act, skipComboCheck: true))
        {
            return true;
        }
        return base.MoveForwardAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.ElusiveJumpPvE)]
    protected override bool MoveBackAbility(IAction nextGCD, out IAction? act)
    {
        if (IsLastAction(false, StardiverPvE))
        {
            return base.MoveBackAbility(nextGCD, out act);
        }

        if (ElusiveJumpPvE.CanUse(out act, skipComboCheck: true))
        {
            return true;
        }
        return base.MoveBackAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.FeintPvE)]
    protected sealed override bool DefenseAreaAbility(IAction nextGCD, out IAction? act)
    {
        if (IsLastAction(false, StardiverPvE))
        {
            return base.DefenseAreaAbility(nextGCD, out act);
        }

        if (FeintPvE.CanUse(out act, skipComboCheck: true))
        {
            return true;
        }
        return base.DefenseAreaAbility(nextGCD, out act);
    }
    #endregion

    #region oGCD Logic
    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        if (IsLastAction() == IsLastGCD())
        {
            if (StardiverPvE.CanUse(out act))
            {
                if (StardiverPvE.Target.Target.DistanceToPlayer() <= StardiverDistance)
                {
                    return true;
                }
            }
        }

        if (HasBattleLitany && InCombat && UseBurstMedicine(out act))
        {
            return true;
        }

        return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {
        if (IsLastAction(false, StardiverPvE))
        {
            return base.AttackAbility(nextGCD, out act);
        }

        if (DisembowelPvE.EnoughLevel)
        {
            if (!HasPowerSurge)
            {
                return base.AttackAbility(nextGCD, out act);
            }
        }

        if (IsBurst && InCombat && HasHostilesInRange)
        {
            bool lsOnBigHit =
                nextGCD.IsTheSameTo(true, HeavensThrustPvE, DrakesbanePvE) &&
                (HasBattleLitany || LOTDEndAfter(1000) ||
                 LanceChargePvE.Cooldown.IsCoolingDown || BattleLitanyPvE.Cooldown.IsCoolingDown);

            bool lsOnAltFinishersDuringBuff =
                HasBattleLitany &&
                LOTDEndAfter(1000) &&
                nextGCD.IsTheSameTo(true, ChaoticSpringPvE, LanceBarragePvE, WheelingThrustPvE, FangAndClawPvE);

            bool lsLowLevel =
                (!DisembowelPvE.EnoughLevel && nextGCD.IsTheSameTo(true, VorpalThrustPvE)) ||
                (!FullThrustPvE.EnoughLevel && nextGCD.IsTheSameTo(true, VorpalThrustPvE, DisembowelPvE)) ||
                (!LanceChargePvE.EnoughLevel && nextGCD.IsTheSameTo(true, DisembowelPvE, FullThrustPvE)) ||
                (!BattleLitanyPvE.EnoughLevel && nextGCD.IsTheSameTo(true, FullThrustPvE)) ||
                (!HeavensThrustPvE.EnoughLevel && !DrakesbanePvE.EnoughLevel &&
                 HasBattleLitany && HasLanceCharge && LOTDEndAfter(1000) &&
                 nextGCD.IsTheSameTo(true, ChaoticSpringPvE, LanceBarragePvE, WheelingThrustPvE, FangAndClawPvE));

            bool lifeSurgeReady = lsOnBigHit || lsOnAltFinishersDuringBuff || lsLowLevel;

            if ((!BattleLitanyPvE.Cooldown.ElapsedAfter(60) || !BattleLitanyPvE.EnoughLevel)
                && LanceChargePvE.CanUse(out act))
            {
                return true;
            }

            if (BattleLitanyPvE.CanUse(out act))
            {
                return true;
            }

            if (lifeSurgeReady && LifeSurgePvE.CanUse(out act, usedUp: HasLanceCharge))
            {
                return true;
            }
        }

        if (GeirskogulPvE.CanUse(out act))
        {
            return true;
        }

        if ((BattleLitanyPvE.EnoughLevel && HasBattleLitany) || (!BattleLitanyPvE.EnoughLevel))
        {
            if (DragonfireDivePvE.CanUse(out act))
            {
                if (DragonfireDivePvE.Target.Target.DistanceToPlayer() <= DragonfireDiveDistance)
                {
                    return true;
                }
            }
        }

        if (HasBattleLitany || LOTDEndAfter(1000)
            || nextGCD.IsTheSameTo(true, RaidenThrustPvE, DraconianFuryPvE))
        {
            if (WyrmwindThrustPvE.CanUse(out act, usedUp: true))
            {
                return true;
            }
        }

        if (!HighJumpPvE.EnoughLevel && LanceChargePvE.Cooldown.IsCoolingDown)
        {
            if (JumpPvE.CanUse(out act))
            {
                return true;
            }
        }

        if (GeirskogulPvE.Cooldown.IsCoolingDown || LOTDTime > 0)
        {
            if (HighJumpPvE.CanUse(out act))
            {
                return true;
            }
        }

        if (NastrondPvE.CanUse(out act))
        {
            return true;
        }

        if (StarcrossPvE.CanUse(out act))
        {
            return true;
        }

        if (RiseOfTheDragonPvE.CanUse(out act))
        {
            return true;
        }

        if (MirageDivePvE.CanUse(out act))
        {
            return true;
        }

        return base.AttackAbility(nextGCD, out act);
    }
    #endregion

    #region GCD Logic
    protected override bool GeneralGCD(out IAction? act)
    {
        bool doomSpikeRightNow = DoomSpikeWhenever;

        if (CoerthanTormentPvE.CanUse(out act))
        {
            return true;
        }

        //bool isTargetBoss = CurrentTarget?.IsBossFromTTK() ?? false;
        if (CurrentTarget is not null && InCombat && UseLBPvE && !UseLBMaxPvE && MyLimitBreakLevel >= UseLBValuePvE && !IsDead && Target.DistanceToPlayer() < 20)
        {
            Chat.ExecuteCommand("/action ���~�b�g�u���C�N <t>");
        }

        if (SonicThrustPvE.CanUse(out act, skipStatusProvideCheck: true))
        {
            return true;
        }

        if (LanceMasteryTrait.EnoughLevel)
        {
            if (HasDraconianFire)
            {
                if (DraconianFuryPvE.CanUse(out act, skipComboCheck: doomSpikeRightNow))
                {
                    return true;
                }
            }
            if (!HasDraconianFire)
            {
                if (DoomSpikePvE.CanUse(out act, skipComboCheck: doomSpikeRightNow))
                {
                    return true;
                }
            }
        }
        if (!LanceMasteryTrait.EnoughLevel)
        {
            if (DoomSpikePvE.CanUse(out act, skipComboCheck: doomSpikeRightNow))
            {
                return true;
            }
        }

        if (DrakesbanePvE.CanUse(out act, skipStatusProvideCheck: true))
        {
            return true;
        }

        if (FangAndClawPvE.CanUse(out act))
        {
            return true;
        }

        if (WheelingThrustPvE.CanUse(out act))
        {
            return true;
        }

        if (LanceMasteryIiTrait.EnoughLevel)
        {
            if (HeavensThrustPvE.CanUse(out act))
            {
                return true;
            }
        }
        if (!LanceMasteryIiTrait.EnoughLevel)
        {
            if (FullThrustPvE.CanUse(out act))
            {
                return true;
            }
        }

        if (LanceMasteryIiTrait.EnoughLevel)
        {
            if (ChaoticSpringPvE.CanUse(out act, skipStatusProvideCheck: true))
            {
                return true;
            }
        }
        if (!LanceMasteryIiTrait.EnoughLevel)
        {
            if (ChaosThrustPvE.CanUse(out act, skipStatusProvideCheck: true))
            {
                return true;
            }
        }

        if (LanceMasteryIvTrait.EnoughLevel)
        {
            if (SpiralBlowPvE.CanUse(out act))
            {
                return true;
            }
        }
        if (!LanceMasteryIvTrait.EnoughLevel)
        {
            if (DisembowelPvE.CanUse(out act))
            {
                return true;
            }
        }

        if (LanceMasteryIvTrait.EnoughLevel)
        {
            if (LanceBarragePvE.CanUse(out act))
            {
                return true;
            }
        }
        if (!LanceMasteryIvTrait.EnoughLevel)
        {
            if (VorpalThrustPvE.CanUse(out act))
            {
                return true;
            }
        }

        if (LanceMasteryTrait.EnoughLevel)
        {
            if (HasDraconianFire)
            {
                if (RaidenThrustPvE.CanUse(out act))
                {
                    return true;
                }
            }
            if (!HasDraconianFire)
            {
                if (TrueThrustPvE.CanUse(out act))
                {
                    return true;
                }
            }
        }
        if (!LanceMasteryTrait.EnoughLevel)
        {
            if (TrueThrustPvE.CanUse(out act))
            {
                return true;
            }
        }

        if (!IsLastAction(true, WingedGlidePvE) && PiercingTalonPvE.CanUse(out act))
        {
            return true;
        }

        return base.GeneralGCD(out act);
    }
    #endregion
}
