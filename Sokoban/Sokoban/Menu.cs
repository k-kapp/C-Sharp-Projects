using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;



namespace Sokoban
{
    public class Menu : XNAForm
    {
        SpriteBatch _spriteBatch;
        Texture2D _texture, _cursor, _background;
        Button _myButton;

        SpriteFont _titleFont;
        string _menuHeading;

        float _scale = 1.5f;

        int _titleOffset = 20;
        int _titleX;
        int _buttonsYStart;

        Rectangle _titleRect;

        MouseState _mstate;

        List<Button> _buttons;

        int _buttonsWidth, _buttonsHeight;
        int _buttonsX;
        int _buttonsYOffset;
        int _buttonsYSpacing;

        public void VoidFunction(object caller, ButtonEventArgs args)
        {
            
        }

        public void GotoGame(object caller, ButtonEventArgs args)
        {
            _gameMgr.StateGame();
        }

        public void GotoExit(object caller, ButtonEventArgs args)
        {
            _gameMgr.StateExit();
        }

        public override void ImportTextures()
        {
            base.ImportTextures();
            _texture = _gameMgr.Content.Load<Texture2D>("BlackBox");
            _background = _gameMgr.Content.Load<Texture2D>("WhiteBlock");
        }

        public Menu(StateBase stateMgr, string menuHeading, int x, int y, int width, int height) : base(x, y, width, height, stateMgr, "", false)
        {
            _menuHeading = menuHeading;
            _buttons = new List<Button>();

            _titleFont = _gameMgr.Content.Load<SpriteFont>("Courier New");

            Vector2 titleSize = _titleFont.MeasureString(_menuHeading);

            _titleRect = new Rectangle((int)(_mainRect.X + (width - titleSize.X) / 2), _mainRect.Y + _titleOffset, (int)titleSize.X, (int)titleSize.Y);

            ImportTextures();

            _buttonsYStart = _mainRect.Y + _titleOffset + _titleRect.Height;

            num = 10;
        }

        private Button _insertButtonNoCallback(string buttonStr, int index, XNAForm parent)
        {
            Button newButton = new Button(buttonStr, 0, 0, 0, 0, parent);

            newButton.Height = _buttonsHeight;
            newButton.Width = _buttonsWidth;
            _buttons.Insert(index, newButton);

            _centerButton(newButton);
            SetButtonsY();

            return newButton;
        }

        public void InsertButton(string buttonStr, int index, List<Button.ButtonClickCallback> callbacks, XNAForm parent)
        {
            Button newButton = _insertButtonNoCallback(buttonStr, index, parent);
            foreach(var callback in callbacks)
                newButton.EventCalls += callback;
        }

        public void InsertButton(string buttonStr, int index, Button.ButtonClickCallback callback, XNAForm parent)
        {
            Button newButton = _insertButtonNoCallback(buttonStr, index, parent);
            newButton.EventCalls += callback;
        }

        public void AddButton(string buttonStr, Button.ButtonClickCallback callback, XNAForm parent)
        {
            Button newButton = _insertButtonNoCallback(buttonStr, _buttons.Count, parent);
            newButton.EventCalls += callback;
        }


        public void AddButton(string buttonStr, List<Button.ButtonClickCallback> callbacks, XNAForm parent)
        {
            Button newButton = _insertButtonNoCallback(buttonStr, _buttons.Count, parent);
            foreach(var callback in callbacks)
                newButton.EventCalls += callback;
        }

        public void SetButtonSizes(int width, int height)
        {
            _buttonsWidth = width;
            _buttonsHeight = height;

            foreach(var button in _buttons)
            {
                button.Height = _buttonsHeight;
                button.Width = _buttonsWidth;
            }
        }

        public int ButtonsYOffset
        {
            set
            {
                _buttonsYOffset = value;
            }
        }

        public int ButtonsYSpacing
        {
            set
            {
                _buttonsYSpacing = value;
            }
        }

        private void _centerButton(Button button)
        {
                button.X = _buttonsX;
        }

        public void CenterButtonsX()
        {
            _buttonsX = _mainRect.X + (_mainRect.Width - _buttonsWidth) / 2;

            foreach(var button in _buttons)
            {
                _centerButton(button);
            }
        }

        public void CenterButtonsY()
        {
            int availableSpace = _mainRect.Height - _titleOffset - (int)(_titleFont.MeasureString(_menuHeading).Y * _scale);
            int totalButtonSpace = _buttonsHeight * _buttons.Count + _buttonsYSpacing * (_buttons.Count - 1);

            _buttonsYOffset = (availableSpace - totalButtonSpace) / 2;
        }

        public void SetButtonsY()
        {
            int currY = _mainRect.Y + _titleOffset + (int)(_titleFont.MeasureString(_menuHeading).Y*_scale) + _buttonsYOffset;

            foreach(var button in _buttons)
            {
                button.Y = currY;
                currY += _buttonsHeight + _buttonsYSpacing;
            }
        }


        public void CenterTitle()
        {
            _titleX = _mainRect.X + (_mainRect.Width - (int)(_titleFont.MeasureString(_menuHeading).X*_scale)) / 2;
        }

        public void CenterAll()
        {
            CenterButtonsX();
            CenterButtonsY();
            SetButtonsY();
            CenterTitle();

        }

        public int X
        {
            set
            {
                _mainOuterRect.X = value;
            }
        }

        public int Y
        {
            set
            {
                _mainOuterRect.Y = value;
            }
        }

        public void SetXY(int x, int y)
        {
            X = x;
            Y = y;
        }


        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _gameMgr.SetRenderTarget(renderTarget);

            //_gameMgr.SpriteBatch.Draw(_background, _mainRect, Color.Yellow);
            _gameMgr.DrawString(_titleFont, _menuHeading, new Vector2(_titleX, _titleOffset), Color.Black, 1.5f);
            foreach(var button in _buttons)
            {
                button.Draw();
            }

            _gameMgr.SpriteBatch.End();

            _gameMgr.SetRenderTarget(null);

            _gameMgr.SpriteBatch.Begin();

            Texture2D temp = renderTarget;

            _gameMgr.DrawSprite(temp, _mainOuterRect, Color.White);
        }

        public override void Update(GameTime gameTime)
        {
            foreach(var button in _buttons)
            {
                button.Update();
            }
        }
    }
}
