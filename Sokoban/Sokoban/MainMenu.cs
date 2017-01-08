using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


/*
 * The code for the main menu, still to be completed (have not added any of this to the
 * main project yet, i.e., there is no main menu that the player will see at the present moment)
 */

namespace Sokoban
{

    public class ButtonEventArgs : EventArgs
    {

    }

    class Button
    {
        public delegate void ButtonClickCallback(object caller, ButtonEventArgs args);

        string _text;
        int _x, _y, _width, _height;
        ButtonClickCallback _callback;

        Vector2 _stringPos;

        SpriteFont _font;

        Game1 _gameObj;

        Rectangle _buttonRect;

        int _textHeight, _textWidth;

        float _scale;

        public Button(string text, int x, int y, int width, int height, Game1 gameObj)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;

            _gameObj = gameObj;

            _text = text;

            Initialize();
        }

        public Button(string text, int x, int y, int width, int height, ButtonClickCallback callback, Game1 gameObj)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
            _callback = callback;

            _gameObj = gameObj;

            _text = text;


            Initialize();
        }

        protected void Initialize()
        {
            _stringPos = new Vector2(_x, _y);
            _font = _gameObj.Content.Load<SpriteFont>("Courier New");
            _textHeight = _font.LineSpacing;
            _textWidth = (int)_font.MeasureString(_text).X;

            _buttonRect.X = _x;
            _buttonRect.Y = _y;
            _buttonRect.Width = _width;
            _buttonRect.Height = _height;

            float currRatio = (float)_textHeight / _textWidth;
            float ratio = (float)_height / _width;

            Console.WriteLine("currRatio: " + currRatio.ToString() + ", desired ratio: " + ratio.ToString());
            Console.WriteLine("Line spacing before change: " + _font.LineSpacing.ToString());

            if (currRatio != ratio)
            {
                //_font.LineSpacing = (int)(ratio * _textWidth);
            }

            Console.WriteLine("Line spacing after change: " + _font.LineSpacing.ToString());
        }

        public virtual void OnClick()
        {
            ButtonEventArgs args = new ButtonEventArgs();
            _callback(this, args);
        }

        public virtual void Draw()
        {
            Console.WriteLine("Font size: " + _font.LineSpacing);
            _gameObj.SpriteBatch.DrawString(_font, _text, _stringPos, Color.White, 0, new Vector2(0, 0), 1.5f, 0, 0);
        }
    }

    public class MainMenu
    {
        SpriteBatch _spriteBatch;
        Texture2D _texture;
        Game1 _gameObj;
        Button _myButton;

        public void VoidFunction(object caller, ButtonEventArgs args)
        {
            
        }

        public MainMenu(Game1 gameObj)
        {
            _gameObj = gameObj;
            _myButton = new Button("Button1", 10, 10, 10, 10, VoidFunction, gameObj);
            _texture = _gameObj.Content.Load<Texture2D>("BlackBox");
        }

        public void Draw()
        {
            int x = _gameObj.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int y = _gameObj.GraphicsDevice.PresentationParameters.BackBufferHeight;

            Rectangle rect = new Rectangle(0, 0, x, y);
            _gameObj.SpriteBatch.Draw(_texture, rect, Color.White);

            _myButton.Draw();

        }
    }
}
