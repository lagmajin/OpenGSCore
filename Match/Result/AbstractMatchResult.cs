using System;
using System.Collections.Generic;




namespace OpenGSCore
{
    public interface IMatchResult
    {

    }

    public abstract class AbstractMatchResult
    {
        List<PlayerInfo> scoreList;

        public bool Won { get; } = false;
        public bool Lost { get; } = false;

        public AbstractMatchResult()
        {

        }




    }


}
