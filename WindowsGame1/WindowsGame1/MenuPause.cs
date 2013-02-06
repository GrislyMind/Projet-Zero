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
        private bool pause;
        private Texture2D _texture;
        private Texture2D jouer;
        private Texture2D options;
        private Texture2D quitter;
        private bool jouerIn, OptionIn, QuitterIn, optionsMenu, inGame, quitte;


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

        public bool isQuitte
        {
            get { return quitte; }
            set { quitte = value; }
        }
        
       public bool Pause
        {
             get { return pause; }
            set { pause = value; }
        }

        public virtual void Initialize()
        {
            _position = new Vector2(200,150); //new Vector2(800 / 2 - 259 / 2, 400 / 2 - 194 / 2); // a ameliorer pour sadapter a la taille de la fenetre / ecran
            jouerIn = OptionIn = QuitterIn = optionsMenu = inGame = quitte = false;
            pause = true;

        }

        public virtual void LoadContent(ContentManager content, string assetFond, string assetJouer, string assetOptions, string assetQuitter)
        {
            _texture = content.Load<Texture2D>(assetFond);
            jouer = content.Load<Texture2D>(assetJouer);
            options = content.Load<Texture2D>(assetOptions);
            quitter = content.Load<Texture2D>(assetQuitter);
        }

        public virtual void Update(GameTime gameTime)
        {
            _keyboardState = Keyboard.GetState();
            _mouseState = Mouse.GetState();

            HandleInput(_keyboardState, _mouseState);
        }
               
        public virtual void HandleInput(KeyboardState keyboardState, MouseState mouseState)
        {
            

            jouerIn = false;
            OptionIn = false;
            QuitterIn = false;

            if (_mouseState.X > _position.X && _mouseState.X < _position.X + 300 && _mouseState.Y > _position.Y && _mouseState.Y < _position.Y + 100)
            {
                jouerIn = true;
                if (_mouseState.LeftButton == ButtonState.Pressed)
                {
                    System.Threading.Thread.Sleep(150);
                    pause = false;
                }
            }

            else if (_mouseState.X > _position.X && _mouseState.X < _position.X + 300 && _mouseState.Y > _position.Y + 150 && _mouseState.Y < _position.Y + 250)
            {
                OptionIn = true;
                if (_mouseState.LeftButton == ButtonState.Pressed)
                    optionsMenu = true;
            }

            else if (_mouseState.X > _position.X && _mouseState.X < _position.X + 300 && _mouseState.Y > _position.Y + 300 && _mouseState.Y < _position.Y + 400)
            {
                QuitterIn = true;
                if (_mouseState.LeftButton == ButtonState.Pressed)
                    quitte = true;
            }

        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Vector2 _position2 = new Vector2(_position.X, _position.Y + 150);
            Vector2 _position3 = new Vector2(_position.X, _position.Y + 300);

            if (pause)
            {
                spriteBatch.Draw(_texture, Vector2.Zero, Color.White);

                if (jouerIn)
                    spriteBatch.Draw(jouer, _position, Color.Blue);
                else
                    spriteBatch.Draw(jouer, _position, Color.White);

                if (OptionIn)
                    spriteBatch.Draw(options, _position2, Color.Blue);
                else
                    spriteBatch.Draw(options, _position2, Color.White);

                if (QuitterIn)
                    spriteBatch.Draw(quitter, _position3, Color.Blue);
                else
                    spriteBatch.Draw(quitter, _position3, Color.White);

            }
        }
    }
}
