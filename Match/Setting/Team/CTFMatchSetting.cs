using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public class CTFMatchSetting:AbstractMatchSetting
    {
        private int winConditionPoint=3;

        


        public CTFMatchSetting(int winCondition=3,bool teamBalance=true):base(eGameMode.CTF,0,true)
        {

        }
    }
}
