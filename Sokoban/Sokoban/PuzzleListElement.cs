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
    class PuzzleListElement : XNAListElement
    {

        public string Filepath;

        public PuzzleListElement(XNAList parent) : base(parent)
        { }

        public PuzzleListElement(string filepath, XNAList parent) : base(parent)
        {
            PuzzleGrid grid = new Sokoban.PuzzleGrid(filepath, FileFormat.DAT, parent.GameMgr);
            Filepath = grid.Filepath;
            _makeBackground(grid);
        }

        public PuzzleListElement(int puzzleNum, XNAList parent) : base(parent)
        {
            PuzzleGrid grid = new PuzzleGrid(puzzleNum, parent.GameMgr);
            Filepath = grid.Filepath;
            _makeBackground(grid);
        }

        private void _makeBackground(PuzzleGrid grid)
        {
            int tileSize = 10;

            grid.TileSize = tileSize;

            RenderTarget2D gridRenderTarget = new RenderTarget2D(_gameMgr.GraphicsDevice, tileSize*grid.NumCols(), tileSize*grid.NumRows());
            Rectangle gridRect = new Rectangle((int)(Width*0.05), (int)(Height*0.05), (int)(Width * 0.9), (int)(Height * 0.9));

            RenderTarget2D elementRenderTarget = new RenderTarget2D(_gameMgr.GraphicsDevice, Width, Height);
            //Rectangle elementRect = new Rectangle(0, 0, Width, Height);

            RenderTarget2D tempRenderTarget = _parent.RenderTarget;

            _gameMgr.SpriteBatch.End();

            _gameMgr.SetRenderTarget(gridRenderTarget);

            _gameMgr.SpriteBatch.Begin();
            grid.DrawGrid();
            _gameMgr.SpriteBatch.End();

            _gameMgr.SetRenderTarget(elementRenderTarget);

            _gameMgr.SpriteBatch.Begin();
            _gameMgr.DrawSprite(gridRenderTarget, gridRect, Color.White);
            _gameMgr.SpriteBatch.End();

            _gameMgr.SetRenderTarget(tempRenderTarget);

            Background = elementRenderTarget;

        }
    } 
}
