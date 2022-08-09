using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apollo.Scripts
{
    class PushableObject : GameObject
    {

        private void Movement()
        {



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

                        transform.position.Y += depth.Y;

                        collider = new Rectangle((int)transform.position.X, (int)transform.position.Y, (int)transform.scale.X, (int)transform.scale.Y);

                    }

                }

            }



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
