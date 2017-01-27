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
    public class ButtonEventArgs : EventArgs
    {
        
    }

    
    public class Button
    {
        public delegate void ButtonClickCallback(object caller, ButtonEventArgs args);

        public event ButtonClickCallback EventCalls = delegate { };

        int _horizOffset = 0;
        int _vertOffset = 0;

        string _text;
        ButtonClickCallback _callback;
        bool _buttonHeld;


        Color _stringCol, _activeCol, _inactiveCol;
        Color _drawCol;

        Vector2 _stringPos;

        SpriteFont _font;

        XNAForm _parent;
        GameMgr _gameMgr;

        Rectangle _buttonRect;

        int _textHeight, _textWidth;

        Texture2D _background;

        float _scale = 1.5f;

        public bool MouseButtonReleaseEvent = true;

        public Button(string text, int x, int y, int width, int height, XNAForm parent)
        {
            _parent = parent;
            _gameMgr = _parent.GameMgr;

            _text = text;

            _buttonRect.X = x;
            _buttonRect.Y = y;
            _buttonRect.Width = width;
            _buttonRect.Height = height;

            Initialize();
        }

        public Button(string text, int x, int y, int width, int height, ButtonClickCallback callback, XNAForm parent) : this(text, x, y, width, height, parent)
        {
            EventCalls += callback;
        }

        public void newBackground(Texture2D texture)
        {
            _background = texture;
        }

        public void newActiveColor(Color color)
        {
            _activeCol = color;
        }

        public void newInactiveColor(Color color)
        {
            _inactiveCol = color;
        }

        public int X
        {
            set
            {
                _buttonRect.X = value;
                updateStringPos();
            }

            get
            {
                return _buttonRect.X;
            }
        }

        public int Y
        {
            set
            {
                _buttonRect.Y = value;
                updateStringPos();
            }

            get
            {
                return _buttonRect.Y;
            }
        }

        public int Width
        {
            set
            {
                _buttonRect.Width = value;
                updateStringPos();
            }

            get
            {
                return _buttonRect.Width;
            }
        }

        public int Height
        {
            set
            {
                _buttonRect.Height = value;
                updateStringPos();
            }

            get
            {
                return _buttonRect.Height;
            }
        }

        public bool StringFitsX()
        {
            return !(_textWidth >= _buttonRect.Width);
        }

        public bool StringFitsY()
        {
            return !(_textHeight >= _buttonRect.Height);
        }

        public void AutoSize(double xScale = 1.3, double yScale = 1.1)
        {
            FitToStringX(xScale);
            FitToStringY(yScale);
        }

        public void FitToStringX(double scale)
        {
            Width = (int)(_textWidth * scale);
        }

        public void FitToStringY(double scale)
        {
            Height = (int)(_textHeight * scale);
        }

        protected void updateStringPos()
        {
            _textHeight = (int)(_font.MeasureString(_text).Y*_scale);
            _textWidth = (int)(_font.MeasureString(_text).X*_scale);
            _stringPos = new Vector2(_buttonRect.X + (_buttonRect.Width - _textWidth)/2 + _horizOffset, _buttonRect.Y + (_buttonRect.Height - _textHeight)/2 + _vertOffset);
        }

        protected void Initialize()
        {
            _buttonHeld = false;

            _stringCol = Color.White;
            _inactiveCol = Color.Red;
            _activeCol = Color.OrangeRed;

            _background = _gameMgr.Content.Load<Texture2D>("WhiteBlock");

            _font = _gameMgr.Content.Load<SpriteFont>("Courier New");
            _textHeight = (int)(_font.MeasureString(_text).Y*_scale);
            _textWidth = (int)(_font.MeasureString(_text).X*_scale);


            Console.WriteLine("Button X pos: " + _buttonRect.X.ToString());
            Console.WriteLine("Button Y pos: " + _buttonRect.Y.ToString());

            updateStringPos();

            Console.WriteLine("textwidth: " + _textWidth.ToString());
            Console.WriteLine("textheight: " + _textHeight.ToString());
        }

        public virtual void OnClick()
        {
            //Console.WriteLine("Button clicked");

            ButtonEventArgs args = new ButtonEventArgs();
            /*
            _callback(this, args);
            */

            EventCalls(this, args);
        }

        public virtual void Draw()
        {
            _gameMgr.DrawSprite(_background, _buttonRect, _drawCol);
            _gameMgr.SpriteBatch.DrawString(_font, _text, _stringPos, _stringCol, 0, new Vector2(0, 0), _scale, 0, 0);
        }

        public Texture2D BackgroundTexture
        {
            get
            {
                return _background;
            }

            set
            {
                _background = value;
            }
        }

        public virtual void Update()
        {
            MouseState mstate = Mouse.GetState();
            Point mPos = mstate.Position;

            mPos.X = mPos.X - _parent.X;
            mPos.Y = mPos.Y - _parent.Y;

            if ((mPos.X > _buttonRect.X) && (mPos.X < _buttonRect.X + _buttonRect.Width)
                    && (mPos.Y > _buttonRect.Y) && (mPos.Y < _buttonRect.Y + _buttonRect.Height))
            {
                _drawCol = _activeCol;
                //Console.WriteLine("Making Color red");
                if (MouseButtonReleaseEvent)
                {
                    if (_buttonHeld && mstate.LeftButton == ButtonState.Released)
                    {
                        OnClick();
                    }
                    else if (mstate.LeftButton == ButtonState.Pressed)
                    {
                        _buttonHeld = true;
                    }
                }
                else
                {
                    if (mstate.LeftButton == ButtonState.Pressed)
                    {
                        OnClick();
                    }
                }
            }
            else
            {
                _drawCol = _inactiveCol;
            }

            if (mstate.LeftButton == ButtonState.Released)
            {
                _buttonHeld = false;
            }
        }
    }
}
