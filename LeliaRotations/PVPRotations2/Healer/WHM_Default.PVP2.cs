using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RebornRotations.PVPRotations.Healer;

[Rotation("Lelia Default PVP", CombatType.PvP, GameVersion = "7.35")]
[SourceCode(Path = "main/RebornRotations/PVPRotations/Healer/WHM_Default.PVP.cs")]
//[Api(6)]
public class WHM_DefaultPVP2 : WhiteMageRotation
{
    public static IBaseAction AfflatusPurgationPvP { get; } = new BaseAction((ActionID)29230);
    public static IBaseAction MySeraphStrikePvP { get; } = new BaseAction((ActionID)29229);
    public static IBaseAction MyAquaveilPvP { get; } = new BaseAction((ActionID)29227);
    public static unsafe int LimitBreakValue => LimitBreakController.Instance()->CurrentUnits;
    public static unsafe int MyLimitBreakLevel => LimitBreakController.Instance()->BarUnits == 0 ? 0 : LimitBreakValue / LimitBreakController.Instance()->BarUnits;

    #region Configurations

    [RotationConfig(CombatType.PvP, Name = "Use Aquaveil on other players")]
    public bool AquaveilEsuna { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "Stop attacking while in Guard.")]
    public bool RespectGuard { get; set; } = true;

    [RotationConfig(CombatType.PvP, Name = "セラフストライクを使用します")]
    public bool UseSeraph { get; set; } = false;

    [Range(1, 66000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "セラフストライク:敵のHPは？")]
    public int SeraphValue { get; set; } = 30000;

    [RotationConfig(CombatType.PvP, Name = "ミラクル・オブ・ネイチャー")]
    public bool UseMiracle { get; set; } = true;

    [Range(1, 66000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "ミラクル・オブ・ネイチャー:敵のHPは？")]
    public int MiracleValue { get; set; } = 30000;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvP, Name = "快気を使用するHP%%")]
    public float RecuperateValue { get; set; } = 0.9f;

    [RotationConfig(CombatType.PvP, Name = "LBを使用します")]
    public bool UseLB { get; set; } = false;

    [Range(1, 66000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "LB:敵のHPは？")]
    public int LBValue { get; set; } = 50000;

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

        if (AquaveilEsuna && AquaveilPvP.CanUse(out action))
        {
            return true;
        }
        if (Player.HasStatus(false, StatusHelper.PurifyPvPStatuses))
        {
            if (AquaveilPvP.CanUse(out action))
            {
                if (AquaveilPvP.Target.Target == Player)
                {
                    return true;
                }
            }
        }

        if (PurifyPvP.CanUse(out action))
        {
            return true;
        }

        return base.EmergencyAbility(nextGCD, out action);
    }

    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.DefenseSingleAbility(nextGCD, out action);
        }

        return base.DefenseSingleAbility(nextGCD, out action);
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

        if (DiabrosisPvP.CanUse(out action))
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
            return base.DefenseSingleGCD(out action);
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

        if (Player.CurrentMp >= 2500 && HaelanPvP.CanUse(out action))
        {
            return true;
        }

        if (Player.HasStatus(true,StatusID.CureIiiReady) && CureIiiPvP.CanUse(out action))
        {
            return true;
        }

        if (CureIiPvP.CanUse(out action))
        {
            return true;
        }

        return base.HealSingleGCD(out action);
    }

    protected override bool GeneralGCD(out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.GeneralGCD(out action);
        }

        if (RecuperatePvP.CanUse(out action) && Player.GetHealthRatio() <= RecuperateValue)
        {
            return true;
        }

        if (CurrentTarget is not null && UseLB && InCombat && MyLimitBreakLevel >= 1 && Target.CurrentHp <= LBValue)
        {
            AfflatusPurgationPvP.CanUse(out action,skipAoeCheck: true);
            return true;
        }

        /*if (UseLB && InCombat && MyLimitBreakLevel >= 1 && Target.CurrentHp <= LBValue)
        {
            action = AfflatusPurgationPvP;
            return true;
        }*/

        if (Player.HasStatus(true, StatusID.CureIiiReady) && CureIiiPvP.CanUse(out action))
        {
            return true;
        }

        if (AfflatusMiseryPvP.CanUse(out action))
        {
            return true;
        }

        if (UseSeraph && SeraphStrikePvP.Cooldown.IsCoolingDown && MyAquaveilPvP.CanUse(out action))
        {
            return true;
        }

        if (UseSeraph && Target.CurrentHp <= SeraphValue  && MySeraphStrikePvP.CanUse(out action,skipAoeCheck: true))
        {
            return true;
        }

        if (Target.CurrentHp <= MiracleValue && MiracleOfNaturePvP.CanUse(out action))
        {
            return true;
        }

        if (GlareIvPvP.CanUse(out action))
        {
            return true;
        }

        if (GlareIiiPvP.CanUse(out action))
        {
            return true;
        }

        if ((!Player?.HasStatus(true, StatusID.Guard) ?? false) && (!Player?.HasStatus(true, StatusID.Sprint) ?? false) &&
            SprintPvP.CanUse(out action)) return true;

        return base.GeneralGCD(out action);
    }
    #endregion
}