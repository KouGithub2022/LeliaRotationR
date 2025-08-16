namespace RebornRotations.PVPRotations.Healer;

[Rotation("Lelia Default PVP", CombatType.PvP, GameVersion = "7.25")]
[SourceCode(Path = "main/RebornRotations/PVPRotations/Healer/SGE_Default.PVP.cs")]
//[Api(6)]
public class SGE_DefaultPVP2 : SageRotation
{
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

        if (InCombat && KardiaPvP.CanUse(out action))
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

        if (DiabrosisPvP.CanUse(out action))
        {
            return true;
        }

        if (ToxikonPvP.CanUse(out action, usedUp: true))
        {
            return true;
        }

        return base.AttackAbility(nextGCD, out action);
    }
    #endregion

    #region GCDs
    protected override bool DefenseSingleGCD(out IAction? action)
    {
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        if (StoneskinIiPvP.CanUse(out action))
        {
            return true;
        }

        return base.DefenseSingleGCD(out action);
    }

    protected override bool HealSingleGCD(out IAction? action)
    {
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        if (HaelanPvP.CanUse(out action))
        {
            return true;
        }

        return base.HealSingleGCD(out action);
    }

    protected override bool GeneralGCD(out IAction? action)
    {
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        //if (InCombat && !Target.HasStatus(true, StatusID.EukrasianDosisIii_3108) /*&& EukrasiaPvP.Cooldown.CurrentCharges > 0*/)
        //{
        //    EukrasiaPvP.CanUse(out action, usedUp: true);
        //    return true;
        //}
        if (EukrasiaPvP.CanUse(out action, usedUp: true) && InCombat && !Target.HasStatus(true, StatusID.EukrasianDosisIii_3108))
        {
            return true;
        }

        if (PneumaPvP.CanUse(out action))
        {
            return true;
        }

        if (PsychePvP.CanUse(out action))
        {
            return true;
        }

        if (PhlegmaIiiPvP.CanUse(out action))
        {
            return true;
        }

        if (DosisIiiPvP.CanUse(out action))
        {
            return true;
        }

        if ((!Player?.HasStatus(true, StatusID.Guard) ?? false) && (!Player?.HasStatus(true, StatusID.Sprint) ?? false) &&
            SprintPvP.CanUse(out action)) return true;

        return base.GeneralGCD(out action);
    }
    #endregion
}