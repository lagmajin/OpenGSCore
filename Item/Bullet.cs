using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public abstract class AbstractBullet:AbstractGameObject
    {

        public float Damage { get; set; }
        public float Angle { get; set; }
        public float Speed { get; set; }
        public float StoppingPower { get; set; }
        
        public float PosX { get; set; }
        public float PosY { get; set; }
  

        
    }

    public class BulletGameObject :AbstractBullet
    {



        public BulletGameObject(float x, float y, float damage)
        {

        }

    }
}
