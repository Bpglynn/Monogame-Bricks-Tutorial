﻿using System;
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
    class Paddle
    {
        public float X { get; set; } // X position of paddle on screen
        public float Y { get; set; } // Y position of paddle on screen
        public float Width { get; set; } // Width of paddle
        public float Height { get; set; } // Height of paddle
        public float ScreenWidth { get; set; } // Width of game screen

        private Texture2D imgPaddle { get; set; }  // Cached image of the paddle
        private SpriteBatch spriteBatch;  // Allows us to write on backbuffer when we need to draw self

        public Paddle(float x, float y, float screenWidth, SpriteBatch spriteBatch, GameContent gameContent)
        {
            X = x;
            Y = y;
            imgPaddle = gameContent.imgPaddle;
            Width = imgPaddle.Width;
            Height = imgPaddle.Height;
            this.spriteBatch = spriteBatch;
            ScreenWidth = screenWidth;
        }

        public void Draw()
        {
            spriteBatch.Draw(imgPaddle, new Vector2(X, Y), null, Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
        }

        public void MoveLeft()
        {
            X = X - 5;
            if (X < 1)
            {
                X = 1;
            }
        }
        public void MoveRight()
        {
            X = X + 5;
            if ((X + Width) > ScreenWidth)
            {
                X = ScreenWidth - Width;
            }
        }

        public void MoveTo(float x)
        {
            if (x >= 0)
            {
                if (x < ScreenWidth - Width)
                {
                    X = x;
                }
                else
                {
                    X = ScreenWidth - Width;
                }
            }
            else
            {
                if (x < 0)
                {
                    X = 0;
                }
            }
        }
    }
}
