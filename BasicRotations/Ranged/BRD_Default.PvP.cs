namespace DefaultRotations.Ranged;

[Rotation("Lelia's PvP", CombatType.PvP, GameVersion = "6.58",
    Description = "Please make sure that the three song times add up to 120 seconds!")]
[SourceCode(Path = "main/DefaultRotations/Ranged/BRD_Default.PvP.cs")]

[Api(4)]
public sealed class BRD_LeliaDefaultPvP : BardRotation
{
    public static IBaseAction FinalFantasiaPvP = new BaseAction((ActionID)29401);

    [RotationConfig(CombatType.PvP, Name = "LBを使用します。")]
    private bool LBInPvP { get; set; } = false;

    [Range(1, 100000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "LB:英雄のファンタジアを行うために必要な敵のHPは？")]
    public int FFValue { get; set; } = 50000;

    [Range(1, 3, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "エンピリアルアローを使うチャージ数。")]
    public int EmpyrealCh { get; set; } = 1;

    [RotationConfig(CombatType.PvP, Name = "リペリングショットを使用します。")]
    private bool UseRepelling { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "黙者のノクターンを使用します。")]
    private bool SNocturne { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "スプリントを使います。")]
    private bool UseSprintPvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "快気を使います。\nRecuperate")]
    public bool UseRecuperatePvP { get; set; } = false;

    [Range(1, 100, ConfigUnitType.Percent, 1)]
    [RotationConfig(CombatType.PvP, Name = "快気を使うプレイヤーのHP%%は？\nRecuperateHP%%?")]
    public int RCValue { get; set; } = 75;

    [RotationConfig(CombatType.PvP, Name = "浄化を使います。\nUse Purify")]
    public bool UsePurifyPvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "スタン\nUse Purify on Stun")]
    public bool Use1343PvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "氷結\nUse Purify on DeepFreeze")]
    public bool Use3219PvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "徐々に睡眠\nUse Purify on HalfAsleep")]
    public bool Use3022PvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "睡眠\nUse Purify on Sleep")]
    public bool Use1348PvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "バインド\nUse Purify on Bind")]
    public bool Use1345PvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "ヘヴィ\nUse Purify on Heavy")]
    public bool Use1344PvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "沈黙\nUse Purify on Silence")]
    public bool Use1347PvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "自分が防御中は攻撃を中止します。")]
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
                return PurifyPvP.CanUse(out action);
            }
        }

        return false;
    }

    protected override bool GeneralGCD(out IAction? act)
    {
        act = null;
        if (GuardCancel && Player.HasStatus(true, StatusID.Guard)) return false;
        if ((Player.CurrentHp / Player.MaxHp) < 1.00)
        {
            if (RecuperatePvP.CanUse(out act, skipAoeCheck: true, skipComboCheck: true)) return true;
        }
        if (InCombat && LimitBreakLevel >= 1 && LBInPvP && HostileTarget?.CurrentHp <= FFValue && FinalFantasiaPvP.CanUse(out act)) return true;

        if ((!HostileTarget?.HasStatus(true, StatusID.Guard) ?? false))
        {
            if (BlastArrowPvP.CanUse(out act, skipAoeCheck: true)) return true;
            if (ApexArrowPvP.CanUse(out act, skipAoeCheck: true)) return true;
            //if (PitchPerfectPvP.CanUse(out act)) return true;
        }


        if (PowerfulShotPvP.CanUse(out act)) return true;

        if (GuardCancel && (Player?.HasStatus(true, StatusID.Guard) ?? false)) return false;
        if ((!Player?.HasStatus(true, StatusID.Guard) ?? false) && UseSprintPvP && (!Player?.HasStatus(true, StatusID.Sprint) ?? false) &&
            SprintPvP.CanUse(out act)) return true;

        return base.GeneralGCD(out act);
    }

    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        //if (Player.CurrentHp / Player.MaxHp < 1.00 && RecuperatePvP.CanUse(out act)) return true;
        //if (Player.GetHealthRatio()<=1.00 && RecuperatePvP.CanUse(out act)) return true; 
        //if ((Player.HasStatus(true,(StatusID)1345)) && PurifyPvP.CanUse(out act)) return true;
        //if (InCombat && HostileTarget && PurifyPvP.CanUse(out act)) return true;
        if (UseRecuperatePvP && Player.GetHealthRatio() * 100 < RCValue && RecuperatePvP.CanUse(out act)) return true;

        if (TryPurify(out act)) return true;

        return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {
        act = null;
        if (GuardCancel && Player.HasStatus(true, StatusID.Guard)) return false;

        if (InCombat && (!HostileTarget?.HasStatus(true, StatusID.Guard) ?? false))
        {
            //if ((HostileTarget?.CurrentHp <= 6000 || EmpyrealArrowPvP.Cooldown.CurrentCharges >= EmpyrealCh) &&
            //    EmpyrealArrowPvP.CanUse(out act, usedUp: true)) return true;
            if ((EmpyrealArrowPvP.Cooldown.CurrentCharges >= EmpyrealCh) && EmpyrealArrowPvP.CanUse(out act, usedUp: true)) return true;
            if (!Player.HasStatus(true, (StatusID)3137) && SNocturne && SilentNocturnePvP.CanUse(out act)) return true;
            if (!Player.HasStatus(true,(StatusID)3137) && TheWardensPaeanPvP.CanUse(out act)) return true;
            if (UseRepelling && RepellingShotPvP.CanUse(out act)) return true;
        }

        return base.AttackAbility(nextGCD, out act);
    }

}