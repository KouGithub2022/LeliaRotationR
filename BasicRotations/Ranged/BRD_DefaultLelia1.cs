using static FFXIVClientStructs.FFXIV.Client.Game.Character.Character.Delegates;

namespace DefaultRotations.Ranged;

[Rotation("Lelia's LowLevel", CombatType.PvE, GameVersion = "7.05",
    Description = "Please make sure that the three song times add up to 120 seconds, Wanderers default first song for now.")]
[SourceCode(Path = "main/DefaultRotations/Ranged/BRD_Default.cs")]
[Api(3)]
public sealed class BRD_DefaultLelia1 : BardRotation
{
    #region Config Options
    //[RotationConfig(CombatType.PvE, Name = @"Use Raging Strikes on ""Wanderer's Minuet""")]
    //[RotationConfig(CombatType.PvE, Name = "猛者をメヌエット時に使用する。")]
    //public bool BindWAND { get; set; } = false;

    [Range(1, 45, ConfigUnitType.Seconds, 1)]
    [RotationConfig(CombatType.PvE, Name = "旅神のメヌエットの使用時間")]
    public float WANDTime { get; set; } = 43;

    [Range(0, 45, ConfigUnitType.Seconds, 1)]
    [RotationConfig(CombatType.PvE, Name = "賢人のバラードの使用時間")]
    public float MAGETime { get; set; } = 40;

    [Range(0, 45, ConfigUnitType.Seconds, 1)]
    [RotationConfig(CombatType.PvE, Name = "軍神のパイオンの使用時間")]
    public float ARMYTime { get; set; } = 37;

    [RotationConfig(CombatType.PvE, Name = "First Song")]
    private Song FirstSong { get; set; } = Song.WANDERER;

    //private bool BindWANDEnough => BindWAND && this.TheWanderersMinuetPvE.EnoughLevel;
    private float WANDRemainTime => 45 - WANDTime;
    private float MAGERemainTime => 45 - MAGETime;
    private float ARMYRemainTime => 45 - ARMYTime;
    #endregion

    #region oGCD Logic
    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        if (nextGCD.IsTheSameTo(true, StraightShotPvE, VenomousBitePvE, WindbitePvE, IronJawsPvE))
        {
            return base.EmergencyAbility(nextGCD, out act);
        }
        else if ((!RagingStrikesPvE.EnoughLevel || Player.HasStatus(true, StatusID.RagingStrikes)) && (!BattleVoicePvE.EnoughLevel || Player.HasStatus(true, StatusID.BattleVoice)))
        {
            if ((EmpyrealArrowPvE.Cooldown.IsCoolingDown && !EmpyrealArrowPvE.Cooldown.WillHaveOneChargeGCD(1) || !EmpyrealArrowPvE.EnoughLevel) && Repertoire != 3)
            {
                if (!Player.HasStatus(true, StatusID.HawksEye_3861) && Player.HasStatus(true, StatusID.RagingStrikes) && BarragePvE.CanUse(out act)) return true;
            }
        }

        return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {
        act = null;
        if (Song == Song.NONE && InCombat)
        {
            switch (FirstSong)
            {
                case Song.WANDERER:
                    if (TheWanderersMinuetPvE.CanUse(out act)) return true;
                    break;

                case Song.ARMY:
                    if (ArmysPaeonPvE.CanUse(out act)) return true;
                    break;

                case Song.MAGE:
                    if (MagesBalladPvE.CanUse(out act)) return true;
                    break;
            }
            if (TheWanderersMinuetPvE.CanUse(out act)) return true;
            if (MagesBalladPvE.CanUse(out act)) return true;
            if (ArmysPaeonPvE.CanUse(out act)) return true;
        }

        //UpDate

        if (IsBurst && Song != Song.NONE && MagesBalladPvE.EnoughLevel)
        {
            if (RadiantFinalePvE.EnoughLevel)
            {
                if (BloodletterPvE.CanUse(out act)) return true;

                if (RadiantFinalePvE.CanUse(out act) && Song == Song.WANDERER && (!BattleVoicePvE.Cooldown.IsCoolingDown || BattleVoicePvE.Cooldown.ElapsedAfter(118)))
                {
                    if (TheWanderersMinuetPvE.EnoughLevel) return true;
                }

                if (BattleVoicePvE.CanUse(out act, skipAoeCheck: true) && Song == Song.WANDERER)
                {

                    if (Player.HasStatus(true, StatusID.RadiantFinale_2964) && RadiantFinalePvE.Cooldown.ElapsedOneChargeAfterGCD(1)) return true;
                }

                if (RagingStrikesPvE.CanUse(out act, isLastAbility: true) && Song == Song.WANDERER)
                {
                    if (nextGCD.IsTheSameTo(true, BattleVoicePvE)) return true;
                    if (nextGCD.IsTheSameTo(true, RadiantEncorePvE)) return true;

                    if (Player.HasStatus(true, StatusID.BattleVoice) && RadiantFinalePvE.Cooldown.ElapsedOneChargeAfterGCD(1)) return true;
                }

                if (!Player.HasStatus(true, StatusID.HawksEye_3861) && Player.HasStatus(true, StatusID.RagingStrikes) && BarragePvE.CanUse(out act))
                {
                    if (Song == Song.WANDERER && TheWanderersMinuetPvE.EnoughLevel) return true;
                }
            }

            if (!RadiantFinalePvE.EnoughLevel)
            {
                if (!Player.HasStatus(true, StatusID.RagingStrikes) && BattleVoicePvE.CanUse(out act, skipAoeCheck: true))
                {
                    if ((TheWanderersMinuetPvE.IsEnabled || ArmysPaeonPvE.IsEnabled || MagesBalladPvE.IsEnabled)) return true;
                    if (Song != Song.NONE && (TheWanderersMinuetPvE.IsEnabled || ArmysPaeonPvE.IsEnabled || MagesBalladPvE.IsEnabled)) return true;
                    if (Song == Song.WANDERER && TheWanderersMinuetPvE.EnoughLevel) return true;
                    //if (Song != Song.NONE && !TheWanderersMinuetPvE.EnoughLevel) return true;
                }

                if (RagingStrikesPvE.CanUse(out act, isLastAbility: true))
                {
                    if ((TheWanderersMinuetPvE.IsEnabled || ArmysPaeonPvE.IsEnabled || MagesBalladPvE.IsEnabled)) return true;
                    if (Song != Song.NONE && (TheWanderersMinuetPvE.IsEnabled || ArmysPaeonPvE.IsEnabled || MagesBalladPvE.IsEnabled)) return true;
                    if (Song == Song.WANDERER && TheWanderersMinuetPvE.EnoughLevel) return true;
                    //if (Song != Song.NONE && !TheWanderersMinuetPvE.EnoughLevel) return true;
                }
                if (Player.HasStatus(true, StatusID.RagingStrikes) && !Player.HasStatus(true, StatusID.HawksEye_3861) && BarragePvE.CanUse(out act))
                {
                    if ((TheWanderersMinuetPvE.IsEnabled || ArmysPaeonPvE.IsEnabled || MagesBalladPvE.IsEnabled)) return true;
                    if (Song != Song.NONE && (TheWanderersMinuetPvE.IsEnabled || ArmysPaeonPvE.IsEnabled || MagesBalladPvE.IsEnabled)) return true;
                    if (Song == Song.WANDERER && TheWanderersMinuetPvE.EnoughLevel) return true;
                    //if (Song != Song.NONE && !TheWanderersMinuetPvE.EnoughLevel) return true;
                }
                    
            }
        }
        else if (Song == Song.NONE && !MagesBalladPvE.Cooldown.IsCoolingDown)
        {
             if ((BloodletterPvE.Cooldown.CurrentCharges > 1) &&  BloodletterPvE.CanUse(out act, usedUp: true)) return true;
             if (InCombat && BattleVoicePvE.CanUse(out act, skipAoeCheck: true)) return true;
             if (InCombat && RagingStrikesPvE.CanUse(out act)) return true;
             if (InCombat && Player.HasStatus(true, StatusID.RagingStrikes) && BarragePvE.CanUse(out act)) return true;
        }

        //UpDateEnd

        if (RadiantFinalePvE.EnoughLevel && RadiantFinalePvE.Cooldown.IsCoolingDown && BattleVoicePvE.EnoughLevel && !BattleVoicePvE.Cooldown.IsCoolingDown) return false;

        if (TheWanderersMinuetPvE.CanUse(out act) && InCombat)
        {
            if (SongEndAfter(ARMYRemainTime) && (Song != Song.NONE || Player.HasStatus(true, StatusID.ArmysEthos))) return true;
        }

//UpDate
        if (/*Song != Song.NONE &&*/ EmpyrealArrowPvE.CanUse(out act)) return true;
//UpDateEnd

        if (PitchPerfectPvE.CanUse(out act, skipCastingCheck: true, skipAoeCheck: true, skipComboCheck: true))
        {
            if (SongEndAfter(3) && Repertoire > 0) return true;

            if (Repertoire == 3) return true;

            if (Repertoire == 2 && EmpyrealArrowPvE.Cooldown.WillHaveOneChargeGCD()) return true;
        }

        if (MagesBalladPvE.CanUse(out act) && InCombat)
        {
            if (Song == Song.WANDERER && SongEndAfter(WANDRemainTime) && Repertoire == 0) return true;
            if (Song == Song.ARMY && SongEndAfterGCD(2) && TheWanderersMinuetPvE.Cooldown.IsCoolingDown) return true;
        }

        if (ArmysPaeonPvE.CanUse(out act) && InCombat)
        {
            if (TheWanderersMinuetPvE.EnoughLevel && SongEndAfter(MAGERemainTime) && Song == Song.MAGE) return true;
            if (TheWanderersMinuetPvE.EnoughLevel && SongEndAfter(2) && MagesBalladPvE.Cooldown.IsCoolingDown && Song == Song.WANDERER) return true;
            if (!TheWanderersMinuetPvE.EnoughLevel && SongEndAfter(2)) return true;
        }

        if (SidewinderPvE.CanUse(out act))
        {
            if (Player.HasStatus(true, StatusID.BattleVoice) && (Player.HasStatus(true, StatusID.RadiantFinale) || !RadiantFinalePvE.EnoughLevel)) return true;

            if (!BattleVoicePvE.Cooldown.WillHaveOneCharge(10) && !RadiantFinalePvE.Cooldown.WillHaveOneCharge(10)) return true;

            if (RagingStrikesPvE.Cooldown.IsCoolingDown && !Player.HasStatus(true, StatusID.RagingStrikes)) return true;
        }

        // Bloodletter Overcap protection
        if (RagingStrikesPvE.Cooldown.IsCoolingDown && BloodletterMax == BloodletterPvE.Cooldown.CurrentCharges)
        {
            if (HeartbreakShotPvE.CanUse(out act)) return true;

            if (RainOfDeathPvE.CanUse(out act)) return true;

            if (BloodletterPvE.CanUse(out act)) return true;
        }

        // Prevents Bloodletter bumpcapping when MAGE is the song due to Repetoire procs
        if ((BloodletterPvE.Cooldown.CurrentCharges > 1) && Song == Song.MAGE)
        {
            if (HeartbreakShotPvE.CanUse(out act, usedUp: true)) return true;

            if (RainOfDeathPvE.CanUse(out act, usedUp: true)) return true;

            if (BloodletterPvE.CanUse(out act, usedUp: true)) return true;
        }

        // Allow full use of RagingStrikes
        if (Player.HasStatus(true, StatusID.RagingStrikes))
        {
            if (HeartbreakShotPvE.CanUse(out act, usedUp: true)) return true;

            if (RainOfDeathPvE.CanUse(out act, usedUp: true)) return true;

            if (BloodletterPvE.CanUse(out act, usedUp: true)) return true;
        }

        if (BloodletterMax == 3)
        {
            if (BloodletterLogic(out act)) return true;
        }
        else
        {
            if (BloodletterPvE.CanUse(out act, usedUp: true)) return true;
        }

        return base.AttackAbility(nextGCD, out act);
    }
    #endregion

    #region GCD Logic
    protected override bool GeneralGCD(out IAction? act)
    {
        if (IronJawsPvE.CanUse(out act)) return true;
//UpDate
        if (IronJawsPvE.CanUse(out act, skipStatusProvideCheck: true) && (IronJawsPvE.Target.Target?.WillStatusEnd(30, true, IronJawsPvE.Setting.TargetStatusProvide ?? []) ?? false))
        {
            if (IronJawsPvE.CanUse(out act, skipStatusProvideCheck: true) &&
                (IronJawsPvE.Target.Target?.WillStatusEnd(30, true, IronJawsPvE.Setting.TargetStatusProvide ?? []) ?? false))
            {
                if (Player.HasStatus(true, StatusID.RagingStrikes) &&
                    Player.WillStatusEndGCD(1, 0, true, StatusID.RagingStrikes))
                {
                    return true;
                }
                else if (Player.HasStatus(true, StatusID.RadiantFinale) &&
                         Player.WillStatusEndGCD(1, 0, true, StatusID.RadiantFinale))
                {
                    return true;
                }
            }
        }
//UpDateEnd

        if (ResonantArrowPvE.CanUse(out act)) return true;

        if (CanUseApexArrow(out act)) return true;
//UpDate
        if (Player.HasStatus(true, StatusID.RagingStrikes) && RadiantEncorePvE.CanUse(out act, skipComboCheck: true)) return true;
//UpDateEnd

        if (BlastArrowPvE.CanUse(out act))
        {
            if (!Player.HasStatus(true, StatusID.RagingStrikes)) return true;
            if (Player.HasStatus(true, StatusID.RagingStrikes) && BarragePvE.Cooldown.IsCoolingDown) return true;
        }

        //aoe
        if (ShadowbitePvE.CanUse(out act)) return true;
        if (WideVolleyPvE.CanUse(out act)) return true;
        if (QuickNockPvE.CanUse(out act)) return true;

        if (IronJawsPvE.EnoughLevel && (HostileTarget?.HasStatus(true, StatusID.Windbite, StatusID.Stormbite) == true) && (HostileTarget?.HasStatus(true, StatusID.VenomousBite, StatusID.CausticBite) == true))
        {
            // Do not use WindbitePvE or VenomousBitePvE if both statuses are present and IronJawsPvE has enough level
        }
        else
        {
            if (WindbitePvE.CanUse(out act)) return true;
            if (VenomousBitePvE.CanUse(out act)) return true;
        }


        if (RefulgentArrowPvE.CanUse(out act, skipComboCheck: true)) return true;
        if (StraightShotPvE.CanUse(out act)) return true;
        if (HeavyShotPvE.CanUse(out act) && !Player.HasStatus(true, StatusID.HawksEye_3861)) return true;

        return base.GeneralGCD(out act);
    }
    #endregion

    #region Extra Methods
    private bool CanUseApexArrow(out IAction act)
    {
        if (!ApexArrowPvE.CanUse(out act, skipAoeCheck: true)) return false;

        if (QuickNockPvE.CanUse(out _) && SoulVoice == 100) return true;

        if (SoulVoice == 100 && BattleVoicePvE.Cooldown.WillHaveOneCharge(25)) return false;

        if (SoulVoice >= 80 && Player.HasStatus(true, StatusID.RagingStrikes) && Player.WillStatusEnd(10, false, StatusID.RagingStrikes)) return true;

        if (SoulVoice == 100 && Player.HasStatus(true, StatusID.RagingStrikes) && Player.HasStatus(true, StatusID.BattleVoice)) return true;

        if (Song == Song.MAGE && SoulVoice >= 80 && SongEndAfter(22) && SongEndAfter(18)) return true;

        if (!Player.HasStatus(true, StatusID.RagingStrikes) && SoulVoice == 100) return true;

        return false;
    }
    private bool BloodletterLogic(out IAction? act)
    {
        bool isRagingStrikesLevel = RagingStrikesPvE.EnoughLevel;
        bool isBattleVoiceLevel = BattleVoicePvE.EnoughLevel;
        bool isRadiantFinaleLevel = RadiantFinalePvE.EnoughLevel;

        if (HeartbreakShotPvE.CanUse(out act, usedUp: true))
        {
            if ((!isRagingStrikesLevel)
                || (isRagingStrikesLevel && !isBattleVoiceLevel && Player.HasStatus(true, StatusID.RagingStrikes))
                || (isBattleVoiceLevel && !isRadiantFinaleLevel && Player.HasStatus(true, StatusID.RagingStrikes) && Player.HasStatus(true, StatusID.BattleVoice))
                || isRadiantFinaleLevel && Player.HasStatus(true, StatusID.RagingStrikes) && Player.HasStatus(true, StatusID.BattleVoice) && Player.HasStatus(true, StatusID.RadiantFinale)) return true;
        }

        if (RainOfDeathPvE.CanUse(out act, usedUp: true))
        {
            if ((!isRagingStrikesLevel)
                || (isRagingStrikesLevel && !isBattleVoiceLevel && Player.HasStatus(true, StatusID.RagingStrikes))
                || (isBattleVoiceLevel && !isRadiantFinaleLevel && Player.HasStatus(true, StatusID.RagingStrikes) && Player.HasStatus(true, StatusID.BattleVoice))
                || isRadiantFinaleLevel && Player.HasStatus(true, StatusID.RagingStrikes) && Player.HasStatus(true, StatusID.BattleVoice) && Player.HasStatus(true, StatusID.RadiantFinale)) return true;
        }

        if (BloodletterPvE.CanUse(out act, usedUp: true))
        {
            if ((!isRagingStrikesLevel)
                || (isRagingStrikesLevel && !isBattleVoiceLevel && Player.HasStatus(true, StatusID.RagingStrikes))
                || (isBattleVoiceLevel && !isRadiantFinaleLevel && Player.HasStatus(true, StatusID.RagingStrikes) && Player.HasStatus(true, StatusID.BattleVoice))
                || isRadiantFinaleLevel && Player.HasStatus(true, StatusID.RagingStrikes) && Player.HasStatus(true, StatusID.BattleVoice) && Player.HasStatus(true, StatusID.RadiantFinale)) return true;
        }
        return false;
    }
    #endregion
}
