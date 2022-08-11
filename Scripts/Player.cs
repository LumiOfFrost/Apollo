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

        float bufferJump;

        protected KeyboardState prevKeyState;

        public override void Init()
        {

            

        }

        public override void Update(Main main, GameTime gameTime)
        {

            collider = new Rectangle((int)transform.position.X, (int)transform.position.Y,(int)transform.scale.X, (int)transform.scale.Y);

            if (prevKeyState == null)
            {

                prevKeyState = Keyboard.GetState();

            }

            Movement(prevKeyState, gameTime);

            Collisions(main.gameObjects);

            if (transform.position.Y > main._graphics.GraphicsDevice.Viewport.Height * 2)
            {

                transform.position = new Vector2(main._graphics.GraphicsDevice.Viewport.Width / 2 - 20, main._graphics.GraphicsDevice.Viewport.Height / 3 * 1.5f);

                velocity.Y = 0;

            }

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

            if (KeyUtils.IsKeyJustPressed(Keys.Space, prevKeyState))
            {

                bufferJump = 0.15f;

            }

            if (bufferJump > 0 && isGrounded)
            {

                bufferJump = 0;

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
            bufferJump -= (float)gameTime.ElapsedGameTime.TotalSeconds;

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

                if (collider.Intersects(other.collider) && other != this)
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

                if (collider.Intersects(other.collider) && other != this)
                {

                    Vector2 depth = RectExtras.GetIntersectionDepth(collider, other.collider);

                    if (Math.Abs(depth.Y) > 0)
                    {
                        
                        if (Math.Abs(depth.X) < 15 && velocity.Y < 0)
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

        public Player(Transform tform, RenderType rType, Color col, Color bdr, float bdrThickness = 0) : base(tform, rType, col, bdr, bdrThickness)
        {

            tag = "player";

            outlineColor = bdr;

            color = col;

            transform = tform;

            velocity = Vector2.Zero;

            collider = new Rectangle((int)transform.position.X, (int)transform.position.Y, (int)transform.scale.X, (int)transform.scale.Y);

            renderType = rType;

        }

    }
}
