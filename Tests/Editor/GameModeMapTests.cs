using NUnit.Framework;
using OpenGSCore;

namespace OpenGSCore.Tests
{
    /// <summary>
    /// Which map a room ends up on, for a given mode.
    /// <para>
    /// A room that names no map used to keep an unknown one, and the client fell
    /// back to a map of its own choosing. For capture the flag that was a stage
    /// with no flag stands on it, so a room could be created for a mode nobody
    /// could win. The map a mode can be played on is a fact about the mode, and
    /// this is where it is stated.
    /// </para>
    /// </summary>
    public class GameModeMapTests
    {
        [Test]
        public void ACaptureTheFlagRoomGetsAFlagMap()
        {
            var map = GameMode.DefaultMapFor(EGameMode.CaptureTheFlag);

            Assert.That(map.HasValue, Is.True, "capture the flag has no map to play on");
            Assert.That(GameMode.MapsFor(EGameMode.CaptureTheFlag), Does.Contain(map.Value));
        }

        [Test]
        public void ACaptureTheFlagRoomNeverGetsADeathMatchMap()
        {
            // The whole point: a stage with no flag stands cannot be captured on.
            // The maps a capture the flag room can use are the ones built for it,
            // and nothing else is playable in that mode.
            foreach (var map in GameMode.MapsFor(EGameMode.CaptureTheFlag))
            {
                Assert.That(
                    map.ToString().EndsWith("CTF", System.StringComparison.Ordinal),
                    Is.True,
                    $"{map} has no flag stands and cannot be captured on");
            }
        }

        [Test]
        public void EveryModeHasAMapToPlayOn()
        {
            foreach (var mode in GameMode.AllGameMode())
            {
                Assert.That(
                    GameMode.DefaultMapFor(mode),
                    Is.Not.Null,
                    $"{mode} is offered to a player with nowhere to play it");
            }
        }

        [Test]
        public void AModeWithNoListOfItsOwnFallsBackOnThePlainMaps()
        {
            // A mode with no list is not a mode with nowhere to play: it plays on
            // the plain maps, which is what the default bucket is for. The
            // capture the flag mode is the one that cannot do this, because a
            // plain map has no flag stands.
            Assert.That(
                GameMode.DefaultMapFor(EGameMode.TowerMatch),
                Is.EqualTo(GameMode.DefaultMapFor(EGameMode.DeathMatch)));
        }

        /// <summary>
        /// Builds the setting a create-room request would produce.
        /// <para>
        /// The has-flags on a setting say whether the request actually named the
        /// field, and they are set by reading the request rather than by whoever
        /// is building one, so a test goes through the same reader a request
        /// would. Building one by hand would set a field the request never sent
        /// and prove something else.
        /// </para>
        /// </summary>
        private static RoomSetting SettingFrom(params (string Key, object? Value)[] fields)
        {
            var json = new Newtonsoft.Json.Linq.JObject();
            foreach (var (key, value) in fields)
            {
                json[key] = value == null
                    ? null
                    : new Newtonsoft.Json.Linq.JValue(value);
            }

            return RoomSetting.FromJson(json);
        }

        [Test]
        public void ARoomThatNamesAMapKeepsIt()
        {
            var room = new WaitRoom("host", "Room");
            room.ChangeGameMode(EGameMode.DeathMatch);
            room.Map = EMap.Nocturne;

            SettingFrom(("Map", nameof(EMap.Nocturne))).ApplyTo(room);

            Assert.That(room.Map, Is.EqualTo(EMap.Nocturne));
        }

        [Test]
        public void ARoomThatNamesNoMapGetsOneTheModeCanBePlayedOn()
        {
            var room = new WaitRoom("host", "Room");

            SettingFrom(("GameMode", nameof(EGameMode.CaptureTheFlag))).ApplyTo(room);

            Assert.That(room.Map, Is.EqualTo(GameMode.DefaultMapFor(EGameMode.CaptureTheFlag)));
            Assert.That(room.Map, Is.Not.EqualTo(EMap.Unknown));
        }

        [Test]
        public void ARoomThatNamesAMapThatDoesNotExistGetsOneTheModeCanBePlayedOn()
        {
            var room = new WaitRoom("host", "Room");

            SettingFrom(
                ("GameMode", nameof(EGameMode.CaptureTheFlag)),
                ("Map", "NoSuchMap")).ApplyTo(room);

            Assert.That(room.Map, Is.EqualTo(GameMode.DefaultMapFor(EGameMode.CaptureTheFlag)));
        }

        [Test]
        public void ChangingModeMovesTheRoomOffTheOldModesMap()
        {
            var room = new WaitRoom("host", "Room");
            room.ChangeGameMode(EGameMode.DeathMatch);
            Assert.That(GameMode.MapsFor(EGameMode.DeathMatch), Does.Contain(room.Map));

            room.ChangeGameMode(EGameMode.CaptureTheFlag);

            // A room that kept the death match map is a capture the flag room on a
            // stage with no flag stands, which is a match nobody can win.
            Assert.That(GameMode.MapsFor(EGameMode.CaptureTheFlag), Does.Contain(room.Map));
        }
    }
}
