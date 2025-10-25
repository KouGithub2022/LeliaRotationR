using ECommons.Automation;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RebornRotations.PVPRotations.Magical;

[Rotation("Lelia Default PVP", CombatType.PvP, GameVersion = "7.35")]
[SourceCode(Path = "main/RebornRotations/PVPRotations/Magical/PCT_Default.PVP.cs")]
//[Api(6)]
public class PCT_DefaultPvP2 : PictomancerRotation
{
    #region Configurations

    [RotationConfig(CombatType.PvP, Name = "Stop attacking while in Guard.")]
    public bool RespectGuard { get; set; } = true;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvP, Name = "Health threshold needed to use Tempura Coat")]
    public float TempuraThreshold { get; set; } = 0.8f;

    [RotationConfig(CombatType.PvP, Name = "Freely use burst damage oGCDs")]
    public bool FreeBurst { get; set; } = true;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvP, Name = "Enemy HP threshold needed to use burst oGCDs on if previous config disabled")]
    public float BurstThreshold { get; set; } = 0.55f;

    [RotationConfig(CombatType.PvP, Name = "コメットを自動で使います。")]
    public bool UseCometPvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "スマッジを使います。")]
    public bool UseSmudgePvP { get; set; } = false;

    [Range(1, 64500, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "スマッジを使用する敵のHPは？")]
    public int SmudgeValue { get; set; } = 10000;

    public static unsafe int LimitBreakValue => LimitBreakController.Instance()->CurrentUnits;
    public static unsafe int MyLimitBreakLevel => LimitBreakController.Instance()->BarUnits == 0 ? 0 : LimitBreakValue / LimitBreakController.Instance()->BarUnits;
    public static unsafe int LimitBreakMax => LimitBreakController.Instance()->BarCount;
    public static IBaseAction LBNamePvP = new BaseAction((ActionID)39215);

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

    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.DefenseSingleAbility(nextGCD, out action);
        }

        if (Player.GetHealthRatio() <= TempuraThreshold)
        {
            TemperaCoatPvP.CanUse(out action);
            return true;
        }

        return base.DefenseSingleAbility(nextGCD, out action);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.AttackAbility(nextGCD, out action);
        }

        if (RecuperatePvP.CanUse(out action) && Player.GetHealthRatio() <= RecuperateValue)
        {
            return true;
        }

        if (Player.GetHealthRatio() <= TempuraThreshold)
        {
            TemperaCoatPvP.CanUse(out action,usedUp: true);
            return true;
        }

        if (UseSmudgePvP && Target.CurrentHp <= SmudgeValue &&
            !Player.HasStatus(true, StatusID.Chocobastion) && SmudgePvP.CanUse(out action)) { return true; }

        if (RustPvP.CanUse(out action))
        {
            return true;
        }

        if (PhantomDartPvP.CanUse(out action))
        {
            return true;
        }

        if (FreeBurst || CurrentTarget?.GetHealthRatio() <= BurstThreshold)
        {
            // Use all Muses in sequence for maximum burst
            if (PomMusePvP.CanUse(out action, usedUp: true))
            {
                return true;
            }

            if (WingedMusePvP.CanUse(out action, usedUp: true))
            {
                return true;
            }

            if (ClawedMusePvP.CanUse(out action, usedUp: true))
            {
                return true;
            }

            if (FangedMusePvP.CanUse(out action, usedUp: true))
            {
                return true;
            }
        }

        if (!SubtractivePalettePvP.Cooldown.IsCoolingDown && !Player.HasStatus(true, StatusID.SubtractivePalette_4102) && 
            SubtractivePalettePvP.CanUse(out action))
        {
            return true;
        }

        if ((IsMoving && InCombat))
        {
            if (ReleaseSubtractivePalettePvP.CanUse(out action))
            {
                return true;
            }
        }
        else if (!SubtractivePalettePvP.Cooldown.IsCoolingDown && SubtractivePalettePvP.CanUse(out action))
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

        if (RecuperatePvP.CanUse(out action) && Player.GetHealthRatio() <= RecuperateValue)
        {
            return true;
        }

        if (CurrentTarget is not null && InCombat && UseLBPvP && Target.DistanceToPlayer() <= 25 && MyLimitBreakLevel == 1 && Target.CurrentHp <= LBValuePvP)
        {
            //Chat.ExecuteCommand("/pvpaction ウォール・オブ・ファット <me>");
            Chat.ExecuteCommand($"/pvpac {LBNamePvP.Name} <me>");
        }

        //Ability
        if (!SubtractivePalettePvP.Cooldown.IsCoolingDown && !Player.HasStatus(true, StatusID.SubtractivePalette_4102) && SubtractivePalettePvP.CanUse(out action))
        {
            return true;
        }

        if (UseCometPvP && CometPvP.CanUse(out action)) { return true; }

        if (InCombat && Player.HasStatus(true,StatusID.Starstruck_4118) && 
            Target.DistanceToPlayer() < 25 && StarPrismPvP.CanUse(out action, skipAoeCheck: true, usedUp: true)) 
        {
            return true;
        }

        if (MogOfTheAgesPvP.CanUse(out action))
        {
            return true;
        }

        if (RetributionOfTheMadeenPvP.CanUse(out action))
        {
            return true;
        }

        if (CometInBlackPvP.CanUse(out action, usedUp: true))
        {
            return true;
        }

        if (PomMotifPvP.CanUse(out action))
        {
            return true;
        }

        if (WingMotifPvP.CanUse(out action))
        {
            return true;
        }

        if (ClawMotifPvP.CanUse(out action))
        {
            return true;
        }

        if (MawMotifPvP.CanUse(out action))
        {
            return true;
        }

        if (FireInRedPvP.CanUse(out action))
        {
            return true;
        }

        if ((!Player?.HasStatus(true, StatusID.Guard) ?? false) && (!Player?.HasStatus(true, StatusID.Sprint) ?? false) &&
            SprintPvP.CanUse(out action)) return true;

        return base.GeneralGCD(out action);
    }
    #endregion
}