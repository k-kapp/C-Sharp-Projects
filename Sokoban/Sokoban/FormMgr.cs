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
    public abstract class FormMgr
    {
        protected List<XNAForm> forms;
        protected List<XNAForm> formsRemove;

        protected GameMgr _gameMgr;

        protected RenderTarget2D renderTarget;

        public FormMgr(GameMgr gameMgr)
        {
            _gameMgr = gameMgr;

            forms = new List<XNAForm>();
            formsRemove = new List<XNAForm>();
        }

        public virtual void Draw(GameTime gameTime)
        {
            foreach(XNAForm form in forms)
            {
                form.Draw(gameTime);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (XNAForm form in forms)
            {
                form.Update(gameTime);
            }

            foreach (XNAForm form in formsRemove)
            {
                forms.Remove(form);
            }

            formsRemove.Clear();
        }

        public void RemoveForm(XNAForm form)
        {
            formsRemove.Add(form);
        }

        public GameMgr GameMgr
        {
            get
            {
                return _gameMgr;
            }
        }

        public RenderTarget2D RenderTarget
        {
            get
            {
                return renderTarget;
            }
        }
    }
}
