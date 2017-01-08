using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban
{
    public enum Occpr
    {
        EMPTY,
        HUMAN,
        CRATE,
        WALL,
        VOID
    }

    public enum Dir
    {
        UP,
        RIGHT,
        DOWN,
        LEFT
    }

    public enum MoveCode
    {
        NOOP,
        FREE,
        CRATE,
        ERROR
    }


    class Tile
    {
        public Occpr State;
        public bool Target;
        public Tile(bool target, Occpr state)
        {
            this.Target = target;
            this.State = state;
        }
    }

    class PuzzleGrid
    {

        public Tile [,] Tiles;
        int _numRows, _numCols;
        int _numTargets;
        Tuple<int, int> _currPos;
        Dir _currDir = Dir.RIGHT;

        public PuzzleGrid(int numRows, int numCols)
        {
            Tiles = new Tile[numRows, numCols];
        }

        public static Tile MakeTile(char c)
        {
            switch (c)
            {
                case ('T'):
                    return new Tile(true, Occpr.EMPTY);
                case ('H'):
                    return new Tile(false, Occpr.HUMAN);
                case ('C'):
                    return new Tile(false, Occpr.CRATE);
                case ('E'):
                    return new Tile(false, Occpr.EMPTY);
                case ('W'):
                    return new Tile(false, Occpr.WALL);
                case ('V'):
                    return new Tile(false, Occpr.VOID);
                default:
                    return null;
            }
        }

        public PuzzleGrid(string inputFile)
        {
            _numTargets = 0;

            string[] allLines = System.IO.File.ReadAllLines(inputFile);

            _numRows = allLines.Length;
            _numCols = allLines[0].Split(null).Length;

            Tiles = new Tile[_numRows, _numCols];

            for (int i = 0; i < allLines.Length; i++)
            {
                string[] rowElements = allLines[i].Split(null);

                for (int j = 0; j < rowElements.Length; j++)
                {
                    Tiles[i, j] = MakeTile(rowElements[j][0]);

                    if (Tiles[i, j] == null)
                    {
                        Console.WriteLine("Error in reading puzzle file. Exiting...");
                        return;
                    }

                    if (Tiles[i, j].Target)
                    {
                        _numTargets++;
                    }

                    if (Tiles[i, j].State == Occpr.HUMAN)
                    {
                        _currPos = new Tuple<int, int>(i, j);
                    }
                }
            }
        }

        public int NumCols()
        {
            return _numCols;
        }

        public int NumRows()
        {
            return _numRows;
        }

        public int NumTargets()
        {
            return _numTargets;
        }

        public Dir CurrDir()
        {
            return _currDir;
        }

        public Tuple<int, int> CurrPos()
        {
            return _currPos;
        }


        private Tuple<int, int> getNewPos(Dir dir, Tuple<int, int> pos)
        {

            Tuple<int, int> candidate = null;
            switch (dir)
            {
                case (Dir.UP):
                    candidate = new Tuple<int, int>(pos.Item1 - 1, pos.Item2);
                    if (candidate.Item1 < 0)
                    {
                        return null;
                    }

                    break;
                case (Dir.RIGHT):
                    candidate = new Tuple<int, int>(pos.Item1, pos.Item2 + 1);
                    if (candidate.Item2 >= _numCols)
                    {
                        return null;
                    }

                    break;
                case (Dir.DOWN):
                    candidate = new Tuple<int, int>(pos.Item1 + 1, pos.Item2);
                    if (candidate.Item1 >= _numRows)
                    {
                        return null;
                    }

                    break;
                case (Dir.LEFT):
                    candidate = new Tuple<int, int>(pos.Item1, pos.Item2 - 1);
                    if (candidate.Item2 < 0)
                    {
                        return null;
                    }

                    break;
            }
            return candidate;

        }

        private MoveCode validMove(Dir dir, Tuple<int, int> pos)
        {
            bool human;
            if (pos == null)
            {
                human = true;
                pos = _currPos;
            }
            else
            {
                human = false;
            }

            Tile newTile;

            Tuple<int, int> candidate = getNewPos(dir, pos);
            if (candidate == null)
            {
                return MoveCode.ERROR;
            }

            newTile = Tiles[candidate.Item1, candidate.Item2];
            
            switch (newTile.State)
            {
                case (Occpr.CRATE):
                    if (human)
                    {
                        MoveCode crateMove = validMove(dir, candidate);
                        if (crateMove == MoveCode.FREE)
                        {
                            return MoveCode.CRATE;
                        }
                        else if (crateMove == MoveCode.ERROR)
                            return MoveCode.ERROR;
                        else
                            return MoveCode.NOOP;
                    }
                    else
                    {
                        return MoveCode.NOOP;
                    }
                case (Occpr.WALL):
                    return MoveCode.NOOP;
                case (Occpr.VOID):
                    return MoveCode.ERROR;
                case (Occpr.HUMAN):
                    return MoveCode.ERROR;
                case (Occpr.EMPTY):
                    return MoveCode.FREE;
                default:
                    return MoveCode.ERROR;
            }
        }

        public void moveCrate(Tuple<int, int> oldPos, Tuple<int, int> newPos)
        {
            Tiles[newPos.Item1, newPos.Item2].State = Occpr.CRATE;
            if (Tiles[oldPos.Item1, oldPos.Item2].Target)
            {
                _numTargets++;
            }
            if (Tiles[newPos.Item1, newPos.Item2].Target)
            {
                _numTargets--;
            }
        }

        public void moveHuman(Tuple<int, int> oldPos, Tuple<int, int> newPos)
        {
            Tiles[oldPos.Item1, oldPos.Item2].State = Occpr.EMPTY;
            Tiles[newPos.Item1, newPos.Item2].State = Occpr.HUMAN;
            _currPos = newPos;
        }

        public MoveCode move(Dir dir)
        {
            _currDir = dir;

            Tuple<int, int> newPos = getNewPos(dir, _currPos);

            MoveCode moveResult = validMove(dir, null);

            switch (moveResult)
            {
                case (MoveCode.NOOP):
                    break;
                case (MoveCode.FREE):
                    moveHuman(_currPos, newPos);
                    break;
                case (MoveCode.CRATE):
                    moveHuman(_currPos, newPos);
                    Tuple<int, int> newCratePos = getNewPos(dir, newPos);
                    moveCrate(newPos, newCratePos);
                    break;
                default:
                    //handle error here
                    break;
            }
            return moveResult;
        }
    }
}
