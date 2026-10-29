using NUnit.Framework;

namespace OpenGSCore.Tests
{
    /// <summary>
    /// Player id lookups in MatchRoom have to agree about casing.
    /// <para>
    /// RemovePlayer compared ids without case while ContainsPlayer,
    /// TryGetPlayer, AddNewPlayer, and the pose state accessors used a plain
    /// string equality. A pose was also stored under whatever id the caller
    /// passed rather than the one the player actually had, so a lookup could
    /// never find it again. Ids are guids in the N format, which does not fix
    /// the casing of one that arrives from a client.
    /// </para>
    /// </summary>
    public class MatchRoomPlayerIdTests
    {
        private const string MixedCaseId = "AbCdEf0123456789ABCDEF012345678";

        private static MatchRoom NewRoomWithPlayer(out string id)
        {
            var bus = new MatchRoomEventBus();
            var room = new MatchRoom(0, "room", "owner", new DeathMatchSetting(20, true), bus);
            id = MixedCaseId;
            room.AddNewPlayer(new PlayerInfo(id, "Player"));
            return room;
        }

        [Test]
        public void ContainsPlayerIgnoresCasing()
        {
            var room = NewRoomWithPlayer(out var id);

            Assert.That(room.ContainsPlayer(id), Is.True);
            Assert.That(room.ContainsPlayer(id.ToUpperInvariant()), Is.True);
            Assert.That(room.ContainsPlayer(id.ToLowerInvariant()), Is.True);
        }

        [Test]
        public void TryGetPlayerIgnoresCasing()
        {
            var room = NewRoomWithPlayer(out var id);

            Assert.That(room.TryGetPlayer(id.ToUpperInvariant(), out var upper), Is.True);
            Assert.That(upper!.Id, Is.EqualTo(id));
            Assert.That(room.TryGetPlayer(id.ToLowerInvariant(), out var lower), Is.True);
            Assert.That(lower!.Id, Is.EqualTo(id));
        }

        [Test]
        public void AddNewPlayerDoesNotDuplicateTheSamePlayerInAnotherCasing()
        {
            var room = NewRoomWithPlayer(out var id);

            room.AddNewPlayer(new PlayerInfo(id.ToUpperInvariant(), "Player"));

            Assert.That(room.Players.Count, Is.EqualTo(1));
        }

        [Test]
        public void RemovePlayerAndContainsPlayerAgreeOnCasing()
        {
            var room = NewRoomWithPlayer(out var id);

            // RemovePlayer already ignored case; the other lookups did not, so a
            // player could be found by one call and missed by another.
            room.RemovePlayer(id.ToUpperInvariant());

            Assert.That(room.ContainsPlayer(id), Is.False);
            Assert.That(room.ContainsPlayer(id.ToUpperInvariant()), Is.False);
        }

        [Test]
        public void APoseSetInAnotherCasingIsStillReadable()
        {
            var room = NewRoomWithPlayer(out var id);

            room.SetPlayerPoseState(id.ToUpperInvariant(), EPlayerPoseState.LieDown);

            Assert.That(room.GetPlayerPoseState(id), Is.EqualTo(EPlayerPoseState.LieDown));
            Assert.That(room.GetPlayerPoseState(id.ToUpperInvariant()), Is.EqualTo(EPlayerPoseState.LieDown));
            Assert.That(room.GetPlayerPoseState(id.ToLowerInvariant()), Is.EqualTo(EPlayerPoseState.LieDown));
        }

        [Test]
        public void AnUnknownIdReadsAsStanding()
        {
            var room = NewRoomWithPlayer(out _);

            room.SetPlayerPoseState("nobody", EPlayerPoseState.Sit);

            Assert.That(room.GetPlayerPoseState("nobody"), Is.EqualTo(EPlayerPoseState.Stand));
        }

        [Test]
        public void ABlankIdIsNeverAPlayer()
        {
            var room = NewRoomWithPlayer(out _);

            Assert.That(room.ContainsPlayer(""), Is.False);
            Assert.That(room.ContainsPlayer("   "), Is.False);
            Assert.That(room.ContainsPlayer(null!), Is.False);
            Assert.That(room.TryGetPlayer("", out _), Is.False);
        }

        [Test]
        public void ChangeOwnerCarriesAPlayersOwnIdCasing()
        {
            var bus = new MatchRoomEventBus();
            var room = new MatchRoom(0, "room", "owner", new DeathMatchSetting(20, true), bus);
            room.AddNewPlayer(new PlayerInfo(MixedCaseId, "Player"));
            room.AddNewPlayer(new PlayerInfo("second", "Second"));

            room.ChangeOwnerRandom();

            Assert.That(room.OwnerId, Is.Not.Null);
            Assert.That(room.ContainsPlayer(room.OwnerId!), Is.True);
        }
    }
}