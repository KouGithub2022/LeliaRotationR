namespace DefaultRotations.Ranged;

[Rotation("Default_Custom", CombatType.PvE, GameVersion = "7.05",
    Description = "Please make sure that the three song times add up to 120 seconds, Wanderers default first song for now.")]
[SourceCode(Path = "main/DefaultRotations/Ranged/BRD_Default.cs")]
[Api(4)]
public sealed class BRD_DefaultCustom : BardRotation
{
    #region Config Options

    [Range(1, 45, ConfigUnitType.Seconds, 1)]
    [RotationConfig(CombatType.PvE, Name = "Wanderer's Minuet Uptime")]
    public float WANDTime { get; set; } = 43;

    [Range(0, 45, ConfigUnitType.Seconds, 1)]
    [RotationConfig(CombatType.PvE, Name = "Mage's Ballad Uptime")]
    public float MAGETime { get; set; } = 40;

    [Range(0, 45, ConfigUnitType.Seconds, 1)]
    [RotationConfig(CombatType.PvE, Name = "Army's Paeon Uptime")]
    public float ARMYTime { get; set; } = 37;

    [RotationConfig(CombatType.PvE, Name = "First Song")]
    private Song FirstSong { get; set; } = Song.WANDERER;

    private float WANDRemainTime => 45 - WANDTime;
    private float MAGERemainTime => 45 - MAGETime;
    private float ARMYRemainTime => 45 - ARMYTime;

    private static bool InBurstStatus => (Player.Level > 50 && !Player.WillStatusEnd(0, true, StatusID.RagingStrikes))
        || (Player.Level >= 50 && Player.Level < 90 && !Player.WillStatusEnd(0, true, StatusID.RagingStrikes) && !Player.WillStatusEnd(0, true, StatusID.BattleVoice)) 
        || (MinstrelsCodaTrait.EnoughLevel && !Player.WillStatusEnd(0, true, StatusID.RagingStrikes) && !Player.WillStatusEnd(0, true, StatusID.RadiantFinale) && !Player.WillStatusEnd(0, true, StatusID.BattleVoice));

    #endregion

    #region Countdown logic
    // Defines logic for actions to take during the countdown before combat starts.
    protected override IAction? CountDownAction(float remainTime)
    {
        // tincture needs to be used on -0.7s exactly
        if (remainTime <= 0.7f && UseBurstMedicine(out var act)) return act;
        return base.CountDownAction(remainTime);
    }
    #endregion

    #region oGCD Logic
    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        if (nextGCD.IsTheSameTo(true, StraightShotPvE, VenomousBitePvE, WindbitePvE, IronJawsPvE))
        {
            return base.EmergencyAbility(nextGCD, out act);
        }
        else if (!RagingStrikesPvE.EnoughLevel || Player.HasStatus(true, StatusID.RagingStrikes))
        {
            if ((EmpyrealArrowPvE.Cooldown.IsCoolingDown && !EmpyrealArrowPvE.Cooldown.WillHaveOneChargeGCD(1) || !EmpyrealArrowPvE.EnoughLevel) && Repertoire != 3)
            {
                if (!Player.HasStatus(true, StatusID.HawksEye_3861) && BarragePvE.CanUse(out act)) return true;
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

        if (IsBurst && Song != Song.NONE && MagesBalladPvE.EnoughLevel)
        {
            if (BloodletterPvE.CanUse(out act)) return true;

            /*if (BloodletterPvE.CanUse(out act, usedUp: true) && HeartbreakShotPvE.Cooldown.CurrentCharges == 3) return true;
            if (BloodletterPvE.CanUse(out act, usedUp: true) && HeartbreakShotPvE.Cooldown.CurrentCharges == 2)
            {
                if (!BattleVoicePvE.Cooldown.ElapsedAfter(116) || !BattleVoicePvE.Cooldown.IsCoolingDown) return true;
            }*/

            if (((!RadiantFinalePvE.EnoughLevel && !RagingStrikesPvE.Cooldown.IsCoolingDown)
                    || (RadiantFinalePvE.EnoughLevel && !RadiantFinalePvE.Cooldown.IsCoolingDown && RagingStrikesPvE.EnoughLevel && !RagingStrikesPvE.Cooldown.IsCoolingDown))
                    && BattleVoicePvE.CanUse(out act, isLastAbility: false)) return true;

            if (!Player.WillStatusEnd(0, true, StatusID.BattleVoice) && RadiantFinalePvE.CanUse(out act)) return true;

            if (((RadiantFinalePvE.EnoughLevel && !Player.WillStatusEnd(0, true, StatusID.RadiantFinale) && !Player.WillStatusEnd(0, true, StatusID.BattleVoice))
                || (!RadiantFinalePvE.EnoughLevel && BattleVoicePvE.EnoughLevel && !Player.WillStatusEnd(0, true, StatusID.BattleVoice))
                || (!RadiantFinalePvE.EnoughLevel && !BattleVoicePvE.EnoughLevel))
                && RagingStrikesPvE.CanUse(out act)) return true;
        }
        
        if (RadiantFinalePvE.EnoughLevel && RadiantFinalePvE.Cooldown.IsCoolingDown && BattleVoicePvE.EnoughLevel && !BattleVoicePvE.Cooldown.IsCoolingDown) return false;

        if (TheWanderersMinuetPvE.CanUse(out act) && InCombat)
        {
            if (SongEndAfter(ARMYRemainTime) && (Song != Song.NONE || Player.HasStatus(true, StatusID.ArmysEthos))) return true;
        }

        if (Song != Song.NONE && EmpyrealArrowPvE.CanUse(out act)) return true;

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

        if (BetterBloodletterLogic(out act)) return true;

        return base.AttackAbility(nextGCD, out act);
    }
    #endregion

    #region GCD Logic
    protected override bool GeneralGCD(out IAction? act)
    {
        if (IronJawsPvE.CanUse(out act)) return true;
        if (IronJawsPvE.CanUse(out act, skipStatusProvideCheck: true) && (IronJawsPvE.Target.Target?.WillStatusEnd(30, true, IronJawsPvE.Setting.TargetStatusProvide ?? []) ?? false))
        {
            if (Player.HasStatus(true, StatusID.BattleVoice) && Player.WillStatusEndGCD(1, 0, true, StatusID.BattleVoice)) return true;
        }

        //UpDate
        /*if (IronJawsPvE.CanUse(out act, skipStatusProvideCheck: true) && (IronJawsPvE.Target.Target?.WillStatusEnd(30, true, IronJawsPvE.Setting.TargetStatusProvide ?? []) ?? false))
        {
            if (IronJawsPvE.CanUse(out act, skipStatusProvideCheck: true) &&
                (IronJawsPvE.Target.Target?.WillStatusEnd(30, true, IronJawsPvE.Setting.TargetStatusProvide ?? []) ?? false))
            {
                if (Player.HasStatus(true, StatusID.BattleVoice) &&
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
        }*/
        //UpDateEnd

        if (Song == Song.WANDERER && Player.HasStatus(true, StatusID.RagingStrikes) && SoulVoice == 100)
        {
            if (ApexArrowPvE.CanUse(out act, skipAoeCheck: true)) return true;
        }
        if (BlastArrowPvE.CanUse(out act)) return true;

        if (ResonantArrowPvE.CanUse(out act)) return true;

        if (CanUseApexArrow(out act)) return true;
        if (RadiantEncorePvE.CanUse(out act, skipComboCheck: true))
        {
            if (InBurstStatus) return true;
        }

        if (BlastArrowPvE.CanUse(out act))
        {
            if (!Player.HasStatus(true, StatusID.RagingStrikes)) return true;
            if (Player.HasStatus(true, StatusID.RagingStrikes) /*&& BarragePvE.Cooldown.IsCoolingDown*/) return true;
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
        if (Player.HasStatus(true, StatusID.RadiantFinale)) return false;

        if (QuickNockPvE.CanUse(out _) && SoulVoice == 100) return true;

        if (SoulVoice == 100 && BattleVoicePvE.Cooldown.WillHaveOneCharge(25)) return false;

        if (SoulVoice >= 80 && Player.HasStatus(true, StatusID.RagingStrikes) && Player.WillStatusEnd(10, false, StatusID.RagingStrikes)) return true;

        if (SoulVoice == 100 && Player.HasStatus(true, StatusID.RagingStrikes) && Player.HasStatus(true, StatusID.BattleVoice)) return true;

        if (Song == Song.MAGE && SoulVoice >= 80 && SongEndAfter(22) && SongEndAfter(18)) return true;

        if (!Player.HasStatus(true, StatusID.RagingStrikes) && SoulVoice == 100) return true;

        return false;
    }
    private bool BetterBloodletterLogic(out IAction? act)
    {
        bool isRagingStrikesLevel = RagingStrikesPvE.EnoughLevel;
        bool isBattleVoiceLevel = BattleVoicePvE.EnoughLevel;
        bool isRadiantFinaleLevel = RadiantFinalePvE.EnoughLevel;

        if (HeartbreakShotPvE.CanUse(out act, usedUp: true))
        {
            if ((!isRagingStrikesLevel)
                /*|| (Song == Song.WANDERER && !Player.HasStatus(true, StatusID.BattleVoice) && HeartbreakShotPvE.Cooldown.CurrentCharges >= 2)*/
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