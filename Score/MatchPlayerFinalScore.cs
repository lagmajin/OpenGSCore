using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public interface IPlayerMatchFinalScore
    {
        public float CalcTotalPoint();
    }

    public abstract class AbstractPlayerMatchFinalScore
    {
        //public 
        public string playerName{ get; private set; }

        public int Kill{get;private set;}

        public int Death { get; private set; } = 0;

        public int Suicide { get; private set; } = 0;

        public int? Rank { get; private set; } = null;

        public float TotalPoint { get; private set; } = 0;

        public AbstractPlayerMatchFinalScore()
        {
            
        }
        

    }
    
    
    public class PlayerDeathMatchFinalScore:AbstractPlayerMatchFinalScore
    {


    }
    
    public class PlayerTeamDeathMatchFinalScore:AbstractPlayerMatchFinalScore
    {


    }

    public class PlayerCTFMatchFinalScore
    {
        
    }
}
