using ECommons.Automation;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RebornRotations.PVPRotations.Tank;

[Rotation("Lelia Default PVP", CombatType.PvP, GameVersion = "7.35")]
[SourceCode(Path = "main/RebornRotations/PVPRotations/Tank/GNB_Default.PvP.cs")]
//[Api(6)]
public sealed class GNB_DefaultPvP2 : GunbreakerRotation
{
    public static IBaseAction MyRoughDividePvP { get; } = new BaseAction((ActionID)29123);

    #region Configurations

    [RotationConfig(CombatType.PvP, Name = "Stop attacking while in Guard.")]
    public bool RespectGuard { get; set; } = true;

    [RotationConfig(CombatType.PvP, Name = "ラフディバイドを使います。")]
    public bool UseRoughDividePvP { get; set; } = false;

    [Range(1, 64500, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "ラフディバイドを使用する敵のHPは？")]
    public int RoughDivideValue { get; set; } = 30000;

    public static unsafe int LimitBreakValue => LimitBreakController.Instance()->CurrentUnits;
    public static unsafe int MyLimitBreakLevel => LimitBreakController.Instance()->BarUnits == 0 ? 0 : LimitBreakValue / LimitBreakController.Instance()->BarUnits;
    public static unsafe int LimitBreakMax => LimitBreakController.Instance()->BarCount;

    public static IBaseAction LBNamePvP = new BaseAction((ActionID)29130);

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvP, Name = "快気を使用するHP%%")]
    public float RecuperateValue { get; set; } = 0.75f;

    [RotationConfig(CombatType.PvP, Name = "LBを使用します")]
    public bool UseLBPvP { get; set; } = false;

    [Range(1, 66000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "LB:敵のHPは？")]
    public int LBValuePvP { get; set; } = 50000;

    #endregion

    #region Gunbreaker Utilities

    [RotationDesc(ActionID.HeartOfCorundumPvP)]
    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.DefenseSingleAbility(nextGCD, out action);
        }

        if (HeartOfCorundumPvP.CanUse(out action))
        {
            return true;
        }

        if (RampartPvP.CanUse(out action))
        {
            return true;
        }

        return base.DefenseSingleAbility(nextGCD, out action);
    }

    private static bool ReadyToRock()
    {
        if (SavageClawPvPReady)
        {
            return true;
        }

        if (WickedTalonPvPReady)
        {
            return true;
        }

        if (HypervelocityPvPReady)
        {
            return true;
        }

        return false;
    }
    private static bool ReadyToRoll()
    {
        if (EyeGougePvPReady)
        {
            return true;
        }

        if (AbdomenTearPvPReady)
        {
            return true;
        }

        if (JugularRipPvPReady)
        {
            return true;
        }

        if (FatedBrandPvPReady)
        {
            return true;
        }

        return false;
    }
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

        if (PurifyPvP.CanUse(out action))
        {
            return true;
        }

        //You WILL try to save yourself. Configs be damned!
        if (HeartOfCorundumPvP.CanUse(out action) && Player.GetHealthRatio() * 100 <= 30)
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

        //快気
        if (RecuperatePvP.CanUse(out action) && Player.GetHealthRatio() <= RecuperateValue)
        {
            return true;
        }

        if (Target.GetHealthRatio() * 100 <= 50 && BlastingZonePvP.CanUse(out action))
        {
            return true;
        }

        if (RampagePvP.CanUse(out action))
        {
            return true;
        }

        if (FullSwingPvP.CanUse(out action))
        {
            return true;
        }

        if (EyeGougePvP.CanUse(out action))
        {
            return true;
        }

        if (AbdomenTearPvP.CanUse(out action))
        {
            return true;
        }

        if (JugularRipPvP.CanUse(out action))
        {
            return true;
        }

        if (HypervelocityPvP.CanUse(out action))
        {
            return true;
        }

        if (FatedBrandPvP.CanUse(out action))
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

        if (CurrentTarget is not null && InCombat && UseLBPvP && Target.DistanceToPlayer() <= 6 && MyLimitBreakLevel == 1 && Target.CurrentHp <= LBValuePvP)
        {
            if (LBNamePvP.CanUse(out action, skipAoeCheck: true))
            {
                return true;
            }
        }

        /*if (CurrentTarget is not null && InCombat && UseLB && MyLimitBreakLevel == 1 &&
            Target.DistanceToPlayer() <= 6 &&
            //Player.GetHealthRatio() < 0.8f &&
            RelentlessRushPvP.Target.Target != null &&
            Target.CurrentHp <= LBValue &&
            RelentlessRushPvP.CanUse(out action, skipAoeCheck: true)) 
        {
            return true; 
        }*/

        if (UseRoughDividePvP && !Player.HasStatus(true, StatusID.NoMercy_3042) &&
            (Target.CurrentHp <= RoughDivideValue || HasHostilesInRange || Target.DistanceToPlayer() >= 5) &&
            MyRoughDividePvP.CanUse(out action, usedUp: true))
        {
            return true;
        }

        // I could totally collapse these into one function but *dab*
        if (!ReadyToRoll())
        {
            if (SavageClawPvP.CanUse(out action, usedUp: true))
            {
                return true;
            }

            if (WickedTalonPvP.CanUse(out action, usedUp: true))
            {
                return true;
            }

            if (GnashingFangPvP.CanUse(out action, usedUp: true))
            {
                return true;
            }
        }

        if (!ReadyToRoll() && FatedCirclePvP.CanUse(out action))
        {
            return true;
        }

        if (!ReadyToRock())
        {
            if (BurstStrikePvP.CanUse(out action))
            {
                return true;
            }

            if (SolidBarrelPvP.CanUse(out action))
            {
                return true;
            }

            if (BrutalShellPvP.CanUse(out action))
            {
                return true;
            }

            if (KeenEdgePvP.CanUse(out action))
            {
                return true;
            }
        }

        if ((!Player?.HasStatus(true, StatusID.Guard) ?? false) && (!Player?.HasStatus(true, StatusID.Sprint) ?? false) &&
            SprintPvP.CanUse(out action)) return true;

        return base.GeneralGCD(out action);

    }
    #endregion

}