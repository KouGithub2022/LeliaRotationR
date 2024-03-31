namespace DefaultRotations.Ranged;

[Rotation("Lelia's PvP", CombatType.PvP, GameVersion = "6.58",
    Description = "Please make sure that the three song times add up to 120 seconds!")]
[SourceCode(Path = "main/DefaultRotations/Ranged/BRD_Default.PvP.cs")]


public sealed class BRD_LeliaDefaultPvP : BardRotation
{
    public static IBaseAction FinalFantasiaPvP { get; } = new BaseAction((ActionID)29401);

    [RotationConfig(CombatType.PvP, Name = "LB���g�p���܂��B")]
    private bool LBInPvP { get; set; } = false;

    [Range(1, 100000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "LB:�p�Y�̃t�@���^�W�A���s�����߂ɕK�v�ȓG��HP�́H")]
    public int FFValue { get; set; } = 50000;

    [Range(1, 3, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "�G���s���A���A���[���g���`���[�W���B")]
    public int EmpyrealCh { get; set; } = 1;

    [RotationConfig(CombatType.PvP, Name = "���y�����O�V���b�g���g�p���܂��B")]
    private bool UseRepelling { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "�َ҂̃m�N�^�[�����g�p���܂��B")]
    private bool SNocturne { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "�X�v�����g���g���܂��B")]
    private bool UseSprintPvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "���C���g���܂��B")]
    private bool UseRecuperatePvP { get; set; } = false;

    [Range(1, 100, ConfigUnitType.Percent, 1)]
    [RotationConfig(CombatType.PvP, Name = "���C���g���v���C���[��HP%%�́H")]
    public int RCValue { get; set; } = 75;

    [RotationConfig(CombatType.PvP, Name = "�򉻂��g���܂��B")]
    private bool UsePurifyPvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "�X�^��:Stun")]
    private bool Use1343PvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "�X��:DeepFreeze")]
    private bool Use3219PvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "���X�ɐ���:HalfAsleep")]
    private bool Use3022PvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "����:Sleep")]
    private bool Use1348PvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "�o�C���h:Bind")]
    private bool Use1345PvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "�w���B:Heavy")]
    private bool Use1344PvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "����:Silence")]
    private bool Use1347PvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "�������h�䒆�͍U���𒆎~���܂��B")]
    private bool GuardCancel { get; set; } = false;

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
        #region PvP
        act = null;
        if (GuardCancel && Player.HasStatus(true, StatusID.Guard)) return false;

        if (GuardCancel && Player.HasStatus(true, StatusID.Guard)) return false;

        if (LimitBreakLevel>=1 && LBInPvP && HostileTarget.CurrentHp <= FFValue && FinalFantasiaPvP.CanUse(out act)) return true;

        if (!HostileTarget.HasStatus(true, StatusID.Guard) && BlastArrowPvP.CanUse(out act, skipAoeCheck: true)) return true;
        if (!HostileTarget.HasStatus(true, StatusID.Guard) && ApexArrowPvP.CanUse(out act, skipAoeCheck: true)) return true;

        if (!HostileTarget.HasStatus(true, StatusID.Guard) && PitchPerfectPvP.CanUse(out act)) return true;
        if (PowerfulShotPvP.CanUse(out act)) return true;

        if (GuardCancel && Player.HasStatus(true, StatusID.Guard)) return false;
        //(StatusID)29054 => Guard , (StatusID)1342 => Sprint
        if (!Player.HasStatus(true, StatusID.Guard) && UseSprintPvP && !Player.HasStatus(true, StatusID.Sprint) &&
            SprintPvP.CanUse(out act)) return true;
        #endregion

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
        #region PvP
        act = null;
        if (GuardCancel && Player.HasStatus(true, StatusID.Guard)) return false;
        //if (PvP_FinalFantasia.CanUse(out act, CanUseOption.MustUse)) return true;

        if (!HostileTarget.HasStatus(true, StatusID.Guard) && SNocturne && SilentNocturnePvP.CanUse(out act)) return true;
        if (!HostileTarget.HasStatus(true, StatusID.Guard) && TheWardensPaeanPvP.CanUse(out act)) return true;
        if (!HostileTarget.HasStatus(true, StatusID.Guard) && EmpyrealArrowPvP.CanUse(out act, usedUp: true) && 
            EmpyrealArrowPvP.Cooldown.CurrentCharges >= EmpyrealCh) return true;

        if (!HostileTarget.HasStatus(true, StatusID.Guard) && UseRepelling && RepellingShotPvP.CanUse(out act)) return true;
        if (!HostileTarget.HasStatus(true, StatusID.Guard) && UseRepelling && RepellingShotPvP.CanUse(out act)) return true;

        //(StatusID)29054 => Guard , (StatusID)1342 => Sprint
        if (!Player.HasStatus(true, StatusID.Guard) && UseSprintPvP && !Player.HasStatus(true, StatusID.Sprint) &&
            SprintPvP.CanUse(out act)) return true;
        #endregion

        return base.AttackAbility(out act);
    }

}
