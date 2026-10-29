using NUnit.Framework;
using OpenGSCore;

namespace OpenGSCore.Tests
{
    /// <summary>
    /// The match length every free for all mode is configured with.
    /// <para>
    /// None of these settings could express one, and the two rules for them were
    /// built with only a kill limit, so they took the five minute default of an
    /// unrelated rule. A match nobody chose a length for ran on another mode's.
    /// </para>
    /// </summary>
    public class FreeForAllSettingTests
    {
        [Test]
        public void NoSettingTheFactoryAcceptsLeavesAMatchWithNoTimeOnIt()
        {
            // The default used to be five minutes, which hid the mistake: a
            // setting that never wrote a length produced a rule reading zero, and
            // zero is the same number a finished match reads, so the rule ended
            // the match on its first look. The sentinel is now explicit, which
            // means it can be held to never reaching a room.
            var settings = new AbstractMatchSetting[]
            {
                new DeathMatchSetting(),
                new OneShotKillMatchSetting(),
                new ArmsRaceMatchSetting(),
                new TDMMatchSetting(),
                new SuvMatchSetting(8, false),
                new TeamSurvivalMatchSetting(),
                new CaptureTheFlagMatchSetting()
            };

            foreach (var setting in settings)
            {
                var rule = MatchRuleFactory.CreateMatchRule(setting);
                Assert.That(
                    rule!.MatchTimeMSec(),
                    Is.Not.EqualTo(AbstractMatchRule.NoTimeLimit),
                    $"{setting.Mode} produced a rule with no time on it, which reads as a match already over");
            }
        }

        [Test]
        public void ARuleBuiltWithNoLengthSaysSoRatherThanSayingZero()
        {
            // Zero is what a finished match reports, so a rule that means "I was
            // never told" must not say it. That is the whole reason the sentinel
            // exists.
            Assert.That(AbstractMatchRule.NoTimeLimit, Is.EqualTo(0));
            Assert.That(
                new UntimedRule().MatchTimeMSec(),
                Is.EqualTo(AbstractMatchRule.NoTimeLimit));
        }

        [Test]
        public void AOneShotKillRoomCanAskForADifferentNumberOfKills()
        {
            // The kill limit was a private field behind a getter, so nothing
            // outside the constructor could change it and a "first to three" room
            // could not be created.
            var setting = new OneShotKillMatchSetting { WinConditionKill = 3 };

            Assert.That(setting.WinConditionKill, Is.EqualTo(3));
        }

        [Test]
        public void AnArmsRaceRoomCanAskForADifferentNumberOfKillsAndALength()
        {
            var setting = new ArmsRaceMatchSetting
            {
                WinConditionKill = 45,
                MatchTimeMinutes = 9
            };

            Assert.That(setting.WinConditionKill, Is.EqualTo(45));
            Assert.That(setting.MatchTimeMinutes, Is.EqualTo(9));
        }

        [Test]
        public void AOneShotKillMatchRunsOnTheLengthItsSettingAsksFor()
        {
            // The factory built this mode's rule with only a kill limit, so it
            // inherited the five minute default of a mode that was not playing.
            var setting = new OneShotKillMatchSetting { MatchTimeMinutes = 4 };
            var rule = (DeathMatchRule)MatchRuleFactory.CreateMatchRule(setting)!;

            Assert.That(rule.MatchTimeMSec(), Is.EqualTo(4 * 60 * 1000));
            Assert.That(rule.KillLimit, Is.EqualTo(1));
        }

        [Test]
        public void AnArmsRaceMatchRunsOnTheLengthItsSettingAsksFor()
        {
            var setting = new ArmsRaceMatchSetting
            {
                MatchTimeMinutes = 12,
                WinConditionKill = 40
            };
            var rule = (DeathMatchRule)MatchRuleFactory.CreateMatchRule(setting)!;

            Assert.That(rule.MatchTimeMSec(), Is.EqualTo(12 * 60 * 1000));
            Assert.That(rule.KillLimit, Is.EqualTo(40));
        }

        [Test]
        public void ADeathMatchRunsOnTheLengthItsSettingAsksFor()
        {
            var setting = new DeathMatchSetting { MatchTimeMinutes = 6 };
            var rule = (DeathMatchRule)MatchRuleFactory.CreateMatchRule(setting)!;

            Assert.That(rule.MatchTimeMSec(), Is.EqualTo(6 * 60 * 1000));
        }

        [Test]
        public void EveryFreeForAllRoomHasTimeOnItsClockFromTheStart()
        {
            // The room asks its rule for a duration and sets the match's remaining
            // time from it, so a rule answering nothing is a match already over.
            foreach (var setting in new AbstractMatchSetting[]
            {
                new DeathMatchSetting { MatchTimeMinutes = 6 },
                new OneShotKillMatchSetting { MatchTimeMinutes = 4 },
                new ArmsRaceMatchSetting { MatchTimeMinutes = 8 }
            })
            {
                var room = new MatchRoom(1, "ffa", "host", setting, new MatchRoomEventBus());
                room.AddNewPlayer(new PlayerInfo("one", "One") { Health = 100 });
                room.AddNewPlayer(new PlayerInfo("two", "Two") { Health = 100 });
                room.GameStart();

                Assert.That(
                    room.RemainingTimeSeconds,
                    Is.GreaterThan(0f),
                    $"{setting.Mode} started with no time on its clock");
                Assert.That(room.IsMatchFinished(), Is.False);
            }
        }

        /// <summary>
        /// A rule that was never told how long its match runs.
        /// <para>
        /// The rules for the two modes that had no length to give were built
        /// without one, so this is the shape they actually had. It is here so the
        /// sentinel can be tested without a rule in the package whose only purpose
        /// is to be incomplete.
        /// </para>
        /// </summary>
        private sealed class UntimedRule : AbstractMatchRule
        {
            public UntimedRule()
                : base(EGameMode.DeathMatch)
            {
            }

            public override bool IsMatchFinished(AbstractMatchSituation situation) => false;
        }
    }
}
