using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RebornRotations.PVPRotations.Tank;

[Rotation("Lelia Default PVP", CombatType.PvP, GameVersion = "7.35")]
[SourceCode(Path = "main/RebornRotations/PVPRotations/Tank/PLD_Default.PvP.cs")]
//[Api(6)]
public sealed class PLD_DefaultPvP2 : PaladinRotation
{
    public static IBaseAction MyIntervenePvP = new BaseAction((ActionID)29065);
  
    #region Configurations

    [RotationConfig(CombatType.PvP, Name = "Stop attacking while in Guard.")]
    public bool RespectGuard { get; set; } = true;

    [RotationConfig(CombatType.PvP, Name = "インタービーンを使います。")]
    public bool UseIntervenePvP { get; set; } = false;

    [Range(1, 64500, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "インタービーンを使用する敵のHPは？")]
    public int InterveneValue { get; set; } = 30000;

    [RotationConfig(CombatType.PvP, Name = "ホーリースピリットを使用する自分のHPは？")]
    public int HolySpiritValue { get; set; } = 60000;

    [RotationConfig(CombatType.PvP, Name = "ホーリーシェルトロンを使用する自分のHPは？")]
    public int HolyShelValue { get; set; } = 60000;

    [RotationConfig(CombatType.PvP, Name = "ガーディアンを使います。")]
    public bool UseGuardianPvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "ガーディアンを使用する自分のHPは？(未検証)")]
    public int GuardianValue { get; set; } = 60000;

    public static unsafe int LimitBreakValue => LimitBreakController.Instance()->CurrentUnits;
    public static unsafe int MyLimitBreakLevel => LimitBreakController.Instance()->BarUnits == 0 ? 0 : LimitBreakValue / LimitBreakController.Instance()->BarUnits;
    public static unsafe int LimitBreakMax => LimitBreakController.Instance()->BarCount;

    public static IBaseAction LBNamePvP = new BaseAction((ActionID)29069);

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvP, Name = "快気を使用するHP%%")]
    public float RecuperateValue { get; set; } = 0.75f;

    [RotationConfig(CombatType.PvP, Name = "LBを使用します")]
    public bool UseLBPvP { get; set; } = false;

    [Range(1, 66000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "LB:プレイヤーのHPは？")]
    public int LBValuePvP { get; set; } = 60000;

    [Range(1, 64500, ConfigUnitType.Pixels, 1)]
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

        if (InCombat && !HolySheltronPvP.Cooldown.IsCoolingDown &&
            Player.CurrentHp <= HolyShelValue && HolySheltronPvP.CanUse(out action)) //Ability
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

        if (GuardPvP.CanUse(out action) && nextGCD.IsTheSameTo(true,(ActionID)GuardianPvP.ID))
        {
            return true;
        }

        if (InCombat && !RampartPvP.Cooldown.IsCoolingDown && Player.CurrentHp <= RampartValue && RampartPvP.CanUse(out action))
        {
            return true;
        }

        /*if (InCombat && !HolySheltronPvP.Cooldown.IsCoolingDown)
        {
            HolySheltronPvP.CanUse(out action);
            return true;
        }

        if (UseIntervenePvP && Target.DistanceToPlayer() < 20 && !IntervenePvP.Cooldown.IsCoolingDown && Player.CurrentHp > InterveneValue)
        { 
            IntervenePvP.CanUse(out action);
            return true;
        }*/

        if (RampagePvP.CanUse(out action))
        {
            return true;
        }

        if (FullSwingPvP.CanUse(out action))
        {
            return true;
        }

        /*if (ImperatorPvP.CanUse(out action,skipAoeCheck: true))
        {
            return true;
        }*/

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

        if (CurrentTarget is not null && InCombat && UseLBPvP && Target.DistanceToPlayer() <= 5 && MyLimitBreakLevel == 1 && Player.CurrentHp <= LBValuePvP)
        {
            if (LBNamePvP.CanUse(out action, skipAoeCheck: true))
            {
                return true;
            }
        }

        /*if (CurrentTarget is not null && UseLB && InCombat && MyLimitBreakLevel >= 1 && 
            Player.CurrentHp <= LBValue && PhalanxPvP.CanUse(out action,skipAoeCheck: true)) return true;*/

        //Ability
        if (GuardianPvP.CanUse(out action) && InCombat && UseGuardianPvP && 
            !GuardPvP.Cooldown.IsCoolingDown && Player.CurrentHp >= GuardianValue && GuardianPvP.Target.Target.GetHealthRatio() <= 0.5f)
        {
            return true;
        }

        //Ability
        if (UseIntervenePvP && !MyIntervenePvP.Cooldown.IsCoolingDown &&
            (Target.DistanceToPlayer() > 5 || Target.CurrentHp <= InterveneValue) && 
            MyIntervenePvP.CanUse(out action,skipAoeCheck: true)) //Ability
        {
            //IntervenePvP.CanUse(out action);
            return true;
        }

        if (HolySpiritPvP.CanUse(out action, usedUp: true) && Player.CurrentHp <= HolySpiritValue)
        {
            return true;
        }

        if (ImperatorPvP.CanUse(out action, skipAoeCheck: true)) //Ability
        {
            return true;
        }

        if (InCombat && !HolySheltronPvP.Cooldown.IsCoolingDown && 
            Player.CurrentHp <= HolyShelValue && HolySheltronPvP.CanUse(out action)) //Ability
        {
            return true;
        }

        if (BladeOfFaithPvP.CanUse(out action))
        {
            return true;
        }

        if (BladeOfTruthPvP.CanUse(out action))
        {
            return true;
        }

        if (BladeOfValorPvP.CanUse(out action))
        {
            return true;
        }

        if (ConfiteorPvP.CanUse(out action))
        {
            return true;
        }

        if (ShieldSmitePvP.CanUse(out action))
        {
            return true;
        }

        if (AtonementPvP.CanUse(out action))
        {
            return true;
        }

        if (SupplicationPvP.CanUse(out action))
        {
            return true;
        }

        if (SepulchrePvP.CanUse(out action))
        {
            return true;
        }

        if (RoyalAuthorityPvP.CanUse(out action))
        {
            return true;
        }

        if (RiotBladePvP.CanUse(out action))
        {
            return true;
        }

        if (FastBladePvP.CanUse(out action))
        {
            return true;
        }

        if ((!Player?.HasStatus(true, StatusID.Guard) ?? false) && (!Player?.HasStatus(true, StatusID.Sprint) ?? false) &&
            SprintPvP.CanUse(out action)) return true;

        return base.GeneralGCD(out action);
    }
    #endregion
}