using Microsoft.Xna.Framework;
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

        float coyoteTime;

        bool isGrounded;

        bool isJumping;

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

            if (KeyUtils.IsKeyJustPressed(Keys.Space, prevKeyState) && isGrounded)
            {

                isJumping = true;

                velocity.Y = -10;

            }
            else
            {

                velocity.Y += 19.6f * (float)gameTime.ElapsedGameTime.TotalSeconds;

            }

            if (KeyUtils.IsKeyJustReleased(Keys.Space, prevKeyState) && isJumping && velocity.Y < 0)
            {

                isJumping = false;

                velocity.Y /= 3;

            }

            coyoteTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (velocity.Y < 0 || coyoteTime < 0)
            {

                isGrounded = false;

            }

            if (velocity.Y > 0)
            {

                isJumping = false;

            }

        }

        private void Collisions(List<GameObject> gameObjects)
        {

            transform.position.X += velocity.X;
            transform.position = MathUtils.RoundVector2(transform.position);

            collider = new Rectangle((int)transform.position.X, (int)transform.position.Y, (int)transform.scale.X, (int)transform.scale.Y);

            foreach (GameObject other in gameObjects)
            {

                if (collider.Intersects(other.collider) && other.GetType().Name != "Player")
                {

                    Vector2 depth = RectExtras.GetIntersectionDepth(collider, other.collider);

                    if (Math.Abs(depth.X) > 0)
                    {

                        transform.position.X += depth.X;

                        velocity.X = 0;

                        collider = new Rectangle((int)transform.position.X, (int)transform.position.Y, (int)transform.scale.X, (int)transform.scale.Y);

                    }

                }

            }

            transform.position.Y += velocity.Y;
            transform.position = MathUtils.RoundVector2(transform.position);

            collider = new Rectangle((int)transform.position.X, (int)transform.position.Y, (int)transform.scale.X, (int)transform.scale.Y);

            foreach (GameObject other in gameObjects)
            {

                if (collider.Intersects(other.collider) && other.GetType().Name != "Player")
                {

                    Vector2 depth = RectExtras.GetIntersectionDepth(collider, other.collider);

                    if (Math.Abs(depth.Y) > 0)
                    {

                        if (Math.Abs(depth.X) < transform.scale.X / 3 && velocity.Y < 0)
                        {

                            transform.position.X += depth.X;

                        } else
                        {

                            transform.position.Y += depth.Y;

                            if (velocity.Y < 0)
                            {

                                velocity.Y = 0 - velocity.Y / 2;

                            }
                            else
                            {

                                coyoteTime = 0.15f;

                                isGrounded = true;

                                velocity.Y = 0;

                            }

                        }

                        collider = new Rectangle((int)transform.position.X, (int)transform.position.Y, (int)transform.scale.X, (int)transform.scale.Y);

                    }

                }

            }



        }

        public Player(Transform tform, RenderType rType, Color col) : base(tform, rType, col)
        {

            color = col;

            transform = tform;

            velocity = Vector2.Zero;

            collider = new Rectangle((int)transform.position.X, (int)transform.position.Y, (int)transform.scale.X, (int)transform.scale.Y);

            renderType = rType;

        }

    }
}
