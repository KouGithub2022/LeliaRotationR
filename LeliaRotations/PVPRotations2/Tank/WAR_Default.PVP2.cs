using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RebornRotations.PVPRotations.Tank;

[Rotation("Lelia Default PVP", CombatType.PvP, GameVersion = "7.35")]
[SourceCode(Path = "main/RebornRotations/PVPRotations/Tank/WAR_Default.PvP.cs")]
//[Api(6)]
public sealed class WAR_DefaultPvP2 : WarriorRotation
{
    #region Configurations

    [RotationConfig(CombatType.PvP, Name = "Stop attacking while in Guard.")]
    public bool RespectGuard { get; set; } = true;

    [RotationConfig(CombatType.PvP, Name = "オンスロートを使います。")]
    public bool UseOnslaughtPvP { get; set; } = false;

    [Range(1, 66000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "オンスロートを使用するプレイヤーの最低HPは？")]
    public int OnslaughtValue { get; set; } = 40000;

    public static unsafe int LimitBreakValue => LimitBreakController.Instance()->CurrentUnits;
    public static unsafe int MyLimitBreakLevel => LimitBreakController.Instance()->BarUnits == 0 ? 0 : LimitBreakValue / LimitBreakController.Instance()->BarUnits;
    public static unsafe int LimitBreakMax => LimitBreakController.Instance()->BarCount;

    public static IBaseAction LBNamePvP = new BaseAction((ActionID)29083);

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvP, Name = "快気を使用するHP%%")]
    public float RecuperateValue { get; set; } = 0.75f;

    [RotationConfig(CombatType.PvP, Name = "LBを使用します")]
    public bool UseLBPvP { get; set; } = false;

    [Range(1, 66000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "LB:敵のHPは？")]
    public int LBValuePvP { get; set; } = 35000;

    [Range(1, 66000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "ランパートを使うプレイヤーのHPは？")]
    public int RampartValue { get; set; } = 40000;
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

        return base.EmergencyAbility(nextGCD, out action);
    }

    [RotationDesc(ActionID.BloodwhettingPvP)]
    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.DefenseSingleAbility(nextGCD, out action);
        }

        if (RampartPvP.CanUse(out action))
        {
            return true;
        }

        if (InCombat && BloodwhettingPvP.CanUse(out action))
        {
            return true;
        }

        return base.DefenseSingleAbility(nextGCD, out action);
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

        if (InCombat && BloodwhettingPvP.CanUse(out action))
        {
            return true;
        }

        if (InCombat && !RampartPvP.Cooldown.IsCoolingDown && Player.CurrentHp <= RampartValue && RampartPvP.CanUse(out action))
        {
            return true;
        }

        if (PrimalWrathPvP.CanUse(out action,skipAoeCheck: true))
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

        //Ability
        if (InCombat && !BlotaPvP.Cooldown.IsCoolingDown && Target.DistanceToPlayer() < 20 && 
            !Player.HasStatus(true,(StatusID.InnerChaosReady)) && nextGCD.IsTheSameTo(false,(ActionID)PrimalRendPvP.ID))
        {
            BlotaPvP.CanUse(out action);
            return true;
        }

        if (InCombat && OrogenyPvP.CanUse(out action))
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
            if (LBNamePvP.CanUse(out action))
            {
                return true;
            }
        }

        /*if (CurrentTarget is not null && UseLB && InCombat && MyLimitBreakLevel >= 1 && Target.CurrentHp <= LBValue)
        {
            PrimalScreamPvP.CanUse(out action);
            return true;
        }*/

        //Ability
        if (InCombat && UseOnslaughtPvP && Target.DistanceToPlayer() < 20 && 
            !OnslaughtPvP.Cooldown.IsCoolingDown && Player.CurrentHp >= OnslaughtValue)
        {
            OnslaughtPvP.CanUse(out action);
            return true;
        }

        //Ability
        if (InCombat && OrogenyPvP.CanUse(out action))
        {
            return true;
        }

        if (InnerChaosPvP.CanUse(out action)) return true;

        //Ability
        /*if (InCombat && !BlotaPvP.Cooldown.IsCoolingDown && Target.DistanceToPlayer() < 20 && !Player.HasStatus(true,(StatusID.InnerChaosReady)))
        {
            BlotaPvP.CanUse(out action);
            return true;
        }*/

        if (InnerChaosPvP.CanUse(out action))
        {
            return true;
        }

        if (PrimalRuinationPvP.CanUse(out action))
        {
            return true;
        }

        if (PrimalRendPvP.CanUse(out action))
        {
            return true;
        }

        if (ChaoticCyclonePvP.CanUse(out action, skipAoeCheck: true))
        {
            return true;
        }

        if (FellCleavePvP.CanUse(out action))
        {
            return true;
        }

        if (StormsPathPvP.CanUse(out action))
        {
            return true;
        }

        if (MaimPvP.CanUse(out action))
        {
            return true;
        }

        if (HeavySwingPvP.CanUse(out action))
        {
            return true;
        }

        if ((!Player?.HasStatus(true, StatusID.Guard) ?? false) && (!Player?.HasStatus(true, StatusID.Sprint) ?? false) &&
            SprintPvP.CanUse(out action)) return true;

        return base.GeneralGCD(out action);
    }
    #endregion
}