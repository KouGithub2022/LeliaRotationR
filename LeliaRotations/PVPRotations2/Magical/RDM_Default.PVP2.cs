using ECommons;
using ECommons.GameFunctions;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RebornRotations.PVPRotations.Magical;

[Rotation("Lelia Default PVP", CombatType.PvP, GameVersion = "7.35")]
[SourceCode(Path = "main/RebornRotations/PVPRotations/Magical/RDM_Default.PVP.cs")]
//[Api(6)]
public class RDM_DefaultPvP2 : RedMageRotation
{
    #region Configurations

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
    public int CCTValue { get; set; } = 10000;

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

    public static unsafe int LimitBreakValue => LimitBreakController.Instance()->CurrentUnits;
    public static unsafe int MyLimitBreakLevel => LimitBreakController.Instance()->BarUnits == 0 ? 0 : LimitBreakValue / LimitBreakController.Instance()->BarUnits;
    public static unsafe int LimitBreakMax => LimitBreakController.Instance()->BarCount;
    public static IBaseAction LBNamePvP = new BaseAction((ActionID)41498);

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvP, Name = "快気を使用するHP%%")]
    public float RecuperateValue { get; set; } = 0.75f;

    [RotationConfig(CombatType.PvP, Name = "LBを使用します")]
    public bool UseLBPvP { get; set; } = false;

    [Range(1, 66000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "LB:敵のHPは？")]
    public int LBValuePvP { get; set; } = 50000;

    #endregion

    #region oGCDs
    protected override bool EmergencyAbility(IAction nextGCD, out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.EmergencyAbility(nextGCD, out action);
        }

        //快気
        if (RecuperatePvP.CanUse(out action) && Player.GetHealthRatio() <= RecuperateValue)
        {
            return true;
        }

        if (PurifyPvP.CanUse(out action))
        {
            return true;
        }

        return base.EmergencyAbility(nextGCD, out action);
    }

    [RotationDesc(ActionID.FortePvP)]
    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.DefenseSingleAbility(nextGCD, out action);
        }

        if (FortePvP.CanUse(out action))
        {
            return true;
        }

        return base.DefenseSingleAbility(nextGCD, out action);
    }

    [RotationDesc(ActionID.DisplacementPvP)]
    protected override bool MoveBackAbility(IAction nextGCD, out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            base.MoveBackAbility(nextGCD, out action);
        }

        // displace yourself
        // if (DisplacementPvP.CanUse(out action)) return true;

        return base.MoveBackAbility(nextGCD, out action);
    }

    [RotationDesc(ActionID.CorpsacorpsPvP)]
    protected override bool MoveForwardAbility(IAction nextGCD, out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.MoveForwardAbility(nextGCD, out action);
        }

        // corpse yourself
        // if (CorpsacorpsPvP.CanUse(out action)) return true;

        return base.MoveForwardAbility(nextGCD, out action);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.AttackAbility(nextGCD, out action);
        }

        //快気
        if (RecuperatePvP.CanUse(out action) && Player.GetHealthRatio() <= RecuperateValue)
        {
            return true;
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

        /*if (InCombat && !EmboldenPvP.Cooldown.IsCoolingDown)
        {
            EmboldenPvP.CanUse(out action);
            return true;
        }*/

        if (nextGCD.IsTheSameTo(false, ActionID.ResolutionPvP, ActionID.EnchantedRedoublementPvP, ActionID.ScorchPvP))
        {
            if (EmboldenPvP.CanUse(out action))
            {
                return true;
            }
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
        if (Player.CurrentHp <= DispValue || (UseDisplacement && 
            (nextGCD.IsTheSameTo(true, (ActionID)JoltIiiPvP.ID) ||
            nextGCD.IsTheSameTo(true, (ActionID)FortePvP.ID) ||
            nextGCD.IsTheSameTo(true, (ActionID)EmboldenPvP.ID) ||
            nextGCD.IsTheSameTo(true, (ActionID)ScorchPvP.ID))) &&
            DisplacementPvP.CanUse(out action,usedUp: true)) 
        { 
            return true; 
        }

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
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.GeneralGCD(out action);
        }

        //快気
        if (RecuperatePvP.CanUse(out action) && Player.GetHealthRatio() <= RecuperateValue)
        {
            return true;
        }

        if (CurrentTarget is not null && InCombat && UseLBPvP && Target.DistanceToPlayer() <= 25 && MyLimitBreakLevel == 1 && Target.CurrentHp <= LBValuePvP)
        {
            if (LBNamePvP.CanUse(out action,skipAoeCheck: true))
            {
                return true;
            }
        }


        if (UseCometPvP && CometPvP.CanUse(out action)) return true;

        /*if (ScorchPvP.CanUse(out action))
        {
            return true;
        }*/

        //if (/*EnchantedRedoublementPvP.Target.Target.DistanceToPlayer() <= 5 &&*/ EnchantedRedoublementPvP.CanUse(out action))
        //{
        //    return true;
        //}

        //if (/*EnchantedZwerchhauPvP.Target.Target.DistanceToPlayer() <= 5 &&*/ EnchantedZwerchhauPvP.CanUse(out action))
        //{
        //    return true;
        //}

        //if (/*EnchantedRipostePvP.Target.Target.DistanceToPlayer() <= 5 &&*/ EnchantedRipostePvP.CanUse(out action))
        //{
        //    return true;
        //}

        if (PrefulgencePvP.CanUse(out action))
        {
            return true;
        }

        if (ResolutionPvP.CanUse(out action))
        {
            return true;
        }

        if (ScorchPvP.CanUse(out action))
        {
            return true;
        }

        if (EnchantedRedoublementPvP.CanUse(out action))
        {
            return true;
        }

        if (EnchantedZwerchhauPvP.CanUse(out action))
        {
            return true;
        }

        if (EnchantedRipostePvP.CanUse(out action))
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