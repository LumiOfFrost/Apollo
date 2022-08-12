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

        private Vector2 cameraOffset;
        private Vector2 defaultOffset;

        //Input

        private bool paused;
        private string command;
        private int cursorPos;

        //Gameplay

        public static float gameSpeed = 1;

        public List<GameObject> gameObjectsToDestroy;

        public List<GameObject> gameObjects = new List<GameObject>();

        // Graphics

        private SpriteFont lucidaConsole;

        private OrthographicCamera _camera;

        private GameObject cameraTarget;

        private float defaultZoom = 0.8f;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {

            command = "";

            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 1280, 720);
            _camera = new OrthographicCamera(viewportAdapter);

            _camera.Zoom = defaultZoom;

            defaultOffset = new Vector2((_camera.BoundingRectangle.Width / 2), ((_camera.BoundingRectangle.Height / 4) * 3));

            cameraOffset = defaultOffset;

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

            lucidaConsole = Content.Load<SpriteFont>("Fonts/lucidaConsole");

        }

        protected override void Update(GameTime gameTime)
        {

            if (Keyboard.GetState().IsKeyDown(Keys.OemTilde) && !prevKeyState.IsKeyDown(Keys.OemTilde))
            {
                switch (paused)
                {

                    case false:
                        InputManager.inputActive = false;
                        gameSpeed = 0;
                        paused = true;
                        Debug.WriteLine("paused");
                        break;
                    case true:
                        InputManager.inputActive = true;
                        gameSpeed = 1;
                        paused = false;
                        command = "";
                        Debug.WriteLine("unpaused");
                        break;

                }
                
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F11) && !prevKeyState.IsKeyDown(Keys.F11))
            {
                _graphics.ToggleFullScreen();
            }

            if (!paused)
            {

                prevKeyState = Keyboard.GetState();

                foreach (GameObject obj in gameObjects)
                {

                    obj.Update(this, gameTime);

                }

                foreach (GameObject obj in gameObjectsToDestroy)
                {

                    gameObjects.Remove(obj);

                }

                gameObjectsToDestroy.Clear();

            } else
            {

                if (Keyboard.GetState().GetPressedKeys().Length == 1 && Keyboard.GetState().GetPressedKeys()[0] != Keys.OemTilde)
                {
                    CharEntered(Keyboard.GetState().GetPressedKeys()[0].ToString().ToCharArray()[0]);
                }

            }
            
            UpdateCamera();

            base.Update(gameTime);
        }

        private void UpdateCamera()
        {

            defaultOffset = new Vector2((_camera.BoundingRectangle.Width / 2), ((_camera.BoundingRectangle.Height / 4) * 3));

            cameraOffset = defaultOffset;

            _camera.Position = new Vector2(
            MathHelper.Lerp(_camera.Position.X, cameraTarget.GetCenter().X - cameraOffset.X * _camera.Zoom, 0.08f),
            MathHelper.Lerp(_camera.Position.Y, cameraTarget.GetCenter().Y - cameraOffset.Y * _camera.Zoom, 0.08f));

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

            _shapeBatch.Begin();

            _spriteBatch.Begin();

            if (paused)
            {
                _shapeBatch.FillRectangle(Vector2.Zero, _camera.BoundingRectangle.Size, new Color(0, 0, 0, 0.5f));
                _shapeBatch.FillRectangle(new Vector2(0, _graphics.GraphicsDevice.Viewport.Height - 100), new Vector2((_graphics.GraphicsDevice.Viewport.Width / 3) * 2, 20), new Color(0, 0, 0, 0.5f));

                Vector2 middleText = lucidaConsole.MeasureString(command) / 2;

                _spriteBatch.DrawString(lucidaConsole, command, new Vector2(middleText.X + 5, _graphics.GraphicsDevice.Viewport.Height - 100), Color.White, 0, middleText, 1.0f, SpriteEffects.None, 0.5f);

            }

            _shapeBatch.End();

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void CharEntered(char c)
        {

            string newText = command.Insert(cursorPos, c.ToString()); //Insert the char
            command = newText; //Set the text
            cursorPos++; //Move the text cursor
            
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
