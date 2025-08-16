using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RebornRotations.PVPRotations.Melee;

[Rotation("Test Default PVP", CombatType.PvP, GameVersion = "7.25")]
[SourceCode(Path = "main/RebornRotations/PVPRotations/Melee/SAM_Default.PvP.cs")]
//[Api(6)]
public sealed class SAM_DefaultPvP2 : SamuraiRotation
{
    public static IBaseAction ZantetsukenPvP = new BaseAction((ActionID)29537);
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

    [RotationConfig(CombatType.PvP, Name = "峰打ちを、「崩し」状態の対象に限定せず、任意の対象に使用できるようにする。")]
    public bool MineuchiAny { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "必殺剣・早天を、距離に関係なく任意の対象に使用できるようにする（幸運を祈る）")]
    public bool SotenYeet { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "LBを使用します(現在崩しを認識できない為、不正確です)")]
    public bool UseLB { get; set; } = false;

    /*[Range(1, 66000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "LB:敵のHPは？")]
    public int LBValue { get; set; } = 50000;*/


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

        if (BloodbathPvP.CanUse(out action))
        {
            if (Player.GetHealthRatio() < BloodBathPvPPercent)
            {
                return true;
            }
        }

        if (SwiftPvP.CanUse(out action))
        {
            return true;
        }

        if (SmitePvP.CanUse(out action))
        {
            if (CurrentTarget?.GetHealthRatio() <= SmitePvPPercent)
            {
                return true;
            }
        }

        if (HissatsuSotenPvP.CanUse(out action, usedUp: true))
        {
            if (!HasHostilesInRange && SotenYeet)
            {
                return true;
            }
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

        if (HissatsuChitenPvP.CanUse(out action))
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

        if (HissatsuSotenPvP.CanUse(out action, usedUp: true))
        {
            if (nextGCD.IsTheSameTo(false, ActionID.YukikazePvP, ActionID.GekkoPvP, ActionID.KashaPvP))
            {
                return true;
            }
        }

        if (ZanshinPvP.CanUse(out action, usedUp: true))
        {
            return true;
        }

        if (MineuchiPvP.CanUse(out action, skipTargetStatusNeedCheck: MineuchiAny))
        {
            return true;
        }

        if (HissatsuChitenPvP.CanUse(out action))
        {
            if (HasHostilesInRange)
            {
                return true;
            }
        }

        if (MeikyoShisuiPvP.CanUse(out action))
        {
            if (HasHostilesInRange)
            {
                return true;
            }
        }

        return base.AttackAbility(nextGCD, out action);
    }

    [RotationDesc(ActionID.HissatsuSotenPvP)]
    protected override bool MoveForwardAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        if (HissatsuSotenPvP.CanUse(out action))
        {
            return true;
        }

        return base.MoveForwardAbility(nextGCD, out action);
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

        //if (Target.HasStatus(true,StatusID.Kuzushi) && UseLB && MyLimitBreakLevel == 1 && Target.CurrentHp <= LBValue && ZantetsukenPvP.CanUse(out action, skipAoeCheck: true, usedUp: true)) return true;
        if (UseLB && MyLimitBreakLevel >= 1 && /*CurrentTarget.HasStatus(true, StatusID.Kuzushi) &&*/  MineuchiPvP.Cooldown.IsCoolingDown && !MineuchiPvP.Cooldown .ElapsedAfterGCD(0,3))
        {
            ZantetsukenPvP.CanUse(out action, skipAoeCheck: true, usedUp: true);
            return true;
        }

        if (TendoKaeshiSetsugekkaPvP.CanUse(out action))
        {
            return true;
        }

        if (TendoSetsugekkaPvP.CanUse(out action))
        {
            return true;
        }

        if (KaeshiNamikiriPvP.CanUse(out action))
        {
            return true;
        }

        if (OgiNamikiriPvP.CanUse(out action))
        {
            return true;
        }

        if (KashaPvP.CanUse(out action))
        {
            return true;
        }

        if (GekkoPvP.CanUse(out action))
        {
            return true;
        }

        if (YukikazePvP.CanUse(out action))
        {
            return true;
        }

        if ((!Player?.HasStatus(true, StatusID.Guard) ?? false) && (!Player?.HasStatus(true, StatusID.Sprint) ?? false) &&
            SprintPvP.CanUse(out action)) return true;

        return base.GeneralGCD(out action);
    }
    #endregion
}