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

        private bool consoleOpen;
        private string command;

        //Gameplay

        public static float gameSpeed = 1;

        public List<GameObject> gameObjectsToDestroy;

        public List<GameObject> gameObjects = new List<GameObject>();

        Player player;

        // Graphics

        private RenderTarget2D _renderTarget;

        private SpriteFont lucidaConsole;

        private OrthographicCamera _camera;

        private GameObject cameraTarget;

        private float defaultZoom = 0.8f;

        private float pauseDelay = 0;

        private Effect gbEffect;

        private int paletteId;

        private Texture2D colorPalettes;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {

            paletteId = 0;

            command = "";

            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 1280, 720);
            _camera = new OrthographicCamera(viewportAdapter);

            _camera.Zoom = defaultZoom;

            defaultOffset = new Vector2((_camera.BoundingRectangle.Width / 2), ((_camera.BoundingRectangle.Height / 4) * 3));

            cameraOffset = defaultOffset;

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;

            _renderTarget = new RenderTarget2D(_graphics.GraphicsDevice, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

            gameObjects.Add(new GameObject(new Transform(new Vector2((_graphics.GraphicsDevice.Viewport.Width / 3) * 2, _graphics.GraphicsDevice.Viewport.Height / 2), new Vector2(_graphics.GraphicsDevice.Viewport.Width / 3, 30), 0), RenderType.Square, Color.White, Color.Transparent));

            gameObjects.Add(new GameObject(new Transform(new Vector2(_graphics.GraphicsDevice.Viewport.Width / 3, _graphics.GraphicsDevice.Viewport.Height / 4), new Vector2(_graphics.GraphicsDevice.Viewport.Width / 3, 30), 0), RenderType.Square, Color.Black, Color.Transparent));

            gameObjects.Add(new GameObject(new Transform(new Vector2(0, _graphics.GraphicsDevice.Viewport.Height - 100), new Vector2(_graphics.GraphicsDevice.Viewport.Width, 100), 0), RenderType.Square, Color.White, Color.White));

            gameObjects.Add(new GameObject(new Transform(new Vector2(0, 0), new Vector2(50, _graphics.GraphicsDevice.Viewport.Height - 100), 0), RenderType.Square, Color.White, Color.White));

            gameObjects.Add(new Player(new Transform(new Vector2(100, 0), new Vector2(30, 60), 0), RenderType.Square, Color.White, Color.Transparent));

            player = gameObjects.OfType<Player>().First();

            cameraTarget = gameObjects.OfType<Player>().First();

            foreach (GameObject obj in gameObjects)
            {

                obj.Init();

            }

            gameObjectsToDestroy = new List<GameObject>();

            Window.TextInput += Window_TextInput;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _shapeBatch = new ShapeBatch(GraphicsDevice, Content);

            lucidaConsole = Content.Load<SpriteFont>("Fonts/lucidaConsole");

            gbEffect = Content.Load<Effect>("Shaders/GBShader");
            gbEffect.Parameters["PaletteTexture"].SetValue(colorPalettes);
            gbEffect.Parameters["PaletteId"].SetValue(0);

            colorPalettes = Content.Load<Texture2D>("Sprites/ColorPalettes");

        }

        protected override void Update(GameTime gameTime)
        {

            if (Keyboard.GetState().IsKeyDown(Keys.OemTilde) && !prevKeyState.IsKeyDown(Keys.OemTilde) && pauseDelay <= 0)
            {
                switch (consoleOpen)
                {

                    case false:
                        InputManager.inputActive = false;
                        gameSpeed = 0;
                        consoleOpen = true;
                        pauseDelay = 0.3f;
                        break;
                    case true:
                        InputManager.inputActive = true;
                        gameSpeed = 1;
                        consoleOpen = false;
                        command = "";
                        pauseDelay = 0.3f;
                        break;

                }
                
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && !prevKeyState.IsKeyDown(Keys.Escape) && pauseDelay <= 0 && consoleOpen)
            {

                InputManager.inputActive = true;
                gameSpeed = 1;
                consoleOpen = false;
                command = "";
                pauseDelay = 0.3f;

            }

            if (Keyboard.GetState().IsKeyDown(Keys.F11) && !prevKeyState.IsKeyDown(Keys.F11))
            {
                _graphics.ToggleFullScreen();
            }

            if (!consoleOpen)
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

                

            }
            
            UpdateCamera();

            pauseDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);
        }

        private void Window_TextInput(object sender, TextInputEventArgs e)
        {

            if (consoleOpen)
            {

                if (!char.IsControl(e.Character))
                {

                    command += e.Character;

                } else if (e.Key == Keys.Back && command.Length > 0)
                {

                    command = command.Substring(0, command.Length - 1);

                } else if (e.Key == Keys.Enter && pauseDelay <= 0)
                {

                    InputManager.inputActive = true;
                    gameSpeed = 1;
                    consoleOpen = false;
                    pauseDelay = 0.3f;
                    RunCommand();
                    command = "";

                }

            }

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

            gbEffect.Parameters["PaletteTexture"].SetValue(colorPalettes);
            gbEffect.Parameters["PaletteId"].SetValue(0);

            GraphicsDevice.Clear(new Color(0,0.02f,0.07f));

            GraphicsDevice.SetRenderTarget(_renderTarget);

            var transformMatrix = _camera.GetViewMatrix();

            _shapeBatch.Begin(transformMatrix);

            _spriteBatch.Begin(samplerState:SamplerState.PointClamp, sortMode:SpriteSortMode.BackToFront, transformMatrix:transformMatrix);

            _spriteBatch.DrawRectangle(new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), new Color(0, 0.02f, 0.07f));

            Render();

            _spriteBatch.End();

            _shapeBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            _spriteBatch.Begin(SpriteSortMode.Immediate, effect: gbEffect);

            _spriteBatch.Draw(_renderTarget, new Rectangle(0,0,GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

            _spriteBatch.End();

            _shapeBatch.Begin();

            _spriteBatch.Begin();

            if (consoleOpen)
            {
                _spriteBatch.FillRectangle(Vector2.Zero, new Vector2(_graphics.GraphicsDevice.Viewport.Width, _graphics.GraphicsDevice.Viewport.Height), new Color(0, 0, 0, 0.5f));
                _spriteBatch.FillRectangle(new Vector2(0, _graphics.GraphicsDevice.Viewport.Height - 100), new Vector2((_graphics.GraphicsDevice.Viewport.Width / 3) * 2, 20), new Color(0, 0, 0, 0.5f));

                Vector2 middleText = lucidaConsole.MeasureString(command) / 2;

                _spriteBatch.DrawString(lucidaConsole, command, new Vector2(middleText.X + 5, _graphics.GraphicsDevice.Viewport.Height - 90), Color.White, 0, middleText, 1.0f, SpriteEffects.None, 0.5f);

            }

            _shapeBatch.End();

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void RunCommand()
        {

            string[] commandSplit = command.Split(" ");

            switch (commandSplit[0].ToLower())
            {

                case "":
                    break;

                case "end":
                    Exit();
                    break;
                case "tp":
                    if (commandSplit.Length == 3 && float.TryParse(commandSplit[1], out float xValue) && float.TryParse(commandSplit[2], out float yValue))
                    {
                        player.transform.position = new Vector2(xValue, yValue);
                    } else
                    {
                        Debug.WriteLine("Invalid Parameter! Do you know how to use this command?");
                    }
                    break;
                case "zoom":
                    if (commandSplit.Length == 2)
                    {
                        if (float.TryParse(commandSplit[1], out float newZoom))
                        {
                            if (newZoom > 0)
                            {
                                _camera.Zoom = newZoom;
                            } else
                            {
                                Debug.WriteLine("Can't zoom negative or zero number!");
                            }

                        }
                        else
                        {
                            Debug.WriteLine("Invalid Parameter! Do you know how to use this command?");
                        }
                    } else
                    {

                        Debug.WriteLine("Invalid Parameter! Do you know how to use this command?");

                    }
                    break;

                default:
                    Debug.WriteLine("Invalid Command! Did you spell it correctly?");
                    break;

            }

        }

        protected void Render()
        {

            foreach (GameObject obj in gameObjects)
            {

                switch(obj.renderType)
                {

                    case RenderType.Square:
                        _shapeBatch.FillRectangle(obj.transform.position, obj.transform.scale, obj.color);
                        break;

                    default:

                        break;

                }

            }

        }

    }
}
