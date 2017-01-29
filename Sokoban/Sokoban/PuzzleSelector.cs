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
    class PuzzleSelector : StateBase
    {


        private XNAForm _mainForm;

        PuzzleList _listForm1, _listForm2;

        public PuzzleSelector(GameMgr gameMgr) : base(gameMgr)
        {
            _initialize();

        }

        public void LeftToRight(object sender, ButtonEventArgs args)
        {
            var element = _listForm1.RemoveActiveElement();
            if (element != null)
                _listForm2.AddElement(element);
        }

        public void RightToLeft(object sender, ButtonEventArgs args)
        {
            var element = _listForm2.RemoveActiveElement();
            if (element != null)
                _listForm1.AddElement(element);
        }

        protected override void ImportTextures()
        { }

        public void SaveAll(object sender, ButtonEventArgs args)
        {
            _gameMgr.PuzzlePaths = _listForm2.GetActivePuzzleFilepaths();

            _gameMgr.MainMenuCallback(sender, args);
        }

        private void _initialize()
        {
            int inbetweenSpace = 100;
            int listWidth = 300;
            int listHeight = 500;
            int totalListWidth = listWidth * 2 + inbetweenSpace;


            _mainForm = new Sokoban.XNAForm(10, 10, 750, 700, this, "Select Puzzles", true);

            int startX = (_mainForm.Width - totalListWidth) / 2;

            _listForm1 = new PuzzleList(startX, 50, listWidth, listHeight, "Reserve puzzles", 5, _mainForm);
            _listForm1.AddAllElements("Puzzles");
            _listForm2 = new PuzzleList(startX + listWidth + inbetweenSpace, 50, listWidth, listHeight, "Active puzzles", 5, _mainForm);

            Button okButton = new Button("OK", (_mainForm.Width - 50)/2, 580, 100, 50, _mainForm);
            okButton.EventCalls += SaveAll;

            Button rightButton = new Sokoban.Button("", (_mainForm.Width - 60)/2, 100, 60, 60, _mainForm);
            rightButton.newBackground(_gameMgr.Content.Load<Texture2D>("RightArrow"));
            rightButton.EventCalls += LeftToRight;

            Button leftButton = new Sokoban.Button("", (_mainForm.Width - 60)/2, 200, 60, 60, _mainForm);
            leftButton.newBackground(_gameMgr.Content.Load<Texture2D>("LeftArrow"));
            leftButton.EventCalls += RightToLeft;

            _mainForm.AddForm(_listForm1);
            _mainForm.AddForm(_listForm2);
            _mainForm.AddButton(leftButton);
            _mainForm.AddButton(rightButton);
            _mainForm.AddButton(okButton);

            //XNAForm form = new XNAForm(100, 100, 300, 300, this);

            forms.Add(_mainForm);

        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

    }
}
