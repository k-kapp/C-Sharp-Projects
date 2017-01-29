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
    public class XNAListElement : Clickable
    {
        Color _activeColor, _inactiveColor, _drawColor;

        Texture2D _background;

        new XNAList _parent;

        bool active = false;

        public XNAListElement(XNAList parent) : base(0, 0, parent.ElementsWidth, parent.ElementsHeight, false, parent)
        {
            _parent = parent;

            _activeColor = Color.Gray;
            _inactiveColor = Color.White;

            _drawColor = _inactiveColor;
        }

        public XNAListElement(Texture2D background, XNAList parent) : this(parent)
        {
            _background = background;
        }

        public XNAListElement(Texture2D background, Color activeColor, Color inactiveColor, XNAList parent)
            : this(background, parent)
        {
            ActiveColor = activeColor;
            InactiveColor = inactiveColor;
        }

        public Color ActiveColor
        {
            set
            {
                _activeColor = value;

                if (active)
                    _drawColor = _activeColor;
            }
        }

        public Color InactiveColor
        {
            set
            {
                _inactiveColor = value;

                if (!active)
                    _drawColor = _inactiveColor;
            }
        }

        public Texture2D Background
        {
            private get
            {
                return _background;
            }

            set
            {
                _background = value;
            }
        }

        public int X
        {
            get
            {
                return _mainRect.X;
            }

            set
            {
                _mainRect.X = value;
            }
        }

        public int Y
        {
            get
            {
                return _mainRect.Y;
            }

            set
            {
                _mainRect.Y = value;
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
            }
        }

        public XNAForm Parent
        {
            set
            {
                var parent = value as XNAList;
                if (parent == null)
                    return;

                _parent = parent;

                base._parent = value;
            }
        }

        public void SetPosition(int index)
        {
            X = 0;
            Y = index * _parent.ElementsHeight;
        }

        public void SetSize()
        {
            Width = _parent.ElementsWidth;
            Height = _parent.ElementsHeight;
        }

        public override void OnClick()
        {
            Console.WriteLine("List element clicked");
            _parent.ElementClicked(this);
        }

        public void MakeInactive()
        {
            active = false;
            Console.WriteLine("Making inactive");
            _drawColor = _inactiveColor;
        }

        public void MakeActive()
        {
            active = true;
            Console.WriteLine("Making active");
            _drawColor = _activeColor;
        }

        public override void Draw()
        {
            _gameMgr.DrawSprite(_background, _mainRect, _drawColor);
        }
    }
}
