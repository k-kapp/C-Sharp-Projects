using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGame
{

    enum dir
    {
        NORTH = 0,
        EAST,
        SOUTH,
        WEST
    }

    class MazeBlock
    {
        public bool N_open = false;
        public bool S_open = false;
        public bool W_open = false;
        public bool E_open = false;
    }

    class SubMaze
    {

        Random randGen;
        public SubMaze(int north, int east, int south, int west, dir splitDir, Random randGen)
        {
            this.south = south;
            this.north = north;
            this.east = east;
            this.west = west;
            this.splitDir = splitDir;
            this.randGen = randGen;

            sizeX = this.east - this.west - 1;
            sizeY = this.south - this.north - 1;

            if (sizeX == 0 || sizeY == 0)
            {
                splitSize = 0;
            }
            else
            {
                splitSize = 1;
                setSplitIdx();
            }
        }

        private void setSplitIdx()
        {
            if ((int)splitDir % 2 == 0)
            {
                /*
                splitSize = sizeX - 2;
                if (splitSize <= 0)
                {
                    return;
                }
                */
                int lowBound, upBound;
                if (sizeX > 5)
                {
                    lowBound = 1 + sizeX / 4;
                    upBound = sizeX + 1 - sizeX / 4;
                }
                else
                {
                    lowBound = 1;
                    upBound = sizeX + 1;
                }
                splitIdx = randGen.Next(lowBound, upBound);
                Console.WriteLine("SplitIdx: " + splitIdx.ToString());
            }
            else
            {
                /*
                splitSize = sizeY - 2;
                if (splitSize <= 0)
                {
                    return;
                }
                */
                int lowBound, upBound;
                if (sizeY > 5)
                {
                    lowBound = 1 + sizeY / 4;
                    upBound = sizeY + 1 - sizeY / 4;
                }
                else
                {
                    lowBound = 1;
                    upBound = sizeY + 1;
                }
                splitIdx = randGen.Next(lowBound, upBound);
                Console.WriteLine("SplitIdx: " + splitIdx.ToString());
            }
        }

        public Tuple<SubMaze, SubMaze> split()
        {
            SubMaze sub1, sub2;
            if ((int)splitDir % 2 == 0)
            {
                sub1 = new SubMaze(north, west + splitIdx, south, west, dir.WEST, randGen);
                sub2 = new SubMaze(north, east, south, west + splitIdx, dir.EAST, randGen);
                
                if (sub1.splitSize <= 0)
                {
                    sub1 = null;
                }
                if (sub2.splitSize <= 0)
                {
                    sub2 = null;
                }

                return new Tuple<SubMaze, SubMaze>(sub1, sub2);
            }
            else if ((int)splitDir % 2 == 1)
            {
                sub1 = new SubMaze(north, east, north + splitIdx, west, dir.NORTH, randGen);
                sub2 = new SubMaze(north + splitIdx, east, south, west, dir.SOUTH, randGen);

                if (sub1.splitSize <= 0)
                {
                    sub1 = null;
                }
                if (sub2.splitSize <= 0)
                {
                    sub2 = null;
                }
                return new Tuple<SubMaze, SubMaze>(sub1, sub2);
            }
            else return null;   //will never happen

        }

        public int south;
        public int north;
        public int east;
        public int west;
        public dir splitDir;
        public int sizeX, sizeY, splitSize;
        public int splitIdx;
    }

    class Maze
    {
        int sizeX, sizeY;
        int splitCount;
        int startCol;

        MazeBlock[,] blocks;
        List<SubMaze> subs;

        Random randGen;

        public Maze(int sizeX, int sizeY)
        {
            this.sizeX = sizeX;
            this.sizeY = sizeY;

            initMaze();
        }

        public MazeBlock this[int row, int col]
        {
            get
            {
                return blocks[row, col];
            }
        }

        public int getSizeX()
        {
            return sizeX;
        }

        public int getSizeY()
        {
            return sizeY;
        }

        private void initMaze()
        {
            blocks = new MazeBlock[sizeY, sizeX];

            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    blocks[y, x] = new MazeBlock();
                }
            }
            subs = new List<SubMaze>();
            randGen = new Random();
            subs.Add(new MazeGame.SubMaze(-1, sizeX, sizeY, -1, dir.NORTH, randGen));
            startCol = subs[0].splitIdx + -1;
        }

        public void split()
        {
            splitCount = 0;
            while (subs.Count > 0 && splitCount < 12120)
            {
                splitMaze();
                splitCount++;
            }
            Console.WriteLine("Split count: " + splitCount.ToString());
            fixCol(startCol);
        }

        private void splitMaze()
        {
            Tuple<SubMaze, SubMaze> newSubs = subs[0].split();

            if (newSubs.Item1 != null)
            {
                subs.Add(newSubs.Item1);
            }
            if (newSubs.Item2 != null)
            {
                subs.Add(newSubs.Item2);
            }

            if ((int)subs[0].splitDir % 2 == 0)
            {
                int splitIdx = subs[0].west + subs[0].splitIdx;

                /*
                 * open all blocks in a vertical (north-south) direction where the split took place
                 */
                for (int i = subs[0].north + 2; i < subs[0].south - 1; i++)
                {
                    blocks[i, splitIdx].N_open = true;
                    blocks[i, splitIdx].S_open = true;
                }
                if (subs[0].north + 1 != subs[0].south - 1)
                {
                    blocks[subs[0].north + 1, splitIdx].S_open = true;
                    blocks[subs[0].south - 1, splitIdx].N_open = true;
                }
                /*
                 * if we split in a northerly direction, then open up on the southern side
                 */
                if (subs[0].splitDir == dir.NORTH)
                {
                    if (subs[0].south < sizeY)
                    {
                        blocks[subs[0].south, splitIdx].N_open = true;
                    }
                    blocks[subs[0].south - 1, splitIdx].S_open = true;
                }
                /*
                 * if we split in a southerly direction, then open up on the northern side
                 */
                else if (subs[0].splitDir == dir.SOUTH)
                {
                    blocks[subs[0].north, splitIdx].S_open = true;
                    blocks[subs[0].north + 1, splitIdx].N_open = true;
                }
            }
            else
            {
                int splitIdx = subs[0].north + subs[0].splitIdx;

                /*
                 * open all blocks in a horizontal (west-east) direction
                 */
                for (int i = subs[0].west + 2; i < subs[0].east - 1; i++)
                {
                    blocks[splitIdx, i].W_open = true;
                    blocks[splitIdx, i].E_open = true;
                }
                if (subs[0].west + 1 != subs[0].east - 1)
                {
                    blocks[splitIdx, subs[0].west + 1].E_open = true;
                    blocks[splitIdx, subs[0].east - 1].W_open = true;
                }

                /*
                 * if we split in an easterly direction, then open up on the western side
                 */
                if (subs[0].splitDir == dir.EAST)
                {
                    blocks[splitIdx, subs[0].west].E_open = true;
                    blocks[splitIdx, subs[0].west + 1].W_open = true;
                }

                /*
                 * if we split in a westerly direction, then open up on the eastern side
                 */
                else if (subs[0].splitDir == dir.WEST)
                {
                    blocks[splitIdx, subs[0].east].W_open = true;
                    blocks[splitIdx, subs[0].east - 1].E_open = true;
                }
            }
            subs.RemoveAt(0);
        }

        private void fixCol(int currCol)
        {
            int leftIdx, rightIdx;
            int topIdx, bottomIdx;
            topIdx = bottomIdx = -1;

            for (int i = 0; i < sizeY; i++)
            {
                if (blocks[i, currCol].W_open || blocks[i, currCol].E_open)
                {
                    if (topIdx == -1)
                    {
                        topIdx = i;
                    }
                    else
                        bottomIdx = i;
                }
            }

            int fixRow = randGen.Next(2, topIdx - 1);
            blocks[fixRow, currCol].S_open = false;
            blocks[fixRow + 1, currCol].N_open = false;

            int openBlock = randGen.Next(0, fixRow);
            if (randGen.NextDouble() < 0.5)
            {
                blocks[openBlock, currCol].E_open = true;
                blocks[openBlock, currCol + 1].W_open = true;
            }
            else
            {
                blocks[openBlock, currCol].W_open = true;
                blocks[openBlock, currCol - 1].E_open = true;
            }

            fixRow = randGen.Next(bottomIdx + 1, sizeY);
            blocks[fixRow, currCol].N_open = false;
            blocks[fixRow - 1, currCol].S_open = false;

            openBlock = randGen.Next(fixRow, sizeY);
            if (randGen.NextDouble() < 0.5)
            {
                blocks[openBlock, currCol].E_open = true;
                blocks[openBlock, currCol + 1].W_open = true;
            }
            else
            {
                blocks[openBlock, currCol].W_open = true;
                blocks[openBlock, currCol - 1].E_open = true;
            }
        }

        private void fixRow(int currRow, int startCol, int endCol)
        {
            int bottomIdx, topIdx;
            bottomIdx = topIdx = -1;

            for (int i = startCol; i <= endCol; i++)
            {
                if (blocks[currRow, i].N_open || blocks[currRow, i].S_open)
                {
                    if (bottomIdx == -1)
                    {
                        bottomIdx = i;
                    }
                    else
                    {
                        topIdx = i;
                        break;
                    }
                }
            }

            int divIdx = randGen.Next(startCol, bottomIdx);
            blocks[currRow, divIdx].W_open = false;
            blocks[currRow, divIdx - 1].E_open = false;
            int openIdx = randGen.Next(divIdx, bottomIdx);
            if (randGen.NextDouble() < 0.5)
            {
                blocks[currRow, openIdx].N_open = true;
                blocks[currRow, openIdx - 1].S_open = true;
            }
            else
            {
                blocks[currRow, openIdx].S_open = true;
                blocks[currRow, openIdx + 1].N_open = true;
            }

            divIdx = randGen.Next(topIdx, endCol + 1);
            blocks[currRow, divIdx].W_open = false;
            blocks[currRow, divIdx - 1].E_open = false;
            openIdx = randGen.Next(divIdx, endCol + 1);
            if (randGen.NextDouble() < 0.5)
            {
                blocks[currRow, openIdx].N_open = true;
                blocks[currRow, openIdx - 1].S_open = true;
            }
            else
            {
                blocks[currRow, openIdx].S_open = true;
                blocks[currRow, openIdx + 1].N_open = true;
            }
        }
    }

}
