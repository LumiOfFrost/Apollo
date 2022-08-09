using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Apollo.Scripts;
using Apos.Shapes;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Apollo
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private KeyboardState prevKeyState;
        private ShapeBatch _shapeBatch;

        private ButtonState prevButtonState;

        public Vector2 cameraPosition;

        public Vector2 cameraOffset;

        //Gameplay

        private List<GameObject> gameObjectsToDestroy;

        private List<GameObject> gameObjects = new List<GameObject>();

        private Player player;

        // Sprites

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {

            cameraPosition = Vector2.Zero;

            gameObjects.Add(new Player(new Transform(new Vector2(_graphics.GraphicsDevice.Viewport.Width / 2 - 20, _graphics.GraphicsDevice.Viewport.Height / 3 * 1.5f), new Vector2(40, 75), 0), RenderType.Square, Color.Aquamarine, Color.Transparent));
            
            player = gameObjects.OfType<Player>().First();

            gameObjects.Add(new GameObject(new Transform(new Vector2((_graphics.GraphicsDevice.Viewport.Width / 3) * 2, _graphics.GraphicsDevice.Viewport.Height / 2), new Vector2(_graphics.GraphicsDevice.Viewport.Width / 3, 30), 0), RenderType.Square, Color.White, Color.Transparent));

            gameObjects.Add(new GameObject(new Transform(new Vector2(_graphics.GraphicsDevice.Viewport.Width / 3, _graphics.GraphicsDevice.Viewport.Height / 4), new Vector2(_graphics.GraphicsDevice.Viewport.Width / 3, 30), 0), RenderType.Square, Color.White, Color.Transparent));

            gameObjects.Add(new GameObject(new Transform(new Vector2(0, _graphics.GraphicsDevice.Viewport.Height - 100), new Vector2(_graphics.GraphicsDevice.Viewport.Width, 100), 0), RenderType.Square, Color.White, Color.Transparent));

            gameObjects.Add(new GameObject(new Transform(new Vector2(0, 0), new Vector2(50, _graphics.GraphicsDevice.Viewport.Height - 100), 0), RenderType.Square, Color.White, Color.Transparent));

            foreach (GameObject obj in gameObjects)
            {

                obj.Init();

            }

            gameObjectsToDestroy = new List<GameObject>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _shapeBatch = new ShapeBatch(GraphicsDevice, Content);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (KeyUtils.IsKeyJustPressed(Keys.F11, prevKeyState))
            {
                _graphics.ToggleFullScreen();
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && prevButtonState == ButtonState.Released)
            {

                gameObjects.Add(new PushableObject(new Transform(new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Position.Y), new Vector2(40, 40), 0), RenderType.Square, Color.RosyBrown, Color.SaddleBrown, 10));

            }

            prevKeyState = Keyboard.GetState();

            foreach (GameObject obj in gameObjects)
            {

                obj.Update(gameTime, gameObjects, _graphics, gameObjectsToDestroy);

            }

            foreach(GameObject obj in gameObjectsToDestroy)
            {

                gameObjects.Remove(obj);

            }
            gameObjectsToDestroy.Clear();

            Debug.WriteLine(gameObjects.Count);

            prevButtonState = Mouse.GetState().LeftButton;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(0,0.02f,0.07f));

            _shapeBatch.Begin();

            _spriteBatch.Begin(samplerState:SamplerState.PointClamp, sortMode:SpriteSortMode.BackToFront);

            Render();

            _spriteBatch.End();

            _shapeBatch.End();

            base.Draw(gameTime);
        }

        protected void Render()
        {

            foreach (GameObject obj in gameObjects)
            {

                switch(obj.renderType)
                {

                    case RenderType.Square:
                        _shapeBatch.FillRectangle(obj.transform.position - cameraPosition, obj.transform.scale, obj.color);
                        _shapeBatch.BorderRectangle(obj.transform.position - cameraPosition, obj.transform.scale, obj.outlineColor, obj.borderThickness);
                        break;

                    default:

                        break;

                }

            }

        }

    }
}
