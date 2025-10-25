using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RebornRotations.PVPRotations.Tank;

[Rotation("Lelia Default PVP", CombatType.PvP, GameVersion = "7.35")]
[SourceCode(Path = "main/RebornRotations/PVPRotations/Tank/DRK_Default.PvP.cs")]
//[Api(6)]
public sealed class DRK_DefaultPvP2 : DarkKnightRotation
{
    #region Configurations

    [Range(1, 100, ConfigUnitType.Percent, 1)]
    [RotationConfig(CombatType.PvP, Name = "Shadowbringer Threshold")]
    public int ShadowbringerThreshold { get; set; } = 50;

    [RotationConfig(CombatType.PvP, Name = "Stop attacking while in Guard.")]
    public bool RespectGuard { get; set; } = true;

    [RotationConfig(CombatType.PvP, Name = "プランジカットを使います。")]
    public bool UsePlungePvP { get; set; } = false;

    [Range(1, 64500, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "プランジカットを使用するプレイヤーの最低HPは？")]
    public int PlungeValue { get; set; } = 40000;

    [Range(1, 66000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "ランパートを使うプレイヤーのHPは？")]
    public int RampartValue { get; set; } = 40000;
    public static unsafe int LimitBreakValue => LimitBreakController.Instance()->CurrentUnits;
    public static unsafe int MyLimitBreakLevel => LimitBreakController.Instance()->BarUnits == 0 ? 0 : LimitBreakValue / LimitBreakController.Instance()->BarUnits;
    public static unsafe int LimitBreakMax => LimitBreakController.Instance()->BarCount;

    public static IBaseAction LBNamePvP = new BaseAction((ActionID)29097);

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

        if (TheBlackestNightPvP.CanUse(out action) && TheBlackestNightPvP.Cooldown.CurrentCharges == 2 && InCombat)
        {
            return true;
        }

        return base.EmergencyAbility(nextGCD, out action);
    }

    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? act)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.DefenseSingleAbility(nextGCD, out act);
        }

        if (RampartPvP.CanUse(out act))
        {
            return true;
        }

        if (InCombat && TheBlackestNightPvP.CanUse(out act))
        {
            return true;
        }

        return base.DefenseSingleAbility(nextGCD, out act);
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

        if (UsePlungePvP && Target.DistanceToPlayer() < 20 && !PlungePvP.Cooldown.IsCoolingDown && Player.CurrentHp >= PlungeValue)
        {
            PlungePvP.CanUse(out action);
            return true;
        }

        if (InCombat && !RampartPvP.Cooldown.IsCoolingDown && Player.CurrentHp <= RampartValue)
        {
            RampartPvP.CanUse(out action);
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

        if (HasHostilesInRange && SaltedEarthPvP.CanUse(out action))
        {
            return true;
        }

        if (!Player.HasStatus(true, StatusID.Blackblood) && ((Player.GetHealthRatio() * 100) > ShadowbringerThreshold || Player.HasStatus(true, StatusID.DarkArts_3034)) && ShadowbringerPvP.CanUse(out action))
        {
            return true;
        }

        if (SaltAndDarknessPvP.CanUse(out action))
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

        if (CurrentTarget is not null && InCombat && UseLBPvP && Target.DistanceToPlayer() <= 5 && MyLimitBreakLevel == 1 && Target.CurrentHp <= LBValuePvP)
        {
            if (LBNamePvP.CanUse(out action, skipAoeCheck: true))
            {
                return true;
            }
        }

        //if (CurrentTarget is not null && InCombat && UseLB && InCombat && MyLimitBreakLevel >= 1 && EventidePvP.CanUse(out action,skipAoeCheck: true)) return true;

        if (DisesteemPvP.CanUse(out action))
        {
            return true;
        }

        if ((Player.GetHealthRatio() * 100) < 60 && ImpalementPvP.CanUse(out action))
        {
            return true;
        }

        if (TorcleaverPvP.CanUse(out action))
        {
            return true;
        }

        if (ComeuppancePvP.CanUse(out action))
        {
            return true;
        }

        if (ScarletDeliriumPvP.CanUse(out action))
        {
            return true;
        }

        if (SouleaterPvP.CanUse(out action))
        {
            return true;
        }

        if (SyphonStrikePvP.CanUse(out action))
        {
            return true;
        }

        if (HardSlashPvP.CanUse(out action))
        {
            return true;
        }

        return base.GeneralGCD(out action);
    }
    #endregion
}