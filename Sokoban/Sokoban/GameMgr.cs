using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Sokoban
{

    public static class Utilities
    {
        public static Texture2D CursorTexture;

        public static void DrawCursor(GameMgr gameMgr)
        {
            MouseState mstate = Mouse.GetState();
            gameMgr.DrawSprite(CursorTexture, new Rectangle(mstate.Position, new Point(20, 20)), Color.White);
        }
    }

    public class GameMgr : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        bool _changeState;

        StateBase _currState;
        Texture2D _whiteTexture;

        MainMenu _menu;
        MainGame _game;
        StateBase _savedState;

        int _screenHeight, _screenWidth;

        XNAForm _form;

        public List<string> PuzzlePaths;

        public GameMgr(int screenWidth, int screenHeight)
        {

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;

            PuzzlePaths = new List<string>();
        }

        public int ScreenWidth
        {
            set
            {
                _screenWidth = value;
                graphics.PreferredBackBufferWidth = value;
            }

            get
            {
                return _screenWidth;
            }
        }

        public int ScreenHeight
        {
            set
            {
                _screenHeight = value;
                graphics.PreferredBackBufferHeight = value;

            }

            get
            {
                return _screenHeight;
            }
        }

        public void ChangeState()
        {
        }

        public void StateGame()
        {
            _currState = new MainGame(this);
        }

        public void StateMainMenu()
        {
            _currState = _makeMainMenu();
        }

        public void StateMainMenuResume()
        {
            _currState = _makeMainMenu();
        }

        public void StateExit()
        {
            Exit();
        }

        public void DoNothing(object caller, ButtonEventArgs args)
        {

        }

        public void GotoDesigner(object caller, ButtonEventArgs args)
        {
            LevelDesigner designer = new LevelDesigner(8, 9, this);
            _currState = designer;
        }

        public void GotoPuzzleSelector(object caller, ButtonEventArgs args)
        {
            PuzzleSelector selector = new PuzzleSelector(this);
            _currState = selector;
        }

        public void SaveGameState(object caller, ButtonEventArgs args)
        {
            _savedState = _currState;
        }

        public void MainMenuCallback(object caller, ButtonEventArgs args)
        {
            _currState = _makeMainMenu();
        }

        public void GotoButtonTest(object caller, ButtonEventArgs args)
        {
            _currState = new ButtonTest(this);
        }

        public void DestroyForm(XNAForm form, object sender, ButtonEventArgs args)
        {
            form.Destroy();
        }

        public void ResumeGameCallback(object caller, ButtonEventArgs args)
        {
            MainGame tempSaved;
            tempSaved = _savedState as MainGame;
            if (_savedState != null)
                _currState = _savedState;
            else
                _currState = new MainGame(this);
        }

        public void NewGameCallback(object caller, ButtonEventArgs args)
        {
            _currState = new MainGame(this);
        }

        public void ExitCallback(object caller, ButtonEventArgs args)
        {
            Exit();
        }

        public SpriteBatch SpriteBatch
        {
            get
            {
                return spriteBatch;
            }
        }

        public void centerFormX(XNAForm form)
        {
            form.X = (ScreenWidth - form.Width) / 2;
        }

        public void centerFormY(XNAForm form)
        {
            form.Y = (ScreenHeight - form.Height) / 2;
        }

        public void centerMenuXY(Menu menu)
        {
            centerFormX(menu);
            centerFormY(menu);
        }


        private MainMenu _makeMainMenu()
        {
            int menuHeight = 500;
            int menuWidth = 300;

            MainGame temp = _currState as MainGame;
            bool resume = temp != null;
            _menu = new MainMenu(10, 10, menuWidth, menuHeight, "Sokoban", resume, this);

            return _menu;
        }

        public void SetRenderTarget(RenderTarget2D target)
        {
            GraphicsDevice.SetRenderTarget(target);
        }

        protected override void Initialize()
        {
            base.Initialize();

            spriteBatch = new SpriteBatch(GraphicsDevice);

            int menuHeight = 800;
            int menuWidth = 400;

            _currState = _makeMainMenu();

            _whiteTexture = Content.Load<Texture2D>("WhiteBlock");


            //so that background images do not get wiped when we overlay textures
            GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;

            Utilities.CursorTexture = Content.Load<Texture2D>("StandardCursor");
        }


        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _currState.Update(gameTime);

        }

        public void GotoGame()
        {
            _currState = new MainGame(this);
        }

        public void DrawSprite(Texture2D texture, Rectangle destRect, Color color)
        {
            spriteBatch.Draw(texture, destRect, color);
        }

        public void DrawString(SpriteFont font, string text, Vector2 stringPos, Color stringCol, float scale)
        {
            spriteBatch.DrawString(font, text, stringPos, stringCol, 0, new Vector2(0, 0), scale, 0, 0);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);


            int screenWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            int screenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

            Rectangle backRect = new Rectangle(0, 0, screenWidth, screenHeight);


            spriteBatch.Begin();
            spriteBatch.Draw(_whiteTexture, backRect, Color.Black);

            _currState.Draw(gameTime);

            spriteBatch.End();
        }
    }
}
