using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RebornRotations.PVPRotations.Tank;

[Rotation("with Wrath Lelia PVP", CombatType.PvP, GameVersion = "7.25")]
[SourceCode(Path = "main/RebornRotations/PVPRotations/Tank/WAR_Default.PvP.cs")]
//[Api(6)]
public sealed class WAR_DefaultPvPWrath : WarriorRotation
{
    public static IBaseAction PrimalScreamPvP = new BaseAction((ActionID)29083);
    public static unsafe int LimitBreakValue => LimitBreakController.Instance()->CurrentUnits;
    public static unsafe int MyLimitBreakLevel => LimitBreakController.Instance()->BarUnits == 0 ? 0 : LimitBreakValue / LimitBreakController.Instance()->BarUnits;

    #region Configurations

    [RotationConfig(CombatType.PvP, Name = "Use Purify")]
    public bool UsePurifyPvP { get; set; } = true;

    [RotationConfig(CombatType.PvP, Name = "Stop attacking while in Guard.")]
    public bool RespectGuard { get; set; } = true;

    [RotationConfig(CombatType.PvP, Name = "オンスロートを使います。")]
    public bool UseOnslaughtPvP { get; set; } = false;

    [Range(1, 66000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "オンスロートを使用するプレイヤーの最低HPは？")]
    public int OnslaughtValue { get; set; } = 40000;

    [RotationConfig(CombatType.PvP, Name = "LBを使用します")]
    public bool UseLB { get; set; } = false;

    /*[Range(1, 66000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "ランパートを使うプレイヤーのHPは？")]
    public int RampartValue { get; set; } = 40000;*/

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

        return DoPurify(out action) || base.EmergencyAbility(nextGCD, out action);
    }
    [RotationDesc(ActionID.BloodwhettingPvP)]
    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        /*if (RampartPvP.CanUse(out action))
        {
            return true;
        }*/

        if (InCombat && BloodwhettingPvP.CanUse(out action))
        {
            return true;
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

        if (InCombat && BloodwhettingPvP.CanUse(out action))
        {
            return true;
        }

        /*if (InCombat && !RampartPvP.Cooldown.IsCoolingDown && Player.CurrentHp <= RampartValue && RampartPvP.CanUse(out action))
        {
            return true;
        }*/

        if (UseOnslaughtPvP && Target.DistanceToPlayer() < 20 && !OnslaughtPvP.Cooldown.IsCoolingDown && Player.CurrentHp  >= OnslaughtValue)
        {
            OnslaughtPvP.CanUse(out action);
            return true;
        }

        if (PrimalWrathPvP.CanUse(out action,skipAoeCheck: true))
        {
            return true;
        }

        /*if (RampagePvP.CanUse(out action))
        {
            return true;
        }

        if (FullSwingPvP.CanUse(out action))
        {
            return true;
        }*/

        if (BlotaPvP.CanUse(out action))
        {
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
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        if (UseLB && InCombat && MyLimitBreakLevel >= 1 && PrimalScreamPvP.CanUse(out action,skipAoeCheck: true)) return true;

        /*if (InnerChaosPvP.CanUse(out action))
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
        }*/

        if (ChaoticCyclonePvP.CanUse(out action,skipAoeCheck: true))
        {
            return true;
        }

        /*if (FellCleavePvP.CanUse(out action))
        {
            return true;
        }*/

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

        return base.GeneralGCD(out action);
    }
    #endregion
}