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
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private const int longueur = 30;
        private const int largeur = 30;
        int lengthx = 132;
        int lengthy = 66;
        int posDepartX;
        int posDepartY;
        int nbCaseAffiche;
        Tile[,] map = new Tile[longueur, largeur];
        private SpriteFont _font;
        private Perso _perso;
        private MenuPause _menuPause;
        private MouseState _mouseState;
        private Vector2 caseUsed;

      
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }


        protected override void Initialize()
        {
            graphics.PreferredBackBufferHeight = graphics.PreferredBackBufferWidth = 800;
            this.Window.AllowUserResizing = true;
            this.IsMouseVisible = true;
            graphics.ApplyChanges();

            caseUsed = new Vector2(-1, -1);
            Vector2 pos = new Vector2();
            nbCaseAffiche = 30; // graphics.PreferredBackBufferHeight / lengthx;

           /* if (nbCaseAffiche > 10)
                nbCaseAffiche = 10;*/

            posDepartX = (graphics.PreferredBackBufferWidth - graphics.PreferredBackBufferHeight) / 2;
            posDepartY = graphics.PreferredBackBufferHeight / 2;

            for (int i = 0; i < longueur; i++) //On parcourt les lignes du tableau
            {
                for (int j = 0; j < largeur; j++)
                {
                    if (i % 2 != 0 || i == 1)
                    {
                        pos.X = ((j * lengthx + lengthx / 2) + (((i - 1) * lengthx) / 2)) - j * lengthx / 2 + posDepartX;
                        pos.Y = (((i * lengthy) / 2)) - j * lengthy / 2 + posDepartY;
                    }
                    else
                    {
                        pos.X = (j * lengthx + ((i * lengthx) / 2)) - j * lengthx / 2 + posDepartX;
                        pos.Y = (((i * lengthy) / 2)) - j * lengthy / 2 + posDepartY;
                    }
                    map[i, j] = new Tile();
                    map[i, j].Initialize(pos, lengthx);
                    map[i, j].SetnbTexture = fichTexture(i, j);
                }
            }

            _perso = new Perso();
            _perso.Initialize(map[0, 0].SetPosition);
            _menuPause = new MenuPause();
            _menuPause.Initialize();

            base.Initialize();
        }

        public char fichTexture(int posLongueur, int posLargeur)
        {
            string[] lines = System.IO.File.ReadAllLines("posTexture.txt");
            string text = lines[posLargeur];

            return text[posLongueur*2];

        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _perso.LoadContent(Content, "perso3");
            _menuPause.LoadContent(Content, "menuPause");

            for (int l = 0; l < longueur; l++)
            {
                for (int h = 0; h < largeur; h++)
                {
                    map[l, h].LoadContent(Content);
                }
            }


            _font = Content.Load<SpriteFont>("MaPolice");

        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
           /* if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();*/

            if (!_menuPause.Pause())
                _perso.Update(gameTime);

            _menuPause.Update(gameTime);

            HandleInput();
                        
            base.Update(gameTime);
        }
        

        public virtual void HandleInput()
        {
            double distanceToCase = 0;
            int caseX = 0;
            int caseY = 0;
            Vector2 mousePos;

            _mouseState = Mouse.GetState();
            mousePos.X = _mouseState.X;
            mousePos.Y = _mouseState.Y;

            //distanceToCase = map[0, 0].SetCenter.Length; (//_mouseState.X  + _mouseState.Y * 2;

            distanceToCase = distanceVect(mousePos, Vector2.Zero);


            for (int l = 0; l < longueur; l++)
            {
                for (int h = 0; h < largeur; h++)
                {
                    if (distanceToCase > distanceVect(mousePos, map[l, h].SetCenter))
                    {
                        distanceToCase = distanceVect(mousePos, map[l, h].SetCenter);
                        caseX = l;
                        caseY = h;
                    }
                    map[l, h].SetcaseCheck = false;
                }
            }

            
            if (caseX < longueur && caseY < largeur)
                map[caseX, caseY].SetcaseCheck = true;

            if (caseX < longueur && caseY < largeur && _mouseState.LeftButton == ButtonState.Pressed)
            {
                _perso.SetCaseDirection = map[caseX, caseY].SetPosition;
            }
        }

        public double distanceVect(Vector2 vect1, Vector2 vect2)
        {
             return (System.Math.Pow(vect1.X - vect2.X, 2) + System.Math.Pow(vect1.Y - vect2.Y, 2));
        }

        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            GraphicsDevice.Clear(Color.CornflowerBlue);

            for (int l = 0; l < nbCaseAffiche; l++)
            {
                for (int h = 0; h < nbCaseAffiche; h++)
                {
                    map[l, h].Draw(spriteBatch, gameTime);
                }
            }
            //spriteBatch.DrawString(_font, Convert.ToString(distanceToCase), new Vector2(10, 20), Color.White);
            _perso.Draw(spriteBatch, gameTime);
            _menuPause.Draw(spriteBatch, gameTime);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
