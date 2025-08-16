using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RebornRotations.PVPRotations.Melee;

[Rotation("Test Default PVP", CombatType.PvP, GameVersion = "7.25")]
[SourceCode(Path = "main/RebornRotations/PVPRotations/Tank/NIN_Default.PvP.cs")]
//[Api(6)]
public sealed class NIN_DefaultPvP2 : NinjaRotation
{
    public static IBaseAction SeitonTenchuPvP = new BaseAction((ActionID)29515);
    public static unsafe int LimitBreakValue => LimitBreakController.Instance()->CurrentUnits;
    public static unsafe int MyLimitBreakLevel => LimitBreakController.Instance()->BarUnits == 0 ? 0 : LimitBreakValue / LimitBreakController.Instance()->BarUnits;

    #region Configurations

    [RotationConfig(CombatType.PvP, Name = "Use Purify")]
    public bool UsePurifyPvP { get; set; } = true;

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

    [RotationConfig(CombatType.PvP, Name = "LBを使用します")]
    public bool UseLB { get; set; } = false;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvP, Name = "LB:敵のHP%%は？")]
    public float LBValue { get; set; } = 0.5f;

    #endregion

    #region Standard PVP Utilities
    private bool DoPurify(out IAction? action)
    {
        action = null;
        if (!UsePurifyPvP)
        {
            return false;
        }

        List<int> purifiableStatusesIDs = new()
        {
            // Stun, DeepFreeze, HalfAsleep, Sleep, Bind, Heavy, Silence
            1343, 3219, 3022, 1348, 1345, 1344, 1347
        };

        return purifiableStatusesIDs.Any(id => Player.HasStatus(false, (StatusID)id)) && PurifyPvP.CanUse(out action);
    }
    #endregion

    #region oGCDs
    protected override bool EmergencyAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        if (Player.HasStatus(true, StatusID.Hidden_1316))
        {
            return false;
        }

        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        if (DoPurify(out action))
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

        if (SmitePvP.CanUse(out action) && CurrentTarget?.GetHealthRatio() <= SmitePvPPercent)
        {
            return true;
        }


        return base.EmergencyAbility(nextGCD, out action);
    }

    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        if (Player.HasStatus(true, StatusID.Hidden_1316))
        {
            return false;
        }

        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        return base.DefenseSingleAbility(nextGCD, out action);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        if (Player.HasStatus(true, StatusID.Hidden_1316))
        {
            return false;
        }

        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        if (DokumoriPvP.CanUse(out action))
        {
            return true;
        }

        if (BunshinPvP.CanUse(out action) && !Player.HasStatus(true, StatusID.ThreeMudra) && HasHostilesInMaxRange)
        {
            return true;
        }

        if (ThreeMudraPvP.CanUse(out action, usedUp: true) && !Player.HasStatus(true, StatusID.ThreeMudra) && HasHostilesInMaxRange)
        {
            return true;
        }

        if (UseShukuchi && Target.CurrentHp <= ShukuchiValue /*&& ShukuchiPvP.CanUse(out action,skipAoeCheck: true)) return true;*/)
        {
            ShukuchiPvP.CanUse(out action, skipAoeCheck: true);
            return true;
        }

        return base.AttackAbility(nextGCD, out action);
    }

    #endregion

    #region GCDs
    protected override bool GeneralGCD(out IAction? action)
    {
        action = null;
        if (Player.HasStatus(true, StatusID.Hidden_1316))
        {
            return AssassinatePvP.CanUse(out action);
        }

        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        if (UseLB && Player.HasStatus(true,StatusID.UnsealedSeitonTenchu) && SeitonTenchuPvP.CanUse(out action)) return true;
        if (UseLB && MyLimitBreakLevel == 1 &&  Target.GetHealthRatio() <= LBValue && SeitonTenchuPvP.CanUse(out action, skipAoeCheck: true, usedUp: true)) return true;

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

        if (HyoshoRanryuPvP.CanUse(out action))
        {
            return true;
        }

        if (Player.WillStatusEnd(1, true, StatusID.ThreeMudra) && HutonPvP.CanUse(out action))
        {
            return true;
        }

        if (FumaShurikenPvP.CanUse(out action, usedUp: true))
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