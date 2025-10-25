using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RebornRotations.PVPRotations.Healer;

[Rotation("Lelia Default PVP", CombatType.PvP, GameVersion = "7.35")]
[SourceCode(Path = "main/RebornRotations/PVPRotations/Healer/SCH_Default.PVP.cs")]
//[Api(6)]
public class SCH_DefaultPVP2 : ScholarRotation
{
    #region Configurations

    [RotationConfig(CombatType.PvP, Name = "Stop attacking while in Guard.")]
    public bool RespectGuard { get; set; } = true;
    public static unsafe int LimitBreakValue => LimitBreakController.Instance()->CurrentUnits;
    public static unsafe int MyLimitBreakLevel => LimitBreakController.Instance()->BarUnits == 0 ? 0 : LimitBreakValue / LimitBreakController.Instance()->BarUnits;
    public static unsafe int LimitBreakMax => LimitBreakController.Instance()->BarCount;
    public static IBaseAction LBNamePvP = new BaseAction((ActionID)41502);

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvP, Name = "快気を使用するHP%%")]
    public float RecuperateValue { get; set; } = 0.75f;

    [RotationConfig(CombatType.PvP, Name = "LBを使用します")]
    public bool UseLBPvP { get; set; } = false;

    [Range(1, 66000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "LB:敵のHPは？")]
    public int LBValuePvP { get; set; } = 50000;    
    
    #endregion

    #region oGCDs
    protected override bool EmergencyAbility(IAction nextGCD, out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            base.EmergencyAbility(nextGCD, out action);
        }

        if (RecuperatePvP.CanUse(out action) && Player.GetHealthRatio() <= RecuperateValue)
        {
            return true;
        }

        if (PurifyPvP.CanUse(out action))
        {
            return true;
        }

        if (ChainStratagemPvP.CanUse(out action) && Target.HasStatus(false, StatusID.Guard))
        {
            return true;
        }

        if (ChainStratagemPvP.CanUse(out action))
        {
            return true;
        }

        return base.EmergencyAbility(nextGCD, out action);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.AttackAbility(nextGCD, out action);
        }

        if (RecuperatePvP.CanUse(out action) && Player.GetHealthRatio() <= RecuperateValue)
        {
            return true;
        }

        if (InCombat && ExpedientPvP.CanUse(out action, usedUp: true))
        {
            return true;
        }

        if (InCombat && SummonSeraphPvP.CanUse(out action, usedUp: true))
        {
            return true;
        }

        if (DiabrosisPvP.CanUse(out action))
        {
            return true;
        }

        if (DeploymentTacticsPvP.CanUse(out action, usedUp: true) && Target.HasStatus(true, StatusID.Biolysis_3089))
        {
            return true;
        }

        return base.AttackAbility(nextGCD, out action);
    }

    protected override bool DefenseAreaAbility(IAction nextGCD, out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.DefenseAreaAbility(nextGCD, out action);
        }

        if (ExpedientPvP.CanUse(out action, usedUp: true))
        {
            return true;
        }

        return base.DefenseAreaAbility(nextGCD, out action);
    }

    protected override bool HealAreaAbility(IAction nextGCD, out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.HealAreaAbility(nextGCD, out action);
        }

        if (SummonSeraphPvP.CanUse(out action, usedUp: true))
        {
            return true;
        }

        return base.HealAreaAbility(nextGCD, out action);
    }
    #endregion

    #region GCDs
    protected override bool DefenseSingleGCD(out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.DefenseSingleGCD(out action);
        }

        if (StoneskinIiPvP.CanUse(out action, usedUp: true))
        {
            return true;
        }

        return base.DefenseSingleGCD(out action);
    }

    protected override bool HealSingleGCD(out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.HealSingleGCD(out action);
        }

        if (HaelanPvP.CanUse(out action))
        {
            return true;
        }

        if (AdloquiumPvP.CanUse(out action, usedUp: true))
        {
            return true;
        }

        return base.HealSingleGCD(out action);
    }

    protected override bool GeneralGCD(out IAction? action)
    {
        if (RecuperatePvP.CanUse(out action) && Player.GetHealthRatio() <= RecuperateValue)
        {
            return true;
        }

        if (CurrentTarget is not null && InCombat && UseLBPvP && Target.DistanceToPlayer() <= 25 && MyLimitBreakLevel == 1 && Target.CurrentHp <= LBValuePvP)
        {
            if (LBNamePvP.CanUse(out action))
            {
                return true;
            }
        }

        if (BiolysisPvP.CanUse(out action) && Player.HasStatus(true, StatusID.Recitation_3094))
        {
            return true;
        }

        if (BiolysisPvP.CanUse(out action))
        {
            return true;
        }

        if (AccessionPvP.CanUse(out action))
        {
            return true;
        }

        if (SeraphicHaloPvP.CanUse(out action))
        {
            return true;
        }

        if (BroilIvPvP.CanUse(out action))
        {
            return true;
        }

        if ((!Player?.HasStatus(true, StatusID.Guard) ?? false) && (!Player?.HasStatus(true, StatusID.Sprint) ?? false) &&
            SprintPvP.CanUse(out action)) return true;
        
        return base.GeneralGCD(out action);
    }
    #endregion
}