using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apollo.Scripts
{
    class PushableObject : GameObject
    {

        public override void Update(GameTime gameTime, List<GameObject> gameObjects)
        {

            Movement(gameTime);
            Collisions(gameObjects);
            collider = new Rectangle((int)transform.position.X, (int)transform.position.Y, (int)transform.scale.X, (int)transform.scale.Y);

        }

        private void Movement(GameTime gameTime)
        {

            velocity.Y += 18.6f * (float)gameTime.ElapsedGameTime.TotalSeconds;

        }

        public void Collisions(List<GameObject> gameObjects)
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

                        if (other.GetType().Name == "PushableObject")
                        {

                            other.transform.position.X -= depth.X;

                            velocity.X /= 2;

                            PushableObject pushable = other as PushableObject;

                            if (pushable.isColliding(gameObjects))
                            {

                                transform.position.X += depth.X;

                                velocity.X = 0;

                            }

                        } else
                        {

                            transform.position.X += depth.X;
                            velocity.X = 0;

                        }

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

                        transform.position.Y += depth.Y;

                        velocity.Y = 0;

                        velocity.X = other.velocity.X;

                        collider = new Rectangle((int)transform.position.X, (int)transform.position.Y, (int)transform.scale.X, (int)transform.scale.Y);

                    }

                }

            }



        }

        public bool isColliding(List<GameObject> gameObjects)
        {

            collider = new Rectangle((int)transform.position.X, (int)transform.position.Y, (int)transform.scale.X, (int)transform.scale.Y);

            foreach (GameObject other in gameObjects)
            {

                if (collider.Intersects(other.collider) && other != this)
                {

                    Vector2 depth = RectExtras.GetIntersectionDepth(collider, other.collider);

                    if (Math.Abs(depth.X) > 0)
                    {

                        if (other.GetType().Name == "PushableObject")
                        {

                            other.transform.position.X -= depth.X;

                            velocity.X /= 2;

                            PushableObject pushable = other as PushableObject;

                            if (pushable.isColliding(gameObjects))
                            {

                                transform.position.X += depth.X;

                                velocity.X = 0;

                            }

                        }
                        else
                        {

                            transform.position.X += depth.X;
                            velocity.X = 0;

                            return true;

                        }

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

                        transform.position.Y += depth.Y;

                        velocity.Y = 0;

                        velocity.X = other.velocity.X;

                        collider = new Rectangle((int)transform.position.X, (int)transform.position.Y, (int)transform.scale.X, (int)transform.scale.Y);

                        return true;

                    }

                }

            }

            return false;

        }

        public PushableObject(Transform tform, RenderType rType, Color col) : base(tform, rType, col)
        {

            transform = tform;
            renderType = rType;
            color = col;

            collider = new Rectangle((int)transform.position.X, (int)transform.position.Y, (int)transform.scale.X, (int)transform.scale.Y);

        }

    }
}
