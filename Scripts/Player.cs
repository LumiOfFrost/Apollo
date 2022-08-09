﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Apollo.Scripts;
using MonoGame.Extended;
using System.Diagnostics;

namespace Apollo.Scripts
{

    class Player : GameObject
    {

        protected KeyboardState prevKeyState;

        public void Init()
        {

            

        }

        public void Update(GameTime gameTime, List<GameObject> gameObjects)
        {

            collider = new Rectangle((int)transform.position.X, (int)transform.position.Y,(int)transform.scale.X, (int)transform.scale.Y);

            if (prevKeyState == null)
            {

                prevKeyState = Keyboard.GetState();

            }

            Movement(prevKeyState, gameTime);

            Collisions(gameObjects);

            prevKeyState = Keyboard.GetState();

        }

        private void Movement(KeyboardState prevKeyState, GameTime gameTime)
        {

            if (Keyboard.GetState().IsKeyDown(Keys.A) && !Keyboard.GetState().IsKeyDown(Keys.D))
            {
                velocity.X = -10;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D) && !Keyboard.GetState().IsKeyDown(Keys.A))
            {
                velocity.X = 10;
            }
            else
            {
                velocity.X = 0;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && prevKeyState.IsKeyUp(Keys.Space))
            {

                velocity.Y = -10;

            }
            else
            {

                velocity.Y += 19.6f * (float)gameTime.ElapsedGameTime.TotalSeconds;

            }

        }

        private void Collisions(List<GameObject> gameObjects)
        {

            transform.position += velocity;

            foreach (GameObject other in gameObjects)
            {

                if (collider.Intersects(other.collider) && other.GetType().Name != "Player")
                {

                    Vector2 depth = RectExtras.GetIntersectionDepth(collider, other.collider);

                    if (depth != Vector2.Zero)
                    {

                        float absDepthX = Math.Abs(depth.X);
                        float absDepthY = Math.Abs(depth.Y);

                        if (absDepthY < absDepthX)
                        {

                            transform.position.Y += depth.Y;

                            collider = new Rectangle((int)transform.position.X, (int)transform.position.Y, (int)transform.scale.X, (int)transform.scale.Y);

                            velocity.Y = 0;

                        } else
                        {

                            transform.position.X += depth.X;

                            collider = new Rectangle((int)transform.position.X, (int)transform.position.Y, (int)transform.scale.X, (int)transform.scale.Y);

                            velocity.X = 0;

                        }

                    }

                }

            }

            

        }

        public Player(Transform tform, RenderType rType) : base(tform, rType)
        {

            transform = tform;

            velocity = Vector2.Zero;

            collider = new Rectangle((int)transform.position.X, (int)transform.position.Y, (int)transform.scale.X, (int)transform.scale.Y);

            renderType = rType;

        }

    }
}