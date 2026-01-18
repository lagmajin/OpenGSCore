using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public enum eGameObjectType
    {
        Grenade,
        Character,
        FieldItem,

    }

    public abstract class AbstractGameObject
    {
        String name;
        float posx = 0;
        float posy = 0;
        String id = Guid.NewGuid().ToString("N");
        bool updated = false;
        int frame = 0;
        DateTime time = DateTime.Now;

        public string Name { get => name; set => name = value; }
        public float Posx { get => posx; set => posx = value; }
        public float Posy { get => posy; set => posy = value; }
        public string Id { get => id; set => id = value; }
        public bool Updated { get => updated; }

        public virtual void Update()
        {


        }

        public virtual JObject ToJson()
        {
            var json = new JObject();

            return json;
        }

    }

    public class PlayerGameObject : AbstractGameObject
    {
        PlayerStatus status;

        public PlayerGameObject(String name, float x, float y) 
            //: base(x, y)
        {

        }

        public override JObject ToJson()
        {
            JObject json = new JObject();


            return json;
        }

    }


    public class Character : AbstractGameObject
    {



        public Character()
        {

        }

        
    }

    class FieldItem : AbstractGameObject
    {

    }

    class NormalGranade : AbstractGameObject
    {

        

        public NormalGranade()
        {

        }
    }






}
