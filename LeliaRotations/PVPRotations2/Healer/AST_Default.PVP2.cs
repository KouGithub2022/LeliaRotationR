using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RebornRotations.PVPRotations.Healer;

[Rotation("Lelia Default PVP", CombatType.PvP, GameVersion = "7.35")]
[SourceCode(Path = "main/RebornRotations/PVPRotations/Healer/AST_Default.PVP.cs")]
//[Api(6)]
public class AST_DefaultPVP2 : AstrologianRotation
{
    #region Configurations

    [RotationConfig(CombatType.PvP, Name = "Stop attacking while in Guard.")]
    public bool RespectGuard { get; set; } = true;

    [RotationConfig(CombatType.PvP, Name = "エピサイクルを使用します")]
    public bool UseEpicycle { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "レトログレードを使用します")]
    public bool UseRetrograde { get; set; } = false;

    [Range(1, 66000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "エピサイクル:敵のHPは？")]
    public int EpicycleValue { get; set; } = 30000;

    public static unsafe int LimitBreakValue => LimitBreakController.Instance()->CurrentUnits;
    public static unsafe int MyLimitBreakLevel => LimitBreakController.Instance()->BarUnits == 0 ? 0 : LimitBreakValue / LimitBreakController.Instance()->BarUnits;
    public static unsafe int LimitBreakMax => LimitBreakController.Instance()->BarCount;
    public static IBaseAction LBNamePvP = new BaseAction((ActionID)29255);

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvP, Name = "快気を使用するHP%%")]
    public float RecuperateValue { get; set; } = 0.9f;

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
            return base.EmergencyAbility(nextGCD, out action);
        }

        if (RecuperatePvP.CanUse(out action) && Player.GetHealthRatio() <= RecuperateValue)
        {
            return true;
        }

        if (PurifyPvP.CanUse(out action))
        {
            return true;
        }

        if (AspectedBeneficPvP_29247.CanUse(out action, usedUp: true))
        {
            return true;
        }

        if (Player.WillStatusEnd(1, true, StatusID.Macrocosmos_3104) && MicrocosmosPvP.CanUse(out action))
        {
            return true;
        }

        if (Player.WillStatusEnd(1, true, StatusID.LadyOfCrowns_4328) && LadyOfCrownsPvE.CanUse(out action))
        {
            return true;
        }

        if (Player.GetHealthRatio() < 0.5 && MicrocosmosPvP.CanUse(out action))
        {
            return true;
        }

        if (OraclePvP.CanUse(out action))
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

        if (FallMaleficPvP_29246.CanUse(out action, usedUp: true))
        {
            return true;
        }

        if (GravityIiPvP_29248.CanUse(out action, usedUp: true))
        {
            return true;
        }

        if (DiabrosisPvP.CanUse(out action))
        {
            return true;
        }

        if (InCombat && MinorArcanaPvP.CanUse(out action))
        {
            return true;
        }

        if (LordOfCrownsPvP.CanUse(out action))
        {
            return true;
        }


        return base.AttackAbility(nextGCD, out action);
    }
    #endregion

    #region GCDs
    protected override bool DefenseSingleGCD(out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            base.DefenseSingleGCD(out action);
        }

        if (RecuperatePvP.CanUse(out action) && Player.GetHealthRatio() <= RecuperateValue)
        {
            return true;
        }

        if (CurrentTarget is not null && InCombat && UseLBPvP && Target.DistanceToPlayer() <= 25 &&
            MyLimitBreakLevel == 1 && Target.CurrentHp <= LBValuePvP)
        {
            if (LBNamePvP.CanUse(out action))
            {
                return true;
            }
        }

        if (StoneskinIiPvP.CanUse(out action))
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

        if (AspectedBeneficPvP.CanUse(out action, usedUp: true))
        {
            return true;
        }

        return base.HealSingleGCD(out action);
    }

    protected override bool HealAreaGCD(out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.HealAreaGCD(out action);
        }

        if (LadyOfCrownsPvP.CanUse(out action))
        {
            return true;
        }

        return base.HealAreaGCD(out action);
    }

    protected override bool GeneralGCD(out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.GeneralGCD(out action);
        }

        if (UseRetrograde && Target.DistanceToPlayer() < 5 && RetrogradePvP.CanUse(out action))
        {
            return true;
        }

        if (CurrentTarget is not null && InCombat && UseLBPvP && Target.DistanceToPlayer() <= 25 &&
            MyLimitBreakLevel == 1 && Target.CurrentHp <= LBValuePvP)
        {
            if (LBNamePvP.CanUse(out action))
            {
                return true;
            }
        }

        if (UseEpicycle && Target.CurrentHp <= EpicycleValue && EpicyclePvP.CanUse(out action))
        {
            return true;
        }

        if (InCombat && MacrocosmosPvP.CanUse(out action))
        {
            return true;
        }

        if (GravityIiPvP.CanUse(out action))
        {
            return true;
        }

        if (FallMaleficPvP.CanUse(out action))
        {
            return true;
        }

        if ((!Player?.HasStatus(true, StatusID.Guard) ?? false) && (!Player?.HasStatus(true, StatusID.Sprint) ?? false) &&
            SprintPvP.CanUse(out action)) return true;

        return base.GeneralGCD(out action);
    }
    #endregion
}