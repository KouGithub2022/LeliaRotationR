using ECommons.Automation;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RebornRotations.PVPRotations.Melee;

[Rotation("Lelia Default PVP_Test", CombatType.PvP, GameVersion = "7.35")]
[SourceCode(Path = "main/RebornRotations/PVPRotations/Tank/NIN_Default.PvP.cs")]
//[Api(6)]
public sealed class NIN_DefaultPvPTest : NinjaRotation
{
    //public static IBaseAction MyFumaShurikenPvP = new BaseAction((ActionID)29505);

    #region Configurations

    [RotationConfig(CombatType.PvP, Name = "Stop attacking while in Guard.")]
    public bool RespectGuard { get; set; } = true;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvP, Name = "Player health threshold needed for Bloodbath use")]
    public float BloodBathPvPPercent { get; set; } = 0.75f;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvP, Name = "Enemy health threshold needed for Smite use")]
    public float SmitePvPPercent { get; set; } = 0.25f;

    [RotationConfig(CombatType.PvP, Name = "縮地を使用します(現在使用できません…連打はしますw)")]
    public bool UseShukuchi { get; set; } = false;

    [Range(1, 66000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "縮地:敵のHPは？")]
    public int ShukuchiValue { get; set; } = 30000;

    public static unsafe int LimitBreakValue => LimitBreakController.Instance()->CurrentUnits;
    public static unsafe int MyLimitBreakLevel => LimitBreakController.Instance()->BarUnits == 0 ? 0 : LimitBreakValue / LimitBreakController.Instance()->BarUnits;
    public static unsafe int LimitBreakMax => LimitBreakController.Instance()->BarCount;
    public static IBaseAction LBNamePvP = new BaseAction((ActionID)29515);

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvP, Name = "快気を使用するHP%%")]
    public float RecuperateValue { get; set; } = 0.75f;

    [RotationConfig(CombatType.PvP, Name = "LBを使用します")]
    public bool UseLBPvP { get; set; } = false;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvP, Name = "LB:敵のHPは？")]
    public float LBValuePvP { get; set; } = 0.45f;
    #endregion

    #region oGCDs
    protected override bool EmergencyAbility(IAction nextGCD, out IAction? action)
    {
        if (Player.HasStatus(true, StatusID.Hidden_1316))
        {
            return base.EmergencyAbility(nextGCD, out action);
        }

        //快気
        if (RecuperatePvP.CanUse(out action) && Player.GetHealthRatio() <= RecuperateValue)
        {
            return true;
        }

        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.EmergencyAbility(nextGCD, out action);
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
        if (Player.HasStatus(true, StatusID.Hidden_1316))
        {
            return base.DefenseSingleAbility(nextGCD, out action);
        }

        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.DefenseSingleAbility(nextGCD, out action);
        }

        return base.DefenseSingleAbility(nextGCD, out action);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? action)
    {
        if (Player.HasStatus(true, StatusID.Hidden_1316))
        {
            return base.AttackAbility(nextGCD, out action);
        }

        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.AttackAbility(nextGCD, out action);
        }

        //快気
        if (RecuperatePvP.CanUse(out action) && Player.GetHealthRatio() <= RecuperateValue)
        {
            return true;
        }

        //バースト
        if (CurrentTarget is not null && !Player.HasStatus(true, StatusID.ThreeMudra))
        {
            if (BunshinPvP.CanUse(out action, usedUp: true))
            {
                return true;
            }
        }

        if (CurrentTarget is not null && !Player.HasStatus(true, StatusID.ThreeMudra) && BunshinPvP.Cooldown.IsCoolingDown)
        {
            if (ThreeMudraPvP.CanUse(out action, usedUp: true))
            {
               return true;
            }
        }

        if (CurrentTarget is not null && HyoshoRanryuPvP.CanUse(out action))
        {
            return true;
        }

        if (CurrentTarget is not null && !ShukuchiPvP.Cooldown.IsCoolingDown && BunshinPvP.Cooldown.IsCoolingDown)
        {
            Chat.ExecuteCommand($"/pvpac {ShukuchiPvP.Name} <t>");
            //ShukuchiPvP.CanUse(out action, skipAoeCheck: true);
            //return true;
        }

        if (CurrentTarget is not null && Player.HasStatus(true, StatusID.Hidden_1316))
        {
            return AssassinatePvP.CanUse(out action);
        }

        //バーストここまで










        if (DokumoriPvP.CanUse(out action))
        {
            return true;
        }


        return base.AttackAbility(nextGCD, out action);
    }

    #endregion

    #region GCDs
    protected override bool GeneralGCD(out IAction? action)
    {
        if (Player.HasStatus(true, StatusID.Hidden_1316))
        {
            return AssassinatePvP.CanUse(out action);
        }

        //快気
        if (RecuperatePvP.CanUse(out action) && Player.GetHealthRatio() <= RecuperateValue)
        {
            return true;
        }

        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.GeneralGCD(out action);
        }

        if (CurrentTarget is not null && InCombat && UseLBPvP && Target.DistanceToPlayer() <= 25 && MyLimitBreakLevel == 1 && Target.GetHealthRatio() <= LBValuePvP)
        {
            if (LBNamePvP.CanUse(out action))
            {
                return true;
            }
        }

        if (CurrentTarget is not null && UseLBPvP && Player.HasStatus(true,StatusID.UnsealedSeitonTenchu) && LBNamePvP.CanUse(out action)) return true;

        if (FumaShurikenPvP.Cooldown.CurrentCharges > 0 && FumaShurikenPvP.CanUse(out action, usedUp: true))
        {
            //Chat.ExecuteCommand("/pvpac 風魔手裏剣");
            //FumaShurikenPvP.CanUse(out action, usedUp: true);
            return true;
        }

        if (ZeshoMeppoPvP.CanUse(out action))
        {
            return true;
        }

        if (Player.GetHealthRatio() < .5)
        {
            if (MeisuiPvP.CanUse(out action))
            {
                return true;
            }
        }

        if (ForkedRaijuPvP.CanUse(out action))
        {
            return true;
        }

        if (FleetingRaijuPvP.CanUse(out action))
        {
            return true;
        }

        if (GokaMekkyakuPvP.CanUse(out action))
        {
            return true;
        }



        if (Player.WillStatusEnd(1, true, StatusID.ThreeMudra) && HutonPvP.CanUse(out action))
        {
            return true;
        }

        if (AeolianEdgePvP.CanUse(out action))
        {
            return true;
        }

        if (GustSlashPvP.CanUse(out action))
        {
            return true;
        }

        if (SpinningEdgePvP.CanUse(out action))
        {
            return true;
        }

        if ((!Player?.HasStatus(true, StatusID.Guard) ?? false) && (!Player?.HasStatus(true, StatusID.Sprint) ?? false) &&
            SprintPvP.CanUse(out action)) return true;

        return base.GeneralGCD(out action);
    }
    #endregion
}