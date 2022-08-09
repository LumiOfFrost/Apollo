using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Apollo.Scripts
{

    public enum RenderType
    {

        Square,
        Sprite,
        Custom

    }

    public class GameObject
    {

        public void Update(GameTime gameTime, List<GameObject> gameObjects)
        {

        }

        public void Init()
        {

        }

        public Color color;

        public Rectangle collider;

        public Texture2D sprite;

        public Transform transform;

        public Vector2 velocity;

        public RenderType renderType = new RenderType();

        public GameObject(Transform tform, RenderType rType, Color clr)
        {

            color = clr;

            transform = tform;

            velocity = Vector2.Zero;

            collider = new Rectangle((int)transform.position.X, (int)transform.position.Y, (int)transform.scale.X, (int)transform.scale.Y);

            renderType = rType;

        }

    }

}
