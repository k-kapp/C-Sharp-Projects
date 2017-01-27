using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Sokoban
{
    public class XNAForm : FormMgr
    {
        protected StateBase _stateMgr;

        SpriteFont _defaultFont;
        SpriteFont _font;

        Texture2D _baseTexture;
        Color _baseColor = Color.Yellow;
        Color _borderColor = Color.Gray;
        Color _titleBarColor = Color.LightYellow;

        protected int num;

        protected Rectangle _mainRect;
        protected Rectangle _mainOuterRect;
        protected Rectangle _mainInnerRect;
        Rectangle _topBorderRect;
        Rectangle _rightBorderRect;
        Rectangle _bottomBorderRect;
        Rectangle _leftBorderRect;
        Rectangle _titleBarRect;

        FormMgr _parent;

        bool _showTitleBar;

        protected string _title;

        int _x, _y, _width, _height;

        int _mainBorderWidth = 5;

        protected int _titleBarHeight = 50;

        int _fontHeight;

        protected List<Button> _buttons;

        bool Draggable = true;
        bool _titleBar = true;

        public XNAForm(int x, int y, int width, int height, FormMgr parent, string title = "XNAForm", bool titleBar = true) : base(parent.GameMgr)
        {
            _initArgs(x, y, width, height, title, titleBar);

            _parent = parent;

            Initialize();
        }

        private void _initArgs(int x, int y, int width, int height, string title, bool titleBar)
        {
            _title = title;

            _x = x;
            _y = y;

            _mainRect = new Rectangle(0, 0, width, height);
            _mainOuterRect = new Rectangle(_x, _y, width, height);
            _mainInnerRect = new Rectangle(_x + _mainBorderWidth, _y + _mainBorderWidth, 
                                            width - _mainBorderWidth * 2, height - _mainBorderWidth * 2);
            _showTitleBar = titleBar;
        }

        public void Destroy()
        {
            _parent.RemoveForm(this);
        }

        public virtual void AddButton(int x, int y, Button.ButtonClickCallback callback, string text)
        {
            _buttons.Add(new Sokoban.Button(text, x, y, 100, 50, callback, this));
        }

        public virtual void AddButton(int x, int y, int width, int height, Button.ButtonClickCallback callback, string text)
        {
            _buttons.Add(new Button(text, x, y, width, height, callback, this));
        }

        public virtual void AddButton(Button button)
        {
            _buttons.Add(button);
        }

        public void AddForm(XNAForm form)
        {
            form.X += _mainBorderWidth;
            form.Y += _mainBorderWidth;
            forms.Add(form);
        }

        private void _updateBorderRects()
        {
            _topBorderRect = new Rectangle(0, 0, _mainRect.Width, _mainBorderWidth);
            _rightBorderRect = new Rectangle(_mainRect.Width - _mainBorderWidth, 0,
                                                _mainBorderWidth, _mainRect.Height);
            _bottomBorderRect = new Rectangle(0, _mainRect.Height - _mainBorderWidth, _mainRect.Width, _mainBorderWidth);
            _leftBorderRect = new Rectangle(0, 0, _mainBorderWidth, _mainRect.Height);
        }

        private void _updateTitleBarRect()
        {
            _titleBarRect = new Rectangle(0, 0, _mainRect.Width, _titleBarHeight);
        }

        protected virtual void Initialize()
        {
            _buttons = new List<Button>();

            _defaultFont = _gameMgr.Content.Load<SpriteFont>("Courier New");
            _font = _defaultFont;
            _fontHeight = (int)_font.MeasureString("A").Y;

            _topBorderRect = new Rectangle(0, 0, _mainRect.Width, _mainBorderWidth);
            _rightBorderRect = new Rectangle(_mainRect.Width - _mainBorderWidth, 0, 
                                                _mainBorderWidth, _mainRect.Height);
            _bottomBorderRect = new Rectangle(0, _mainRect.Height - _mainBorderWidth, _mainRect.Width, _mainBorderWidth);
            _leftBorderRect = new Rectangle(0, 0, _mainBorderWidth, _mainRect.Height);

            _titleBarRect = new Rectangle(0, 0, _mainRect.Width, _titleBarHeight);

            ImportTextures();

            if (_mainRect.Width == 0)
                _mainRect.Width = 10;
            if (_mainRect.Height == 0)
                _mainRect.Height = 10;

            renderTarget = new RenderTarget2D(_gameMgr.GraphicsDevice, _mainRect.Width, _mainRect.Height, false, _gameMgr.GraphicsDevice.PresentationParameters.BackBufferFormat, 
                                                _gameMgr.GraphicsDevice.PresentationParameters.DepthStencilFormat, _gameMgr.GraphicsDevice.PresentationParameters.MultiSampleCount,  RenderTargetUsage.PreserveContents);
        }

        private void _updateRenderTarget()
        {
            renderTarget = new RenderTarget2D(_gameMgr.GraphicsDevice, _mainRect.Width, _mainRect.Height, false, _gameMgr.GraphicsDevice.PresentationParameters.BackBufferFormat, 
                                                _gameMgr.GraphicsDevice.PresentationParameters.DepthStencilFormat, _gameMgr.GraphicsDevice.PresentationParameters.MultiSampleCount,  RenderTargetUsage.PreserveContents);
        }

        public GameMgr GameMgr
        {
            get
            {
                return _gameMgr;
            }
        }

        public bool TitleBar
        {
            set
            {
                _titleBar = value;
                if (!_titleBar)
                {
                    Draggable = false;
                }
            }
        }

        public int X
        {
            get
            {
                return _mainOuterRect.X;
            }

            set
            {
                _mainOuterRect.X = value;
                _mainInnerRect.X = value + _mainBorderWidth;
            }
        }

        public int Y
        {
            get
            {
                return _mainOuterRect.Y;
            }

            set
            {
                _mainOuterRect.Y = value;
                _mainInnerRect.Y = value + _mainBorderWidth;
            }
        }

        public int InnerX
        {
            get
            {
                return _mainInnerRect.X;
            }
        }

        public int InnerY
        {
            get
            {
                return _mainInnerRect.Y;
            }
        }


        public int Width
        {
            get
            {
                return _mainRect.Width;
            }

            set
            {
                _mainRect.Width = value;
                _mainOuterRect.Width = value;

                _updateBorderRects();
                _updateTitleBarRect();

                _updateRenderTarget();
            }
        }

        public int Height
        {
            get
            {
                return _mainRect.Height;
            }

            set
            {
                _mainRect.Height = value;
                _mainOuterRect.Height = value;

                _updateBorderRects();
                _updateTitleBarRect();

                _updateRenderTarget();
            }
        }

        public RenderTarget2D RenderTarget
        {
            get
            {
                return renderTarget;
            }
        }

        public SpriteFont Font
        {
            get
            {
                return _font;
            }

            set
            {
                _font = value;
                _fontHeight = (int)_font.MeasureString("A").Y;
                _titleBarHeight = _fontHeight + 10;
            }
        }

        public void SetFont(string fontName)
        {
            SpriteFont temp = _gameMgr.Content.Load<SpriteFont>(fontName);

            if (temp != null)
            {
                Font = temp;
            }
        }

        public void ResetFont()
        {
            _font = _defaultFont;
        }


        public virtual void ImportTextures()
        {
            _baseTexture = _gameMgr.Content.Load<Texture2D>("WhiteBlock");
        }

        public void DrawBase()
        {
            _gameMgr.DrawSprite(_baseTexture, _mainRect, _baseColor);

            if (_showTitleBar)
            {
                _gameMgr.DrawSprite(_baseTexture, _titleBarRect, _titleBarColor);
                _gameMgr.DrawString(_font, _title, new Vector2(10, (_titleBarHeight - _fontHeight) / 2), Color.Black, 1.0f);
            }
            
            _gameMgr.DrawSprite(_baseTexture, _topBorderRect, _borderColor);
            _gameMgr.DrawSprite(_baseTexture, _rightBorderRect, _borderColor);
            _gameMgr.DrawSprite(_baseTexture, _bottomBorderRect, _borderColor);
            _gameMgr.DrawSprite(_baseTexture, _leftBorderRect, _borderColor);
        }

        public virtual void Draw(GameTime gameTime)
        {
            num = 20;

            _gameMgr.SpriteBatch.End();
            _gameMgr.SpriteBatch.Begin();
            _gameMgr.SetRenderTarget(renderTarget);

            DrawBase();

            foreach (Button button in _buttons)
            {
                button.Draw();
            }

            base.Draw(gameTime);

            _gameMgr.SpriteBatch.End();

            _restoreRenderTarget();

            _gameMgr.SpriteBatch.Begin();

            _gameMgr.DrawSprite(renderTarget, _mainOuterRect, Color.White);
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (Button button in _buttons)
            {
                button.Update();
            }

            base.Update(gameTime);

        }

        private void _restoreRenderTarget()
        {
            _gameMgr.SetRenderTarget(_parent.RenderTarget);
        }

    }
}
