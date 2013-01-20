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
        private const int longueur = 8;
        private const int largeur = 8;
        int lengthx = 132;
        int lengthy = 66;
        int posDepartX;
        int posDepartY;
        int nbCaseCharge;
        Tile[,] map = new Tile[longueur, largeur];
        private SpriteFont _font;
        private Perso _perso;
        private MenuPause _menuPause;
        private MouseState _mouseState;
        private KeyboardState _keyboardState;
        private Vector2 caseUsed;
        private int caseLeft;
        int nbCaseAffiche;
        int caseX;
        int caseY;
        int oldCaseX;
        int oldCaseY;
        bool pause;
        
      
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }


        protected override void Initialize()
        {
            //graphics.PreferredBackBufferHeight = graphics.PreferredBackBufferWidth = 800;
           // this.Window.AllowUserResizing = true;
            this.IsMouseVisible = true;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            caseUsed = new Vector2(-1, -1);
            Vector2 pos = new Vector2();
            nbCaseCharge = longueur; // graphics.PreferredBackBufferHeight / lengthx;
            caseLeft = 0;
            nbCaseAffiche = 8;
            caseX = 0;
            caseY = 0;
            oldCaseX = -1;
            oldCaseY = -1;
            pause = true;
            
           /* if (nbCaseAffiche > 10)
                nbCaseAffiche = 10;*/

            posDepartX = (graphics.PreferredBackBufferWidth - graphics.PreferredBackBufferHeight) / 2;
            posDepartY = graphics.PreferredBackBufferHeight / 2;

            for (int i = 0; i < nbCaseCharge; i++) //On parcourt les lignes du tableau
            {
                for (int j = 0; j < nbCaseCharge; j++)
                {
                    if (i % 2 != 0)
                    {
                        pos.X = (j * lengthx + lengthx / 2) + (((i - 1) * lengthx) / 2) - j * lengthx / 2 + posDepartX ;
                        pos.Y = (((i * lengthy) / 2)) - j * lengthy / 2 + posDepartY ;
                    }
                    else
                    {
                        pos.X = (j * lengthx + ((i * lengthx) / 2)) - j * lengthx / 2 + posDepartX ;
                        pos.Y = (((i * lengthy) / 2)) - j * lengthy / 2 + posDepartY ;
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
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _perso.LoadContent(Content, "perso_up");
            _menuPause.LoadContent(Content, "fondMenu", "BoutonJouer", "BoutonOptions", "BoutonQuitter");

            for (int l = 0; l < nbCaseCharge; l++)
            {
                for (int h = 0; h < nbCaseCharge; h++)
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

            IsPause();

            if (!pause)
            {
                _perso.Update(gameTime);
                HandleInput();
            }

            else
                _menuPause.Update(gameTime);

            //scrolling();
            

            if (_menuPause.isQuitte)
                this.Exit();

            base.Update(gameTime);
        }

        public virtual void IsPause()
        {
            _keyboardState = Keyboard.GetState();

            if (!_menuPause.Pause)
                pause = false;

            if (_keyboardState.IsKeyDown(Keys.Escape))
            {
                if (!pause)
                {
                    pause = true;
                    _menuPause.Pause = true;
                    System.Threading.Thread.Sleep(150);
                }
                else
                {
                    pause = false;
                    _menuPause.Pause = false;
                    System.Threading.Thread.Sleep(150);
                }
            }
        }

        public virtual void HandleInput()
        {
           
            Vector2 mousePos;

            _mouseState = Mouse.GetState();
            mousePos.X = _mouseState.X;
            mousePos.Y = _mouseState.Y;

            caseX = (int)(mousePos.X / lengthx + 1 - (mousePos.Y / lengthy * -1 + 6 +0.5));//- (mousePos.Y / lengthy * -1 + 2)); 
            caseY = (int)(mousePos.Y / lengthy * -1 + 7.5  + mousePos.X / lengthx - 1.5 - graphics.PreferredBackBufferWidth / lengthx / 2); // + mousePos.X / lengthx - 1 - graphics.PreferredBackBufferWidth / lengthx / 2); 

            if (oldCaseX < nbCaseCharge && oldCaseY < nbCaseCharge && oldCaseX >= 0 && oldCaseY >= 0)
                map[oldCaseX, oldCaseY].SetcaseCheck = false;

            if (caseX < nbCaseCharge && caseY < nbCaseCharge && caseX >= 0 && caseY >= 0 && caseX == oldCaseX && caseY == oldCaseY)
                map[caseX, caseY].SetcaseCheck = true;

            if (caseX < nbCaseCharge && caseY < nbCaseCharge && _mouseState.LeftButton == ButtonState.Pressed && caseX >= 0 && caseY >= 0)
            {
                _perso.SetCaseDirection = map[caseX, caseY].SetPosition;
            }
            oldCaseX = caseX;
            oldCaseY = caseY;
            
        }

        public double distanceVect(Vector2 vect1, Vector2 vect2)
        {
            return (System.Math.Pow(vect1.X - vect2.X, 2) + System.Math.Pow(vect1.Y - vect2.Y, 2));
        }

        // Version de Scrolling inefficace, je garde ici si jamais on a encore besion d'une ou deux fonctions
        public void scrolling()
        {
            KeyboardState _keyboardState;
            _keyboardState = Keyboard.GetState();
            _mouseState = Mouse.GetState();
            Vector2 replace = Vector2.Zero;

             if (_keyboardState.IsKeyDown(Keys.Right))
            //if (nbCaseAffiche < 50 && _mouseState.X >= graphics.PreferredBackBufferWidth + 300)
            {
                

                if (nbCaseAffiche++ > 40)
                    nbCaseAffiche = 40;

                replace = new Vector2(-lengthx, 0);
                    
                    for (int l = caseLeft; l < nbCaseAffiche; l++)
                    {
                        for (int h = caseLeft; h < nbCaseAffiche; h++)
                        {
                            map[l, h].SetCenter += replace;
                            map[l, h].SetPosition += replace;
                            _perso.Position += replace;

                        }
                    }
                    //System.Threading.Thread.Sleep(150);
                    //caseLeft++;
                   // nbCaseAffiche++;

                }

             else if (_keyboardState.IsKeyDown(Keys.Left))
                 //if (nbCaseAffiche > 29 && _mouseState.X <= graphics.PreferredBackBufferWidth - 300)
            {
                if (nbCaseAffiche < 10)
                    nbCaseAffiche = 10;
                
                        replace = new Vector2(lengthx, 0);
                        nbCaseAffiche--;
                    
                    
                    for (int l = caseLeft; l < nbCaseAffiche; l++)
                    {
                        for (int h = caseLeft; h < nbCaseAffiche; h++)
                        {
                            map[l, h].SetCenter += replace;
                            map[l, h].SetPosition += replace;
                            _perso.Position += replace;

                        }
                    }
                   // System.Threading.Thread.Sleep(150);
                    //caseLeft++;
                   // nbCaseAffiche++;

                }
            
        }

        

        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            GraphicsDevice.Clear(Color.Black);

            for (int l = caseLeft; l < nbCaseAffiche; l++)
            {
                for (int h = caseLeft; h < nbCaseAffiche; h++)
                {
                    map[l, h].Draw(spriteBatch, gameTime);
                    spriteBatch.DrawString(_font, Convert.ToString(caseX) + "," + Convert.ToString(caseY), Vector2.Zero, Color.White); // Convert.ToString(map[l, h].SetCenter.X) + "," + Convert.ToString(map[l, h].SetCenter.Y)
                }
            }
            
            _perso.Draw(spriteBatch, gameTime);
            _menuPause.Draw(spriteBatch, gameTime);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
