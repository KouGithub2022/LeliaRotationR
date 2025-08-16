using ECommons;
using ECommons.GameFunctions;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RebornRotations.PVPRotations.Magical;

[Rotation("Lelia Default PVP", CombatType.PvP, GameVersion = "7.25")]
[SourceCode(Path = "main/RebornRotations/PVPRotations/Magical/RDM_Default.PVP.cs")]
//[Api(6)]
public class RDM_DefaultPvP2 : RedMageRotation
{
    public static IBaseAction SouthernCrossPvP = new BaseAction((ActionID)41498);
    public static unsafe int LimitBreakValue => LimitBreakController.Instance()->CurrentUnits;
    public static unsafe int MyLimitBreakLevel => LimitBreakController.Instance()->BarUnits == 0 ? 0 : LimitBreakValue / LimitBreakController.Instance()->BarUnits;

    #region Configurations

    [RotationConfig(CombatType.PvP, Name = "Use Purify")]
    public bool UsePurifyPvP { get; set; } = true;

    [RotationConfig(CombatType.PvP, Name = "Stop attacking while in Guard.")]
    public bool RespectGuard { get; set; } = true;

    [RotationConfig(CombatType.PvP, Name = "コル・ア・コルを使用します")]
    public bool UseCC { get; set; } = false;

    [Range(1, 66000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "コル・ア・コル:使用できる自分の最低HPは？")]
    public int CCHPValue { get; set; } = 40000;

    [Range(1, 66000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "コル・ア・コル:使用できる自分の最低MPは？")]
    public int CCMPValue { get; set; } = 5000;

    [Range(1, 66000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "コル・ア・コル:使用できる敵のHPは？")]
    public int CCTValue { get; set; } = 45000;

    [Range(0, 25, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "コル・ア・コル:使用できる敵との距離は？")]
    public int CCDValue { get; set; } = 25;

    [RotationConfig(CombatType.PvP, Name = "デプラスマンを使用します")]
    public bool UseDisplacement { get; set; } = false;

    [Range(1, 66000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "デプラスマン:強制使用する自分ののMaxHPは？")]
    public int DispValue { get; set; } = 20000;

    [RotationConfig(CombatType.PvP, Name = "コメットを自動で使います。")]
    public bool UseCometPvP { get; set; } = false;

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

        return base.EmergencyAbility(nextGCD, out action);
    }

    [RotationDesc(ActionID.FortePvP)]
    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        if (FortePvP.CanUse(out action))
        {
            return true;
        }

        return base.DefenseSingleAbility(nextGCD, out action);
    }

    [RotationDesc(ActionID.EmboldenPvP)]
    protected override bool DefenseAreaAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        // cast embolden yourself
        // if (EmboldenPvP.CanUse(out action)) return true;

        return base.DefenseAreaAbility(nextGCD, out action);
    }

    [RotationDesc(ActionID.DisplacementPvP)]
    protected override bool MoveBackAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        // displace yourself
        // if (DisplacementPvP.CanUse(out action)) return true;

        return base.MoveBackAbility(nextGCD, out action);
    }

    [RotationDesc(ActionID.CorpsacorpsPvP)]
    protected override bool MoveForwardAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        // corpse yourself
        // if (CorpsacorpsPvP.CanUse(out action)) return true;

        return base.MoveForwardAbility(nextGCD, out action);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        //if (CometPvP.CanUse(out action)) return true;
        if (RustPvP.CanUse(out action))
        {
            return true;
        }

        if (PhantomDartPvP.CanUse(out action))
        {
            return true;
        }

        if (ViceOfThornsPvP.CanUse(out action,skipAoeCheck: true))
        {
            return true;
        }

        if (InCombat && !EmboldenPvP.Cooldown.IsCoolingDown)
        {
            EmboldenPvP.CanUse(out action);
            return true;
        }

        if (UseCC && Player.CurrentHp > CCHPValue && Player.CurrentMp > CCMPValue && Target.CurrentHp <= CCTValue &&
            Target.DistanceToPlayer() > 5 &&
            CorpsacorpsPvP.Cooldown.CurrentCharges > 0 &&
            EnchantedRipostePvP.IsEnabled &&
            CorpsacorpsPvP.Target.Target.DistanceToPlayer() <= CCDValue &&
            !Player.HasStatus(true, StatusID.Monomachy_3242) &&
            /*(EnchantedRipostePvP.IsEnabled || EnchantedZwerchhauPvP.IsEnabled || EnchantedRedoublementPvP.IsEnabled) &&*/
            CorpsacorpsPvP.CanUse(out action, usedUp: true))
        {
            return true;
        }

        //if (UseDisplacement && (CorpsacorpsPvP.Cooldown.CurrentCharges == 2 || Player.CurrentHp <= DispValue) &&
        //    DisplacementPvP.CanUse(out action)) return true;
        if (UseDisplacement &&
            (nextGCD.IsTheSameTo(true, (ActionID)JoltIiiPvP.ID) ||
            nextGCD.IsTheSameTo(true, (ActionID)FortePvP.ID) ||
            nextGCD.IsTheSameTo(true, (ActionID)EmboldenPvP.ID)) &&
            DisplacementPvP.CanUse(out action)) return true;

        if (ViceOfThornsPvP.CanUse(out action))
        {
            return true;
        }

        if (InCombat && FortePvP.CanUse(out action))
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
        

        if (InCombat && UseLB && MyLimitBreakLevel >= 1 && Target.CurrentHp <= LBValue && 
            SouthernCrossPvP.CanUse(out action,skipAoeCheck: true)) return true;


        if (UseCometPvP && CometPvP.CanUse(out action)) return true;

        if (ScorchPvP.CanUse(out action))
        {
            return true;
        }

        if (/*EnchantedRedoublementPvP.Target.Target.DistanceToPlayer() <= 5 &&*/ EnchantedRedoublementPvP.CanUse(out action))
        {
            return true;
        }

        if (/*EnchantedZwerchhauPvP.Target.Target.DistanceToPlayer() <= 5 &&*/ EnchantedZwerchhauPvP.CanUse(out action))
        {
            return true;
        }

        if (/*EnchantedRipostePvP.Target.Target.DistanceToPlayer() <= 5 &&*/ EnchantedRipostePvP.CanUse(out action))
        {
            return true;
        }

        if (PrefulgencePvP.CanUse(out action))
        {
            return true;
        }

        if (ResolutionPvP.CanUse(out action))
        {
            return true;
        }

        if (GrandImpactPvP.CanUse(out action))
        {
            return true;
        }

        if (JoltIiiPvP.CanUse(out action))
        {
            return true;
        }


        if ((!Player.HasStatus(true, StatusID.Guard)) && (!Player.HasStatus(true, StatusID.Sprint)) &&
            SprintPvP.CanUse(out action)) return true;

        return base.GeneralGCD(out action);
    }
    #endregion

}