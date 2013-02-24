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
using System.Threading;

namespace WindowsGame1
{
    // Ajout par Riked : Pour le pathfinding (Dernière modif : le 06/02/2013)
    public class PathStruct
    {
        public double x = 0;
        public double y = 0;
        public int id = 0;
        public bool traversable = false;
        public bool dejaparcouru = false;
        public double parcouru = 0;
        public int idfather = -1;
    }
    //Fin d'ajout


    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private const int longueur = 150;
        private const int largeur = 150;
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
        bool escDown = false;

        // Ajout par Riked : Pour le pathfinding (Dernière modif : le 24/02/2013)
        List<PathStruct> carte = new List<PathStruct>(longueur * largeur);
        List<PathStruct> itineraire = new List<PathStruct>();
        int perso_case = 15 + 15*longueur;
        int last_case_id = 15 + 15*longueur;
        //Fin d'ajout
        
      
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
            nbCaseAffiche = 25;
            caseX = 0;
            caseY = 0;
            oldCaseX = -1;
            oldCaseY = -1;
            pause = true;
            
           /* if (nbCaseAffiche > 10)
                nbCaseAffiche = 10;*/

            posDepartX = -7 * lengthx;
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

            // Ajout par Riked : Pour le pathfinding (Dernière modif : le 24/02/2013)
            for (int i = 0; i < nbCaseCharge; ++i)
            {
                for (int j = 0; j < nbCaseCharge; ++j)
                {
                    PathStruct c = new PathStruct();
                    c.x = (double)map[i, j].SetPosition.X;
                    c.y = (double)map[i, j].SetPosition.Y;
                    c.id = (i * longueur + j);
                    c.traversable = map[i, j].IsTraversable();
                    carte.Add(c);
                }
            }

            perso_case = 15 + 15*longueur;
            // Fin d'ajout

            _perso = new Perso();

            //Modifié par Riked : Correction de la position du personnage (dernière modif : le 24/02/2013)
            _perso.Initialize(map[15, 15].SetPosition); //ancienne valeur : [7,7]
            //Fin de modif

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

            scrolling();
            

            if (_menuPause.isQuitte)
                this.Exit();

            //Ajout par Riked : Pour le pathfinding (Dernière modif : le 06/02/2013)

            if (itineraire.Count > 0)
            {
                if (_perso.IsMoving == false) //Si le personnage ne bouge pas, cela veut dire qu'il a déjà effectué l'ordre précédent
                {
                    _perso.SetCaseDirection = new Vector2((float)itineraire[0].x, (float)itineraire[0].y);
                    perso_case = itineraire[0].id;//last_case_id;
                    //last_case_id = itineraire[0].id;
                    itineraire.RemoveAt(0);
                }
            }

            //Fin d'ajout

            base.Update(gameTime);
        }

        public virtual void IsPause()
        {
            _keyboardState = Keyboard.GetState();

            if (!_menuPause.Pause)
                pause = false;

            if (_keyboardState.IsKeyDown(Keys.Escape))
                escDown = true;

            if (_keyboardState.IsKeyUp(Keys.Escape) && escDown)
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
                escDown = false;
            }
        }

        public virtual void HandleInput()
        {
           
            Vector2 mousePos;

            _mouseState = Mouse.GetState();
            mousePos.X = _mouseState.X;
            mousePos.Y = _mouseState.Y;

            caseX = (int)(mousePos.X / lengthx + 9.5 - (mousePos.Y / lengthy * -1 + 6 + 0.5) + caseLeft);//- (mousePos.Y / lengthy * -1 + 2)); 
            caseY = (int)(mousePos.Y / lengthy * -1 + 16 + mousePos.X / lengthx - 1.5 - graphics.PreferredBackBufferWidth / lengthx / 2 + caseTop); // + mousePos.X / lengthx - 1 - graphics.PreferredBackBufferWidth / lengthx / 2); 

            if (oldCaseX < nbCaseCharge && oldCaseY < nbCaseCharge && oldCaseX >= 0 && oldCaseY >= 0)
                map[oldCaseX, oldCaseY].SetcaseCheck = false;

            if (caseX < nbCaseCharge && caseY < nbCaseCharge && caseX >= 0 && caseY >= 0 && caseX == oldCaseX && caseY == oldCaseY)
                map[caseX, caseY].SetcaseCheck = true;

            if (caseX < nbCaseCharge && caseY < nbCaseCharge && _mouseState.LeftButton == ButtonState.Pressed && caseX >= 0 && caseY >= 0)
            {
                //Modifié par Riked : pour le pathfinding (Dernière modif : le 06/02/2013)

                int idcase = caseX * longueur + caseY;
                
                itineraire.Clear();

                _perso.IsMoving = false; //Force le personnage à s'arrêter

                List<PathStruct> iti = new List<PathStruct>();

                Dijkstra(carte, perso_case, idcase, ref iti);

                itineraire = iti;

                //Fin modification
            }

            oldCaseX = caseX;
            oldCaseY = caseY;
            
        }

        int reducScroll = 0;
        int caseTop = 0;
        Vector2 replaceTotal = Vector2.Zero;

        public void scrolling()
        {
            KeyboardState _keyboardState;
            _keyboardState = Keyboard.GetState();
            _mouseState = Mouse.GetState();
            Vector2 replace = Vector2.Zero;


            if (replaceTotal.X == -lengthx /*&& replaceTotal.Y == 0*/)
            {
                caseLeft++;
                caseTop++;
                replaceTotal.X = 0;
            }

            else if (replaceTotal.X == lengthx /*&& replaceTotal.Y == 0*/)
            {
                caseLeft--;
                caseTop++;
                replaceTotal.X = 0;
            }

            else if (/*replaceTotal.X == 0 && */replaceTotal.Y <= -lengthy)
            {
                caseLeft++;
                caseTop--;
                replaceTotal.Y = 0;
            }

            else if (/*replaceTotal.X == 0 &&*/ replaceTotal.Y >= lengthy)
            {
                caseLeft--;
                caseTop++;
                replaceTotal.Y = 0;
            }

            if (_keyboardState.IsKeyDown(Keys.Right) || _mouseState.X > graphics.PreferredBackBufferWidth && caseLeft < nbCaseCharge - nbCaseAffiche /*&& reducScroll == 0*/)
            {

                replace = new Vector2(-2, 0);
                replaceTotal += replace;

                for (int l = 0; l < nbCaseCharge; l++)
                {
                    for (int h = 0; h < nbCaseCharge; h++)
                    {
                        //Ajout par Riked : Pour le pathfinding (dernière modif : le 24/02/2013)
                        int idcase = l + h * longueur;
                        carte[idcase].x += replace.X;
                        carte[idcase].y += replace.Y;
                        //Fin d'ajout

                        map[l, h].SetCenter += replace;
                        map[l, h].SetPosition += replace;
                    }
                }

                //Ajout par Riked : Pour le scrolling (dernière modif : le 24/02/2013)
                _perso._added_position += replace;
                //Fin d'ajout
            }


            else if (_keyboardState.IsKeyDown(Keys.Left) || _mouseState.X < 10 && caseLeft > 0 /*&& reducScroll == 0*/)
            {

                replace = new Vector2(2, 0);
                replaceTotal += replace;

                for (int l = 0; l < nbCaseCharge; l++)
                {
                    for (int h = 0; h < nbCaseCharge; h++)
                    {
                        //Ajout par Riked : Pour le pathfinding (dernière modif : le 24/02/2013)
                        int idcase = l + h * longueur;
                        carte[idcase].x += replace.X;
                        carte[idcase].y += replace.Y;
                        //Fin d'ajout

                        map[l, h].SetCenter += replace;
                        map[l, h].SetPosition += replace;
                    }
                }

                //Ajout par Riked : Pour le scrolling (dernière modif : le 24/02/2013)
                _perso._added_position += replace;
                //Fin d'ajout

            }


            else if (_keyboardState.IsKeyDown(Keys.Up) || _mouseState.Y < 10 && caseLeft > 0 && caseTop < nbCaseCharge - nbCaseAffiche/* && reducScroll == 0*/)
            {
                replace = new Vector2(0, 2);
                replaceTotal += replace;

                for (int l = 0; l < nbCaseCharge; l++)
                {
                    for (int h = 0; h < nbCaseCharge; h++)
                    {
                        //Ajout par Riked : Pour le pathfinding (dernière modif : le 24/02/2013)
                        int idcase = l + h * longueur;
                        carte[idcase].x += replace.X;
                        carte[idcase].y += replace.Y;
                        //Fin d'ajout

                        map[l, h].SetCenter += replace;
                        map[l, h].SetPosition += replace;

                    }
                }

                //Ajout par Riked : Pour le scrolling (dernière modif : le 24/02/2013)
                _perso._added_position += replace;
                //Fin d'ajout

            }


            else if (_keyboardState.IsKeyDown(Keys.Down) || _mouseState.Y > graphics.PreferredBackBufferHeight && caseLeft < nbCaseCharge - nbCaseAffiche && caseTop > 0/* && reducScroll == 0*/)
            {
                replace = new Vector2(0, -2);
                replaceTotal += replace;

                for (int l = 0; l < nbCaseCharge; l++)
                {
                    for (int h = 0; h < nbCaseCharge; h++)
                    {
                        //Ajout par Riked : Pour le pathfinding (dernière modif : le 24/02/2013)
                        int idcase = l + h * longueur;
                        carte[idcase].x += replace.X;
                        carte[idcase].y += replace.Y;
                        //Fin d'ajout

                        map[l, h].SetCenter += replace;
                        map[l, h].SetPosition += replace;
                    }
                }

                //Ajout par Riked : Pour le scrolling (dernière modif : le 24/02/2013)
                _perso._added_position += replace;
                //Fin d'ajout

            }

            /*reducScroll++;
            if (reducScroll > 15)
                reducScroll = 0;*/
        }

        // Ajout par Riked : pour le pathfinding (Dernière modif : le 06/02/2013)

        public static int Minimum(ref List<PathStruct> noeuds)
        {
            if (noeuds.Count == 0) return (-1);
            double minimum = noeuds[0].parcouru;
            int idminimum = -1;

            int debut = 0;

            for (debut = 0; debut < noeuds.Count; ++debut)
            {
                if (noeuds[debut].parcouru > 0 && !noeuds[debut].dejaparcouru)
                {
                    minimum = noeuds[debut].parcouru;
                    idminimum = debut;
                    break;
                }
            }

            if (idminimum < 0) return (-1);

            for (int i = debut + 1; i < noeuds.Count; ++i)
            {
                if (noeuds[i].parcouru > 0 && noeuds[i].parcouru < minimum && noeuds[i].dejaparcouru == false)
                {
                    minimum = noeuds[i].parcouru;
                    idminimum = i;
                }
            }

            return idminimum;
        }

        public bool Dijkstra(List<PathStruct> noeuds, int debut, int fin, ref List<PathStruct> path)
        {
            path.Clear();
            if (noeuds[debut].traversable == false) return false;
            if (debut == fin) return true;

            for (int i = 0; i < noeuds.Count; ++i)
            {
                noeuds[i].parcouru = 0;
                noeuds[i].idfather = -1;
                noeuds[i].dejaparcouru = false;
            }

            int goodid = -1;

            //Ajout des fils du noeud actuel (caractérisé par "début")

            int currentid = debut;

            while (goodid < 0)
            {
                List<double> f = new List<double>();
                List<PathStruct> fils = new List<PathStruct>();

                int ycase = currentid % longueur;
                int xcase = currentid / longueur;

                if (noeuds[currentid].id - 1 >= 0 && noeuds[currentid].id - 1 < noeuds.Count && (noeuds[currentid].id - 1) % longueur == ycase - 1)
                {
                    f.Add(0);
                    fils.Add(noeuds[noeuds[currentid].id - 1]); //Y-1
                }
                if (noeuds[currentid].id + 1 >= 0 && noeuds[currentid].id + 1 < noeuds.Count && (noeuds[currentid].id + 1) % longueur == ycase + 1)
                {
                    f.Add(0);
                    fils.Add(noeuds[noeuds[currentid].id + 1]); //Y+1
                }
                if (noeuds[currentid].id + longueur >= 0 && noeuds[currentid].id + longueur < noeuds.Count && (noeuds[currentid].id + longueur) / longueur == xcase + 1)
                {
                    f.Add(0);
                    fils.Add(noeuds[noeuds[currentid].id + longueur]); //X+1
                }
                if (noeuds[currentid].id - longueur >= 0 && noeuds[currentid].id - longueur < noeuds.Count && (noeuds[currentid].id - longueur) / longueur == xcase - 1)
                {
                    f.Add(0);
                    fils.Add(noeuds[noeuds[currentid].id - longueur]); //X-1
                }
                if (noeuds[currentid].id + longueur - 1 >= 0 && noeuds[currentid].id + longueur - 1 < noeuds.Count && (noeuds[currentid].id - 1 + longueur) % longueur == ycase - 1 && (noeuds[currentid].id + longueur - 1) / longueur == xcase + 1)
                {
                    f.Add(0.5);
                    fils.Add(noeuds[noeuds[currentid].id + longueur - 1]); //X+1 Y-1
                }
                if (noeuds[currentid].id + longueur + 1 >= 0 && noeuds[currentid].id + longueur + 1 < noeuds.Count && (noeuds[currentid].id + 1 + longueur) % longueur == ycase + 1 && (noeuds[currentid].id + longueur + 1) / longueur == xcase + 1)
                {
                    f.Add(0.5);
                    fils.Add(noeuds[noeuds[currentid].id + longueur + 1]); //X+1 Y+1
                }
                if (noeuds[currentid].id - longueur - 1 >= 0 && noeuds[currentid].id - longueur - 1 < noeuds.Count && (noeuds[currentid].id - 1 - longueur) % longueur == ycase - 1 && (noeuds[currentid].id - longueur - 1) / longueur == xcase - 1)
                {
                    f.Add(0.5);
                    fils.Add(noeuds[noeuds[currentid].id - longueur - 1]); //X-1 Y-1
                }
                if (noeuds[currentid].id - longueur + 1 >= 0 && noeuds[currentid].id - longueur + 1 < noeuds.Count && (noeuds[currentid].id + 1 - longueur) % longueur == ycase + 1 && (noeuds[currentid].id - longueur + 1) / longueur == xcase - 1)
                {
                    f.Add(0.5);
                    fils.Add(noeuds[noeuds[currentid].id - longueur + 1]); //X-1 Y+1
                }

                for (int i = 0; i < fils.Count; ++i)
                {
                    if (fils[i].dejaparcouru || fils[i].traversable == false || fils[i].parcouru != 0)
                    {
                        fils.RemoveAt(i);
                        f.RemoveAt(i);
                        --i;
                    }
                }

                for (int i = 0; i < fils.Count; ++i)
                {
                    PathStruct n1 = noeuds[currentid];
                    PathStruct n2 = noeuds[fils[i].id];

                    noeuds[fils[i].id].parcouru = n1.parcouru + 1 + f[i];// + distance(n1, n2);
                    noeuds[fils[i].id].idfather = currentid;

                    if (fils[i].id == fin) goodid = fils[i].id;

                    if (goodid >= 0) break;
                }
                if (goodid >= 0) break;

                noeuds[currentid].dejaparcouru = true;

                currentid = Minimum(ref noeuds);
                if (currentid < 0) break;
            }

            if (goodid < 0) return false;

            //path.Clear();

            PathStruct current_node = noeuds[goodid]; //new PathStruct();
       //     current_node = noeuds[goodid];

            List<PathStruct> inverse_path = new List<PathStruct>();

            while (current_node.id != debut)
            {
                inverse_path.Add(current_node);
                //if (current_node.idfather < 0) break;
                current_node = noeuds[current_node.idfather];
            }

            path.Add(noeuds[debut]);

            for (int i = inverse_path.Count - 1; i >= 0; --i)
            {
                path.Add(inverse_path[i]);
            }

            return true;
        }

        // Fin d'ajout

        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            GraphicsDevice.Clear(Color.Black);

            try
            {
                for (int l = caseLeft; l < nbCaseAffiche + caseLeft; l++)
                {
                    for (int h = caseTop; h < nbCaseAffiche + caseTop; h++)
                    {
                        map[l, h].Draw(spriteBatch, gameTime);
                        spriteBatch.DrawString(_font, Convert.ToString(caseX) + "," + Convert.ToString(caseY), Vector2.Zero, Color.White); // Convert.ToString(map[l, h].SetCenter.X) + "," + Convert.ToString(map[l, h].SetCenter.Y)
                    }
                }
            }

            catch
            {
                spriteBatch.DrawString(_font, "Error !", Vector2.Zero, Color.White);
            }
            _perso.Draw(spriteBatch, gameTime);
            _menuPause.Draw(spriteBatch, gameTime);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
