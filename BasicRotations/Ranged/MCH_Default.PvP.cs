namespace DefaultRotations.Ranged;

[Rotation("Lelia's PvP", CombatType.PvP, GameVersion = "6.58")]
[SourceCode(Path = "main/DefaultRotations/Ranged/MCH_Default.PvP.cs")]
public sealed class MCH_LeliaDefaultPvP : MachinistRotation
{

    public static IBaseAction MarksmansSpitePvP { get; } = new BaseAction((ActionID)29415);
    //public static IBaseAction BishopAutoturretPvP2 { get; } = new BaseAction((ActionID)29412);

    [RotationConfig(CombatType.PvP, Name = "LBを使用します。\nUse Limit Break (Note: RSR cannot predict the future, and this has a cast time.")]
    public bool LBInPvP { get; set; } = false;

    [Range(1, 100, ConfigUnitType.Percent, 1)]
    [RotationConfig(CombatType.PvP, Name = "LB:魔弾の射手を行うために必要なターゲットのHP%%は？\nThe target HP%% required to perform LB:Marksman's Spite is")]
    public int MSValue { get; set; } = 45;

    [RotationConfig(CombatType.PvP, Name = "スプリントを使います。\nSprint")]
    public bool UseSprintPvP { get; set; } = false;

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

    [RotationConfig(CombatType.PvP, Name = "自分が防御中は攻撃を中止します。\nStop attacking while in Guard.")]
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
        if (GuardCancel && Player.HasStatus(true, StatusID.Guard)) return false;

        if (Player.HasStatus(true, StatusID.Overheated_3149))
        {
            if (HeatBlastPvP.CanUse(out act)) return true;
        }
        else if ((Player.HasStatus(true, StatusID.BioblasterPrimed) && Target.DistanceToPlayer() <= 12 && BioblasterPvP.CanUse(out act, usedUp: true, skipAoeCheck: true)) ||
                (Player.HasStatus(true, StatusID.AirAnchorPrimed) && AirAnchorPvP.CanUse(out act, usedUp: true, skipAoeCheck: true)) ||
                (Player.HasStatus(true, StatusID.ChainSawPrimed) && ChainSawPvP.CanUse(out act, usedUp: true, skipAoeCheck: true))) return true;
        else
        { 
            if (!Target.HasStatus(true, StatusID.Guard))
            {
                if (Player.HasStatus(true, StatusID.Overheated_3149)) return false;

                if (LimitBreakLevel >= 1 && LBInPvP && Target.GetHealthRatio() * 100 <= MSValue &&
                    MarksmansSpitePvP.CanUse(out act, usedUp: true, skipAoeCheck: true)) return true;
                if (Target.DistanceToPlayer() <= 12 && ScattergunPvP.CanUse(out act, skipAoeCheck: true)) return true;

            }
        }

        if (!Player.HasStatus(true, StatusID.Overheated_3149) && Player.HasStatus(true, StatusID.DrillPrimed) && DrillPvP.CanUse(out act, usedUp: true, skipAoeCheck: true)) return true;
        if (!Player.HasStatus(true, StatusID.Overheated_3149) && BlastChargePvP.CanUse(out act, usedUp: true, skipAoeCheck: true)) return true;

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
        //Not working.
        if (BishopAutoturretPvP.CanUse(out act, usedUp: true, skipAoeCheck: true, skipComboCheck: true, skipClippingCheck: true)) return true;

        // Use WildfirePvP if Overheated
        if (Player.HasStatus(true, StatusID.Overheated_3149) && WildfirePvP.CanUse(out act, skipAoeCheck: true, skipComboCheck: true, skipClippingCheck: true)) return true;

        // Check if BioblasterPvP, AirAnchorPvP, or ChainSawPvP can be used
        if (InCombat && !Player.HasStatus(true, StatusID.Analysis) &&
            ((BioblasterPvP.IsInCooldown) || AirAnchorPvP.IsInCooldown || ChainSawPvP.IsInCooldown) &&
            AnalysisPvP.CanUse(out act, usedUp: true)) return true;

        return base.AttackAbility(out act);
    }
}
