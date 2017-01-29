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

        XNALabel _titleLabel;

        SpriteFont _titleFont;
        string _menuHeading;

        float _scale = 1.5f;

        int _titleOffset = 20;
        int _titleX;
        int _buttonsYStart;

        Rectangle _titleRect;

        MouseState _mstate;

        //List<Button> _buttons;

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
            _labels = new List<XNALabel>();


            _titleFont = _gameMgr.Content.Load<SpriteFont>("Courier New");


            Vector2 titleSize = _titleFont.MeasureString(_menuHeading);

            _titleRect = new Rectangle((int)(_mainOuterRect.X + (width - titleSize.X) / 2), _mainOuterRect.Y + _titleOffset, (int)titleSize.X, (int)titleSize.Y);

            ImportTextures();

            _buttonsYStart = _mainOuterRect.Y + _titleOffset + _titleRect.Height;

            num = 10;

            if (this == null)
            {
                Console.WriteLine("THIS IS NULL IN CTOR");
            }

            _titleLabel = new XNALabel(menuHeading, _titleFont, new Vector2(0, _titleOffset), 1.5f, this);
            AddLabel(_titleLabel);
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
            _buttonsX = (_mainInnerRect.Width - _buttonsWidth) / 2;

            foreach(var button in _buttons)
            {
                _centerButton(button);
            }
        }

        public void CenterButtonsY()
        {
            int availableSpace = _mainInnerRect.Height - _titleOffset - (int)(_titleFont.MeasureString(_menuHeading).Y * _scale);
            int totalButtonSpace = _buttonsHeight * _buttons.Count + _buttonsYSpacing * (_buttons.Count - 1);

            _buttonsYOffset = (availableSpace - totalButtonSpace) / 2;
        }

        public void SetButtonsY()
        {
            int currY = _mainOuterRect.Y + _titleOffset + (int)(_titleFont.MeasureString(_menuHeading).Y*_scale) + _buttonsYOffset;

            foreach(var button in _buttons)
            {
                button.Y = currY;
                currY += _buttonsHeight + _buttonsYSpacing;
            }
        }


        public void CenterTitle()
        {
            _titleX = _mainInnerRect.X + (_mainInnerRect.Width - (int)(_titleFont.MeasureString(_menuHeading).X*_scale)) / 2;
            _titleLabel.Pos = new Vector2(_titleX, _titleLabel.Pos.Y);
        }

        public void CenterAll()
        {
            CenterButtonsX();
            CenterButtonsY();
            SetButtonsY();
            CenterTitle();

        }

        /*
        public int X
        {
            set
            {
                _mainRect.X = value;
            }
        }

        public int Y
        {
            set
            {
                _mainRect.Y = value;
            }
        }
        */

        public void SetXY(int x, int y)
        {
            X = x;
            Y = y;
        }


        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            //_gameMgr.SetRenderTarget(renderTargetInner);

            //_gameMgr.SpriteBatch.Draw(_background, _mainOuterRect, Color.Yellow);
            //_gameMgr.DrawString(_titleFont, _menuHeading, new Vector2(_titleX, _titleOffset), Color.Black, 1.5f);

            /*
            foreach(var button in _buttons)
            {
                button.Draw();
            }

            _gameMgr.SpriteBatch.End();

            _gameMgr.SetRenderTarget(null);

            _gameMgr.SpriteBatch.Begin();

            Texture2D temp = renderTarget;

            _gameMgr.DrawSprite(temp, _mainRect, Color.White);
            */
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
