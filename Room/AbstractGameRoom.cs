using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public interface IAbstractGameRoom
    {

    }
    public class AbstractGameRoom
    {
        private int roomNumber_ = 0;

        private List<PlayerInfo> players = new List<PlayerInfo>();

        private bool isFinished = false;

        private string? ownerID_ = null;
        public int RoomNumber { get => roomNumber_; }

        private string id;

        public List<PlayerInfo> Players { get => players; set => players = value; }
        public string Id { get => id; set => id = value; }
        public AbstractGameRoom(int roomNumber, in string roomOwnerID)
        {
            if (string.IsNullOrEmpty(roomOwnerID))
            {

                return;
            }

            id = Guid.NewGuid().ToString("N");

        }
        public virtual void GameUpdate()
        {




        }

        public virtual AbstractMatchResult GameResult()
        {

            return null;
        }
    }
}
