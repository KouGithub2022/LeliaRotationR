namespace DefaultRotations.Ranged;

[Rotation("Lelia's PvP", CombatType.PvP, GameVersion = "6.58")]
[SourceCode(Path = "main/DefaultRotations/Ranged/DRK_Default.PvP.cs")]
public sealed class DRK_LeliaDefaultPvP : DarkKnightRotation
{

    public static IBaseAction EventidePvP { get; } = new BaseAction((ActionID)29097);
    //public static IBaseAction BishopAutoturretPvP2 { get; } = new BaseAction((ActionID)29412);

    [RotationConfig(CombatType.PvP, Name = "LB���g�p���܂��B\nUse Limit Break (Note: RSR cannot predict the future, and this has a cast time.")]
    public bool LBInPvP { get; set; } = false;

    [Range(1, 100000, ConfigUnitType.Percent, 1)]
    [RotationConfig(CombatType.PvP, Name = "LB:�C�[�u���^�C�h���s�����߂ɕK�v�ȃv���C���[��HP�́H\n")]
    public int EVValue { get; set; } = 45000;

    [Range(1, 100000, ConfigUnitType.Percent, 1)]
    [RotationConfig(CombatType.PvP, Name = "�V���h�E�u�����K�[���g�p����v���C���[��HP�́H�H\n")]
    public int SBValue { get; set; } = 45000;

    [RotationConfig(CombatType.PvP, Name = "�X�v�����g���g���܂��B\nSprint")]
    public bool UseSprintPvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "���C���g���܂��B\nRecuperate")]
    public bool UseRecuperatePvP { get; set; } = false;

    [Range(1, 100, ConfigUnitType.Percent, 1)]
    [RotationConfig(CombatType.PvP, Name = "���C���g���v���C���[��HP%%�́H\nRecuperateHP%%?")]
    public int RCValue { get; set; } = 75;

    [RotationConfig(CombatType.PvP, Name = "�򉻂��g���܂��B\nUse Purify")]
    public bool UsePurifyPvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "�X�^��\nUse Purify on Stun")]
    public bool Use1343PvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "�X��\nUse Purify on DeepFreeze")]
    public bool Use3219PvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "���X�ɐ���\nUse Purify on HalfAsleep")]
    public bool Use3022PvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "����\nUse Purify on Sleep")]
    public bool Use1348PvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "�o�C���h\nUse Purify on Bind")]
    public bool Use1345PvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "�w���B\nUse Purify on Heavy")]
    public bool Use1344PvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "����\nUse Purify on Silence")]
    public bool Use1347PvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "�h����g��")]
    public bool UseGuardPvP { get; set; } = false;

    [Range(1, 100, ConfigUnitType.Percent, 1)]
    [RotationConfig(CombatType.PvP, Name = "�h����g���v���C���[��HP%%�́H\n")]
    public int GuardValue { get; set; } = 75;

    [RotationConfig(CombatType.PvP, Name = "�������h�䒆�͍U���𒆎~���܂��B\nStop attacking while in Guard.")]
    public bool GuardCancel { get; set; } = false;


    private bool TryPurify(out IAction? action)
    {
        action = null;
        if (!UsePurifyPvP) return false;

        var purifyStatuses = new Dictionary<int, bool>
        {
            { 1343, Use1343PvP },
            { 3219, Use3219PvP },
            { 3022, Use3022PvP },
            { 1348, Use1348PvP },
            { 1345, Use1345PvP },
            { 1344, Use1344PvP },
            { 1347, Use1347PvP }
        };

        foreach (var status in purifyStatuses)
        {
            if (status.Value && Player.HasStatus(true, (StatusID)status.Key))
            {
                return PurifyPvP.CanUse(out action, skipClippingCheck: true);
            }
        }

        return false;
    }

    protected override bool GeneralGCD(out IAction? act)
    {
        act = null;
        if (!Player.HasStatus(true, StatusID.Guard) && UseGuardPvP && Player.GetHealthRatio()*100 < GuardValue &&
            GuardPvP.CanUse(out act, usedUp: true) && InCombat) return true;
        if (GuardCancel && Player.HasStatus(true, StatusID.Guard)) return false;



        //if (HostileTarget && PvP_Eventide.IsEnabled && PvP_Guard.CanUse(out act, CanUseOption.MustUse)) return true;
        if (LimitBreakLevel>=1 && !HostileTarget.HasStatus(true, StatusID.Guard) && LBInPvP && HostileTarget && Player.CurrentHp < EVValue )
        {
            if (EventidePvP.CanUse(out act, usedUp: true, skipAoeCheck: true)) return true;
        }

        if (!HostileTarget.HasStatus(true, StatusID.Guard) && QuietusPvP.CanUse(out act, usedUp: true, skipAoeCheck: true) && InCombat) return true;
        if (!HostileTarget.HasStatus(true, StatusID.Guard) && Player.HasStatus(true, StatusID.Blackblood) && BloodspillerPvP.CanUse(out act, usedUp: true, skipAoeCheck: true)) return true;



        if (SouleaterPvP.CanUse(out act, usedUp: true)) return true;
        if (SyphonStrikePvP.CanUse(out act, usedUp: true)) return true;
        if (HardSlashPvP.CanUse(out act, usedUp: true)) return true;

        if (!Player.HasStatus(true, StatusID.Guard) && UseSprintPvP && !Player.HasStatus(true, StatusID.Sprint) && SprintPvP.CanUse(out act)) return true;

        return base.GeneralGCD(out act);
    }

    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        if (UseRecuperatePvP && Player.CurrentHp / Player.MaxHp * 100 < RCValue && RecuperatePvP.CanUse(out act)) return true;

        if (TryPurify(out act)) return true;

        return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool AttackAbility(out IAction? act)
    {
        if (ShadowbringerPvP.CanUse(out act)) return true;
        if (!Player.HasStatus(true, StatusID.Sprint) && !HostileTarget.HasStatus(true, StatusID.Guard) && (ShadowbringerPvP.IsInCooldown && Player.CurrentHp > SBValue) ||
            Player.HasStatus(true, StatusID.UndeadRedemption) ||
            Player.HasStatus(true, StatusID.DarkArts_3034) ||
            Player.HasStatus(true, StatusID.UndeadRedemption) && Player.HasStatus(true, StatusID.Guard))
        {
            act = ShadowbringerPvP;
            return true;
        }
        if(!HostileTarget.HasStatus(true, StatusID.Guard) && !Player.HasStatus(true, StatusID.UndeadRedemption))
        {
            if (TheBlackestNightPvP.CanUse(out act, skipAoeCheck: true, skipComboCheck: true, skipClippingCheck: true) && InCombat) return true;
            if (PlungePvP.CanUse(out act, usedUp: true)) return true;
            if (Player.HasStatus(true, StatusID.SaltedEarth_3036) && SaltAndDarknessPvP.CanUse(out act, usedUp: true, skipAoeCheck: true)) return true;
            if (SaltedEarthPvP.CanUse(out act, usedUp: true, skipAoeCheck: true) && InCombat) return true;
        }

        return base.AttackAbility(out act);
    }
}