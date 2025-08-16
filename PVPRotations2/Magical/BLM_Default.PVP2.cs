using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RebornRotations.PVPRotations.Magical;

[Rotation("Lelia Default PVP", CombatType.PvP, GameVersion = "7.3")]
[SourceCode(Path = "main/RebornRotations/PVPRotations/Magical/BLM_Default.PVP.cs")]
//[Api(6)]
public class BLM_DefaultPVP2 : BlackMageRotation
{
    public static IBaseAction SoulResonancePvP = new BaseAction((ActionID)29662);
    public static unsafe int LimitBreakValue => LimitBreakController.Instance()->CurrentUnits;
    public static unsafe int MyLimitBreakLevel => LimitBreakController.Instance()->BarUnits == 0 ? 0 : LimitBreakValue / LimitBreakController.Instance()->BarUnits;
    #region Configurations

    [RotationConfig(CombatType.PvP, Name = "Use Purify")]
    public bool UsePurifyPvP { get; set; } = true;

    [RotationConfig(CombatType.PvP, Name = "Stop attacking while in Guard.")]
    public bool RespectGuard { get; set; } = true;

    /*[Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvP, Name = "Upper HP threshold you need to be to use Xenoglossy as a damage oGCD")]
    public float XenoglossyHighHP { get; set; } = 0.8f;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvP, Name = "Lower HP threshold you need to be to use Xenoglossy as a heal oGCD")]
    public float XenoglossyLowHP { get; set; } = 0.5f;*/

    [RotationConfig(CombatType.PvP, Name = "LB時、エーテリアルステップを使用します。")]
    public bool UseAMPvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "コメットを自動で使います。")]
    public bool UseCometPvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "ファイア系を主に使います。")]
    public bool UseFirePvP { get; set; } = true;

    /*[RotationConfig(CombatType.PvP, Name = "ブリザード系を主に使います。(パラドックス祭りｗ)")]
    public bool UseBlizzardPvP { get; set; } = true;*/

    [RotationConfig(CombatType.PvP, Name = "LBを使用します")]
    public bool UseLB { get; set; } = false;

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

        return DoPurify(out action) || base.EmergencyAbility(nextGCD, out action);
    }

    [RotationDesc(ActionID.WreathOfIcePvP)]
    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        if (WreathOfIcePvP.CanUse(out action))
        {
            return true;
        }

        return base.DefenseSingleAbility(nextGCD, out action);
    }

    [RotationDesc(ActionID.AetherialManipulationPvP)]
    protected override bool MoveForwardAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        // Manip yourself
        // if (AetherialManipulationPvP.CanUse(out action)) return true;

        return base.MoveForwardAbility(nextGCD, out action);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        if (InCombat && UseAMPvP && MyLimitBreakLevel == 1 && !AetherialManipulationPvP.Cooldown.IsCoolingDown)
        {
            AetherialManipulationPvP.CanUse(out action);
            return true;
        }

        if (RustPvP.CanUse(out action))
        {
            return true;
        }

        if (PhantomDartPvP.CanUse(out action))
        {
            return true;
        }

        if (LethargyPvP.CanUse(out action))
        {
            return true;
        }

        return base.AttackAbility(nextGCD, out action);
    }

    protected override bool GeneralAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        if (WreathOfFirePvP.CanUse(out action) && InCombat)
        {
            return true;
        }

        return base.GeneralAbility(nextGCD, out action);
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

        if (Target.GetHealthRatio() < 0.5f && XenoglossyPvP.CanUse(out action, usedUp: true))
        {
            return true;
        }

        if (InCombat && UseLB && MyLimitBreakLevel==1  &&  SoulResonancePvP.CanUse(out action, usedUp: true)) return true;

        if (UseCometPvP && CometPvP.CanUse(out action)) return true;

        if (Player.HasStatus(true,StatusID.Paradox) && ParadoxPvE.IsEnabled && ParadoxPvP.CanUse(out action))
        {
            return true;
        }

        if (Player.GetHealthRatio() <= 0.8f || (InCombat && Target.DistanceToPlayer() <= 6) && BurstPvP.CanUse(out action))
        {
            return true;
        }

        if (UseFirePvP)
        {
            if (FlareStarPvP.CanUse(out action))
            {
                return true;
            }

            if (FrostStarPvP.CanUse(out action))
            {
                return true;
            }

            /*if (BlizzardIvPvP.CanUse(out action))
            {
                return true;
            }

            if (BlizzardIiiPvP.CanUse(out action))
            {
                return true;
            }*/

            if (FirePvP.CanUse(out action) && (HighBlizzardIiPvP.IsTheSameTo(true, (ActionID)HighBlizzardIiPvP.ID)))
            {
                return true;
            }

            if (BlizzardPvP.CanUse(out action) && (HighFireIiPvP.IsTheSameTo(true, (ActionID)HighFireIiPvP.ID)))
            {
                return true;
            }
        }

        if (!UseFirePvP)
        {
            if (FrostStarPvP.CanUse(out action))
            {
                return true;
            }

            if (FlareStarPvP.CanUse(out action))
            {
                return true;
            }

            /*if (FireIiiPvP.CanUse(out action))
            {
                return true;
            }

            if (BlizzardIiiPvP.CanUse(out action))
            {
                return true;
            }

            if (BlizzardPvP.CanUse(out action))
            {
                return true;
            }*/

            if (BlizzardPvP.CanUse(out action) && (HighFireIiPvP.IsTheSameTo(true, (ActionID)HighFireIiPvP.ID)))
            {
                return true;
            }

            if (FirePvP.CanUse(out action) && (HighBlizzardIiPvP.IsTheSameTo(true, (ActionID)HighBlizzardIiPvP.ID)))
            {
                return true;
            }
        }

        if ((!Player.HasStatus(true, StatusID.Guard)) && (!Player.HasStatus(true, StatusID.Sprint)) &&
            SprintPvP.CanUse(out action)) return true;

        return base.GeneralGCD(out action);
    }
    #endregion

}