using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RebornRotations.PVPRotations.Tank;

[Rotation("Lelia Default PVP", CombatType.PvP, GameVersion = "7.25")]
[SourceCode(Path = "main/RebornRotations/PVPRotations/Tank/PLD_Default.PvP.cs")]
//[Api(6)]
public sealed class PLD_DefaultPvP2 : PaladinRotation
{
    public static IBaseAction PhalanxPvP = new BaseAction((ActionID)29069);
    public static unsafe int LimitBreakValue => LimitBreakController.Instance()->CurrentUnits;
    public static unsafe int MyLimitBreakLevel => LimitBreakController.Instance()->BarUnits == 0 ? 0 : LimitBreakValue / LimitBreakController.Instance()->BarUnits;
    
    #region Configurations

    [RotationConfig(CombatType.PvP, Name = "Use Purify")]
    public bool UsePurifyPvP { get; set; } = true;

    [RotationConfig(CombatType.PvP, Name = "Stop attacking while in Guard.")]
    public bool RespectGuard { get; set; } = true;

    [RotationConfig(CombatType.PvP, Name = "インタービーンを使います。")]
    public bool UseIntervenePvP { get; set; } = false;

    [Range(1, 64500, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "インタービーンを使用するプレイヤーの最低HPは？")]
    public int InterveneValue { get; set; } = 40000;

    [RotationConfig(CombatType.PvP, Name = "LBを使用します")]
    public bool UseLB { get; set; } = false;

    [Range(1, 64500, ConfigUnitType.Pixels, 1)]
    [RotationConfig(CombatType.PvP, Name = "ランパートを使うプレイヤーのHPは？")]
    public int RampartValue { get; set; } = 40000;

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

        return base.EmergencyAbility(nextGCD, out action);
    }
    [RotationDesc(ActionID.BloodwhettingPvP)]
    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        if (RampartPvP.CanUse(out action))
        {
            return true;
        }

        if (InCombat && !HolySheltronPvP.Cooldown.IsCoolingDown)
        {
            HolySheltronPvP.CanUse(out action);
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
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        if (UseLB && InCombat && MyLimitBreakLevel >= 1 && PhalanxPvP.CanUse(out action,skipAoeCheck: true)) return true;

        //Ability
        if (UseIntervenePvP && Target.DistanceToPlayer() < 20 && !IntervenePvP.Cooldown.IsCoolingDown && Player.CurrentHp > InterveneValue) //Ability
        {
            IntervenePvP.CanUse(out action);
            return true;
        }

        if (ImperatorPvP.CanUse(out action, skipAoeCheck: true)) //Ability
        {
            return true;
        }

        if (InCombat && !HolySheltronPvP.Cooldown.IsCoolingDown && HolySheltronPvP.CanUse(out action)) //Ability
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

        if (HolySpiritPvP.CanUse(out action))
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