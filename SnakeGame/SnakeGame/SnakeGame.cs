using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;


/*
 * TODO: 
 *       make visible borders
 */


namespace SnakeGame
{
    public enum dir
    {
        LEFT = 0,
        UP,
        RIGHT,
        DOWN
    }

    public class Snake
    {
        public List<Vector2> positions;
        dir currDir;

        public Snake(dir initDir, float x, float y)
        {
            currDir = initDir;

            positions = new List<Vector2>();
            positions.Add(new Vector2(x, y));
        }

        public void addBlock(int incr)
        {
            Vector2 last = positions[positions.Count - 1];
            positions.Add(last);
        }

        public void setNewDir(dir newDir)
        {
            int rem = (int)newDir % 2;
            if ((int)currDir % 2 != rem)
                currDir = newDir;
        }

        public void updatePosition(SnakeGame parent)
        {
            Vector2 prevPos = positions[0];
            Vector2 newPos;

            switch (currDir)
            {
                case dir.UP:
                    positions[0] = new Vector2(positions[0].X, positions[0].Y - parent.getIncr());
                    if (positions[0].Y < parent.getIncr())
                    {
                        int fixedPos = parent.getWindowHeight() - parent.getIncr();
                        fixedPos -= fixedPos % parent.getIncr();
                        positions[0] = new Vector2(positions[0].X, fixedPos);
                    }
                    break;
                case dir.DOWN:
                    positions[0] = new Vector2(positions[0].X, positions[0].Y + parent.getIncr());
                    if (positions[0].Y > parent.getWindowHeight() - parent.getIncr())
                    {
                        int fixedPos = parent.getIncr();
                        positions[0] = new Vector2(positions[0].X, fixedPos);
                    }
                    break;
                case dir.LEFT:
                    positions[0] = new Vector2(positions[0].X - parent.getIncr(), positions[0].Y);
                    if (positions[0].X < parent.getIncr())
                    {
                        int fixedPos = parent.getWindowWidth() - parent.getIncr();
                        fixedPos -= fixedPos % parent.getIncr();
                        positions[0] = new Vector2(fixedPos, positions[0].Y);
                    }
                    break;
                case dir.RIGHT:
                    positions[0] = new Vector2(positions[0].X + parent.getIncr(), positions[0].Y);
                    if (positions[0].X > parent.getWindowWidth() - parent.getIncr())
                    {
                        int fixedPos = parent.getIncr();
                        positions[0] = new Vector2(fixedPos, positions[0].Y);
                    }
                    break;
            }

            for (int i = 1; i < positions.Count; i++)
            {
                newPos = prevPos;
                prevPos = positions[i];
                positions[i] = newPos;
            }

        }

        public bool collided()
        {
            Vector2 frontPos = positions[0];

            for (int i = 2; i < positions.Count; i++)
            {
                if (frontPos == positions[i])
                    return true;
            }
            return false;
        }

        public bool onSnake(Vector2 vec)
        {
            foreach(var pos in positions)
            {
                if (vec == pos)
                    return true;
            }
            return false;
        }
    }

    public class SnakeGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        KeyboardState oldState;

        int incr;

        List<bool> keysDown;

        Snake snake;

        bool quit;

        const int window_width = 640;
        const int window_height = 480;
        Point window_pos = new Point(50, 50);

        LinkedList<Vector2> foods;
        const int numFoods = 4;

        Vector2 foodScale;
        Vector2 snakeScale;
        Vector2 location;

        Texture2D foodTexture;
        Texture2D snakeTexture;

        dir currDir;

        const int fps = 17;

        

        public SnakeGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        
        public int getIncr()
        {
            return incr;
        }

        private void handleKeys()
        {
            KeyboardState newState = Keyboard.GetState();

            if (newState.IsKeyDown(Keys.Left))
            {
                snake.setNewDir(dir.LEFT);
            }
            else if (newState.IsKeyDown(Keys.Up))
            {
                snake.setNewDir(dir.UP);
            }
            else if (newState.IsKeyDown(Keys.Right))
            {
                snake.setNewDir(dir.RIGHT);
            }
            else if (newState.IsKeyDown(Keys.Down))
            {
                snake.setNewDir(dir.DOWN);
            }

            oldState = newState;
        }

        public int getWindowWidth()
        {
            return window_width;
        }

        public int getWindowHeight()
        {
            return window_height;
        }

        public bool onFood(Vector2 pos)
        {
            foreach(var foodPos in foods)
            {
                if (pos == foodPos)
                {
                    return true;
                }
            }
            return false;
        }

        private void makeFood(Vector2 posRemove)
        {
            if (foods.Count == numFoods)
            {
                foods.Remove(posRemove);
            }

            snake.addBlock(incr);

            makeFood();
        }

        private void makeFood()
        {
            Random gen = new Random();

            int xFood, yFood;

            Vector2 foodPos;
            
            do
            {
                xFood = gen.Next(0, window_width/incr);
                yFood = gen.Next(0, window_height/incr);
                foodPos = new Vector2(Math.Max(xFood*incr, incr), Math.Max(yFood*incr, incr));

                System.Console.WriteLine("foodPos: " + foodPos.X + ", " + foodPos.Y);

            } while (snake.onSnake(foodPos) || onFood(foodPos));

            foods.AddLast(foodPos);
        }

        /*
         * Allows the game to perform any initialization it needs to before starting to run.
         * This is where it can query for any required services and load any non-graphic
         * related content.  Calling base.Initialize will enumerate through any components
         * and initialize them as well.
         */
        protected override void Initialize()
        {
            TargetElapsedTime = TimeSpan.FromSeconds(1.0 / fps);

            location = new Vector2(100);
            incr = 15;
            keysDown = new List<bool>();


            oldState = new KeyboardState();

            currDir = dir.RIGHT;

            snake = new Snake(dir.RIGHT, 10*incr, 10*incr);

            quit = false;

            for (int i = 0; i < 4; i++)
            {
                keysDown.Add(false);
            }

            foods = new LinkedList<Vector2>();

            for (int i = 0; i < numFoods; i++)
            {
                makeFood();
            }

            base.Initialize();

            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = window_height;
            graphics.PreferredBackBufferWidth = window_width;
            graphics.ApplyChanges();

            Window.Position = window_pos;
        }

        private void makeScales()
        {
            int w = foodTexture.Width;
            int h = foodTexture.Height;

            foodScale = new Vector2(incr / (float)w, incr / (float)h);

            w = snakeTexture.Width;
            h = snakeTexture.Height;

            snakeScale = new Vector2(incr / (float)w, incr / (float)h);
        }

        // will be called once per game
        protected override void LoadContent()
        {
            

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            foodTexture = Content.Load<Texture2D>("Images/FoodBlock");
            snakeTexture = Content.Load<Texture2D>("Images/WhiteBlock");

            makeScales();
        }

        // will be called once per game
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /*
         * Allows the game to run logic such as updating the world,
         * checking for collisions, gathering input, and playing audio.
         * <param name="gameTime">Provides a snapshot of timing values.</param>
         */
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (quit)
                Exit();

            handleKeys();
            snake.updatePosition(this);
            if (onFood(snake.positions[0]))
            {
                makeFood(snake.positions[0]);
            }

            if (snake.collided())
            {
                quit = true;
            }

            base.Update(gameTime);
        }

        // This is called when the game should draw itself.
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            foreach (var snakePos in snake.positions)
            {
                spriteBatch.Draw(snakeTexture, snakePos, null, Color.White, 0.0f, Vector2.Zero, snakeScale, SpriteEffects.None, 0.0f);
            }
            foreach (var foodPos in foods)
            {
                spriteBatch.Draw(foodTexture, foodPos, null, Color.White, 0.0f, Vector2.Zero, foodScale, SpriteEffects.None, 0.0f);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
