using NUnit.Framework;

namespace OpenGSCore.Tests
{
    /// <summary>
    /// C0 requires the canonical message names to win over legacy aliases.
    /// These aliases used to collapse onto unrelated canonical names, which
    /// made the wire contract ambiguous.
    /// </summary>
    public class MessageTypeTests
    {
        [Test]
        public void LobbyAliasesUseTheirOwnWireNames()
        {
            Assert.That(MessageType.LobbyEnter, Is.EqualTo("LobbyEnter"));
            Assert.That(MessageType.LobbyLeave, Is.EqualTo("LobbyLeave"));
        }

        [Test]
        public void WaitRoomUpdateNotificationIsNotAnAliasOfUpdateRoomResponse()
        {
            // Both are sent on the lobby TCP stream, so they must stay distinct.
            Assert.That(
                MessageType.WaitRoomUpdateNotification,
                Is.Not.EqualTo(MessageType.UpdateRoomResponse));
        }

        [Test]
        public void NormalizeResolvesLegacyMatchEndAlias()
        {
            Assert.That(MessageType.Normalize("MatchEnd"), Is.EqualTo(MessageType.MatchEndNotification));
        }

        [Test]
        public void NormalizeKeepsCanonicalNamesUnchanged()
        {
            Assert.That(
                MessageType.Normalize(MessageType.LobbyEnter),
                Is.EqualTo(MessageType.LobbyEnter));

            Assert.That(
                MessageType.Normalize(MessageType.WaitRoomUpdateNotification),
                Is.EqualTo(MessageType.WaitRoomUpdateNotification));
        }

        [Test]
        public void NormalizeResolvesSendEnterRoomAlias()
        {
            Assert.That(MessageType.Normalize("SendEnterRoom"), Is.EqualTo(MessageType.JoinRoomRequest));
        }

        [Test]
        public void NormalizeLeavesUnknownNamesUntouched()
        {
            // Names the contract does not know must survive so the handler can
            // report them instead of silently dispatching elsewhere.
            Assert.That(MessageType.Normalize("JoinRoom"), Is.EqualTo("JoinRoom"));
        }

        [Test]
        public void AFieldItemPickupClaimAndItsRulingShareOneName()
        {
            // The claim and the ruling are the same fact, so a client that
            // dispatches on one name has to receive the other. They were two
            // literals, so the ruling fell into the unhandled branch and every
            // granted pickup was taken back by the client's own timeout.
            Assert.That(MessageType.FieldItemPickup, Is.EqualTo("FieldItemPickup"));
            Assert.That(MessageType.ItemPickup, Is.EqualTo(MessageType.FieldItemPickup));
        }

        [Test]
        public void NormalizeResolvesTheLegacyFieldItemPickupClaim()
        {
            // A client built before the shared contract claimed with the
            // shorter name, so that claim has to reach the same handler.
            Assert.That(
                MessageType.Normalize("ItemPickup"),
                Is.EqualTo(MessageType.FieldItemPickup));
        }

        [Test]
        public void NormalizeKeepsTheCanonicalPickupNameUnchanged()
        {
            Assert.That(
                MessageType.Normalize(MessageType.FieldItemPickup),
                Is.EqualTo(MessageType.FieldItemPickup));
        }

        [Test]
        public void AClientClaimAndAServerRulingAreNamedApart()
        {
            // A claim is something a client asserts about itself and a ruling is
            // what the server decided, so they must not share a name: a client
            // reading the ruling under the name it sent the claim with cannot
            // tell its own request from the answer. Health, death, a weapon
            // claim and a spent item are each a pair of these.
            Assert.That(MessageType.PlayerDamage, Is.Not.EqualTo(MessageType.PlayerDamaged));
            Assert.That(MessageType.PlayerDeath, Is.Not.EqualTo(MessageType.PlayerKilled));
            Assert.That(MessageType.WeaponReserve, Is.Not.EqualTo(MessageType.WeaponReserved));
            Assert.That(MessageType.WeaponRelease, Is.Not.EqualTo(MessageType.WeaponReleased));
            Assert.That(MessageType.ItemUse, Is.Not.EqualTo(MessageType.ItemUsed));
        }

        [Test]
        public void TheServerRulingNamesAreStable()
        {
            // These are the labels the server sends and the client dispatches
            // on. Pinning them means a rename has to be deliberate on both sides
            // rather than drifting apart as a pair of literals did.
            Assert.That(MessageType.PlayerDamaged, Is.EqualTo("PlayerDamaged"));
            Assert.That(MessageType.PlayerKilled, Is.EqualTo("PlayerKilled"));
            Assert.That(MessageType.WeaponReserved, Is.EqualTo("WeaponReserved"));
            Assert.That(MessageType.WeaponReleased, Is.EqualTo("WeaponReleased"));
            Assert.That(MessageType.WeaponDropped, Is.EqualTo("WeaponDropped"));
            Assert.That(MessageType.ItemUsed, Is.EqualTo("ItemUsed"));
            Assert.That(MessageType.ItemUseRefused, Is.EqualTo("ItemUseRefused"));
        }

        [Test]
        public void AFieldItemSpawnHasOneNameWhicheverSideSaysIt()
        {
            // A spawn and a despawn were spelled three ways between the two sides
            // and neither pair matched, so an item the server put on the map
            // reached a client under no name it dispatched on.
            Assert.That(MessageType.FieldItemSpawn, Is.EqualTo(MessageType.ItemSpawnNotification));
            Assert.That(MessageType.FieldItemDespawn, Is.EqualTo(MessageType.ItemDespawnNotification));
        }

        [Test]
        public void EveryFieldItemSpawnSpellingResolvesToTheOneName()
        {
            foreach (var spelling in new[] { "ItemSpawn", "FieldItemSpawn", MessageType.ItemSpawnNotification })
            {
                Assert.That(
                    MessageType.Normalize(spelling),
                    Is.EqualTo(MessageType.ItemSpawnNotification),
                    $"'{spelling}' did not resolve to the name the server sends");
            }

            foreach (var spelling in new[] { "ItemDespawn", "FieldItemDespawn", MessageType.ItemDespawnNotification })
            {
                Assert.That(
                    MessageType.Normalize(spelling),
                    Is.EqualTo(MessageType.ItemDespawnNotification),
                    $"'{spelling}' did not resolve to the name the server sends");
            }
        }

        [Test]
        public void TheRoomStateNamesAreDistinct()
        {
            // A client reads a Snapshot to learn the room state and a MatchJoined
            // to learn it was admitted. They are separate facts: a room state can
            // change several times in a match while the admission happens once, so
            // one name for both would make a client unable to tell a change from
            // a join.
            Assert.That(MessageType.Snapshot, Is.Not.EqualTo(MessageType.MatchJoined));
            Assert.That(MessageType.Snapshot, Is.EqualTo("Snapshot"));
            Assert.That(MessageType.MatchJoined, Is.EqualTo("MatchJoined"));
        }

        [Test]
        public void TheServerRulingNamesSurviveNormalize()
        {
            // A ruling arriving on a channel that normalizes would be renamed and
            // miss its case if these resolved to anything else.
            foreach (var ruling in new[]
            {
                MessageType.PlayerDamaged,
                MessageType.PlayerKilled,
                MessageType.WeaponReserved,
                MessageType.WeaponReleased,
                MessageType.WeaponDropped,
                MessageType.ItemUsed,
                MessageType.ItemUseRefused,
                MessageType.MatchStatus
            })
            {
                Assert.That(MessageType.Normalize(ruling), Is.EqualTo(ruling));
            }
        }
    }
}