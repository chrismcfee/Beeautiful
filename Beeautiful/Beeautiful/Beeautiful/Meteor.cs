﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Beeautiful
{
    public class Beatle
    {
        #region Variables

        Vector2 position;
        Vector2 motion = Vector2.Zero;
        bool isLarge;
        float speed;
        Texture2D texture;
        float BeatleHealth;
        public float baseHealth;
        bool visible;
        bool credited = false;

        #endregion

        #region Fields

        public float Health
        {
            get { return BeatleHealth; }
            set { BeatleHealth = value; }
        }

        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        public Vector2 Motion
        {
            get { return motion; }
            set { motion = value; }
        }

        public Rectangle Bounds
        {
            get { return new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height); }
        }

        #endregion

        public void Damage(float amount)
        {
            BeatleHealth -= amount;
            if (BeatleHealth <= 0 && isLarge)
            {
                SpawnSmallMeteors();
            }
            if (!credited && BeatleHealth <= 0)
            {
                int credit = !isLarge ? 1 : 2;
                Game1.instance.kills += credit;
                Game1.instance.playerScore += credit;
                credited = true;
                Game1.instance.Notifications.Add(new Notification("+" + credit * 100, 200, position));
            }
        }

        private void SpawnSmallBeatles()
        {
            Random rand = new Random();
            int randAmt = rand.Next(2, 6);
            for (int i = 0; i < randAmt; i++)
            {
                Beatle newBeatle = new Beatle(false, rand.Next(2, 8), this.position);
                newBeatle.Motion = new Vector2(rand.Next(-3, 3), rand.Next(0, 2));
                Game1.instance.Meteors.Add(newBeatle);
            }
        }

        public Beatle(bool isLarge, float speed, Vector2 position)
        {
            this.isLarge = isLarge;
            this.texture = !isLarge ? Game1.instance.BeatleSmall : Game1.instance.BeatleBig;
            Random rand = new Random();
            this.speed = speed;
            BeatleHealth = !isLarge ? 20 : 50;
            baseHealth = !isLarge ? 20 : 50;
            this.position = position;
            visible = true;
            motion.Y = 1;
        }

        public void Update()
        {
            if (BeatleHealth <= 0)
                visible = false;
            motion.Normalize();
            position += motion * this.speed;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
                spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
