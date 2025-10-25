using ECommons.Automation;
using ExCSS;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using RotationSolver.Basic.Data;

namespace RebornRotations.PVPRotations.Melee;

[Rotation("Lelia Default PVP_Test", CombatType.PvP, GameVersion = "7.35")]
[SourceCode(Path = "main/RebornRotations/PVPRotations/Melee/DRG_Default.PvP.cs")]
//[Api(6)]
public sealed class DRG_DefaultPvPTest : DragoonRotation
{
    public static IBaseAction MyHighJumpPvP { get; } = new BaseAction((ActionID)29493);
    #region Configurations

    [RotationConfig(CombatType.PvP, Name = "Stop attacking while in Guard.")]
    public bool RespectGuard { get; set; } = true;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvP, Name = "Player health threshold needed for Bloodbath use")]
    public float BloodBathPvPPercent { get; set; } = 0.75f;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvP, Name = "Enemy health threshold needed for Smite use")]
    public float SmitePvPPercent { get; set; } = 0.25f;

    /*[RotationConfig(CombatType.PvP, Name = "Allow the use of high jump if there are enemies in melee range.")]
    public bool JumpYeet { get; set; } = true;*/

    [RotationConfig(CombatType.PvP, Name = "ハイジャンプを使います。")]
    public bool UseHighJumpPvP2 { get; set; } = false;

    [Range(1, 66000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "ハイジャンプを使用できる自分のHPは？")]
    public int HJValue2 { get; set; } = 50000;

    [RotationConfig(CombatType.PvP, Name = "イルーシブジャンプを使います。")]
    public bool UseElusiveJumpPvP { get; set; } = false;

    [Range(1, 66000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "ホリッドロアを使用する自分のHPは？")]
    public int HRValue { get; set; } = 50000;

    public static unsafe int LimitBreakValue => LimitBreakController.Instance()->CurrentUnits;
    public static unsafe int MyLimitBreakLevel => LimitBreakController.Instance()->BarUnits == 0 ? 0 : LimitBreakValue / LimitBreakController.Instance()->BarUnits;
    public static unsafe int LimitBreakMax => LimitBreakController.Instance()->BarCount;
    public static IBaseAction LBNamePvP1 = new BaseAction((ActionID)29497);
    public static IBaseAction LBNamePvP2 = new BaseAction((ActionID)29499);

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

        //快気
        if (RecuperatePvP.CanUse(out action) && Player.GetHealthRatio() <= RecuperateValue)
        {
            return true;
        }

        //バースト:ゲイルスコグル→天竜→スタークロッサー
        if (CurrentTarget is not null && nextGCD.IsTheSameTo(true, (ActionID)GeirskogulPvP.ID))
        {
            Chat.ExecuteCommand($"/pvpac {ElusiveJumpPvP.Name} <t>");
            /*if (ElusiveJumpPvP.CanUse(out action))
            {
                return true;
            }*/
        }

        if (CurrentTarget is not null && nextGCD.IsTheSameTo(true, (ActionID)WyrmwindThrustPvP.ID))
        {
            if (GeirskogulPvP.CanUse(out action))
            {
                return true;
            }

            if (NastrondPvP.CanUse(out action))
            {
                return true;
            }
        }

        if (CurrentTarget is not null && nextGCD.IsTheSameTo(true, (ActionID)StarcrossPvP.ID))
        {
            if (WyrmwindThrustPvP.CanUse(out action))
            {
                return true;
            }
        }

        if (CurrentTarget is not null && WyrmwindThrustPvP.Cooldown.IsCoolingDown)
        {
            if (StarcrossPvP.CanUse(out action))
            {
                return true;
            }
        }
        //----------------------------------
        /*if (CurrentTarget is not null && nextGCD.IsTheSameTo(true, (ActionID)HighJumpPvP.ID))
        {
            if (NastrondPvP.CanUse(out action, skipAoeCheck: true))
            {
                return true;
            }
        }*/

        if (CurrentTarget is not null && nextGCD.IsTheSameTo(true, (ActionID)HeavensThrustPvP.ID))
        {
            if (MyHighJumpPvP.CanUse(out action, skipAoeCheck: true))
            {
                return true;
            }
        }

        if (CurrentTarget is not null && HeavensThrustPvP.Cooldown.IsCoolingDown)
        {
            if (HeavensThrustPvP.CanUse(out action, skipAoeCheck: true))
            {
                return true;
            }
        }
        //----------------------------------





        //Ability
        if (GeirskogulPvP.CanUse(out action) && ElusiveJumpPvP.Cooldown.IsCoolingDown)
        {
            return true;
        }

        /*if (InCombat && HorridRoarPvP.CanUse(out action) && Player.CurrentHp <= HRValue)
        {
            return true;
        }

        if (NastrondPvP.CanUse(out action,skipAoeCheck: true))
        {
            return true;
        }*/

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

    protected override bool MoveBackAbility(IAction nextGCD, out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.MoveBackAbility(nextGCD, out action);
        }

        return base.MoveBackAbility(nextGCD, out action);
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

        if (CurrentTarget is not null && InCombat && UseLBPvP && Target.DistanceToPlayer() <= 25 && MyLimitBreakLevel == 1 && Target.CurrentHp <= LBValuePvP)
        {
            if (LBNamePvP1.CanUse(out action))
            {
                return true;
            }
        }

        if (CurrentTarget is not null && Player.HasStatus(true, StatusID.SkyHigh))
        {
            //Chat.ExecuteCommand($"/pvpac {LBNamePvP1.Name} <t>");
            if (LBNamePvP2.CanUse(out action, usedUp: true, skipAoeCheck: true))
            {
                return true; 
            }
        }



        /*if (CurrentTarget is not null && UseLB && InCombat && Player.HasStatus(true, StatusID.SkyHigh) && SkyShatterPvP.CanUse(out action,usedUp: true, skipAoeCheck: true)) { return true; }
        if (CurrentTarget is not null && UseLB && InCombat && Player.HasStatus(true, StatusID.SkyHigh) && SkyShatterPvP2.CanUse(out action, usedUp: true, skipAoeCheck: true)) { return true; }

        if (CurrentTarget is not null && UseLB && InCombat && MyLimitBreakLevel >= 1 && Target.CurrentHp <= LBValue &&
            SkyHighPvP.CanUse(out action)) { return true; }*/

        /*if (UseElusiveJumpPvP && InCombat && HasHostilesInRange
            && !Player.HasStatus(true, StatusID.Heavensent) && !Player.HasStatus(true, StatusID.LifeOfTheDragon)
            && ElusiveJumpPvP.CanUse(out action))
        {
            return true;
        }*/

        /*if (InCombat && UseHighJumpPvP2
            && (Player.CurrentHp >= HJValue2 || HasHostilesInRange) && MyHighJumpPvP.CanUse(out action))
        {
            return true;
        }

        if (NastrondPvP.CanUse(out action, skipAoeCheck: true))
        {
            return true;
        }

        if (WyrmwindThrustPvP.CanUse(out action,skipAoeCheck: true))
        {
            return true;
        }

        if (HeavensThrustPvP.CanUse(out action))
        {
            return true;
        }

        if (StarcrossPvP.CanUse(out action))
        {
            return true;
        }*/

        if (ChaoticSpringPvP.CanUse(out action))
        {
            return true;
        }

        if (DrakesbanePvP.CanUse(out action))
        {
            return true;
        }

        if (WheelingThrustPvP.CanUse(out action))
        {
            return true;
        }

        if (FangAndClawPvP.CanUse(out action))
        {
            return true;
        }

        if (RaidenThrustPvP.CanUse(out action))
        {
            return true;
        }

        if ((!Player?.HasStatus(true, StatusID.Guard) ?? false) && (!Player?.HasStatus(true, StatusID.Sprint) ?? false) &&
            SprintPvP.CanUse(out action)) return true;

        return base.GeneralGCD(out action);
    }
    #endregion
}