using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban
{
    class PuzzleList : XNAList
    {
        public PuzzleList(int x, int y, int width, int height, string title, int numRows, FormMgr parent) : base(x, y, width, height, title, numRows, parent)
        { }

        public void AddElement(string filepath)
        {
            var puzzleElement = new PuzzleListElement(filepath, this);

            AddElement(puzzleElement);
        }

        public void AddElement(int puzzleNum)
        {
            var puzzleElement = new PuzzleListElement(puzzleNum, this);

            AddElement(puzzleElement);
        }

        public void AddElements(int num)
        {
            for (int i = 0; i < num; i++)
            {
                AddElement(i + 1);
            }
        }

        public void AddAllElements(string targetDir)
        {
            List<string> fileList = PuzzleGrid.getPuzzleFilenames(targetDir);

            foreach(var filename in fileList)
            {
                AddElement(filename);
            }
        }

        public List<string> GetActivePuzzleFilepaths()
        {
            List<string> paths = new List<string>();
            foreach(var el in _elements)
            {
                var element = el as PuzzleListElement;
                paths.Add(element.Filepath);
            }

            return paths;
        }
    }
}
