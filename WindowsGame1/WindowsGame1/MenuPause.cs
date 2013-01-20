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
    class MenuPause
    {
        private Vector2 _position;
        private KeyboardState _keyboardState;
        private MouseState _mouseState;
        private bool pause = false;
        private Texture2D _texture;


        public Texture2D Texture
        {
            get { return _texture; }
            set { _texture = value; }
        }
        
        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }
        
        public bool Pause()
        {
            return pause;
        }

        public virtual void Initialize()
        {
            _position = new Vector2(800 / 2 - 259 / 2, 400 / 2 - 194 / 2); // a ameliorer pour sadapter a la taille de la fenetre / ecran
        }

        public virtual void LoadContent(ContentManager content, string assetName)
        {
            _texture = content.Load<Texture2D>(assetName);
        }

        public virtual void Update(GameTime gameTime)
        {
            _keyboardState = Keyboard.GetState();
            _mouseState = Mouse.GetState();

            HandleInput(_keyboardState, _mouseState);
        }
               
        public virtual void HandleInput(KeyboardState keyboardState, MouseState mouseState)
        {
            if (_keyboardState.IsKeyDown(Keys.Escape))
            {
                if (!pause)
                {
                    pause = true;
                    System.Threading.Thread.Sleep(150);
                }
                else
                {
                    pause = false;
                    System.Threading.Thread.Sleep(150);
                }
            }

            if (_mouseState.X > 250 && _mouseState.X < 550 && _mouseState.LeftButton == ButtonState.Pressed)
                pause = false;
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (pause)
            {
                spriteBatch.Draw(_texture, _position, Color.White);
            }
        }
    }
}
