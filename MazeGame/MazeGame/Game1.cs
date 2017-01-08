using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MazeGame
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D blackBox;

        const int mazeNumBlocks = 50;
        const int mazeSize = 700;
        const int wallThickness = 4;
        const int blockSize = (mazeSize - (mazeNumBlocks + 1)*wallThickness)/mazeNumBlocks;
        const int mazeStartX = 100;
        const int mazeStartY = 100;
        Point[,] blockLocs;

        Maze myMaze;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = 900;
            graphics.PreferredBackBufferWidth = 1200;
            Content.RootDirectory = "Content";
        }

        private void initBlockLocs()
        {
            blockLocs = new Point[myMaze.getSizeY(), myMaze.getSizeX()];
            for (int row = 0; row < myMaze.getSizeY(); row++)
            {
                for (int col = 0; col < myMaze.getSizeX(); col++)
                {
                    blockLocs[row, col] = new Point(mazeStartX + col * blockSize + wallThickness*(col + 1), mazeStartY + row * blockSize + wallThickness*(row + 1));
                }
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            myMaze = new Maze(mazeNumBlocks, mazeNumBlocks);
            myMaze.split();

            initBlockLocs();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            blackBox = Content.Load<Texture2D>("Images\\BlackBox");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here

            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            for (int row = 0; row < myMaze.getSizeY(); row++)
            {
                for (int col = 0; col < myMaze.getSizeX(); col++)
                {
                    drawBlock(row, col);
                }
            }
            //spriteBatch.Draw(blackBox, new Rectangle(100, 100, 10, 100), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);

        }

        Rectangle getSouthRect(Point loc)
        {
            return new Rectangle(loc.X - wallThickness, loc.Y + blockSize, blockSize + 2 * wallThickness, wallThickness);
        }

        Rectangle getNorthRect(Point loc)
        {
            return new Rectangle(loc.X - wallThickness, loc.Y - wallThickness, blockSize + 2 * wallThickness, wallThickness);
        }

        Rectangle getWestRect(Point loc)
        {
            return new Rectangle(loc.X - wallThickness, loc.Y - wallThickness, wallThickness, blockSize + 2 * wallThickness);
        }

        Rectangle getEastRect(Point loc)
        {
            return new Rectangle(loc.X + blockSize, loc.Y - wallThickness, wallThickness, blockSize + 2 * wallThickness);
        }


        void drawBlock(int row, int col)
        {
            Point loc = blockLocs[row, col];
            MazeBlock block = myMaze[row, col];

            if (!block.N_open)
            {
                spriteBatch.Draw(blackBox, getNorthRect(loc), Color.White);
            }
            if (!block.S_open)
            {
                spriteBatch.Draw(blackBox, getSouthRect(loc), Color.White);
            }
            if (!block.E_open)
            {
                spriteBatch.Draw(blackBox, getEastRect(loc), Color.White);
            }
            if (!block.W_open)
            {
                spriteBatch.Draw(blackBox, getWestRect(loc), Color.White);
            }
        }
    }
}
