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
    public class PopupDialog : XNAForm
    {
        string _message;
        SpriteFont _font;

        int _textX;
        int _textY;
        int _textWidth;
        int _textHeight;

        int _titleWidth;


        int _buttonsXSpacing;
        int _buttonsWidth;
        int _buttonsHeight;
        int _buttonsTotalWidth;

        int _buttonsXStart;
        int _buttonsYStart;

        bool _disableOthers = true;

        int _zorder;

        public PopupDialog(string message, string title, bool buttonsAutoSize, StateBase stateMgr) : base(0, 0, 0, 0, stateMgr,  title, true)
        {
            _message = message;

            _initialize();
        }

        private void _initialize()
        {
            _buttonsWidth = 100;
            _buttonsHeight = 50;
            _buttonsXSpacing = 20;

            _buttons = new List<Button>();

            _font = _gameMgr.Content.Load<SpriteFont>("Courier New");
            _textY = 20 + 50;
            _titleWidth = (int)(_font.MeasureString(_title).X);
            _textHeight = (int)_font.MeasureString(_message).Y;
            _textWidth = (int)_font.MeasureString(_message).X;
            Width = _textWidth;
            Width += (int)(0.1 * Width);
            Height = _titleBarHeight + _textHeight + 30 + _buttonsHeight + 30;


            _centerTextX();
            _centerButtonsY();
            _initButtons();
        }

        public bool DisableOthers
        {
            get
            {
                return _disableOthers;
            }

            set
            {
                _disableOthers = value;
            }
        }

        public int ZOrder
        {
            set
            {
                _zorder = value;
            }
        }

        public override void AddButton(Button button)
        {
            base.AddButton(button);
            _buttonAddedUpdate(button);
        }

        public override void AddButton(int x, int y, Button.ButtonClickCallback callback, string text)
        {
            base.AddButton(x, y, callback, text);
            _buttonAddedUpdate(_buttons[_buttons.Count - 1]);
        }

        public override void AddButton(int x, int y, int width, int height, Button.ButtonClickCallback callback, string text)
        {
            base.AddButton(x, y, width, height, callback, text);
            _buttonAddedUpdate(_buttons[_buttons.Count - 1]);
        }

        private void _buttonAddedUpdate(Button button)
        {
            button.Height = _buttonsHeight;
            button.Width = _buttonsWidth;

            bool autoSizedX, autoSizedY;

            autoSizedX = autoSizedY = false;

            if ((button.Height == 0) || (button.Width == 0))
            {
                button.AutoSize();
                autoSizedX = true;
                autoSizedY = true;
            }
            else
            {
                if (!button.StringFitsX())
                {
                    button.FitToStringX(1.3);
                    autoSizedX = true;
                }
                if (!button.StringFitsY())
                {
                    button.FitToStringX(1.1);
                    autoSizedY = true;
                }
            }

            if (autoSizedX)
                _matchSizesX();
            if (autoSizedY)
                _matchSizesY();

            _updateWindowSize();
            _centerButtonsX();
            _centerTextX();
            _positionButtonsX();
            _initButtons();

        }

        private void _matchSizesX()
        {
            int maxX = Int32.MinValue;

            foreach(Button button in _buttons)
            {
                if (button.Width > maxX)
                {
                    maxX = button.Width;
                }
            }

            foreach(Button button in _buttons)
            {
                button.Width = maxX;
            }
        }

        private void _matchSizesY()
        {
            int maxY = Int32.MinValue;

            foreach(Button button in _buttons)
            {
                if (button.Height > maxY)
                {
                    maxY = button.Height;
                }
            }

            foreach(Button button in _buttons)
            {
                button.Height = maxY;
            }
        }

        private void _centerTextX()
        {
            _textX = (Width - _textWidth) / 2;
        }

        private void _updateWindowSize()
        {
            _buttonsTotalWidth = _buttonsXSpacing * (_buttons.Count - 1);

            foreach (var button in _buttons)
            {
                _buttonsTotalWidth += button.Width;
            }

            Width = _textWidth > _buttonsTotalWidth ? _textWidth : _buttonsTotalWidth;
            Width = Math.Max(Math.Max(_textWidth, _titleWidth), _buttonsTotalWidth);
            Width += (int)(Width * 0.3);

        }

        private void _centerButtonsX()
        {
            _buttonsXStart = (Width - _buttonsTotalWidth) / 2;
        }

        private void _centerButtonsY()
        {
            _buttonsYStart = (Height - _titleBarHeight - 30 - _textHeight - _buttonsHeight) / 2 + 30 + _textHeight + _titleBarHeight;
        }

        private void _initButtons()
        {
            foreach(Button button in _buttons)
            {
                button.Y = _buttonsYStart;
            }
        }

        private void _positionButtonsX()
        {
            int currPos = _buttonsXStart;
            foreach(Button button in _buttons)
            {
                button.X = currPos;
                currPos += button.Width + _buttonsXSpacing;
            }
        }

        private void _drawButtons()
        {
            foreach(Button button in _buttons)
            {
                button.Draw();
            }
        }

        private void _updateButtons()
        {
            foreach(Button button in _buttons)
            {
                button.Update();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _updateButtons();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _gameMgr.SpriteBatch.End();
            _gameMgr.SpriteBatch.Begin();

            _gameMgr.SetRenderTarget(renderTarget);

            _gameMgr.DrawString(_font, _message, new Vector2(_textX, _textY), Color.Black, 1.0f);
            _drawButtons();

            _gameMgr.SpriteBatch.End();
            _gameMgr.SetRenderTarget(null);

            _gameMgr.SpriteBatch.Begin();
            _gameMgr.DrawSprite(renderTarget, _mainOuterRect, Color.White);
        }
    }
}
