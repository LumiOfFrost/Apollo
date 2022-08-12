using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Apollo.Scripts
{

    class Player : GameObject
    {

        float coyoteTime;

        bool isGrounded;

        bool isJumping;

        float bufferJump;

        private KeyboardState prevKeyState;

        private GamePadState prevPadState;

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

            if (prevPadState == null)
            {

                prevPadState = GamePad.GetState(PlayerIndex.One);

            }

            Movement(prevKeyState, gameTime);

            Collisions(main.gameObjects);

            if (transform.position.Y > 1500)
            {

                transform.position = new Vector2(300, 60);

                velocity.Y = 0;

            }

            prevKeyState = Keyboard.GetState();
            prevPadState = GamePad.GetState(PlayerIndex.One);

        }

        private void Movement(KeyboardState prevKeyState, GameTime gameTime)
        {

            if (InputManager.IsMovingLeft() && !InputManager.IsMovingRight())
            {
                velocity.X = -10;
            }
            else if (InputManager.IsMovingRight() && !InputManager.IsMovingLeft())
            {
                velocity.X = 10;
            }
            else
            {
                velocity.X = 0;
            }

            if (InputManager.Jump(prevKeyState, prevPadState))
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

                velocity.Y += 19.6f * (float)gameTime.ElapsedGameTime.TotalSeconds * Main.gameSpeed;

            }

            if (InputManager.Fall(prevKeyState, prevPadState) && isJumping)
            {

                isJumping = false;

                velocity.Y /= 3;

            }

            coyoteTime -= (float)gameTime.ElapsedGameTime.TotalSeconds * Main.gameSpeed;
            bufferJump -= (float)gameTime.ElapsedGameTime.TotalSeconds * Main.gameSpeed;

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

            transform.position.X += velocity.X * Main.gameSpeed;
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

            transform.position.Y += velocity.Y * Main.gameSpeed;
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
