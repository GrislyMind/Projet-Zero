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
    class Tile
    {
        private Texture2D texture;
        private Texture2D texture2;
        private Vector2 position;
        private Vector2 center;
        char nbTexture;
        private bool caseCheck;
        private bool traversable;

       /* public Tile(Texture2D tile, Vector2 position)
        {
            // TODO: Complete member initialization
          /*  this.tile = tile;
            this.position = position;
            texture = null;
            position = Vector2.Zero;
            coordonnees = Vector2.Zero;
        }*/

        public Tile()
        {
            texture = null;
            position = Vector2.Zero;
        }

        public virtual void Initialize(Vector2 pos, int lengthX)
        {
            texture = null;
            texture2 = null;
            nbTexture = '0';
            position = pos;
            center.X = pos.X + lengthX / 2;
            center.Y = pos.Y + lengthX / 4;
            caseCheck = false;
            traversable = true;
        }

        public Vector2 SetPosition
        {
            get { return position; }
            set { position = value; }
        }

        public Vector2 SetCenter
        {
            get { return center; }
            set { center = value; }
        }

        public char SetnbTexture
        {
            get { return nbTexture; }
            set { nbTexture = value;
                    if (nbTexture > '9')
                        traversable = false;
                }
        }

        public bool SetcaseCheck
        {
            get { return caseCheck; }
            set { caseCheck = value; }
        }

        public virtual void LoadContent(ContentManager content)
        {
            if (nbTexture == '1')
                texture = content.Load<Texture2D>("textureIso1_mini");

            else if (nbTexture == '2')
                texture = content.Load<Texture2D>("textureIso2_mini");

            else if (nbTexture == '3')
                texture = content.Load<Texture2D>("textureIso3_mini");

            else if (nbTexture == 'p')
            {
                texture = content.Load<Texture2D>("textureIso0_mini");
                texture2 = content.Load<Texture2D>("Potale");
            }

            else// if (nbTexture < 51)
                texture = content.Load<Texture2D>("textureIso0_mini");
        }

        public Texture2D Texture
        {
            get { return this.texture; }
            set { this.texture = value; }
        }

        public virtual void Update(GameTime gameTime)
        {
           // this.LoadContent(content);
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (caseCheck && traversable)
                spriteBatch.Draw(texture, position, Color.Blue);

            else if (caseCheck && !traversable)
                spriteBatch.Draw(texture, position, Color.Red);

            else
                spriteBatch.Draw(texture, position, Color.White);

            if (texture2 != null)
                spriteBatch.Draw(texture2, position, Color.White);
  
        }

    }
}
