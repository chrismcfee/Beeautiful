using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beeautiful2
{
    class enemyBee : Enemy
    {

        private const string ENEMY_ASSETNAME = "eBee";
        private int START_POSITION_X;
        private int START_POSITION_Y;
        private const int ENEMY_SPEED = 160;
        private const int MOVE_UP = -3;
        private const int MOVE_DOWN = 3;
        private const int MOVE_LEFT = -3;
        private const int MOVE_RIGHT = 3;

        enum State
        {
            Alive,
            Dead
        }

        State mCurrentState = State.Alive;
        //Vector2 mDirection = Vector2.Zero;
        //Vector2 mSpeed = Vector2.Zero;
        KeyboardState mPreviousKeyboardState;


        #region ClassConstructor
        //GameObject Enemy;
        public enemyBee(Texture2D _texture, position _position)
        {
            position = _position;
            //Speed = ENEMY_SPEED;
            //Scale = _scale;
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            //RotationSpeed = _rotationSpeed;
            //Rotation = _angle;
        }
        #endregion
        /*#region OverrideRectangles
        public override SourceRectangle ebsRectangle
        {
            get
            {
                sourceRectangle.X = (int)position.X;
                sourceRectangle.Y = (int)position.Y;
                return base.SourceRectangle;
            }
            set
            {
                base.SourceRectangle = value;
            }
        }
        public override CollisionRectangle ebcRectangle
        {
            get
            {
                collisionRectangle.X = (int)position.X;
                collisionRectangle.Y = (int)position.Y;
                return base.CollisionRectangle;
            }
            set
            {
                base.CollisionRectangle = value;
            }
        }
        #endregion
        #region OverrideUpdate
        public override void Update(GameTime gameTime)
        {
            if (Alive)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    position.X += Speed;
                    ObjectDirection = Direction.Right;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    position.X -= Speed;
                    ObjectDirection = Direction.Left;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    position.Y += Speed;
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    position.Y -= Speed;
                if (Keyboard.GetState().IsKeyDown(Keys.E))
                    Rotation += RotationSpeed;
                if (Keyboard.GetState().IsKeyDown(Keys.Q))
                    Rotation -= RotationSpeed;
            }
        }
        #endregion*/
        #region EnemyBeeDraw
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!Hidden)
            {
                if (ObjectDirection == Direction.Right)
                    spriteBatch.Draw(Texture, position, null, Color.White, Rotation, Origin, Scale, SpriteEffects.FlipHorizontally, 0);
                else if (ObjectDirection == Direction.Left)
                    spriteBatch.Draw(Texture, position, null, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 0);
            }
        }
        #endregion
    }
}
