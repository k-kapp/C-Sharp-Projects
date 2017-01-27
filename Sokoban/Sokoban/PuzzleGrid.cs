using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

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

    public enum FileFormat
    {
        TXT,
        DAT
    }

    [Serializable] class Tile : ICloneable
    {
        public Occpr State;
        public bool Target;
        public Tile(bool target, Occpr state)
        {
            this.Target = target;
            this.State = state;

        }

        public object Clone()
        {
            return new Tile(Target, State);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            Tile tileObj = obj as Tile;
            if (tileObj == null)
            {
                return false;
            }

            if (tileObj.State != State)
            {
                if (tileObj.State == Occpr.HUMAN || State == Occpr.HUMAN)
                {
                    if (tileObj.Target == Target)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
                return true;

        }

        public override int GetHashCode()
        {
            return (int)State + 5 * Convert.ToInt16(Target);
        }

    }

    class PuzzleGrid
    {
        string _targetDir = "Puzzles";

        private Tile [,] _tiles;
        private Tile [,] _origArr;
        int _numRows, _numCols;
        int _numTargets;
        Tuple<int, int> _currPos;
        Dir _currDir = Dir.RIGHT;

        string _lastImportedGridFilepath;

        public PuzzleGrid(int numRows, int numCols)
        {
            _tiles = new Tile[numRows, numCols];
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


        public PuzzleGrid(int targetNum = 0)
        {
            Tiles = DeSerialize(_getGridFilepath(targetNum));
            _createBackup(Tiles);
            _initVars();
        }

        public PuzzleGrid(string inputFile, FileFormat format)
        {
            if (format == FileFormat.TXT)
                _importStringFile(inputFile);
            else if (format == FileFormat.DAT)
                Tiles = DeSerialize(inputFile);
            _createBackup(Tiles);
            _initVars();
        }

        public Tile this[int row, int col]
        {
            set
            {
                Tiles[row, col] = value;
            }

            get
            {
                return Tiles[row, col];
            }
        }

        public void Reset()
        {
            Tiles = _copyArr(_origArr);
            _initVars();
            _currDir = Dir.RIGHT;
        }

        private Tile[,] _copyArr(Tile[,] arr)
        {
            Tile[,] arrReturn = new Tile[arr.GetLength(0), arr.GetLength(1)];
            for (int row = 0; row < arr.GetLength(0); row++)
            {
                for (int col = 0; col < arr.GetLength(1); col++)
                {
                    arrReturn[row, col] = arr[row, col].Clone() as Tile;
                }
            }

            return arrReturn;
        }

        private void _createBackup(Tile[,] arr)
        {
            _origArr = _copyArr(arr);
        }

        private void _assignTiles(Tile[,] arr, string filepath)
        {
            _tiles = arr;
        }

        public static bool TilesEqual(Tile[,] arr1, Tile[,] arr2)
        {
            if (arr1.GetLength(0) != arr2.GetLength(0))
            {
                return false;
            }

            if (arr1.GetLength(1) != arr2.GetLength(1))
            {
                return false;
            }

            for (int row = 0; row < arr1.GetLength(0); row++)
            {
                for (int col = 0; col < arr1.GetLength(1); col++)
                {
                    if (!arr1[row, col].Equals(arr2[row, col]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void _initVars()
        {
            _countTargets();
            _setCurrPos();
            _numRows = _tiles.GetLength(0);
            _numCols = _tiles.GetLength(1);
        }

        public Tile[,] Tiles
        {
            set
            {
                _tiles = value;
            }

            get
            {
                return _tiles;
            }
        }

        private void _importStringFile(string inputFilename)
        {
            string[] allLines = System.IO.File.ReadAllLines(inputFilename);

            int numRows = allLines.Length;
            int numCols = allLines[0].Split(null).Length;

            _tiles = new Tile[numRows, numCols];

            for (int i = 0; i < allLines.Length; i++)
            {
                string[] rowElements = allLines[i].Split(null);

                for (int j = 0; j < rowElements.Length; j++)
                {
                    _tiles[i, j] = MakeTile(rowElements[j][0]);

                    if (_tiles[i, j] == null)
                    {
                        Console.WriteLine("Error in reading puzzle file. Exiting...");
                        return;
                    }
                }
            }

        }

        private void _countTargets()
        {
            _numTargets = 0;
            for (int i = 0; i < _tiles.GetLength(0); i++)
            {
                for (int j = 0; j < _tiles.GetLength(1); j++)
                {
                    if (_tiles[i, j].Target)
                    {
                        _numTargets++;
                    }
                }
            }
        }

        private void _setCurrPos()
        {
            for (int i = 0; i < _tiles.GetLength(0); i++)
            {
                for (int j = 0; j < _tiles.GetLength(1); j++)
                {
                    if (_tiles[i, j].State == Occpr.HUMAN)
                    {
                        _currPos = new Tuple<int, int>(i, j);
                    }
                }
            }
        }


        public static int GetFileNum(string filename)
        {
            return Convert.ToInt32(Path.GetFileNameWithoutExtension(filename).Substring("Puzzle".Length));
        }

        public static string[] getPuzzleFilenames(string targetDir)
        {
            string currDir = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            string[] allDirs = Directory.GetDirectories(currDir);
            if (targetDir[0] != '\\')
            {
                targetDir = '\\' + targetDir;
            }
            targetDir = currDir + targetDir;
        
            var dirObj = Directory.CreateDirectory(targetDir);

            return Directory.GetFiles(targetDir);

        }

        public void Serialize()
        {
            string[] allFiles = getPuzzleFilenames(_targetDir);

            int highest = 0;

            foreach(string filename in allFiles)
            {
                int num = GetFileNum(filename);
                if (num > highest)
                {
                    highest = num;
                }
            }

            highest += 1;

            string targetFile = _targetDir + '\\' + "Puzzle" + highest.ToString() + ".dat";

            FileStream fs = new FileStream(targetFile, FileMode.Create);

            BinaryFormatter binaryOut = new BinaryFormatter();
            try
            {
                binaryOut.Serialize(fs, _tiles);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Serialization unsuccessful: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
            
        }

        public bool currTileGridExists()
        {
            List<Tile[,]> allGrids = DeSerializeAll(_targetDir);

            foreach(var tilegrid in allGrids)
            {
                if (_tiles.Equals(tilegrid))
                    return true;
            }

            return false;
        }

        public static List<Tile[,]> DeSerializeAll(string targetDir)
        {
            List<Tile[,]> returnList = new List<Tile[,]>();

            string[] filepaths = getPuzzleFilenames(targetDir);

            foreach(var filename in filepaths)
            {
                Tile[,] temp = DeSerialize(filename);
                returnList.Add(temp);
                
            }

            return returnList;
        }

        public static Tile[,] DeSerialize(string filename)
        {
            Tile[,] returnVal;

            FileStream fs = new FileStream(filename, FileMode.Open);

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                returnVal = (Tile[,])formatter.Deserialize(fs);

            }
            catch (SerializationException e)
            {
                Console.WriteLine("Deserialization unsuccessful: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }

            return returnVal;

        }


        private string _getGridFilepath(int targetNumber)
        {
            string targetDir = "Puzzles";
            string currDir = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

            if (targetDir[0] != '\\')
            {
                targetDir = '\\' + targetDir;
            }

            Console.WriteLine("_targetDir before merge: " + targetDir.ToString());

            targetDir = currDir + targetDir;

            Console.WriteLine("_targetDir after merge: " + targetDir.ToString());

            var dirObj = Directory.CreateDirectory(targetDir);

            string[] allFiles = Directory.GetFiles(targetDir);

            List<int> numbers = new List<int>();

            foreach(string filename in allFiles)
            {
                string ext = Path.GetExtension(filename);
                Console.WriteLine("Ext: " + ext.ToString());
                if (ext == ".dat")
                {
                    int num = GetFileNum(filename);
                    numbers.Add(num);
                }
            }
            if (numbers.Count == 0)
            {
                throw new FileNotFoundException("No puzzle files in directory");
            }

            numbers.Sort();

            string targetFilename = string.Empty;

            for (int i = 0; i < numbers.Count; i++)
            {
                if (numbers[i] >= targetNumber)
                {
                    targetNumber = numbers[i];
                    break;
                }
                if (i == numbers.Count - 1)
                {
                    targetNumber = numbers[numbers.Count - 1];
                }
            }

            return targetDir + '\\' + "Puzzle" + targetNumber.ToString() + ".dat";
        }

        /*
        public static Tile[,] DeSerialize(int targetNumber)
        {
            Tile[,] returnVal;

            _setGridFilepath(targetNumber);

            return DeSerialize(GridFilepath);

        }
        */

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

            newTile = _tiles[candidate.Item1, candidate.Item2];
            
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
            _tiles[newPos.Item1, newPos.Item2].State = Occpr.CRATE;
            if (_tiles[oldPos.Item1, oldPos.Item2].Target)
            {
                _numTargets++;
            }
            if (_tiles[newPos.Item1, newPos.Item2].Target)
            {
                _numTargets--;
            }
        }

        public void moveHuman(Tuple<int, int> oldPos, Tuple<int, int> newPos)
        {
            _tiles[oldPos.Item1, oldPos.Item2].State = Occpr.EMPTY;
            _tiles[newPos.Item1, newPos.Item2].State = Occpr.HUMAN;
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
