using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Beeautiful
{
    class LvlMngr
    {
        // Indicate the last level is complete
        bool gameOver = false;

        public bool GameOver
        {
            get { return gameOver; }
            set { gameOver = value; }
        }
        // Counter for the current level
        int CurrentLevel = 0;
        // Maximum amount of levels

        const int MAXLEVEL = 1;

        // Collection of levels which are created as subclasses of Level
        Level[] Levels;


        public LvlMngr(Game g)
        {
           Levels = new Level[MAXLEVEL];
           Levels[0] = new Level1(g);
            //Levels[1] = new Level2(g);
            //Levels[2] = new Level3(g);

           Levels[0].LevelState = LEVELSTATE.Running;

        }

        public void Update(GameTime t)
        {
            if (!gameOver)
            {
                foreach (Level l in Levels)
                {
                    if (l != null && l.LevelState == LEVELSTATE.Running)
                    {   // Update the current playing level
                        l.Update(t);
                        // if the current level has finished
                        if (l.LevelState == LEVELSTATE.GameOver)
                        {   // Get rid of the level should 
                            Levels[CurrentLevel] = null;
                            // and if the not the last level finished
                            if (++CurrentLevel < MAXLEVEL)
                                // then play the next level
                                Levels[CurrentLevel].LevelState = LEVELSTATE.Running;
                            //Or else we are finished
                            else gameOver = true;
                        }
                    }
                }
            }

        }

        public void Draw(SpriteBatch sp)
        {
            foreach (Level l in Levels)
                // need to check for null as we 
                if (l != null)
                    l.Draw(sp);
        }

        internal Level Level
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
    }
}

