using ExCSS;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace RebornRotations.PVPRotations.Melee;

[Rotation("Lelia Default PVP", CombatType.PvP, GameVersion = "7.25")]
[SourceCode(Path = "main/RebornRotations/PVPRotations/Melee/DRG_Default.PvP.cs")]
//[Api(6)]
public sealed class DRG_DefaultPvP2 : DragoonRotation
{
    public static IBaseAction SkyHighPvP = new BaseAction((ActionID)29497);
    public static IBaseAction SkyShatterPvP { get; } = new BaseAction((ActionID)29499);
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

    /*[RotationConfig(CombatType.PvP, Name = "Allow the use of high jump if there are enemies in melee range.")]
    public bool JumpYeet { get; set; } = true;*/

    [RotationConfig(CombatType.PvP, Name = "ハイジャンプを使います。")]
    public bool UseHighJumpPvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "イルーシブジャンプを使います。")]
    public bool UseElusiveJumpPvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "LBを使用します")]
    public bool UseLB { get; set; } = false;

    [Range(1, 66000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "LBを使う敵のHPは？")]
    public int LBValue { get; set; } = 30000;

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
            return false;
        }

        return base.EmergencyAbility(nextGCD, out action);
    }

    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        return base.DefenseSingleAbility(nextGCD, out action);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        if (UseElusiveJumpPvP && InCombat && HasHostilesInRange && !HighJumpPvP.Cooldown.IsCoolingDown && ElusiveJumpPvP.CanUse(out action))
        {
            return true;
        }

        if (UseHighJumpPvP && !HighJumpPvP.Cooldown.IsCoolingDown && ElusiveJumpPvP.Cooldown.IsCoolingDown)
        {
            HighJumpPvP.CanUse(out action);
            return true;
        }

        if (HorridRoarPvP.CanUse(out action))
        {
            return true;
        }

        if (GeirskogulPvP.CanUse(out action))
        {
            return true;
        }

        if (NastrondPvP.CanUse(out action))
        {
            return true;
        }

        return base.AttackAbility(nextGCD, out action);
    }

    protected override bool MoveForwardAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        if (HighJumpPvP.CanUse(out action))
        {
            return true;
        }

        return base.MoveForwardAbility(nextGCD, out action);
    }

    protected override bool MoveBackAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        if (ElusiveJumpPvP.CanUse(out action))
        {
            return true;
        }

        return base.MoveBackAbility(nextGCD, out action);
    }
    #endregion

    #region GCDs
    protected override bool GeneralGCD(out IAction? action)
    {
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        if (UseLB && InCombat && MyLimitBreakLevel >= 1 && Target.CurrentHp <= LBValue && 
            SkyHighPvP.CanUse(out action)) return true;

        if (WyrmwindThrustPvP.CanUse(out action))
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
        }

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