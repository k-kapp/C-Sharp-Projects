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
    class MainMenu : StateBase
    {
        Menu _menu;
        Texture2D _cursor;

        PopupDialog popup;

        public override void Update(GameTime gameTime)
        {
            _menu.Update(gameTime);
            if (popup != null)
                popup.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _menu.Draw(gameTime);
            if (popup != null)
                popup.Draw(gameTime);

            base.Draw(gameTime);
        }

        protected override void ImportTextures()
        {
            _cursor = _gameMgr.Content.Load<Texture2D>("Crate");
        }

        public void MakePopup(object sender, EventArgs args)
        {
            popup = new Sokoban.PopupDialog("Press any key to continue", "popup", true, this);


            _gameMgr.centerFormX(popup);
            _gameMgr.centerFormY(popup);
        }

        public MainMenu(int x, int y, int width, int height, string heading, bool resume, GameMgr gameMgr) : base(gameMgr)
        {
            _menu = new Sokoban.Menu(this, "Sokoban", 10, 10, width, height);
            _menu.ButtonsYOffset = 10;
            _menu.ButtonsYSpacing = 10;
            _menu.SetButtonSizes(100, 50);

            int screenWidth = _gameMgr.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int screenHeight = _gameMgr.GraphicsDevice.PresentationParameters.BackBufferHeight;


            _menu.SetXY((screenWidth - width) / 2, 10);

            _menu.AddButton("Play", _gameMgr.NewGameCallback, _menu);
            _menu.AddButton("Design", _gameMgr.GotoDesigner, _menu);
            _menu.AddButton("Exit", _gameMgr.ExitCallback, _menu);

            _menu.CenterAll();

            _gameMgr.centerMenuXY(_menu);

            ImportTextures();
        }

    }
}
