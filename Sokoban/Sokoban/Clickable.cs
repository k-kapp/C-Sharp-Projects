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
    public abstract class Clickable
    {
        protected Rectangle _mainRect;
        protected GameMgr _gameMgr;
        protected XNAForm _parent;

        protected bool held;

        protected bool _clickedUp;
        protected bool _clickedDown;

        protected bool _mouseButtonRelease;

        public Clickable(int x, int y, int width, int height, bool mouseButtonRelease, XNAForm parent)
        {
            _mouseButtonRelease = mouseButtonRelease;

            _mainRect = new Rectangle(x, y, width, height);
            _parent = parent;
            _gameMgr = _parent.GameMgr;

            held = false;
        }

        public abstract void OnClick();

        public bool MouseOnClickable()
        {
            MouseState mstate = Mouse.GetState();

            Point mPos = mstate.Position;

            mPos.X = mPos.X - _parent.InnerXAbs;
            mPos.Y = mPos.Y - _parent.InnerYAbs;

            if ((mPos.X > _mainRect.X) && (mPos.X < _mainRect.X + _mainRect.Width)
                    && (mPos.Y > _mainRect.Y) && (mPos.Y < _mainRect.Y + _mainRect.Height))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual void Update()
        {
            _clickedUp = clickedUp();
            _clickedDown = clickedDown();

            if (_mouseButtonRelease)
            {
                if (_clickedUp)
                    OnClick();
            }
            else
            {
                if (_clickedDown)
                    OnClick();
            }

            MouseState mstate = Mouse.GetState();

            held = mstate.LeftButton == ButtonState.Pressed;
        }

        public abstract void Draw();

        public bool ClickedUp
        {
            get
            {
                return _clickedUp;
            }
        }

        public bool ClickedDown
        {
            get
            {
                return _clickedDown;
            }
        }

        protected bool clickedUp()
        {
            if (!MouseOnClickable())
                return false;

            if (!held)
                return false;

            MouseState mstate = Mouse.GetState();

            if (mstate.LeftButton == ButtonState.Released)
            {
                return true;
            }
            else
                return false;
        }

        protected bool clickedDown()
        {
            if (!MouseOnClickable())
                return false;

            if (held)
                return false;

            MouseState mstate = Mouse.GetState();

            if (mstate.LeftButton == ButtonState.Pressed)
            {
                return true;
            }
            else
                return false;
        }
    }
}
