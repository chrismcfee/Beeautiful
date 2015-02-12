using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beeautiful2
{
    public enum Direction { Right, Left };
    abstract public class GameObject
    {
        #region ClassVariables
        private Texture2D texture;
        public Vector2 position;
        private float speed;
        private float scale;
        private float rotation;
        private float rotationSpeed;
        private Vector2 origin;
        private Direction direction;
        protected Rectangle sourceRectangle;
        protected Rectangle collisionRectangle;
        private bool alive;
        private bool hidden;

        #endregion
        #region ClassProperites
        public bool Alive
        {
            get { return alive; }
            set { alive = value; }
        }
        public bool Hidden
        {
            get { return hidden; }
            set { hidden = value; }
        }
        public float RotationSpeed
        {
            get { return rotationSpeed; }
            set { rotationSpeed = value; }
        }
        public virtual Rectangle CollisionRectangle
        {
            get { return collisionRectangle; }
            set { collisionRectangle = value; }
        }
        public virtual Rectangle SourceRectangle
        {
            get { return sourceRectangle; }
            set { sourceRectangle = value; }
        }
        public Direction ObjectDirection
        {
            get { return direction; }
            set { direction = value; }
        }
        public Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }
        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        protected Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }
        #endregion
        #region ClassConstructor
        public GameObject(Texture2D _texture)
        {
            texture = _texture;
            position = Vector2.Zero;
            speed = 0.0f;
            scale = 1.0f;
            rotation = 0.0f;
            rotationSpeed = 0.0f;
            origin = Vector2.Zero;
            sourceRectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            collisionRectangle = sourceRectangle;
            direction = Direction.Right;
            alive = true;
            hidden = false;
        }
        #endregion
        #region AbstractMethods
        abstract public void Update(GameTime gameTime);
        abstract public void Draw(SpriteBatch spriteBatch, GameTime gameTime);
        #endregion
    }
}
