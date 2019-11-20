using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BricksGameTutorial
{
    class Brick
    {
        public float X { get; set; } // X position of brick on screen
        public float Y { get; set; } // Y position of brick on screen
        public float Width { get; set; } // Width of brick
        public float Height { get; set; } // Height of brick
        public bool Visible { get; set; } // Does brick still exist?
        public Color color; // Color of brick

        private static Random randomNum = new Random();

        private Texture2D lParticle { get; set; } // Image for particle explosion
        private Texture2D imgBrick { get; set; }  // Cached image of the brick
        private SpriteBatch spriteBatch;  // Allows us to write on backbuffer when we need to draw self

        public Brick(float x, float y, Color color, SpriteBatch spriteBatch, GameContent gameContent)
        {
            X = x;
            Y = y;
            imgBrick = gameContent.imgBrick;
            lParticle = gameContent.lParticle;
            Width = imgBrick.Width;
            Height = imgBrick.Height;
            this.spriteBatch = spriteBatch;
            Visible = true;
            this.color = color;
        }

        public void Explode() // Draw particle explosion when a brick is cleared
        {
            for (int k = 0; k < 60; k++) // Total particle count
            {
                float speed = 20f * (1f - 1 / randomNum.NextFloat(1f, 10f));
                var state = new ParticleState()
                {
                    Velocity = randomNum.NextVector2(speed, speed),
                    Type = ParticleType.Brick,
                    LengthMultiplier = 1f
                };
                // Pass in sprite, brick position, and color for particles
                GameMain.ParticleManager.CreateParticle(lParticle, new Vector2(X, Y), color, 60, 1.5f, state);
            }
        }
        
        public void Draw()
        {
            if (Visible)
            {
                spriteBatch.Draw(imgBrick, new Vector2(X, Y), null, color, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
            }
        }
    }
}
