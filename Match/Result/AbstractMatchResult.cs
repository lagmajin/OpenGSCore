using System;
using System.Collections.Generic;




namespace OpenGSCore
{
    public interface IMatchResult
    {

        List<PlayerID> winnersList();
        List<PlayerID> losersList();


    }

    public abstract class AbstractMatchResult
    {
        //List<PlayerInfo> scoreList;

        public bool Won { get; private set; } = false;
        public bool Lost { get; private set; } = false;

        public AbstractMatchResult(bool won)
        {
            Won = won;
            Lost = !won;


        }




    }


}
