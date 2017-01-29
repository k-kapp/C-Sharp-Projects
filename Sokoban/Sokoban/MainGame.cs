using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Sokoban
{

    public struct KeyState
    {
        public bool PrevDown;
        public ulong StartTime;
        public int LastIncr;
    }

    public class MainGame : StateBase
    {

        PuzzleGrid _grid;
        Texture2D _manUp, _manRight, _manDown, _manLeft;
        Texture2D _wall, _empty, _crate, _void, _target;
        Texture2D _manShoulder, _manHead;

        bool paused = false;
        bool escapePrevUp = true;

        bool _resumeGame = false;

        int _currLevelCount = 1;
        int _currLevelFilenameCount = 0;
        string _currLevelFilename;

        int _windowXPos = 100;
        int _windowYPos = 100;
        int _windowXSize = 800;
        int _windowYSize = 600;
        int _tileSize = 60;

        int _manHeadSize = 24;
        int _manShoulderWidth = 20;
        int _manWidth = 50;
        int _armLen = 6;
        int _armWidth;

        bool _leftLast = true;
        long _lastMoveTime = -1000000;
        int _armDelay;
        bool _collided = false;

        int currMapNum = 0;

        KeyState _up;
        KeyState _right;
        KeyState _down;
        KeyState _left;

        ulong _interval = 250;

        bool _popForm = false;


        public MainGame(GameMgr parent) : base(parent)
        {

            _armWidth = (_manWidth - _manHeadSize) / 2;
            _armDelay = (int)_interval;

            _initialize();
        }

        /*
         * Allows the game to perform any initialization it needs to before starting to run.
         * This is where it can query for any required services and load any non-graphic
         * related content.  Calling base.Initialize will enumerate through any components
         * and initialize them as well.
         */
        private void _initialize()
        {

            if (_gameMgr.PuzzlePaths.Count == 0)
            {
                _gameMgr.MainMenuCallback(null, null);
                return;
            }

            //_grid = new PuzzleGrid("test1.txt");
            _grid = new PuzzleGrid(_gameMgr.PuzzlePaths[currMapNum], FileFormat.DAT, _gameMgr);
            _grid.TileSize = _tileSize;
            //_currLevelFilename = _grid.GridFilepath;

            //_grid.Serialize();

            _up.PrevDown = false;
            _right.PrevDown = false;
            _down.PrevDown = false;
            _left.PrevDown = false;

            ImportTextures();

            //forms = new List<XNAForm>();
            //LoadContent();

        }

        protected override void ImportTextures()
        {
            _crate = _gameMgr.Content.Load<Texture2D>("Crate");
            _empty = _gameMgr.Content.Load<Texture2D>("Empty");
            _wall = _gameMgr.Content.Load<Texture2D>("BrickWall");
            _void = _gameMgr.Content.Load<Texture2D>("BlackBox");
            _target = _gameMgr.Content.Load<Texture2D>("Target");
            _manShoulder = _gameMgr.Content.Load<Texture2D>("Shoulders");

            _manHead = _void;
        }

        private void _mapDone()
        {
            currMapNum++;

            if (currMapNum >= _gameMgr.PuzzlePaths.Count)
            {
                _gameMgr.MainMenuCallback(null, null);
                return;
            }

            _grid = new PuzzleGrid(_gameMgr.PuzzlePaths[currMapNum], FileFormat.DAT, _gameMgr);
            _grid.TileSize = _tileSize;
            //_currLevelFilename = _grid.GridFilepath;

            //_grid.Serialize();

            _up.PrevDown = false;
            _right.PrevDown = false;
            _down.PrevDown = false;
            _left.PrevDown = false;
        }

        private void _drawTile(int row, int column, Tile tile)
        {
            Texture2D texture = null;
            Rectangle destRect;
            destRect.Width = destRect.Height = _tileSize;
            destRect.X = column * _tileSize;
            destRect.Y = row * _tileSize;
            Color col = Color.White;
            switch (tile.State)
            {
                case (Occpr.VOID):
                    texture = _void;
                    break;
                case (Occpr.CRATE):
                    if (tile.Target)
                        col = Color.Brown;
                    texture = _crate;
                    break;
                case (Occpr.EMPTY):
                    if (tile.Target)
                    {
                        texture = _target;
                    }
                    else
                        texture = _empty;
                    break;
                case (Occpr.HUMAN):
                    texture = _empty;
                    break;
                case (Occpr.WALL):
                    texture = _wall;
                    break;

            }
            _gameMgr.DrawSprite(texture, destRect, col);
        }

        public override void Draw(GameTime gameTime)
        {

            _grid.DrawGrid();
            drawMan(gameTime);

            if (forms.Count > 0)
                forms[forms.Count - 1].Draw(gameTime);

            if (forms.Count > 0)
            {
                Utilities.DrawCursor(_gameMgr);
            }

            
        }

        private void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.


        }

        private void UnloadContent()
        {
            // Unload any non ContentManager content here
        }

        public void PopForm(object sender, EventArgs args)
        {
            _popForm = true;
        }

        public void RestartLevel(object sender, EventArgs args)
        {
            _grid.Reset();
            _popForm = true;
        }

        public void ExitToMainMenu(object sender, EventArgs args)
        {
            ButtonEventArgs buttonArgs = new ButtonEventArgs();
            _gameMgr.MainMenuCallback(sender, buttonArgs);
        }

        public void ExitConfirmDialog(object sender, EventArgs args)
        {
            PopupDialog popup = PopupDialog.MakePopupDialog("Are you sure that you want to quit? (progress on this level will be lost)",
                                                            "Quit?", true, this);
            popup.AddButton(new Button("Quit", 0, 0, 0, 0, ExitToMainMenu, popup));
            popup.AddButton(new Button("Cancel", 0, 0, 0, 0, PopForm, popup));

            _gameMgr.centerFormX(popup);
            _gameMgr.centerFormY(popup);

            forms.Add(popup);
        }

        /*
        * Allows the game to run logic such as updating the world,
        * checking for collisions, gathering input, and playing audio.
        * </summary>
        * <param name="gameTime">Provides a snapshot of timing values.</param>
        */
        public override void Update(GameTime gameTime)
        { 

            KeyboardState state = Keyboard.GetState();
            KeyState currKey;

            if (escapePrevUp && state.IsKeyDown(Keys.Escape))
            {
                if (forms.Count == 0)
                {
                    Menu menuInGame = new Menu(this, "", 100, 100, 300, 500);
                    menuInGame.ButtonsYOffset = 10;
                    menuInGame.SetButtonSizes(150, 50);
                    menuInGame.ButtonsYSpacing = 10;
                    menuInGame.AddButton("Resume", PopForm, menuInGame);
                    menuInGame.AddButton("Restart Level", RestartLevel, menuInGame);
                    menuInGame.AddButton("Quit", ExitConfirmDialog, menuInGame);
                    menuInGame.CenterAll();
                    _gameMgr.centerMenuXY(menuInGame);
                    forms.Add(menuInGame);
                }
                else
                {
                    formsRemove.Add(forms[forms.Count - 1]);
                    //forms.RemoveAt(forms.Count - 1);
                }
                escapePrevUp = false;
                /*
                _gameMgr.SaveGameState(this, null);
                _gameMgr.StateMainMenu();
                */
            }

            if (!escapePrevUp && state.IsKeyUp(Keys.Escape))
            {
                escapePrevUp = true;
            }

            if (forms.Count > 0)
            {
                forms[forms.Count - 1].Update(gameTime);

                if (_popForm)
                {
                    forms.RemoveAt(forms.Count - 1);
                    _popForm = false;
                }
                return;
            }

            if (state.IsKeyDown(Keys.Up))
            {
                move(ref _up, (ulong)gameTime.TotalGameTime.TotalMilliseconds, Dir.UP);
                return;
            }
            else
            {
                _up.PrevDown = false;
            }
            if (state.IsKeyDown(Keys.Right))
            {
                move(ref _right, (ulong)gameTime.TotalGameTime.TotalMilliseconds, Dir.RIGHT);
                return;
            }
            else
            {
                _right.PrevDown = false;
            }
            if (state.IsKeyDown(Keys.Down))
            {
                move(ref _down, (ulong)gameTime.TotalGameTime.TotalMilliseconds, Dir.DOWN);
                return;
            }
            else
            {
                _down.PrevDown = false;
            }
            if (state.IsKeyDown(Keys.Left))
            {
                move(ref _left, (ulong)gameTime.TotalGameTime.TotalMilliseconds, Dir.LEFT);
                return;
            }
            else
            {
                _left.PrevDown = false;
            }

            if (_grid.NumTargets() == 0)
            {
                _mapDone();
            }


        }


        private void move(ref KeyState currKey, ulong gameTime, Dir dir)
        {
            if (currKey.PrevDown)
            {
                int currIncr = (int)((gameTime - currKey.StartTime) / _interval);
                if (currIncr > currKey.LastIncr)
                {
                    _lastMoveTime = (long)gameTime;
                    _leftLast = !_leftLast;
                    moveDir(dir);
                    currKey.LastIncr = currIncr;

                }
            }
            else
            {
                _lastMoveTime = (long)gameTime;
                _leftLast = !_leftLast;
                currKey.PrevDown = true;
                moveDir(dir);
                currKey.StartTime = gameTime;
                currKey.LastIncr = 0;
            }
        }

        private void moveDir(Dir dir)
        {
            MoveCode moveResult = MoveCode.ERROR;

            switch (dir)
            {
                case (Dir.UP):
                    moveResult = _grid.move(Dir.UP);
                    break;
                case (Dir.RIGHT):
                    moveResult = _grid.move(Dir.RIGHT);
                    break;
                case (Dir.DOWN):
                    moveResult = _grid.move(Dir.DOWN);
                    break;
                case (Dir.LEFT):
                    moveResult = _grid.move(Dir.LEFT);
                    break;
            }

            if (moveResult == MoveCode.CRATE || moveResult == MoveCode.NOOP)
            {
                _collided = true;
            }
            else
                _collided = false;
        }


        private System.Tuple<int, int> getShoulderLoc()
        {
            System.Tuple<int, int> pos = _grid.CurrPos();

            bool up = isUp();

            int tileY = pos.Item1 * _tileSize;
            int tileX = pos.Item2 * _tileSize;

            int shoulderX, shoulderY;

            int addShoulderWidth = _tileSize / 2 - _manShoulderWidth / 2;
            int addManWidth = _tileSize / 2 - _manWidth / 2;
            shoulderX = tileX;
            shoulderY = tileY;
            int lenX, lenY;
            if (!up)
            {
                shoulderX += addShoulderWidth;
                shoulderY += addManWidth;
                lenX = _manShoulderWidth;
                lenY = _manWidth;
            }
            else
            {
                shoulderX += addManWidth;
                shoulderY += addShoulderWidth;
                lenX = _manWidth;
                lenY = _manShoulderWidth;
            }


            return new System.Tuple<int, int>(shoulderX, shoulderY);
        }

        private bool isUp()
        {
            if ((int)_grid.CurrDir() % 2 == 0)
            {
                return true;
            }
            else
                return false;

        }

        public static Texture2D drawManStill(GameMgr gameMgr, RenderTarget2D currRenderTarget, RenderTarget2D renderTarget, bool up)
        {

            int tileSize = renderTarget.Width;
            int manHeadSize = (int)(renderTarget.Width * (24.0/60.0));
            int manWidth = (int)(renderTarget.Width * (5.0/6.0));
            int manShoulderWidth = (int)(renderTarget.Width * (1.0/3.0));

            Texture2D background = gameMgr.Content.Load<Texture2D>("Empty");
            Texture2D manShoulder = gameMgr.Content.Load<Texture2D>("Shoulders");
            Texture2D manHead = gameMgr.Content.Load<Texture2D>("BlackBox");

            int tileY = 0;
            int tileX = 0;


            int addShoulderWidth = tileSize / 2 - manShoulderWidth / 2;
            int addManWidth = tileSize / 2 - manWidth / 2;

            int shoulderX, shoulderY;

            int lenX, lenY;
            if (!up)
            {
                shoulderX = addShoulderWidth;
                shoulderY = addManWidth;
                lenX = manShoulderWidth;
                lenY = manWidth;
            }
            else
            {
                shoulderX = addManWidth;
                shoulderY = addShoulderWidth;
                lenX = manWidth;
                lenY = manShoulderWidth;
            }

            int headX = tileX + tileSize / 2 - manHeadSize / 2;
            int headY = tileY + tileSize / 2 - manHeadSize / 2;

            Rectangle shoulderRect = new Rectangle(shoulderX, shoulderY, lenX, lenY);

            gameMgr.SetRenderTarget(renderTarget);

            gameMgr.SpriteBatch.Begin();

            gameMgr.DrawSprite(background, new Rectangle(0, 0, renderTarget.Width, renderTarget.Height), Color.White);
            gameMgr.DrawSprite(manShoulder, shoulderRect, Color.White);
            gameMgr.DrawSprite(manHead, new Rectangle(headX, headY, manHeadSize, manHeadSize), Color.White);

            gameMgr.SpriteBatch.End();

            gameMgr.SetRenderTarget(currRenderTarget);

            return renderTarget;
        }

        private void drawManStill()
        {
            System.Tuple<int, int> pos = _grid.CurrPos();

            int tileY = pos.Item1 * _tileSize;
            int tileX = pos.Item2 * _tileSize;

            bool up = isUp();

            System.Tuple<int, int> shoulderLoc = getShoulderLoc();

            int shoulderX, shoulderY;
            shoulderX = shoulderLoc.Item1;
            shoulderY = shoulderLoc.Item2;

            int lenX, lenY;
            if (!up)
            {
                lenX = _manShoulderWidth;
                lenY = _manWidth;
            }
            else
            {
                lenX = _manWidth;
                lenY = _manShoulderWidth;
            }

            int headX = tileX + _tileSize / 2 - _manHeadSize / 2;
            int headY = tileY + _tileSize / 2 - _manHeadSize / 2;

            Rectangle shoulderRect = new Rectangle(shoulderX, shoulderY, lenX, lenY);

            _gameMgr.DrawSprite(_manShoulder, shoulderRect, Color.White);
            _gameMgr.DrawSprite(_manHead, new Rectangle(headX, headY, _manHeadSize, _manHeadSize), Color.White);

        }

        public void drawMan(GameTime gameTime)
        {
            if (((long)gameTime.TotalGameTime.TotalMilliseconds - _lastMoveTime) < _armDelay)
            {
                if (_collided)
                    drawMan(true, true);
                else
                    drawMan(_leftLast, !_leftLast);
            }
            else
                drawMan(false, false);

        }

        public void drawArm(bool left)
        {
            System.Tuple<int, int> shoulderLocs = getShoulderLoc();

            int shoulderX = shoulderLocs.Item1;
            int shoulderY = shoulderLocs.Item2;

            Rectangle armRect = new Rectangle();

            int armShift = (_manHeadSize + _manWidth) / 2;
            int add;

            switch (_grid.CurrDir())
            {
                case (Dir.UP):

                    if (left)
                        add = 0;
                    else
                        add = armShift;
                    armRect = new Rectangle(shoulderX + add, shoulderY - _armLen, _armWidth, _armLen);
                    break;
                case (Dir.RIGHT):
                    if (left)
                        add = 0;
                    else
                        add = armShift;
                    armRect = new Rectangle(shoulderX + _manShoulderWidth, shoulderY + add, _armLen, _armWidth);
                    break;
                case (Dir.DOWN):
                    if (left)
                        add = armShift;
                    else
                        add = 0;
                    armRect = new Rectangle(shoulderX + add, shoulderY + _manShoulderWidth, _armWidth, _armLen);
                    break;
                case (Dir.LEFT):
                    if (left)
                        add = armShift;
                    else
                        add = 0;
                    armRect = new Rectangle(shoulderX - _armLen, shoulderY + add, _armLen, _armWidth);
                    break;
            }
            _gameMgr.DrawSprite(_manShoulder, armRect, Color.White);

        }

        public void drawMan(bool left, bool right)
        {
            drawManStill();

            if (!(left || right))
                return;

            System.Tuple<int, int> shoulderLocs = getShoulderLoc();

            int shoulderX = shoulderLocs.Item1;
            int shoulderY = shoulderLocs.Item2;


            if (left)
            {
                drawArm(true);
            }
            if (right)
            {
                drawArm(false);
            }
        }
    }
}
