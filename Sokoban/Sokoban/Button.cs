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

    
    public class Button : Clickable
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


        //Rectangle _mainRect;

        int _textHeight, _textWidth;

        Texture2D _background;

        float _scale = 1.5f;

        public bool MouseButtonReleaseEvent = true;

        public Button(string text, int x, int y, int width, int height, XNAForm parent) : base(x, y, width, height, true, parent)
        {
            _gameMgr = _parent.GameMgr;

            _text = text;

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
                _mainRect.X = value;
                updateStringPos();
            }

            get
            {
                return _mainRect.X;
            }
        }

        public int Y
        {
            set
            {
                _mainRect.Y = value;
                updateStringPos();
            }

            get
            {
                return _mainRect.Y;
            }
        }

        public int Width
        {
            set
            {
                _mainRect.Width = value;
                updateStringPos();
            }

            get
            {
                return _mainRect.Width;
            }
        }

        public int Height
        {
            set
            {
                _mainRect.Height = value;
                updateStringPos();
            }

            get
            {
                return _mainRect.Height;
            }
        }

        public bool StringFitsX()
        {
            return !(_textWidth >= _mainRect.Width);
        }

        public bool StringFitsY()
        {
            return !(_textHeight >= _mainRect.Height);
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
            _stringPos = new Vector2(_mainRect.X + (_mainRect.Width - _textWidth)/2 + _horizOffset, _mainRect.Y + (_mainRect.Height - _textHeight)/2 + _vertOffset);
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


            Console.WriteLine("Button X pos: " + _mainRect.X.ToString());
            Console.WriteLine("Button Y pos: " + _mainRect.Y.ToString());

            updateStringPos();

            Console.WriteLine("textwidth: " + _textWidth.ToString());
            Console.WriteLine("textheight: " + _textHeight.ToString());
        }

        public override void OnClick()
        {
            //Console.WriteLine("Button clicked");

            ButtonEventArgs args = new ButtonEventArgs();
            /*
            _callback(this, args);
            */

            EventCalls(this, args);
        }

        public override void Draw()
        {
            _gameMgr.DrawSprite(_background, _mainRect, _drawCol);
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

        public override void Update()
        {
            base.Update();

            if (MouseOnClickable())
            {
                _drawCol = _activeCol;
            }
            else
            {
                _drawCol = _inactiveCol;
            }
        }

        /*
        public virtual void Update()
        {
            MouseState mstate = Mouse.GetState();
            Point mPos = mstate.Position;


            mPos.X = mPos.X - _parent.InnerXAbs;
            mPos.Y = mPos.Y - _parent.InnerYAbs;

            if (false)
            {
                Console.WriteLine("Button text: " + _text);
                Console.WriteLine("Mouse position: " + mPos.X);
                Console.WriteLine(mPos.Y);
                Console.WriteLine("Parent XAbs: " + _parent.XAbs);
                Console.WriteLine("Parent YAbs: " + _parent.YAbs);
                Console.WriteLine("Parent InnerXAbs: " + _parent.InnerXAbs);
                Console.WriteLine("Parent InnerYAbs: " + _parent.InnerYAbs);
            }

            if ((mPos.X > _mainRect.X) && (mPos.X < _mainRect.X + _mainRect.Width)
                    && (mPos.Y > _mainRect.Y) && (mPos.Y < _mainRect.Y + _mainRect.Height))
            {
                _drawCol = _activeCol;
                //Console.WriteLine("Making Color red. Text of button: " + _text);
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

            //_drawCol = Color.White;
        }
        */
    }
}
