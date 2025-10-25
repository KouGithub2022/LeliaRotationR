using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RebornRotations.PVPRotations.Ranged;

[Rotation("Lelia Default PVP", CombatType.PvP, GameVersion = "7.35")]
[SourceCode(Path = "main/RebornRotations/PVPRotations/Ranged/BRD_Default.PvP.cs")]
//[Api(6)]
public sealed class BRD_DefaultPvP2 : BardRotation
{
    public static IBaseAction FinalFantasiaPvP = new BaseAction((ActionID)29401);
    public static unsafe int LimitBreakValue => LimitBreakController.Instance()->CurrentUnits;
    public static unsafe int MyLimitBreakLevel => LimitBreakController.Instance()->BarUnits == 0 ? 0 : LimitBreakValue / LimitBreakController.Instance()->BarUnits;

    #region Configurations

    [RotationConfig(CombatType.PvP, Name = "Use Warden's Paean on other players")]
    public bool BRDEsuna2 { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "Stop attacking while in Guard.")]
    public bool RespectGuard { get; set; } = true;


    [RotationConfig(CombatType.PvP, Name ="ハーモニックアローをターゲットの残HPによって使い分ける。\nチャージ４で強制で使います。\n下記のチャージ数は無視されます。")]
    public bool UseAutoHarmonic { get; set; } = true;

    [Range(1, 60000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "ハーモニックアロー：チャージ１を使う敵のHPは？")]
    public int HNValue1 { get; set; } = 5000;

    [Range(1, 60000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "ハーモニックアロー：チャージ２を使う敵のHPは？")]
    public int HNValue2 { get; set; } = 8000;

    [Range(1, 60000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "ハーモニックアロー：チャージ３を使う敵のHPは？")]
    public int HNValue3 { get; set; } = 11000;

    [Range(1, 60000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "ハーモニックアロー：チャージ４を使う敵のHPは？")]
    public int HNValue4 { get; set; } = 18000;

    [Range(1, 4, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "固定：ハーモニックアローを使うチャージ数。")]
    public int HarmonicCh { get; set; } = 4;

    [RotationConfig(CombatType.PvP, Name = "リペリングショットを使用します。")]
    public bool UseRepellingShot { get; set; } = true;

    [RotationConfig(CombatType.PvP, Name = "黙者のノクターンを使用します。")]
    public bool UseSilentNocturne { get; set; } = true;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvP, Name = "快気を使用するHP%%")]
    public float RecuperateValue { get; set; } = 0.9f;

    [RotationConfig(CombatType.PvP, Name = "LBを使用します")]
    public bool UseLB { get; set; } = false;

    [Range(1, 66000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "LB:敵のHPは？")]
    public int LBValue { get; set; } = 50000;

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

        if (BRDEsuna2 && TheWardensPaeanPvP.CanUse(out action))
        {
            return true;
        }
        if (Player.HasStatus(false, StatusHelper.PurifyPvPStatuses))
        {
            if (TheWardensPaeanPvP.CanUse(out action))
            {
                if (TheWardensPaeanPvP.Target.Target == Player)
                {
                    return true;
                }
            }
        }

        if (PurifyPvP.CanUse(out action))
        {
            return true;
        }

        if (BraveryPvP.CanUse(out action))
        {
            if (InCombat)
            {
                return true;
            }
        }

        if (DervishPvP.CanUse(out action))
        {
            if (InCombat)
            {
                return true;
            }
        }

        return base.EmergencyAbility(nextGCD, out action);
    }
    protected override bool AttackAbility(IAction nextGCD, out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.AttackAbility(nextGCD, out action);
        }

        if (UseRepellingShot && RepellingShotPvP.CanUse(out action))
        {
            if (!Player.HasStatus(true, StatusID.Repertoire))
            {
                return true;
            }
        }

        if (UseSilentNocturne && SilentNocturnePvP.CanUse(out action))
        {
            if (!Player.HasStatus(true, StatusID.Repertoire))
            {
                return true;
            }
        }

        if (EagleEyeShotPvP.CanUse(out action))
        {
            return true;
        }

        if (EncoreOfLightPvP.CanUse(out action, skipAoeCheck: true))
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

        if (CurrentTarget is not null && InCombat && UseLB && MyLimitBreakLevel >= 1 && Target.CurrentHp <= LBValue && FinalFantasiaPvP.CanUse(out action))
        {
            return true;
        }

        if (UseAutoHarmonic && HarmonicArrowPvP.Cooldown.CurrentCharges >=1 && HarmonicArrowPvP.CanUse(out action,usedUp: true))
        {
            if (Target.CurrentHp <= HNValue1)
            {
                return true;
            }
            else if (Target.CurrentHp <= HNValue2 && HarmonicArrowPvP.Cooldown.CurrentCharges >= 2)
            {
                return true;
            }
            else if (Target.CurrentHp <= HNValue3 && HarmonicArrowPvP.Cooldown.CurrentCharges >= 3)
            {
                return true;
            }
            else if (Target.CurrentHp <= HNValue4 && HarmonicArrowPvP.Cooldown.CurrentCharges >= 4)
            {
                return true;
            }
        }
        if (!UseAutoHarmonic && HarmonicArrowPvP.Cooldown.CurrentCharges >= HarmonicCh && HarmonicArrowPvP.CanUse(out action, usedUp: true))
        {
            return true;
        }
        
        if (PitchPerfectPvP.CanUse(out action))
        {
            return true;
        }

        if (BlastArrowPvP.CanUse(out action))
        {
            return true;
        }

        if (ApexArrowPvP.CanUse(out action))
        {
            return true;
        }

        if (PowerfulShotPvP.CanUse(out action))
        {
            return true;
        }

        if ((!Player.HasStatus(true, StatusID.Guard)) && (!Player.HasStatus(true, StatusID.Sprint)) && SprintPvP.CanUse(out action))
        {
            return true;
        }

        return base.GeneralGCD(out action);
    }
    #endregion
}