using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RebornRotations.PVPRotations.Magical;

[Rotation("with Wrath Lelia PVP", CombatType.PvP, GameVersion = "7.25")]
[SourceCode(Path = "main/RebornRotations/PVPRotations/Magical/PCT_Default.PVP.cs")]
//[Api(6)]
public class PCT_DefaultPvPWrath : PictomancerRotation
{
    public static IBaseAction AdventofChocobastionPvP = new BaseAction((ActionID)39215);
    public static unsafe int LimitBreakValue => LimitBreakController.Instance()->CurrentUnits;
    public static unsafe int MyLimitBreakLevel => LimitBreakController.Instance()->BarUnits == 0 ? 0 : LimitBreakValue / LimitBreakController.Instance()->BarUnits;

    #region Configurations

    [RotationConfig(CombatType.PvP, Name = "Use Purify")]
    public bool UsePurifyPvP { get; set; } = true;

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

    /*[RotationConfig(CombatType.PvP, Name = "コメットを自動で使います。")]
    public bool UseCometPvP { get; set; } = false;*/

    [RotationConfig(CombatType.PvP, Name = "スマッジを使います。")]
    public bool UseSmudgePvP { get; set; } = false;

    [Range(1, 64500, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "スマッジを使用する敵のHPは？")]
    public int SmudgeValue { get; set; } = 30000;

    /*[RotationConfig(CombatType.PvP, Name = "LBを使用します(現在使用不可)")]
    public bool UseLB { get; set; } = false;*/

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

        return DoPurify(out action) || base.EmergencyAbility(nextGCD, out action);
    }

    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
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
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        /*if (Player.GetHealthRatio() <= TempuraThreshold)
        {
            TemperaCoatPvP.CanUse(out action,usedUp: true);
            return true;
        }*/

        if (UseSmudgePvP && Target.CurrentHp <= SmudgeValue && SmudgePvP.CanUse(out action)) { return true; }

        /*if (RustPvP.CanUse(out action))
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

        //if (UseLB && InCombat && MyLimitBreakLevel >=1 && AdventofChocobastionPvP.CanUse(out action,skipStatusProvideCheck: true,skipAoeCheck: true)) return true;

        /*if (UseLB && InCombat && Target.DistanceToPlayer() <= 5 && MyLimitBreakLevel >= 1)
        {
            AdventofChocobastionPvP.CanUse(out action);
            return true;
        }*/

        //Ability
        if (!SubtractivePalettePvP.Cooldown.IsCoolingDown && 
            !Player.HasStatus(true, StatusID.SubtractivePalette_4102) && 
            SubtractivePalettePvP.CanUse(out action))
        {
            return true;
        }

        /*if (UseCometPvP && CometPvP.CanUse(out action)) { return true; }

        if (Player.HasStatus(true,StatusID.Starstruck)) 
        {
            StarPrismPvP.CanUse(out action, skipAoeCheck: true, usedUp: true);
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

        if (CreatureMotifPvP.CanUse(out action))
        {
            return true;
        }*/

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