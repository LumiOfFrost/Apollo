using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apollo.Scripts
{
    class PushableObject : GameObject
    {

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, GraphicsDeviceManager _graphics, List<GameObject> gameObjectsToDestroy)
        {

            Movement(gameTime);
            Collisions(gameObjects);
            collider = new Rectangle((int)transform.position.X, (int)transform.position.Y, (int)transform.scale.X, (int)transform.scale.Y);

            if (transform.position.Y > _graphics.GraphicsDevice.Viewport.Height * 2)
            {

                gameObjectsToDestroy.Add(this);

            }

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

                        if (other.tag == "pushable")
                        {

                            other.transform.position.X -= depth.X;

                            velocity.X /= 2;

                            PushableObject pushable = other as PushableObject;

                            if (pushable.IsColliding(gameObjects))
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

        public bool IsColliding(List<GameObject> gameObjects)
        {

            collider = new Rectangle((int)transform.position.X, (int)transform.position.Y, (int)transform.scale.X, (int)transform.scale.Y);

            foreach (GameObject other in gameObjects)
            {

                if (collider.Intersects(other.collider) && other != this)
                {

                    Vector2 depth = RectExtras.GetIntersectionDepth(collider, other.collider);

                    if (Math.Abs(depth.X) > 0)
                    {

                        if (other.tag == "pushable")
                        {

                            collider = new Rectangle((int)transform.position.X, (int)transform.position.Y, (int)transform.scale.X, (int)transform.scale.Y);

                            other.transform.position.X -= depth.X;

                            var pushable = other as PushableObject;

                            if (pushable.IsColliding(gameObjects))
                            {

                                depth = RectExtras.GetIntersectionDepth(collider, other.collider);

                                transform.position.X += depth.X;
                                velocity.X = 0;

                                return true;

                            } 

                        } else
                        {

                            transform.position.X += depth.X;
                            velocity.X = 0;

                            return true;

                        }

                            

                            

                    }

                }

            }

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

        public PushableObject(Transform tform, RenderType rType, Color col, Color bdr, float bdrThickness = 0) : base(tform, rType, col, bdr, bdrThickness)
        {

            tag = "pushable";

            outlineColor = bdr;
            transform = tform;
            renderType = rType;
            color = col;

            collider = new Rectangle((int)transform.position.X, (int)transform.position.Y, (int)transform.scale.X, (int)transform.scale.Y);

        }

    }
}
