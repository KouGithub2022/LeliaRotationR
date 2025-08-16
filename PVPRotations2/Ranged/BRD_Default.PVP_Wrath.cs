using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RebornRotations.PVPRotations.Ranged;

[Rotation("with Wrath Lelia PVP", CombatType.PvP, GameVersion = "7.25")]
[SourceCode(Path = "main/RebornRotations/PVPRotations/Ranged/BRD_Default.PvP.cs")]
//[Api(6)]
public sealed class BRD_DefaultPvPWrath : BardRotation
{
    public static IBaseAction FinalFantasiaPvP = new BaseAction((ActionID)29401);
    public static unsafe int LimitBreakValue => LimitBreakController.Instance()->CurrentUnits;
    public static unsafe int MyLimitBreakLevel => LimitBreakController.Instance()->BarUnits == 0 ? 0 : LimitBreakValue / LimitBreakController.Instance()->BarUnits;

    #region Configurations

    [RotationConfig(CombatType.PvP, Name = "Use Purify")]
    public bool UsePurifyPvP { get; set; } = true;

    [RotationConfig(CombatType.PvP, Name = "Stop attacking while in Guard.")]
    public bool RespectGuard { get; set; } = true;

    [RotationConfig(CombatType.PvP, Name = "Use Warden's Paean on other players")]
    public bool BRDEsuna { get; set; } = true;

    [Range(1, 4, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "ハーモニックアローを使うチャージ数。")]
    public int HarmonicCh { get; set; } = 4;

    [RotationConfig(CombatType.PvP, Name = "LBを使用します")]
    public bool UseLB { get; set; } = false;

    [Range(1, 66000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "LB:敵のHPは？")]
    public int LBValue { get; set; } = 50000;

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

        if (BraveryPvP.CanUse(out action))
        {
            if (InCombat)
            {
                return true;
            }
        }

        if (DervishPvP.CanUse(out action))
        {
            if (InCombat)
            {
                return true;
            }
        }

        return base.EmergencyAbility(nextGCD, out action);
    }

    [RotationDesc(ActionID.TheWardensPaeanPvP)]
    protected override bool DispelAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        if (BRDEsuna && TheWardensPaeanPvP.CanUse(out action))
        {
            return true;
        }

        return base.DispelAbility(nextGCD, out action);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        if (RepellingShotPvP.CanUse(out action))
        {
            if (!Player.HasStatus(true, StatusID.Repertoire))
            {
                return true;
            }
        }

        /*if (SilentNocturnePvP.CanUse(out action))
        {
            if (!Player.HasStatus(true, StatusID.Repertoire))
            {
                return true;
            }
        }

        if (EagleEyeShotPvP.CanUse(out action))
        {
            return true;
        }

        if (EncoreOfLightPvP.CanUse(out action, skipAoeCheck: true))
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

        if (InCombat && UseLB && MyLimitBreakLevel >= 1 && Target.CurrentHp <= LBValue && FinalFantasiaPvP.CanUse(out action)) return true;

        /*if (HarmonicArrowPvP.Cooldown.CurrentCharges >= HarmonicCh && HarmonicArrowPvP.CanUse(out action, usedUp: true))
        {
            return true;
        }
        
        if (PitchPerfectPvP.CanUse(out action))
        {
            return true;
        }

        if (BlastArrowPvP.CanUse(out action))
        {
            return true;
        }

        if (ApexArrowPvP.CanUse(out action))
        {
            return true;
        }*/

        if (PowerfulShotPvP.CanUse(out action))
        {
            return true;
        }

        if ((!Player.HasStatus(true, StatusID.Guard)) && (!Player.HasStatus(true, StatusID.Sprint)) &&
            SprintPvP.CanUse(out action)) return true;

        return base.GeneralGCD(out action);
    }
    #endregion
}