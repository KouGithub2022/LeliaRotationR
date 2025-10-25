using Dalamud.Utility;
using ECommons.Automation;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina;

namespace RebornRotations.PVPRotations.Magical;

[Rotation("Lelia Default PVP", CombatType.PvP, GameVersion = "7.35",
    Description = "WrathCombo設定OFFを推奨します。")]
[SourceCode(Path = "main/RebornRotations/PVPRotations/Magical/BLM_Default.PVP.cs")]
//[Api(6)]
public class BLM_DefaultPVP2 : BlackMageRotation
{
    #region Configurations

    [RotationConfig(CombatType.PvP, Name = "Stop attacking while in Guard.")]
    public bool RespectGuard { get; set; } = true;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvP, Name = "ゼノグロシーをダメージ用として使用するために必要なプレイヤーHPの上限値は？\n100%%では使用しません。")]
    public float XenoglossyHighHP1 { get; set; } = 0.8f;

    [Range(0, 66000, ConfigUnitType.None,1)]
    [RotationConfig(CombatType.PvP, Name = "ゼノグロシーをダメージ用として強制使用するために必要な敵HPは？\n使用したくない場合は数値を０にして下さい。")]
    public int XenoglossyHighHP2 { get; set; } = 0;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvP, Name = "ゼノグロシーを回復用として使用するために必要なプレイヤーHPの下限値は？")]
    public float XenoglossyLowHP1 { get; set; } = 0.5f;

    [RotationConfig(CombatType.PvP, Name = "コメットを自動で使います。")]
    public bool UseCometPvP { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "止まってる時はファイア系を使います。(OFF時は常時氷系になります)")]
    public bool UseFirePvP { get; set; } = true;

    [Range(1, 66000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "氷に切り替える自分のHPは？")]
    public int BZValue { get; set; } = 40000;

    /*[RotationConfig(CombatType.PvP, Name = "ブリザード系を主に使います。(パラドックス祭りｗ)")]
    public bool UseBlizzardPvP { get; set; } = true;*/

    [RotationConfig(CombatType.PvP, Name = "LB時、エーテリアルステップを使用します。")]
    public bool UseAMPvP { get; set; } = false;

    //public static IBaseAction SoulResonancePvP = new BaseAction((ActionID)29662);
    public static IBaseAction LBNamePvP = new BaseAction((ActionID)29662);

    public static unsafe int LimitBreakValue => LimitBreakController.Instance()->CurrentUnits;
    public static unsafe int MyLimitBreakLevel => LimitBreakController.Instance()->BarUnits == 0 ? 0 : LimitBreakValue / LimitBreakController.Instance()->BarUnits;
    public static unsafe int LimitBreakMax => LimitBreakController.Instance()->BarCount;


    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvP, Name = "快気を使用するHP%%")]
    public float RecuperateValue { get; set; } = 0.9f;

    [RotationConfig(CombatType.PvP, Name = "LBを使用します")]
    public bool UseLBPvP { get; set; } = false;

    [Range(1, 66000, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvP, Name = "LB:敵のHPは？")]
    public int LBValuePvP { get; set; } = 50000;
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

        if (PurifyPvP.CanUse(out action))
        {
            return true;
        }

        return base.EmergencyAbility(nextGCD, out action);
    }

    [RotationDesc(ActionID.WreathOfIcePvP)]
    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.AttackAbility(nextGCD, out action);
        }

        if (WreathOfIcePvP.CanUse(out action) && InCombat)
        {
            return true;
        }

        return base.DefenseSingleAbility(nextGCD, out action);
    }

    [RotationDesc(ActionID.AetherialManipulationPvP)]
    protected override bool MoveForwardAbility(IAction nextGCD, out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.MoveForwardAbility(nextGCD, out action);
        }

        // Manip yourself
        // if (AetherialManipulationPvP.CanUse(out action)) return true;

        return base.MoveForwardAbility(nextGCD, out action);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? action)
    {
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.AttackAbility(nextGCD, out action);
        }

        if (RecuperatePvP.CanUse(out action) && Player.GetHealthRatio() <= RecuperateValue)
        {
            return true;
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
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.GeneralAbility(nextGCD, out action);
        }

        if (RecuperatePvP.CanUse(out action) && Player.GetHealthRatio() <= RecuperateValue)
        {
            return true;
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
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return base.GeneralGCD(out action);
        }

        if (CurrentTarget is not null && InCombat && UseLBPvP && Target.DistanceToPlayer() <= 25 &&
            MyLimitBreakLevel == 1 &&
            Target.CurrentHp <= LBValuePvP &&
            LBNamePvP.CanUse(out action))
        {
            return true;
        }

        

        if (Player.GetHealthRatio() <= XenoglossyLowHP1 && XenoglossyPvP.Cooldown.CurrentCharges > 0)
        {
            if (XenoglossyPvP.CanUse(out action, usedUp: true)) { return true; }
        }
        else if (Player.GetHealthRatio() > XenoglossyHighHP1 && XenoglossyPvP.Cooldown.CurrentCharges > 0)
        {
            if (XenoglossyPvP.CanUse(out action, usedUp: true)) { return true; }
        }
        else if (Player.GetHealthRatio() > XenoglossyLowHP1 && Target.CurrentHp <= XenoglossyHighHP2 && XenoglossyPvP.Cooldown.CurrentCharges > 0)
        {
            if (XenoglossyPvP.CanUse(out action, usedUp: true)) { return true; }
        }

        if (UseCometPvP && CometPvP.CanUse(out action)) { return true; }

        if (Player.HasStatus(true,StatusID.Paradox) && ParadoxPvE.IsEnabled && ParadoxPvP.CanUse(out action))
        {
            return true;
        }

        /*if (InCombat && (Player.GetHealthRatio() <= 0.8f || Target.DistanceToPlayer() <= 6) && BurstPvP.CanUse(out action))
        {
            return true;
        }*/

        if (CurrentTarget is not null && InCombat && (Player.GetHealthRatio() <= 0.8f || NumberOfHostilesInRangeOf(6) > 0) && BurstPvP.CanUse(out action))
        {
            return true;
        }

        if (UseFirePvP && !IsMoving && Player.CurrentHp >= BZValue)
        {
            if (FlareStarPvP.CanUse(out action))
            {
                return true;
            }

            if (FlarePvP.CanUse(out action, skipAoeCheck: true))
            {
                return true;
            }

            if (WreathOfFirePvP.CanUse(out action) && InCombat)
            {
                return true;
            }

            if (HighFireIiPvP.CanUse(out action))
            {
                return true;
            }

            if (FireIvPvP.CanUse(out action))
            {
                return true;
            }

            if (FireIiiPvP.CanUse(out action))
            {
                return true;
            }

            if (FirePvP.CanUse(out action) && (HighBlizzardIiPvP.IsTheSameTo(true, (ActionID)HighBlizzardIiPvP.ID)))
            {
                return true;
            }
        }
        
        if (!UseFirePvP || IsMoving || Player.CurrentHp < BZValue)
        {
            if (FrostStarPvP.CanUse(out action))
            {
                return true;
            }

            if (FreezePvP.CanUse(out action, skipAoeCheck: true))
            {
                return true;
            }

            if (InCombat && WreathOfIcePvP.CanUse(out action))
            {
                return true;
            }

            if (HighBlizzardIiPvP.CanUse(out action))
            {
                return true;
            }


            if (BlizzardIvPvP.CanUse(out action))
            {
                return true;
            }

            if (BlizzardIiiPvP.CanUse(out action))
            {
                return true;
            }

            if (BlizzardPvP.CanUse(out action) && (HighFireIiPvP.IsTheSameTo(true, (ActionID)HighFireIiPvP.ID)))
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