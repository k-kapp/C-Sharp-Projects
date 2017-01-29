using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban
{
    public class ButtonTest : StateBase
    {
        public static void ButtonChangeX(object sender, ButtonEventArgs args)
        {
            Button button = sender as Button;

            button.X += 50;
        }

        public ButtonTest(GameMgr gameMgr) : base(gameMgr)
        {
            XNAForm form = new XNAForm(100, 100, 500, 500, this);
            form.AddButton(50, 50, 300, 150, ButtonChangeX, "Click me");
            form.AddButton(50, 250, 300, 150, _gameMgr.MainMenuCallback, "Back");
            forms.Add(form);
        }

        protected override void ImportTextures()
        { }


    }
}
