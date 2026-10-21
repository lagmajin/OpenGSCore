using NUnit.Framework;

namespace OpenGSCore.Tests
{
    /// <summary>
    /// Settings freeze once the room starts running on them.
    /// <para>
    /// The rule is built from the settings when the room is made and reads them
    /// again to decide whether the match is over, so a setting changed in between
    /// means the rule judges the match by limits the room never started under.
    /// </para>
    /// </summary>
    public class MatchSettingLockTests
    {
        [Test]
        public void SettingsStartUnlocked()
        {
            var setting = new DeathMatchSetting(20, true);

            Assert.That(setting.IsLocked, Is.False);
        }

        [Test]
        public void LockingFreezesTheSettings()
        {
            var setting = new DeathMatchSetting(20, true) { MaxPlayerCount = 8 };
            setting.LockSettings();

            setting.MaxPlayerCount = 2;

            Assert.That(setting.MaxPlayerCount, Is.EqualTo(8));
        }

        [Test]
        public void AnUnlockedSettingStillTakesAValue()
        {
            var setting = new DeathMatchSetting(20, true) { MaxPlayerCount = 2 };

            Assert.That(setting.MaxPlayerCount, Is.EqualTo(2));
        }

        [Test]
        public void LockingDoesNotChangeTheValuesAlreadyThere()
        {
            var setting = new DeathMatchSetting(20, true) { MatchTimeMSec = 60000, DamageRate = 2f };

            setting.LockSettings();

            Assert.That(setting.MatchTimeMSec, Is.EqualTo(60000));
            Assert.That(setting.DamageRate, Is.EqualTo(2f));
        }

        [Test]
        public void EverySettingIsFrozenTogether()
        {
            var setting = new DeathMatchSetting(20, true) { DamageRate = 1.5f };
            setting.LockSettings();

            setting.Mode = EGameMode.CaptureTheFlag;
            setting.MatchTimeMSec = 1;
            setting.DamageRate = 9f;
            setting.BoosterRate = 9f;
            setting.TimeLimit = true;
            setting.AllowOvertime = true;

            Assert.That(setting.Mode, Is.EqualTo(EGameMode.DeathMatch));
            Assert.That(setting.MatchTimeMSec, Is.EqualTo(0));
            Assert.That(setting.DamageRate, Is.EqualTo(1.5f));
            Assert.That(setting.BoosterRate, Is.EqualTo(1f));
            Assert.That(setting.TimeLimit, Is.False);
            Assert.That(setting.AllowOvertime, Is.False);
        }

        [Test]
        public void AModeOnlySettingIsFrozenToo()
        {
            // A lock the mode specific settings can walk around is worse than no
            // lock: the room looks frozen while its kill limit still moves.
            var setting = new TDMMatchSetting(maxPlayerCapacity: 4);
            setting.LockSettings();

            setting.WinConditionKill = 1;
            setting.MatchTimeMinutes = 1;

            Assert.That(setting.WinConditionKill, Is.Not.EqualTo(1));
            Assert.That(setting.MatchTimeMinutes, Is.Not.EqualTo(1));
        }

        [Test]
        public void ATeamOnlySettingIsFrozenToo()
        {
            var setting = new TDMMatchSetting(maxPlayerCapacity: 4);
            setting.LockSettings();

            setting.RandomTeam = !setting.RandomTeam;
            setting.TeamBalance = !setting.TeamBalance;

            Assert.That(setting.RandomTeam, Is.True);
            Assert.That(setting.TeamBalance, Is.True);
        }

        [Test]
        public void LockingTwiceIsHarmless()
        {
            var setting = new DeathMatchSetting(20, true) { MaxPlayerCount = 4 };
            setting.LockSettings();

            setting.LockSettings();
            setting.MaxPlayerCount = 3;

            Assert.That(setting.MaxPlayerCount, Is.EqualTo(4));
        }
    }

    /// <summary>
    /// Starting a room freezes the settings it is running on.
    /// <para>
    /// Without this the lock existed but nothing ever took it, so the room's rule
    /// was left judging the match by whatever the settings said at the moment it
    /// decided the match was over rather than by what it started with.
    /// </para>
    /// </summary>
    public class MatchRoomLocksSettingsOnStartTests
    {
        private static MatchRoom NewRoom(AbstractMatchSetting setting)
        {
            return new MatchRoom(1, "room", "host", setting, new MatchRoomEventBus());
        }

        [Test]
        public void StartingARoomFreezesItsSettings()
        {
            var room = NewRoom(new TDMMatchSetting(maxPlayerCapacity: 4));

            room.GameStart();

            Assert.That(room.Setting.IsLocked, Is.True);
        }

        [Test]
        public void SettingsCanStillBeChosenBeforeTheMatchStarts()
        {
            var setting = new TDMMatchSetting(maxPlayerCapacity: 4);
            var room = NewRoom(setting);

            setting.WinConditionKill = 3;

            Assert.That(setting.WinConditionKill, Is.EqualTo(3));
        }

        [Test]
        public void TheMatchCannotBeReconfiguredOnceItIsRunning()
        {
            var setting = new TDMMatchSetting(maxPlayerCapacity: 4);
            var room = NewRoom(setting);
            room.GameStart();
            var killLimitAtStart = setting.WinConditionKill;

            // The room manager writes the capacity it was asked for, and a client
            // can send one at any point in the match.
            setting.MaxPlayerCount = 99;
            setting.WinConditionKill = 1;
            setting.MatchTimeMinutes = 1;

            Assert.That(setting.MaxPlayerCount, Is.EqualTo(4));
            Assert.That(setting.WinConditionKill, Is.EqualTo(killLimitAtStart));
            Assert.That(setting.MatchTimeMinutes, Is.EqualTo(10));
        }

        [Test]
        public void TheRoomKeepsTheSettingsItStartedWith()
        {
            var setting = new TDMMatchSetting(maxPlayerCapacity: 4) { WinConditionKill = 7 };
            var room = NewRoom(setting);

            room.GameStart();

            Assert.That(((TDMMatchSetting)room.Setting).WinConditionKill, Is.EqualTo(7));
        }
    }

    /// <summary>
    /// Searching a player database by key.
    /// <para>
    /// MultipleKey used to drop the id and the address it was built with, so a
    /// key was never valid and every lookup built on one found nothing.
    /// </para>
    /// </summary>
    public class PlayerDatabaseSearchTests
    {
        private static PlayerDatabase NewDatabaseWith(params string[] ids)
        {
            var database = new PlayerDatabase();
            foreach (var id in ids)
            {
                database.AddPlayer(new PlayerData(new PlayerInfo(id, id), new PlayerStatus()));
            }

            return database;
        }

        [Test]
        public void AKeyKeepsTheIdItWasBuiltWith()
        {
            var key = new MultipleKey("player-1", "127.0.0.1");

            Assert.That(key.Id(), Is.EqualTo("player-1"));
            Assert.That(key.HasValidKey(), Is.True);
        }

        [Test]
        public void AKeyWithNothingInItIsNotValid()
        {
            var key = new MultipleKey(null, null);

            Assert.That(key.HasValidKey(), Is.False);
        }

        [Test]
        public void AKeyWithOnlyOneHalfIsNotValid()
        {
            Assert.That(new MultipleKey("player-1", null).HasValidKey(), Is.False);
            Assert.That(new MultipleKey(null, "127.0.0.1").HasValidKey(), Is.False);
        }

        [Test]
        public void SearchingFindsThePlayerWithThatId()
        {
            var database = NewDatabaseWith("one", "two");

            var found = database.Player(new SearchInfo(new MultipleKey("two", "127.0.0.1")));

            Assert.That(found.Count, Is.EqualTo(1));
            Assert.That(found[0].info.Id, Is.EqualTo("two"));
        }

        [Test]
        public void SearchingIgnoresCasing()
        {
            var database = NewDatabaseWith("PlayerOne");

            var found = database.Player(new SearchInfo(new MultipleKey("playerone", "127.0.0.1")));

            Assert.That(found.Count, Is.EqualTo(1));
        }

        [Test]
        public void SearchingForSomebodyWhoIsNotThereFindsNothing()
        {
            var database = NewDatabaseWith("one");

            var found = database.Player(new SearchInfo(new MultipleKey("nobody", "127.0.0.1")));

            Assert.That(found, Is.Empty);
        }

        [Test]
        public void AKeyWithNoIdFindsNothingRatherThanEverybody()
        {
            var database = NewDatabaseWith("one", "two", "three");

            var found = database.Player(new SearchInfo(new MultipleKey(null, "127.0.0.1")));

            Assert.That(found, Is.Empty);
        }

        [Test]
        public void ASearchWithNoKeyAtAllFindsNothing()
        {
            var database = NewDatabaseWith("one");

            Assert.That(database.Player((SearchInfo)null), Is.Empty);
            Assert.That(database.Player(new SearchInfo()), Is.Empty);
        }

        [Test]
        public void APrefixOfAnIdIsNotAMatch()
        {
            var database = NewDatabaseWith("player-one", "player-two");

            var found = database.Player(new SearchInfo(new MultipleKey("player", "127.0.0.1")));

            Assert.That(found, Is.Empty);
        }
    }
}
