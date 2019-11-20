//---------------------------------------------------------------------------------
// Written by Michael Hoffman
// Find the full tutorial at: http://gamedev.tutsplus.com/series/vector-shooter-xna/
//----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BricksGameTutorial
{
    public enum ParticleType { None, Brick, WallHit, PaddleHit }

    public struct ParticleState
    {
        public Vector2 Velocity;
        public ParticleType Type;
        public float LengthMultiplier;

        private static Random randomNum = new Random();

        public ParticleState(Vector2 velocity, ParticleType type, float lengthMultiplier = 1f)
        {
            Velocity = velocity;
            Type = type;
            LengthMultiplier = lengthMultiplier;
        }

        public static ParticleState GetRandom(float minVel, float maxVel)
        {
            var state = new ParticleState();
            state.Velocity = randomNum.NextVector2(minVel, maxVel); 
            //state.Velocity = new Vector2((float)(randomNum.NextDouble() * (maxVel - minVel) + minVel), (float)(randomNum.NextDouble() * (maxVel - minVel) + minVel));
            state.Type = ParticleType.None;
            state.LengthMultiplier = 1;

            return state;
        }

        public static void UpdateParticle(ParticleManager<ParticleState>.Particle particle)
        {
            var velocity = particle.State.Velocity;
            float speed = velocity.Length();

            // using Vector2.Add() should be slightly faster than doing "x.Position += vel;" because the Vector2s
            // are passed by reference and don't need to be copied. Since we may have to update a very large 
            // number of particles, this method is a good candidate for optimizations.
            Vector2.Add(ref particle.Position, ref velocity, out particle.Position);

            // fade the particle if its PercentLife or speed is low.
            float alpha = Math.Min(1, Math.Min(particle.PercentLife * 2, speed * 1f));
            alpha *= alpha;

            particle.Color.A = (byte)(255 * alpha);

            // the length of paddle hit particles will be less dependent on their speed than other particles
            if (particle.State.Type == ParticleType.PaddleHit)
                particle.Scale.X = particle.State.LengthMultiplier * Math.Min(Math.Min(1f, 0.1f * speed + 0.1f), alpha);
            else
                particle.Scale.X = particle.State.LengthMultiplier * Math.Min(Math.Min(1f, 0.2f * speed + 0.1f), alpha);

            particle.Orientation = velocity.ToAngle();

            var pos = particle.Position;
            int width = GameMain.screenWidth;
            int height = GameMain.screenHeight;

            // collide with the edges of the screen
            if (pos.X < 0)
                velocity.X = Math.Abs(velocity.X);
            else if (pos.X > width)
                velocity.X = -Math.Abs(velocity.X);
            if (pos.Y < 0)
                velocity.Y = Math.Abs(velocity.Y);
            else if (pos.Y > height)
                velocity.Y = -Math.Abs(velocity.Y);

            /*
            if (particle.State.Type != ParticleType.IgnoreGravity)
            {
                foreach (var blackHole in EntityManager.BlackHoles)
                {
                    var dPos = blackHole.Position - pos;
                    float distance = dPos.Length();
                    var n = dPos / distance;
                    velocity += 10000 * n / (distance * distance + 10000);

                    // add tangential acceleration for nearby particles
                    if (distance < 400)
                        velocity += 45 * new Vector2(n.Y, -n.X) / (distance + 100);
                }
            }
            */

            if (Math.Abs(velocity.X) + Math.Abs(velocity.Y) < 0.00000000001f) // denormalized floats cause significant performance issues
                velocity = Vector2.Zero;
            else if (particle.State.Type == ParticleType.Brick)
                velocity *= 0.94f;
            else
                velocity *= 0.96f + Math.Abs(pos.X) % 0.04f; // rand.Next() isn't thread-safe, so use the position for pseudo-randomness

            particle.State.Velocity = velocity;
        }
    }
}
