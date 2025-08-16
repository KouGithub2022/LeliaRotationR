using Dalamud.Interface.Colors;
using ExCSS;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Data.Parsing.Scd;


namespace RebornRotations.PVPRotations.Magical;

[Rotation("with Wrath Lelia PVP", CombatType.PvP, GameVersion = "7.25")]
[SourceCode(Path = "main/BasicRotations/PVPRotations/Magical/SMN_Default.PVP.cs")]
//[Api(6)]

public class SMN_DefaultPvPTest : SummonerRotation
{

    public static IBaseAction SummonBahamutPvP = new BaseAction((ActionID)29673);
    public static IBaseAction SummonPhoenixPvP = new BaseAction((ActionID)29678);
    internal static IBaseAction SummonBahamutPvP2 { get; } = new BaseAction((ActionID)29673);
    public static IBaseAction MyCrimsonCyclonePvP = new BaseAction((ActionID)29667);
    public static unsafe int LimitBreakValue => LimitBreakController.Instance()->CurrentUnits;
    public static unsafe int MyLimitBreakLevel => LimitBreakController.Instance()->BarUnits == 0 ? 0 : LimitBreakValue / LimitBreakController.Instance()->BarUnits;

    #region Configurations

    [RotationConfig(CombatType.PvP, Name = "Use Purify")]
    public bool UsePurifyPvP { get; set; } = true;

    [RotationConfig(CombatType.PvP, Name = "Stop attacking while in Guard.")]
    public bool RespectGuard { get; set; } = true;

    [RotationConfig(CombatType.PvP, Name = "クリムゾンサイクロンを使用しますか？")]
    private bool CCPvP { get; set; } = false;

    [Range(1, 100000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "クリムゾンサイクロンを使用するターゲットのHPは？")]
    public int CrimsonValue { get; set; } = 15000;

    /*[Range(1, 100000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "LB:敵のHPは？")]
    public int LBValue { get; set; } = 20000;*/

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
        }*/

        return base.EmergencyAbility(nextGCD, out action);
    }

    [RotationDesc(ActionID.RadiantAegisPvP)]
    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        /*if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        if (RadiantAegisPvP.CanUse(out action))
        {
            return true;
        }*/

        return base.DefenseSingleAbility(nextGCD, out action);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        if (PhantomDartPvP.CanUse(out action))
        {
            return true;
        }

        /*if (DeathflarePvP.CanUse(out action))
        {
            return true;
        }

        if (BrandOfPurgatoryPvP.CanUse(out action))
        {
            return true;
        }

        if (NecrotizePvP.CanUse(out action) && !Player.HasStatus(true, StatusID.FirebirdTrance) && !Player.HasStatus(true, StatusID.DreadwyrmTrance_3228))
        {
            return true;
        }

        if (CCPvP && Target.CurrentHp <= CrimsonValue && MyCrimsonCyclonePvP.CanUse(out action, skipAoeCheck: true, usedUp: true)) return true;*/

        return base.AttackAbility(nextGCD, out action);
    }

    [RotationDesc(ActionID.CrimsonCyclonePvP)]
    protected override bool MoveForwardAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        //if (CometPvP.CanUse(out action)) return true;
        /*if (RustPvP.CanUse(out action))
        {
            return true;
        }*/

        if (PhantomDartPvP.CanUse(out action))
        {
            return true;
        }

        /*if (Target.CurrentHp <= CrimsonValue && MyCrimsonCyclonePvP.CanUse(out action, skipAoeCheck: true, usedUp: true) && Target.DistanceToPlayer() < 5)
        {
            return true;
        }*/

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

        //if (MyLimitBreakLevel == 1 && SummonBahamutPvP.CanUse(out action, skipAoeCheck: true, usedUp: true)) return true;
        //if (SummonBahamutPvP2.CanUse(out action, checkActionManager: false, skipAoeCheck: false, usedUp: true,skipCastingCheck: false,skipComboCheck: false,skipStatusNeed: false)) return true;
        //if (SummonBahamutPvP2.CanUse(out action, skipAoeCheck: true, usedUp: true)) return true;
        //if (SummonPhoenixPvP.CanUse(out action, skipAoeCheck: true, usedUp: true)) return true;
        /*if (MyLimitBreakLevel == 1)
        {
            SummonBahamutPvP.CanUse(out action, skipAoeCheck: true);
            return true;
        }*/

        if (AstralImpulsePvP.CanUse(out action))
        {
            return true;
        }

        /*if (FountainOfFirePvP.CanUse(out action))
        {
            return true;
        }

        if (CrimsonStrikePvP.CanUse(out action))
        {
            return true;
        }*/

        /*if (CrimsonCyclonePvP.CanUse(out action))
        {
            return true;
        }*/

        /*if (MountainBusterPvP.CanUse(out action))
        {
            return true;
        }

        if (SlipstreamPvP.CanUse(out action))
        {
            return true;
        }*/

        if (PhantomDartPvP.CanUse(out action))
        {
            return true;
        }

        if (RuinIiiPvP.CanUse(out action))
        {
            return true;
        }

        if ((!Player.HasStatus(true, StatusID.Guard)) && (!Player.HasStatus(true, StatusID.Sprint)) &&
            SprintPvP.CanUse(out action)) return true;

        return base.GeneralGCD(out action);
    }
    #endregion

}