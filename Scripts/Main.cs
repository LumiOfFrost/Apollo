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

        public Vector2 cameraPosition;

        public Vector2 cameraOffset;

        //Gameplay

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

            gameObjects.Add(new Player(new Transform(new Vector2(_graphics.GraphicsDevice.Viewport.Width / 2 - 30, 0), new Vector2(60, 60), 0), RenderType.Square));
            
            player = gameObjects.OfType<Player>().First();
            
            player.Init();

            gameObjects.Add(new GameObject(new Transform(new Vector2(0, _graphics.GraphicsDevice.Viewport.Height - 100), new Vector2(_graphics.GraphicsDevice.Viewport.Width, 100), 0), RenderType.Square));

            gameObjects.Add(new GameObject(new Transform(new Vector2(0, 0), new Vector2(50, _graphics.GraphicsDevice.Viewport.Height - 100), 0), RenderType.Square));

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

            player.Update(gameTime, gameObjects);

            Debug.WriteLine(player.transform.position);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

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
                        _shapeBatch.FillRectangle(obj.transform.position - cameraPosition, obj.transform.scale, Color.White);
                        break;

                    default:

                        break;

                }

            }

        }

    }
}
