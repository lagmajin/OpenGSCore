using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public class FieldItemService
    {
        private EFieldItemType tempA;
        private EFieldItemType tempB;
        private EFieldItemType tempC;

        public FieldItemService()
        {
            var itemListA = new List<EFieldItemType>
        {
            EFieldItemType.PowerUpItem,
            EFieldItemType.GrenadePack
        };
            Random _random = new Random();
            // ランダムなインデックスを選択
            int randomIndex = _random.Next(itemListA.Count);
        
            tempA= itemListA[randomIndex];


            var itemListB = new List<EFieldItemType>
        {
            EFieldItemType.PowerUpItem,
            EFieldItemType.GrenadePack
        };

            tempB= itemListB[randomIndex];

            var itemListC = new List<EFieldItemType>
            { EFieldItemType.DefenceUpItem

            };

            

        }

        public EFieldItemType GenerateItemListA()
        {

            return EFieldItemType.GrenadePack;
        }

        public EFieldItemType GenerateItemListB()
        {

            return EFieldItemType.GrenadePack;
        }

    }
}
