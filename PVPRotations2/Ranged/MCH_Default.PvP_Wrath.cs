using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RebornRotations.PVPRotations.Ranged;

[Rotation("TEST PVP_Wrath", CombatType.PvP, GameVersion = "7.25")]
[SourceCode(Path = "main/RebornRotations/PVPRotations/Ranged/MCH_Default.PvP.cs")]
//[Api(6)]
public sealed class MCH_DefaultPvP_Wrath : MachinistRotation
{
    public static IBaseAction MarksmansSpitePvP = new BaseAction((ActionID)29415);
    public static unsafe int LimitBreakValue => LimitBreakController.Instance()->CurrentUnits;
    public static unsafe int MyLimitBreakLevel => LimitBreakController.Instance()->BarUnits == 0 ? 0 : LimitBreakValue / LimitBreakController.Instance()->BarUnits;

    #region Configurations

    [RotationConfig(CombatType.PvP, Name = "Use Purify")]
    public bool UsePurifyPvP { get; set; } = true;

    [RotationConfig(CombatType.PvP, Name = "Stop attacking while in Guard.")]
    public bool RespectGuard { get; set; } = true;

    [RotationConfig(CombatType.PvP, Name = "LBÇégópÇµÇ‹Ç∑")]
    public bool UseLB { get; set; } = false;

    [Range(1, 66000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "LB:ìGÇÃHPÇÕÅH")]
    public int LBValue { get; set; } = 35000;

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
        /*if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        if (DoPurify(out action))
        {
            return true;
        }

        if (InCombat && BraveryPvP.CanUse(out action))
        {
            return true;
        }

        if (InCombat && DervishPvP.CanUse(out action))
        {
            return true;
        }*/

        return base.EmergencyAbility(nextGCD, out action);
    }

    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        /*if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }*/

        return base.DefenseSingleAbility(nextGCD, out action);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        /*if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        if (AnalysisPvP.CanUse(out action, usedUp: true))
        {
            if (nextGCD.IsTheSameTo(false, ActionID.DrillPvP, ActionID.BioblasterPvP, ActionID.AirAnchorPvP, ActionID.ChainSawPvP))
            {
                return true;
            }
        }*/

        if (WildfirePvP.CanUse(out action))
        {
            if (Player.HasStatus(true, StatusID.Overheated_3149))
            {
                return true;
            }
        }

        if (BishopAutoturretPvP.CanUse(out action))
        {
            return true;
        }

        /*if (EagleEyeShotPvP.CanUse(out action))
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

        /*if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        if(UseLB && MyLimitBreakLevel == 1 && Target.CurrentHp <= LBValue && MarksmansSpitePvP.CanUse(out action)) return true;

        if (FullMetalFieldPvP.CanUse(out action))
        {
            return true;
        }

        if (BlazingShotPvP.CanUse(out action))
        {
            if (Player.HasStatus(true, StatusID.Overheated_3149) && !Player.HasStatus(true, StatusID.Analysis))
            {
                return true;
            }
        }

        if (DrillPvP.CanUse(out action, usedUp: true))
        {
            return true;
        }

        if (BioblasterPvP.CanUse(out action, usedUp: true))
        {
            return true;
        }

        if (AirAnchorPvP.CanUse(out action, usedUp: true))
        {
            return true;
        }

        if (ChainSawPvP.CanUse(out action, usedUp: true))
        {
            return true;
        }*/

        if (ScattergunPvP.CanUse(out action,skipAoeCheck:true, usedUp: true))
        {
            if (!Player.HasStatus(true, StatusID.Overheated_3149))
            {
                return true;
            }
        }

        if (BlastChargePvP.CanUse(out action))
        {
            return true;
        }

        if ((!Player?.HasStatus(true, StatusID.Guard) ?? false) && (!Player?.HasStatus(true, StatusID.Sprint) ?? false) &&
            SprintPvP.CanUse(out action)) return true;


        return base.GeneralGCD(out action);
    }
    #endregion
}