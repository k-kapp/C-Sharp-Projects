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

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        PuzzleGrid _grid;
        Texture2D _manUp, _manRight, _manDown, _manLeft;
        Texture2D _wall, _empty, _crate, _void, _target;
        Texture2D _manShoulder, _manHead;

        MainMenu _menu;

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


        KeyState _up;
        KeyState _right;
        KeyState _down;
        KeyState _left;

        ulong _interval = 250;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _armWidth = (_manWidth - _manHeadSize) / 2;
            _armDelay = (int)_interval;
        }

        /*
         * Allows the game to perform any initialization it needs to before starting to run.
         * This is where it can query for any required services and load any non-graphic
         * related content.  Calling base.Initialize will enumerate through any components
         * and initialize them as well.
         */
        protected override void Initialize()
        {
            base.Initialize();

            _grid = new PuzzleGrid("test1.txt");

            _up.PrevDown = false;
            _right.PrevDown = false;
            _down.PrevDown = false;
            _left.PrevDown = false;

            _menu = new MainMenu(this);
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);


            _crate = Content.Load<Texture2D>("Crate");
            _empty = Content.Load<Texture2D>("Empty");
            _wall = Content.Load<Texture2D>("BrickWall");
            _void = Content.Load<Texture2D>("BlackBox");
            _target = Content.Load<Texture2D>("Target");

            _manShoulder = Content.Load<Texture2D>("Shoulders");
            _manHead = _void;
        }

        protected override void UnloadContent()
        {
            // Unload any non ContentManager content here
        }

        /*
        * Allows the game to run logic such as updating the world,
        * checking for collisions, gathering input, and playing audio.
        * </summary>
        * <param name="gameTime">Provides a snapshot of timing values.</param>
        */
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);

            KeyboardState state = Keyboard.GetState();
            KeyState currKey;

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
                Exit();
            }

        }

        public SpriteBatch SpriteBatch
        {
            get
            {
                return spriteBatch;
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

        // This is called when the game should draw itself.
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            int numCols = _grid.NumCols();
            int numRows = _grid.NumRows();

            spriteBatch.Begin();

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    Texture2D texture = null;
                    Rectangle destRect;
                    destRect.Width = destRect.Height = _tileSize;
                    destRect.X = j * _tileSize;
                    destRect.Y = i * _tileSize;
                    Color col = Color.White;
                    switch (_grid.Tiles[i, j].State)
                    {
                        case (Occpr.VOID):
                            texture = _void;
                            break;
                        case (Occpr.CRATE):
                            if (_grid.Tiles[i, j].Target)
                                col = Color.Brown;   
                            texture = _crate;
                            break;
                        case (Occpr.EMPTY):
                            if (_grid.Tiles[i, j].Target)
                            {
                                texture = _target;
                            }
                            else
                                texture = _empty;
                            break;
                        case (Occpr.HUMAN): 
                            if (((long)gameTime.TotalGameTime.TotalMilliseconds - _lastMoveTime) < _armDelay)
                            {
                                if (_collided)
                                    drawMan(true, true);
                                else
                                    drawMan(_leftLast, !_leftLast);
                            }
                            else
                                drawMan(false, false);
                            break;
                        case (Occpr.WALL):
                            texture = _wall;
                            break;

                    }
                    if (_grid.Tiles[i, j].State != Occpr.HUMAN)
                        spriteBatch.Draw(texture, destRect, col);
                }
            }

            //_menu.Draw();

            spriteBatch.End();
            base.Draw(gameTime);
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

            spriteBatch.Draw(_empty, new Rectangle(tileX, tileY, _tileSize, _tileSize), Color.White);
            spriteBatch.Draw(_manShoulder, shoulderRect, Color.White);
            spriteBatch.Draw(_manHead, new Rectangle(headX, headY, _manHeadSize, _manHeadSize), Color.White);
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
            spriteBatch.Draw(_manShoulder, armRect, Color.White);

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
