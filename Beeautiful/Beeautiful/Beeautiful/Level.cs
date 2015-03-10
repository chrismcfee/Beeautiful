using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Beeautiful
{
    public enum LEVELSTATE { Loading, StartMenu, Running, Paused, GameOver, Editor }

    class Level
    {

        private LEVELSTATE levelState;
        public LEVELSTATE LevelState
        {
            get { return levelState; }
            set { levelState = value; }
        }
        //protected ChasingEnemy[] enemies;
        //protected Sprite[] collectables;
        //protected Sprite BackGround;
        //protected Player player;

        public virtual void Update(GameTime t)
        {

        }

        public virtual void Draw(SpriteBatch sp)
        {

        }


    }
}

