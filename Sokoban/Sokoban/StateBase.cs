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
    abstract public class StateBase : FormMgr
    {

        abstract protected void ImportTextures();

        public StateBase(GameMgr gameMgr) : base(gameMgr)
        {
            renderTarget = null;
        }

        public virtual void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            Utilities.DrawCursor(_gameMgr);
        }
        
        protected void _exit()
        {
            _gameMgr.ChangeState();
        }

        public GameMgr GameMgr
        {
            get
            {
                return _gameMgr;
            }
        }

    }
}
