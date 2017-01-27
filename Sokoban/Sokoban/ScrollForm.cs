using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * TODO Implement form that can scroll over a grid of objects/widgets
 */

namespace Sokoban
{
    public class ScrollForm : XNAForm
    {
        int _numRows, _numCols;

        int _scrollerWidth = 20;

        public ScrollForm(int x, int y, int width, int height, string title, int numRows, int numCols, FormMgr parent) : base(x, y, width, height, parent, title, true)
        {
            _numRows = numRows;
            _numCols = numCols;
        }
    }
}
