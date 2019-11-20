using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace BricksGameTutorial
{
    class GameContent
    {
        public Texture2D imgBrick { get; set; }
        public Texture2D imgPaddle { get; set; }
        public Texture2D imgBall { get; set; }
        public Texture2D imgPixel { get; set; }
        public Texture2D dParticle { get; set; }
        public Texture2D sParticle { get; set; }
        public Texture2D lParticle { get; set; }
        public SoundEffect startSound { get; set; }
        public SoundEffect brickSound { get; set; }
        public SoundEffect paddleBounceSound { get; set; }
        public SoundEffect wallBounceSound { get; set; }
        public SoundEffect missSound { get; set; }
        public SpriteFont labelFont { get; set; }

        public GameContent(ContentManager Content)
        {
            // Load /images
            imgBall = Content.Load<Texture2D>("images/Ball");
            imgPixel = Content.Load<Texture2D>("images/Pixel");
            imgPaddle = Content.Load<Texture2D>("images/Paddle");
            imgBrick = Content.Load<Texture2D>("images/Brick");
            dParticle = Content.Load<Texture2D>("images/particle_diamond");
            sParticle = Content.Load<Texture2D>("images/particle_star");
            lParticle = Content.Load<Texture2D>("images/particle_line");

            // Load /sounds
            startSound = Content.Load<SoundEffect>("sounds/StartSound");
            brickSound = Content.Load<SoundEffect>("sounds/BrickSound");
            paddleBounceSound = Content.Load<SoundEffect>("sounds/PaddleBounceSound");
            wallBounceSound = Content.Load<SoundEffect>("sounds/WallBounceSound");
            missSound = Content.Load<SoundEffect>("sounds/MissSound");

            // Load /fonts
            labelFont = Content.Load<SpriteFont>("fonts/Arial20");

        }
    }
}
