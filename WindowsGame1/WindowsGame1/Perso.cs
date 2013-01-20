using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WindowsGame1
{
    class Perso
    {
        private Texture2D _texture;
        private Vector2 _position;
        private KeyboardState _keyboardState;
        private MouseState _mouseState;
        private Vector2 _direction;
        private float _speed;
        private Vector2 caseDirection;

        public Texture2D Texture
        {
            get { return _texture; }
            set { _texture = value; }
        }

        public Vector2 SetCaseDirection
        {
            get { return caseDirection; }
            set { caseDirection.X = value.X + 20; caseDirection.Y = value.Y - 65; } // -20 pour un meilleur positionnement en Y
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }
       
        public Vector2 SetDirection
        {
            get { return _direction; }
            set { _direction = Vector2.Normalize(value); }
        }
        
        public float Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }



        public virtual void Initialize(Vector2 pos)
        {
            _position.X = pos.X + 20;
            _position.Y = pos.Y - 65; // -20 pour un meilleur positionnement en Y
            _direction = Vector2.Zero;
            caseDirection = Vector2.Zero;
            _speed = 0.1F;
        }

        public virtual void LoadContent(ContentManager content, string assetName)
        {
            _texture = content.Load<Texture2D>(assetName);
        }

        public virtual void Update(GameTime gameTime)
        {
            _keyboardState = Keyboard.GetState();
            _mouseState = Mouse.GetState();            


            if (caseDirection == Vector2.Zero)
                _direction = Vector2.Zero;

            else
            {
                _direction = caseDirection - _position;

                if (_direction.X < 1 && _direction.X > -1 && _direction.Y < 1 && _direction.Y > -1)
                    _speed = 0;

                else
                {
                    _speed = 0.1F;

                    if (_direction.X > 1 && _direction.Y > 1 || _direction.X < -1 && _direction.Y > 1 || _direction.X > 1 && _direction.Y < -1 || _direction.X < -1 && _direction.Y < -1)
                    {
                        if (_direction.X > 0 && _direction.Y > 0)
                        {
                            _direction.X = 1;
                            _direction.Y = 0.5F;
                        }

                        else if (_direction.X > 0 && _direction.Y < 0)
                        {
                            _direction.X = 1;
                            _direction.Y = -0.5F;
                        }

                        else if (_direction.X < 0 && _direction.Y > 0)
                        {
                            _direction.X = -1;
                            _direction.Y = 0.5F;
                        }

                        else
                        {
                            _direction.X = -1;
                            _direction.Y = -0.5F;
                        }
                    }

                    else
                    {
                        if (_direction.X > 1)
                        {
                            _direction.X = 1;
                            _direction.Y = 0;
                        }

                        else if (_direction.X < -1)
                        {
                            _direction.X = -1;
                            _direction.Y = 0;
                        }

                        else if (_direction.Y > 1)
                        {
                            _direction.X = 0;
                            _direction.Y = 1;
                        }

                        else
                        {
                            _direction.X = 0;
                            _direction.Y = -1;
                        }

                    }
                }
                
            }
               
            _position += _direction * _speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(_texture, _position, Color.White);
        }
    }
}
