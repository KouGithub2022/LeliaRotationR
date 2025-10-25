using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RebornRotations.PVPRotations.Melee;

[Rotation("Lelia Default PVP", CombatType.PvP, GameVersion = "7.35")]
[SourceCode(Path = "main/RebornRotations/PVPRotations/Melee/VPR_Default.PvP.cs")]
//[Api(6)]
public sealed class VPR_DefaultPvP2 : ViperRotation
{
    #region Configurations

    [RotationConfig(CombatType.PvP, Name = "Stop attacking while in Guard.")]
    public bool RespectGuard { get; set; } = true;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvP, Name = "Player health threshold needed for Bloodbath use")]
    public float BloodBathPvPPercent { get; set; } = 0.75f;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvP, Name = "Enemy health threshold needed for Smite use")]
    public float SmitePvPPercent { get; set; } = 0.25f;

    [RotationConfig(CombatType.PvP, Name = "蛇鱗術を使用します")]
    public bool UseSnakeScales { get; set; } = false;

    public static unsafe int LimitBreakValue => LimitBreakController.Instance()->CurrentUnits;
    public static unsafe int MyLimitBreakLevel => LimitBreakController.Instance()->BarUnits == 0 ? 0 : LimitBreakValue / LimitBreakController.Instance()->BarUnits;
    public static unsafe int LimitBreakMax => LimitBreakController.Instance()->BarCount;
    public static IBaseAction LBNamePvP = new BaseAction((ActionID)39190);

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
        if ((RespectGuard && Player.HasStatus(true, StatusID.Guard)) || Player.HasStatus(true, StatusID.HardenedScales))
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

        //these have to stay in Emergency because action weirdness with Serpent's Tail adjust ID
        if (UncoiledTwinbloodPvP.CanUse(out action))
        {
            return true;
        }

        if (UncoiledTwinfangPvP.CanUse(out action))
        {
            return true;
        }

        if (InCombat && RattlingCoilPvP.CanUse(out action))
        {
            if (!UseSnakeScales && UncoiledFuryPvP.Cooldown.IsCoolingDown)
            {
                return true;
            }
            else if (SnakeScalesPvP.Cooldown.IsCoolingDown && UncoiledFuryPvP.Cooldown.IsCoolingDown)
            {
                return true;
            }
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

        if (SmitePvP.CanUse(out action) && SmitePvP.Target.Target.GetHealthRatio() <= SmitePvPPercent)
        {
            return false;
        }


        return base.EmergencyAbility(nextGCD, out action);
    }

    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? action)
    {
        if ((RespectGuard && Player.HasStatus(true, StatusID.Guard)) || Player.HasStatus(true, StatusID.HardenedScales))
        {
            return base.DefenseSingleAbility(nextGCD, out action);
        }

        return base.DefenseSingleAbility(nextGCD, out action);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? action)
    {
        if ((RespectGuard && Player.HasStatus(true, StatusID.Guard)) || Player.HasStatus(true, StatusID.HardenedScales))
        {
            return base.AttackAbility(nextGCD, out action);
        }

        //快気
        if (RecuperatePvP.CanUse(out action) && Player.GetHealthRatio() <= RecuperateValue)
        {
            return true;
        }

        if (BacklashPvP.CanUse(out action, skipAoeCheck: true, usedUp: true))
        {
            return true;
        }

        if (UseSnakeScales && InCombat && !Player.HasStatus(true,StatusID.Reawakened_4094) && SnakeScalesPvP.CanUse(out action))
        {
            return true;
        }

        if (FourthLegacyPvP.CanUse(out action))
        {
            return true;
        }

        if (ThirdLegacyPvP.CanUse(out action))
        {
            return true;
        }

        if (SecondLegacyPvP.CanUse(out action))
        {
            return true;
        }

        if (FirstLegacyPvP.CanUse(out action))
        {
            return true;
        }

        if (TwinbloodBitePvP.CanUse(out action))
        {
            return true;
        }

        if (TwinfangBitePvP.CanUse(out action))
        {
            return true;
        }

        if (DeathRattlePvP.CanUse(out action))
        {
            return true;
        }

        if (!Player.HasStatus(true,StatusID.Slither) && Target.DistanceToPlayer() > 10)
        {
            SlitherPvP.CanUse(out action,usedUp: true);
            return true;
        }

        return base.AttackAbility(nextGCD, out action);
    }

    protected override bool MoveForwardAbility(IAction nextGCD, out IAction? action)
    {
        if ((RespectGuard && Player.HasStatus(true, StatusID.Guard)) || Player.HasStatus(true, StatusID.HardenedScales))
        {
            return base.MoveForwardAbility(nextGCD, out action);
        }

        return base.MoveForwardAbility(nextGCD, out action);
    }
    #endregion

    #region GCDs
    protected override bool GeneralGCD(out IAction? action)
    {
        if ((RespectGuard && Player.HasStatus(true, StatusID.Guard)) || Player.HasStatus(true, StatusID.HardenedScales))
        {
            return base.GeneralGCD(out action);
        }

        //快気
        if (RecuperatePvP.CanUse(out action) && Player.GetHealthRatio() <= RecuperateValue)
        {
            return true;
        }

        if (CurrentTarget is not null && !Player.HasStatus(true, StatusID.SnakesBane) && InCombat && UseLBPvP && Target.DistanceToPlayer() <= 25 && MyLimitBreakLevel == 1 && Target.CurrentHp <= LBValuePvP)
        {
            if (LBNamePvP.CanUse(out action, skipAoeCheck: true))
            {
                return true;
            }
        }

        /*if (CurrentTarget is not null && !Player.HasStatus(true,StatusID.SnakesBane) && UseLB && MyLimitBreakLevel == 1 && 
            Target.CurrentHp <= LBValue && WorldswallowerPvP.CanUse(out action, skipAoeCheck: true, usedUp: true)) return true;*/

        if (BacklashPvP.CanUse(out action, skipAoeCheck: true, usedUp: true))
        {
            return true;
        }

        if (FourthGenerationPvP.CanUse(out action))
        {
            return true;
        }

        if (ThirdGenerationPvP.CanUse(out action))
        {
            return true;
        }

        if (SecondGenerationPvP.CanUse(out action))
        {
            return true;
        }

        if (FirstGenerationPvP.CanUse(out action))
        {
            return true;
        }

        if (OuroborosPvP.CanUse(out action))
        {
            return true;
        }

        if (SanguineFeastPvP.CanUse(out action))
        {
            return true;
        }

        if (BloodcoilPvP.CanUse(out action))
        {
            return true;
        }

        if (UncoiledFuryPvP.CanUse(out action))
        {
            return true;
        }

        if (RavenousBitePvP.CanUse(out action))
        {
            return true;
        }

        if (SwiftskinsStingPvP.CanUse(out action))
        {
            return true;
        }

        if (PiercingFangsPvP.CanUse(out action))
        {
            return true;
        }

        if (BarbarousBitePvP.CanUse(out action))
        {
            return true;
        }

        if (HuntersStingPvP.CanUse(out action))
        {
            return true;
        }

        if (SteelFangsPvP.CanUse(out action))
        {
            return true;
        }

        if (UncoiledFuryPvP.CanUse(out action, usedUp: true))
        {
            return true;
        }

        if ((!Player?.HasStatus(true, StatusID.Guard) ?? false) && (!Player?.HasStatus(true, StatusID.Sprint) ?? false) &&
            SprintPvP.CanUse(out action)) return true;

        return base.GeneralGCD(out action);
    }
    #endregion
}