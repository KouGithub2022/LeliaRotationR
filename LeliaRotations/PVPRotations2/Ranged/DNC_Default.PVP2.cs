using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RebornRotations.PVPRotations.Ranged;

[Rotation("Test Default PVP2", CombatType.PvP, GameVersion = "7.35")]
[SourceCode(Path = "main/RebornRotations/PVPRotations/Ranged/DNC_Default.PvP.cs")]
//[Api(6)]
public sealed class DNC_DefaultPvP2 : DancerRotation
{
    #region Configurations

    [RotationConfig(CombatType.PvP, Name = "Stop attacking while in Guard.")]
    public bool RespectGuard { get; set; } = true;

    public static unsafe int LimitBreakValue => LimitBreakController.Instance()->CurrentUnits;
    public static unsafe int MyLimitBreakLevel => LimitBreakController.Instance()->BarUnits == 0 ? 0 : LimitBreakValue / LimitBreakController.Instance()->BarUnits;
    public static unsafe int LimitBreakMax => LimitBreakController.Instance()->BarCount;
    public static IBaseAction LBNamePvP = new BaseAction((ActionID)29432);

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvP, Name = "快気を使用するHP%%")]
    public float RecuperateValue { get; set; } = 0.75f;

    [RotationConfig(CombatType.PvP, Name = "LBを使用します")]
    public bool UseLBPvP { get; set; } = false;

    [Range(1, 66000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "LB:敵のHPは？")]
    public int LBValuePvP { get; set; } = 35000;

    #endregion

    #region oGCDs
    protected override bool EmergencyAbility(IAction nextGCD, out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.EmergencyAbility(nextGCD, out action);
        }

        //快気
        if (RecuperatePvP.CanUse(out action) && Player.GetHealthRatio() <= RecuperateValue)
        {
            return true;
        }

        if (Player.HasStatus(true, StatusID.HoningDance))
        {
            return base.EmergencyAbility(nextGCD, out action);
        }

        if (PurifyPvP.CanUse(out action))
        {
            return true;
        }

        if (ClosedPositionPvP.CanUse(out action) && !Player.HasStatus(true, StatusID.ClosedPosition_2026))
        {
            return true;
        }

        if (InCombat && BraveryPvP.CanUse(out action))
        {
            return true;
        }

        if (InCombat && DervishPvP.CanUse(out action))
        {
            return true;
        }

        return base.EmergencyAbility(nextGCD, out action);
    }

    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? action)
    {
        if (!RespectGuard || !Player.HasStatus(true, StatusID.Guard))
        {
            return base.DefenseSingleAbility(nextGCD, out action);
        }

        return base.DefenseSingleAbility(nextGCD, out action);
    }

    protected override bool HealAreaAbility(IAction nextGCD, out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.HealAreaAbility(nextGCD, out action);
        }

        if (Player.HasStatus(true, StatusID.HoningDance))
        {
            return base.HealAreaAbility(nextGCD, out action);
        }

        if (CuringWaltzPvP.CanUse(out action))
        {
            return true;
        }

        return base.HealAreaAbility(nextGCD, out action);
    }

    protected override bool MoveBackAbility(IAction nextGCD, out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.MoveBackAbility(nextGCD, out action);
        }

        // if (EnAvantPvP.CanUse(out action)) return true;

        return base.MoveBackAbility(nextGCD, out action);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.AttackAbility(nextGCD, out action);
        }

        //快気
        if (RecuperatePvP.CanUse(out action) && Player.GetHealthRatio() <= RecuperateValue)
        {
            return true;
        }

        if (Player.HasStatus(true, StatusID.HoningDance))
        {
            return base.AttackAbility(nextGCD, out action);
        }

        if (CuringWaltzPvP.CanUse(out action) && Player.GetHealthRatio() <= 0.8f)
        {
            return true;
        }

        if (EnAvantPvP.CanUse(out action,usedUp: true) && 
            EnAvantPvP.Cooldown.CurrentCharges > 1 && 
            !Player.HasStatus(true, StatusID.EnAvant) && 
            Player.HasStatus(true, StatusID.HoningOvation) 
            && Target.DistanceToPlayer() < 15)
        {
            return true;
        }

        if (FanDancePvP.CanUse(out action))
        {
            return true;
        }

        if (EagleEyeShotPvP.CanUse(out action))
        {
            return true;
        }


        return base.AttackAbility(nextGCD, out action);
    }
    #endregion

    #region GCDs
    protected override bool GeneralGCD(out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.GeneralGCD(out action);
        }

        //快気
        if (RecuperatePvP.CanUse(out action) && Player.GetHealthRatio() <= RecuperateValue)
        {
            return true;
        }

        if (Player.HasStatus(true, StatusID.HoningDance))
        {
            return base.GeneralGCD(out action);
        }

        if (CurrentTarget is not null && InCombat && UseLBPvP && Target.DistanceToPlayer() <= 6 && MyLimitBreakLevel == 1 && Target.CurrentHp <= LBValuePvP)
        {
            if (LBNamePvP.CanUse(out action))
            {
                return true;
            }
        }

        /*if (CurrentTarget is not null && InCombat && UseLB && 
            MyLimitBreakLevel == 1 && 
            Target.CurrentHp <= LBValue && Target.DistanceToPlayer() < 6 && 
            ContradancePvP.CanUse(out action))
        {
            return true;
        }*/


        if (DanceOfTheDawnPvP.CanUse(out action))
        {
            return true;
        }

        if (StarfallDancePvP.CanUse(out action))
        {
            return true;
        }

        if (HasHostilesInRange && HoningOvationPvP.CanUse(out action) && Player.HasStatus(true,StatusID.HoningOvation))
        {
            return true;
        }

        /*if (InCombat && HoningDancePvP.CanUse(out action) && 
            Target.DistanceToPlayer() <= 6 &&
            !Player.HasStatus(true, StatusID.EnAvant) && 
            !Player.HasStatus(true,StatusID.HoningDance))
        {
            return true;
        }*/

        if (NumberOfHostilesInRangeOf(6) > 0 && HoningDancePvP.CanUse(out action) && !Player.HasStatus(true, StatusID.EnAvant))
        {
            return true;
        }

        if (SaberDancePvP.CanUse(out action))
        {
            return true;
        }

        if (FountainPvP.CanUse(out action))
        {
            return true;
        }

        if (CascadePvP.CanUse(out action))
        {
            return true;
        }

        if ((!Player?.HasStatus(true, StatusID.Guard) ?? false) && (!Player?.HasStatus(true, StatusID.Sprint) ?? false) &&
            SprintPvP.CanUse(out action)) return true;

        return base.GeneralGCD(out action);
    }
    #endregion

}