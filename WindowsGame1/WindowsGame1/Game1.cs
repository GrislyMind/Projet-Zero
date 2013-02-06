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

        // Ajout par Riked : Pour le pathfinding (Dernière modif : le 06/02/2013)
        List<PathStruct> carte = new List<PathStruct>(longueur * largeur);
        List<PathStruct> itineraire = new List<PathStruct>();
        int perso_case = 0;
        int last_case_id = 0;
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

            // Ajout par Riked : Pour le pathfinding (Dernière modif : le 06/02/2013)
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

            perso_case = 0;
            // Fin d'ajout

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

            //Ajout par Riked : Pour le pathfinding (Dernière modif : le 06/02/2013)

            if (itineraire.Count > 0)
            {
                if (_perso.IsMoving == false) //Si le personnage ne bouge pas, cela veut dire qu'il a déjà effectué l'ordre précédent
                {
                    _perso.SetCaseDirection = new Vector2((float)itineraire[0].x, (float)itineraire[0].y);
                    perso_case = last_case_id;
                    last_case_id = itineraire[0].id;
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

        public static double distance(PathStruct noeud1, PathStruct noeud2)
        {
            return Math.Sqrt((noeud1.x - noeud2.x) * (noeud1.x - noeud2.x) + (noeud1.y - noeud2.y) * (noeud1.y - noeud2.y));
        }

        public static void Swap(ref List<PathStruct> liste, int id1, int id2)
        {
            PathStruct save = liste[id1];
            liste[id1] = liste[id2];
            liste[id2] = save;
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
                List<PathStruct> fils = new List<PathStruct>();

                int ycase = currentid % longueur;
                int xcase = currentid / longueur;

                if (noeuds[currentid].id - 1 >= 0 && noeuds[currentid].id - 1 < noeuds.Count && (noeuds[currentid].id - 1) % longueur == ycase - 1) fils.Add(noeuds[noeuds[currentid].id - 1]); //Y-1
                if (noeuds[currentid].id + 1 >= 0 && noeuds[currentid].id + 1 < noeuds.Count && (noeuds[currentid].id + 1) % longueur == ycase + 1) fils.Add(noeuds[noeuds[currentid].id + 1]); //Y+1
                if (noeuds[currentid].id + longueur >= 0 && noeuds[currentid].id + longueur < noeuds.Count && (noeuds[currentid].id + longueur) / longueur == xcase + 1) fils.Add(noeuds[noeuds[currentid].id + longueur]); //X+1
                if (noeuds[currentid].id - longueur >= 0 && noeuds[currentid].id - longueur < noeuds.Count && (noeuds[currentid].id - longueur) / longueur == xcase - 1) fils.Add(noeuds[noeuds[currentid].id - longueur]); //X-1
                if (noeuds[currentid].id + longueur - 1 >= 0 && noeuds[currentid].id + longueur - 1 < noeuds.Count && (noeuds[currentid].id - 1 + longueur) % longueur == ycase - 1 && (noeuds[currentid].id + longueur - 1) / longueur == xcase + 1) fils.Add(noeuds[noeuds[currentid].id + longueur - 1]); //X+1 Y-1
                if (noeuds[currentid].id + longueur + 1 >= 0 && noeuds[currentid].id + longueur + 1 < noeuds.Count && (noeuds[currentid].id + 1 + longueur) % longueur == ycase + 1 && (noeuds[currentid].id + longueur + 1) / longueur == xcase + 1) fils.Add(noeuds[noeuds[currentid].id + longueur + 1]); //X+1 Y+1
                if (noeuds[currentid].id - longueur - 1 >= 0 && noeuds[currentid].id - longueur - 1 < noeuds.Count && (noeuds[currentid].id - 1 - longueur) % longueur == ycase - 1 && (noeuds[currentid].id - longueur - 1) / longueur == xcase - 1) fils.Add(noeuds[noeuds[currentid].id - longueur - 1]); //X-1 Y-1
                if (noeuds[currentid].id - longueur + 1 >= 0 && noeuds[currentid].id - longueur + 1 < noeuds.Count && (noeuds[currentid].id + 1 - longueur) % longueur == ycase + 1 && (noeuds[currentid].id - longueur + 1) / longueur == xcase - 1) fils.Add(noeuds[noeuds[currentid].id - longueur + 1]); //X-1 Y+1

                for (int i = 0; i < fils.Count; ++i)
                {
                    if (fils[i].dejaparcouru || fils[i].traversable == false || fils[i].parcouru != 0)
                    {
                        fils.RemoveAt(i);
                        --i;
                    }
                }

                for (int i = 0; i < fils.Count; ++i)
                {
                    PathStruct n1 = noeuds[currentid];
                    PathStruct n2 = noeuds[fils[i].id];

                    noeuds[fils[i].id].parcouru = n1.parcouru + distance(n1, n2);
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

            PathStruct current_node = new PathStruct();
            current_node = noeuds[goodid];

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
