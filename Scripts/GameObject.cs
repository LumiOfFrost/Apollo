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
        Custom,
        BorderSquare

    }

    public class GameObject
    {

        public virtual void Update(GameTime gameTime, List<GameObject> gameObjects, GraphicsDeviceManager _graphics, List<GameObject> gameObjectsToDestroy)
        {

        }

        public virtual void Init()
        {

        }

        public float borderThickness;

        public string tag;

        public Color outlineColor;

        public Color color;

        public Rectangle collider;

        public Texture2D sprite;

        public Transform transform;

        public Vector2 velocity;

        public RenderType renderType = new RenderType();

        public GameObject(Transform tform, RenderType rType, Color clr, Color bdr, float bdrThickness = 0, string tg = "")
        {

            color = clr;

            borderThickness = bdrThickness;

            tag = tg;

            outlineColor = bdr;

            transform = tform;

            velocity = Vector2.Zero;

            collider = new Rectangle((int)transform.position.X, (int)transform.position.Y, (int)transform.scale.X, (int)transform.scale.Y);

            renderType = rType;

        }

    }

}
