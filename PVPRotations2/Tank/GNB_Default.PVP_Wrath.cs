using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RebornRotations.PVPRotations.Tank;

[Rotation("with Wrath Lelia PVP", CombatType.PvP, GameVersion = "7.25")]
[SourceCode(Path = "main/RebornRotations/PVPRotations/Tank/GNB_Default.PvP.cs")]
//[Api(6)]
public sealed class GNB_DefaultPvPWrath : GunbreakerRotation
{
    public static IBaseAction RelentlessRushPvP = new BaseAction((ActionID)29130);
    public static unsafe int LimitBreakValue => LimitBreakController.Instance()->CurrentUnits;
    public static unsafe int MyLimitBreakLevel => LimitBreakController.Instance()->BarUnits == 0 ? 0 : LimitBreakValue / LimitBreakController.Instance()->BarUnits;

    #region Configurations
    [RotationConfig(CombatType.PvP, Name = "Use Purify")]
    public bool UsePurifyPvP { get; set; } = true;

    [RotationConfig(CombatType.PvP, Name = "Stop attacking while in Guard.")]
    public bool RespectGuard { get; set; } = true;
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

    #region Gunbreaker Utilities

    [RotationDesc(ActionID.HeartOfCorundumPvP)]
    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        if (HeartOfCorundumPvP.CanUse(out action))
        {
            return true;
        }

        if (InCombat && RampartPvP.CanUse(out action))
        {
            return true;
        }

        return base.DefenseSingleAbility(nextGCD, out action);
    }

    private static bool ReadyToRock()
    {
        /*if (SavageClawPvPReady)
        {
            return true;
        }

        if (WickedTalonPvPReady)
        {
            return true;
        }

        if (HypervelocityPvPReady)
        {
            return true;
        }*/

        return false;
    }
    private static bool ReadyToRoll()
    {
        /*if (EyeGougePvPReady)
        {
            return true;
        }

        if (AbdomenTearPvPReady)
        {
            return true;
        }

        if (JugularRipPvPReady)
        {
            return true;
        }

        if (FatedBrandPvPReady)
        {
            return true;
        }*/

        return false;
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

        //You WILL try to save yourself. Configs be damned!
        if (HeartOfCorundumPvP.CanUse(out action) && Player.GetHealthRatio() * 100 <= 30)
        {
            return true;
        }

        return base.EmergencyAbility(nextGCD, out action);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        /*if (!Player.HasStatus(true, StatusID.NoMercy_3042) && RoughDividePvP.CanUse(out action, usedUp: true))
        {
            return true;
        }

        if (Target.GetHealthRatio() * 100 <= 50 && BlastingZonePvP.CanUse(out action))
        {
            return true;
        }

        if (RampagePvP.CanUse(out action))
        {
            return true;
        }

        if (FullSwingPvP.CanUse(out action))
        {
            return true;
        }

        if (EyeGougePvP.CanUse(out action))
        {
            return true;
        }

        if (AbdomenTearPvP.CanUse(out action))
        {
            return true;
        }

        if (JugularRipPvP.CanUse(out action))
        {
            return true;
        }

        if (HypervelocityPvP.CanUse(out action))
        {
            return true;
        }

        if (FatedBrandPvP.CanUse(out action))
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

        //if (MyLimitBreakLevel == 1 && Player.GetHealthRatio() < 0.8f && RelentlessRushPvP.CanUse(out action,skipAoeCheck: true)) return true; 
        /*if (InCombat && RelentlessRushPvP.Target.Target.DistanceToPlayer() <= 6 && MyLimitBreakLevel >= 1)
        {
            action = RelentlessRushPvP;
            return true;
        }*/

        // I could totally collapse these into one function but *dab*
        /*if (!ReadyToRoll())
        {
            if (SavageClawPvP.CanUse(out action, usedUp: true))
            {
                return true;
            }

            if (WickedTalonPvP.CanUse(out action, usedUp: true))
            {
                return true;
            }

            if (GnashingFangPvP.CanUse(out action, usedUp: true))
            {
                return true;
            }
        }*/

        /*if (!ReadyToRoll() && FatedCirclePvP.CanUse(out action))
        {
            return true;
        }*/

        if (!ReadyToRock())
        {
            if (BurstStrikePvP.CanUse(out action))
            {
                return true;
            }

            if (SolidBarrelPvP.CanUse(out action))
            {
                return true;
            }

            if (BrutalShellPvP.CanUse(out action))
            {
                return true;
            }

            if (KeenEdgePvP.CanUse(out action))
            {
                return true;
            }
        }

        if ((!Player?.HasStatus(true, StatusID.Guard) ?? false) && (!Player?.HasStatus(true, StatusID.Sprint) ?? false) &&
            SprintPvP.CanUse(out action)) return true;

        return base.GeneralGCD(out action);

    }
    #endregion

}