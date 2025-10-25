using ECommons.Automation;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RebornRotations.PVPRotations.Melee;

[Rotation("Lelia Default PVP_Test", CombatType.PvP, GameVersion = "7.35")]
[SourceCode(Path = "main/RebornRotations/PVPRotations/Melee/MNK_Default.PVP.cs")]
//[Api(6)]
public sealed class MNK_DefaultPvPTest : MonkRotation
{
    #region Configurations

    [RotationConfig(CombatType.PvP, Name = "Stop attacking while in Guard.")]
    public bool RespectGuard { get; set; } = true;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvP, Name = "Player health threshold needed for Bloodbath use")]
    public float BloodBathPvPPercent { get; set; } = 0.75f;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvP, Name = "Enemy health threshold needed for Smite use")]
    public float SmitePvPPercent { get; set; } = 0.25f;

    [RotationConfig(CombatType.PvP, Name = "抜重歩法を使用します")]
    public bool UseThunderclap { get; set; } = false;

    [Range(0, 2, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "抜重歩法:残すチャージ数は？")]
    public int ThunderclapValue { get; set; } = 1;

    public static unsafe int LimitBreakValue => LimitBreakController.Instance()->CurrentUnits;
    public static unsafe int MyLimitBreakLevel => LimitBreakController.Instance()->BarUnits == 0 ? 0 : LimitBreakValue / LimitBreakController.Instance()->BarUnits;
    public static unsafe int LimitBreakMax => LimitBreakController.Instance()->BarCount;
    public static IBaseAction LBNamePvP = new BaseAction((ActionID)29485);

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvP, Name = "快気を使用するHP%%")]
    public float RecuperateValue { get; set; } = 0.75f;

    [RotationConfig(CombatType.PvP, Name = "LBを使用します")]
    public bool UseLBPvP { get; set; } = false;

    [Range(1, 66000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "LB:敵のHPは？")]
    public int LBValuePvP { get; set; } = 24000;

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

        if (RiddleOfEarthPvP.CanUse(out action) && InCombat && Player.GetHealthRatio() < 0.8)
        {
            return true;
        }

        if (EarthsReplyPvP.CanUse(out action))
        {
            if (Player.HasStatus(true, StatusID.EarthResonance) && Player.WillStatusEnd(1, true, StatusID.EarthResonance))
            {
                if (Player.GetHealthRatio() < 0.5 || Player.WillStatusEnd(1, true, StatusID.EarthResonance))
                {
                    return true;
                }
            }
        }

        if (BloodbathPvP.CanUse(out action) && Player.GetHealthRatio() < BloodBathPvPPercent)
        {
            return true;
        }

        if (SwiftPvP.CanUse(out action))
        {
            return true;
        }

        if (SmitePvP.CanUse(out action) && SmitePvP.Target.Target.GetHealthRatio() <= SmitePvPPercent)
        {
            return false;
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

        //小バースト：鳳凰の舞→絶空拳→乾坤闘気弾
        if (CurrentTarget is not null && RisingPhoenixPvP.CanUse(out action, skipAoeCheck: true, usedUp: true) && InCombat && Target.DistanceToPlayer() <= 5 && nextGCD.IsTheSameTo(true,(ActionID)WindsReplyPvP.ID) && !Player.HasStatus(true,StatusID.FiresRumination))
        {
            return true;
        }

        if (WindsReplyPvP.CanUse(out action, skipAoeCheck: true, usedUp: true) && InCombat && Target.DistanceToPlayer() <= 5 && nextGCD.IsTheSameTo(true,(ActionID)FiresReplyPvP.ID))
        {
            return true;
        }

        if (FiresReplyPvP.CanUse(out action, usedUp: true) && InCombat && Target.DistanceToPlayer() <= 5 && WindsReplyPvP.Cooldown.IsCoolingDown && nextGCD.IsTheSameTo(true, (ActionID)LBNamePvP.ID))
        {
            return true;
        }
        //小バストここまで

        if (CurrentTarget is not null && RisingPhoenixPvP.CanUse(out action, skipAoeCheck: true, usedUp: true) && InCombat && Target.DistanceToPlayer() <= 5 && nextGCD.IsTheSameTo(true, (ActionID)LBNamePvP.ID) && !Player.HasStatus(true, StatusID.FiresRumination))
        {
            return true;
        }

        if (CurrentTarget is not null && !Player.HasStatus(true, StatusID.SnakesBane) && InCombat && UseLBPvP && Target.DistanceToPlayer() <= 20 && MyLimitBreakLevel == 1 && Target.CurrentHp <= LBValuePvP)
        {
            if (LBNamePvP.CanUse(out action, skipAoeCheck: true, usedUp: true))
            {
                return true;
            }
        }

        /*if (RisingPhoenixPvP.CanUse(out action, usedUp: true) && HasHostilesInRange)
        {
            return true;
        }*/

        /*if (NumberOfHostilesInRangeOf(6) > 0 && RisingPhoenixPvP.CanUse(out action, usedUp: true) && InCombat)
        {
            return true;
        }*/

        if (EarthsReplyPvP.CanUse(out action, usedUp: true) && HasHostilesInRange)
        {
            return true;
        }

        /*if ((nextGCD.IsTheSameTo(true,(ActionID)PhantomRushPvP.ID)))
        //if (WindsReplyPvP.CanUse(out action))
        {
            WindsReplyPvP.CanUse(out action, skipAoeCheck: true, usedUp: true);
            return true;
        }*/


        if (CurrentTarget is not null && InCombat && UseThunderclap && ThunderclapPvP.Cooldown.CurrentCharges > ThunderclapValue &&  
            !Player.HasStatus(true, StatusID.Thunderclap_3173) && ThunderclapPvP.CanUse(out action,usedUp: true)) return true;

        return base.AttackAbility(nextGCD, out action);
    }

    protected override bool MoveForwardAbility(IAction nextGCD, out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.MoveForwardAbility(nextGCD, out action);
        }

        return base.MoveForwardAbility(nextGCD, out action);
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

        if (CurrentTarget is not null && !Player.HasStatus(true, StatusID.SnakesBane) && InCombat && UseLBPvP && Target.DistanceToPlayer() <= 20 && MyLimitBreakLevel == 1 && Target.CurrentHp <= LBValuePvP)
        {
            if (LBNamePvP.CanUse(out action, skipAoeCheck: true, usedUp: true))
            {
                return true;
            }
        }

        //if (CurrentTarget is not null && !Player.HasStatus(true,StatusID.SnakesBane) && UseLB && MyLimitBreakLevel == 1 && Target.CurrentHp <= LBValue && MeteodrivePvP.CanUse(out action, skipAoeCheck: true, usedUp: true)) return true;

        if (PhantomRushPvP.CanUse(out action))
        {
            return true;
        }

        /*if (FiresReplyPvP.CanUse(out action))
        {
            return true;
        }*/

        if (CurrentTarget is not null && RisingPhoenixPvP.CanUse(out action) && !WindsReplyPvP.Cooldown.IsCoolingDown && !Player.HasStatus(true, StatusID.FiresRumination))
        {
            return true;
        }

        if (WindsReplyPvP.CanUse(out action))
        {
            return true;
        }

        if (FlintsReplyPvP.CanUse(out action, usedUp: true) && WindsReplyPvP.Cooldown.IsCoolingDown)
        {
            return true;
        }

        if (PouncingCoeurlPvP.CanUse(out action))
        {
            return true;
        }

        if (RisingRaptorPvP.CanUse(out action))
        {
            return true;
        }

        if (LeapingOpoPvP.CanUse(out action))
        {
            return true;
        }

        if (DemolishPvP.CanUse(out action))
        {
            return true;
        }

        if (TwinSnakesPvP.CanUse(out action))
        {
            return true;
        }

        if (DragonKickPvP.CanUse(out action))
        {
            return true;
        }

        if ((!Player?.HasStatus(true, StatusID.Guard) ?? false) && (!Player?.HasStatus(true, StatusID.Sprint) ?? false) &&
            SprintPvP.CanUse(out action)) return true;

        return base.GeneralGCD(out action);
    }
    #endregion
}