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

        private TileMap tileMap;

        // Graphics

        private int brightness;

        private RenderTarget2D _renderTarget;

        private SpriteFont lucidaConsole;

        private OrthographicCamera _camera;

        private GameObject cameraTarget;

        private float defaultZoom = 0.8f;

        private float pauseDelay = 0;

        private Effect gbEffect;

        public static int paletteId;

        private Texture2D colorPalettes;

        private Texture2D test;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {

            tileMap = new TileMap(100, 100, 60);

            paletteId = 0;

            brightness = 0;

            command = "";

            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 1280, 720);
            _camera = new OrthographicCamera(viewportAdapter);

            _camera.Zoom = defaultZoom;

            defaultOffset = new Vector2((_camera.BoundingRectangle.Width / 2), ((_camera.BoundingRectangle.Height / 4) * 2.5f));

            cameraOffset = defaultOffset;

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;

            _renderTarget = new RenderTarget2D(_graphics.GraphicsDevice, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

            gameObjects.Add(new GameObject(new Transform(new Vector2((_graphics.GraphicsDevice.Viewport.Width / 3) * 2, _graphics.GraphicsDevice.Viewport.Height / 2), new Vector2(_graphics.GraphicsDevice.Viewport.Width / 3, 30), 0), RenderType.Square, new Color(0.5f, 0.5f, 0.5f, 1), Color.Transparent));

            gameObjects.Add(new GameObject(new Transform(new Vector2(_graphics.GraphicsDevice.Viewport.Width / 3, _graphics.GraphicsDevice.Viewport.Height / 4), new Vector2(_graphics.GraphicsDevice.Viewport.Width / 3, 30), 0), RenderType.Square, new Color(0.5f, 0.5f, 0.5f, 1), Color.Transparent));

            gameObjects.Add(new GameObject(new Transform(new Vector2(0, _graphics.GraphicsDevice.Viewport.Height - 100), new Vector2(_graphics.GraphicsDevice.Viewport.Width, 100), 0), RenderType.Square, new Color(0.5f, 0.5f, 0.5f, 1), Color.White));

            gameObjects.Add(new GameObject(new Transform(new Vector2(0, 0), new Vector2(50, _graphics.GraphicsDevice.Viewport.Height - 100), 0), RenderType.Square, new Color(0.5f, 0.5f, 0.5f, 1), Color.White));

            gameObjects.Add(new Player(new Transform(new Vector2(100, 0), new Vector2(30, 60), 0), RenderType.Square, new Color(0.25f, 0.25f, 0.25f, 1), Color.Transparent));

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

            lucidaConsole = Content.Load<SpriteFont>("Fonts/lucidaConsole");

            colorPalettes = Content.Load<Texture2D>("Sprites/ColorPalettes");

            gbEffect = Content.Load<Effect>("Shaders/GBShader");
            test = Content.Load<Texture2D>("Sprites/BrickGroundTileset");

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

                Vector2 mousePos = Vector2.Transform(new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Position.Y), _camera.GetInverseViewMatrix());

                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {

                    if (tileMap.IsInRange(tileMap.GlobalToGrid(mousePos)))
                    {

                        tileMap.AddTile(new Vector2((float)Math.Floor(mousePos.X / tileMap.gridSize), (float)Math.Floor(mousePos.Y / tileMap.gridSize)), true, Tiles.Ground, test);

                    }

                }

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

            _camera.Position = new Vector2(
            MathHelper.Lerp(_camera.Position.X, cameraTarget.GetCenter().X - cameraOffset.X * _camera.Zoom, 0.08f),
            MathHelper.Lerp(_camera.Position.Y, cameraTarget.GetCenter().Y - cameraOffset.Y * _camera.Zoom, 0.08f));

        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.Black);

            GraphicsDevice.SetRenderTarget(_renderTarget);

            var transformMatrix = _camera.GetViewMatrix();

            _spriteBatch.Begin();

            _spriteBatch.FillRectangle(new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White, layerDepth: 1f);

            _spriteBatch.End();

            _spriteBatch.Begin(samplerState:SamplerState.PointClamp, sortMode:SpriteSortMode.BackToFront, transformMatrix:transformMatrix);

            Render();

            for (int i = 0; i < tileMap.width - 1; i++)
            {

                for (int j = 0; j < tileMap.height - 1; j++)
                {

                    if (tileMap.tiles[i,j] != null)
                    {

                        DrawTile(tileMap.tiles[i, j], tileMap);

                    }

                }

            }

            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            _spriteBatch.Begin(SpriteSortMode.Immediate, effect: gbEffect);

            gbEffect.Parameters["PaletteId"].SetValue(paletteId);

            gbEffect.Parameters["PaletteTexture"].SetValue(colorPalettes);

            gbEffect.Parameters["brightness"].SetValue((float)brightness / 5);

            gbEffect.Parameters["paletteHeight"].SetValue(colorPalettes.Height);

            _spriteBatch.Draw(_renderTarget, new Rectangle(0,0,GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

            _spriteBatch.End();

            _spriteBatch.Begin();

            if (consoleOpen)
            {
                _spriteBatch.FillRectangle(Vector2.Zero, new Vector2(_graphics.GraphicsDevice.Viewport.Width, _graphics.GraphicsDevice.Viewport.Height), new Color(0, 0, 0, 0.5f));
                _spriteBatch.FillRectangle(new Vector2(0, _graphics.GraphicsDevice.Viewport.Height - 100), new Vector2((_graphics.GraphicsDevice.Viewport.Width / 3) * 2, 20), new Color(0, 0, 0, 0.5f));

                Vector2 middleText = lucidaConsole.MeasureString(command) / 2;

                _spriteBatch.DrawString(lucidaConsole, command, new Vector2(middleText.X + 5, _graphics.GraphicsDevice.Viewport.Height - 90), Color.White, 0, middleText, 1.0f, SpriteEffects.None, 0.5f);

            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawTile(Tile tile, TileMap tMap)
        {

            _spriteBatch.Draw(
                tile.tileSet,
                new Rectangle(
                    (int)tMap.GridToGlobal(new Vector2(tile.x, tile.y)).X,
                    (int)tMap.GridToGlobal(new Vector2(tile.x, tile.y)).Y,
                    tMap.gridSize, tMap.gridSize),
                new Rectangle(
                    new Point((int)tile.UvPos().X, (int)tile.UvPos().Y),
                    new Point(16, 16)),
                Color.White,
                0,
                Vector2.Zero,
                SpriteEffects.None,
                0f);

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

                case "palette":

                    if (commandSplit.Length == 2)
                    {
                        if(int.TryParse(commandSplit[1], out int newPalette))
                        {

                            if(newPalette >= 0 && newPalette <= colorPalettes.Height - 1)
                            {

                                paletteId = newPalette;

                            } else
                            {

                                Debug.WriteLine("Invalid Palette ID! What are you doing?");

                            }

                        } else
                        {

                            Debug.WriteLine("Invalid Parameter! Do you know how to use this command?");

                        }
                    } else
                    {
                        Debug.WriteLine("Invalid Parameter! Do you know how to use this command?");
                    }

                    break;

                case "brightness":

                    if (commandSplit.Length == 2)
                    {
                        if (int.TryParse(commandSplit[1], out int newBrightness))
                        {

                            if (newBrightness >= -4 && newBrightness <= 4)
                            {

                                brightness = newBrightness;

                            }
                            else
                            {

                                Debug.WriteLine("Invalid Brightness! What are you doing?");

                            }

                        }
                        else
                        {

                            Debug.WriteLine("Invalid Parameter! Do you know how to use this command?");

                        }
                    }
                    else
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
                        _spriteBatch.FillRectangle(obj.transform.position, obj.transform.scale, obj.color, 0.8f);
                        break;

                    default:

                        break;

                }

            }

        }

    }
}
