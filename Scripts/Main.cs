using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Apollo.Scripts;
using Apos.Shapes;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using System;

namespace Apollo
{
    public class Main : Game
    {
        public GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private KeyboardState prevKeyState;
        private ShapeBatch _shapeBatch;
        public Vector2 cameraPosition;

        public Vector2 cameraOffset;

        //Gameplay

        public List<GameObject> gameObjectsToDestroy;

        public List<GameObject> gameObjects = new List<GameObject>();

        // Graphics

        private OrthographicCamera _camera;

        private GameObject cameraTarget;

        private float defaultZoom = 0.6f;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {

            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new OrthographicCamera(viewportAdapter);

            cameraPosition = Vector2.Zero;

            _camera.Zoom = defaultZoom;

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;

            gameObjects.Add(new GameObject(new Transform(new Vector2((_graphics.GraphicsDevice.Viewport.Width / 3) * 2, _graphics.GraphicsDevice.Viewport.Height / 2), new Vector2(_graphics.GraphicsDevice.Viewport.Width / 3, 30), 0), RenderType.Square, Color.White, Color.Transparent));

            gameObjects.Add(new GameObject(new Transform(new Vector2(_graphics.GraphicsDevice.Viewport.Width / 3, _graphics.GraphicsDevice.Viewport.Height / 4), new Vector2(_graphics.GraphicsDevice.Viewport.Width / 3, 30), 0), RenderType.Square, Color.White, Color.Transparent));

            gameObjects.Add(new GameObject(new Transform(new Vector2(0, _graphics.GraphicsDevice.Viewport.Height - 100), new Vector2(_graphics.GraphicsDevice.Viewport.Width, 100), 0), RenderType.Square, Color.White, Color.Transparent));

            gameObjects.Add(new GameObject(new Transform(new Vector2(0, 0), new Vector2(50, _graphics.GraphicsDevice.Viewport.Height - 100), 0), RenderType.Square, Color.White, Color.Transparent));

            gameObjects.Add(new Player(new Transform(new Vector2(100, 0), new Vector2(30, 60), 0), RenderType.Square, Color.Aquamarine, Color.Transparent));

            cameraTarget = gameObjects.OfType<Player>().First();

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

            prevKeyState = Keyboard.GetState();

            foreach (GameObject obj in gameObjects)
            {

                obj.Update(this, gameTime);

            }

            foreach(GameObject obj in gameObjectsToDestroy)
            {

                gameObjects.Remove(obj);

            }

            gameObjectsToDestroy.Clear();

            UpdateCamera();

            base.Update(gameTime);
        }

        private void UpdateCamera()
        {

            _camera.Position = new Vector2(
            MathHelper.Lerp(_camera.Position.X, cameraTarget.GetCenter().X - (_camera.BoundingRectangle.Width / 2) * _camera.Zoom, 0.08f),
            MathHelper.Lerp(_camera.Position.Y, cameraTarget.GetCenter().Y - ((_camera.BoundingRectangle.Height / 4) * 3) * _camera.Zoom, 0.08f));

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(0,0.02f,0.07f));

            var transformMatrix = _camera.GetViewMatrix();

            _shapeBatch.Begin(transformMatrix);

            _spriteBatch.Begin(samplerState:SamplerState.PointClamp, sortMode:SpriteSortMode.BackToFront, transformMatrix:transformMatrix);

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
                        _shapeBatch.FillRectangle(obj.transform.position, obj.transform.scale, obj.color);
                        _shapeBatch.BorderRectangle(obj.transform.position, obj.transform.scale, obj.outlineColor, obj.borderThickness);
                        break;

                    default:

                        break;

                }

            }

        }

    }
}
