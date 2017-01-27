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
    public class LevelDesigner : StateBase
    {
        PuzzleGrid _grid;
        int _rows, _cols;
        int _saveButtonsSizeY = 50;
        int _saveButtonsSizeX = 100;
        int _saveButtonsSpaceX = 35;

        Tile _cursorPaint = new Tile(false, Occpr.VOID);

        XNAForm _designForm;
        XNAForm _designGridForm;
        XNAForm _toolbar;

        int _toolbarButtonSize = 70;
        int _gridTileSize = 50;

        public LevelDesigner(int rows, int cols, GameMgr gameMgr) : base(gameMgr)
        {
            _grid = new Sokoban.PuzzleGrid(rows, cols);

            _rows = rows;
            _cols = cols;

            //forms = new List<XNAForm>();

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    _grid[row, col] = new Sokoban.Tile(false, Occpr.VOID);
                }
            }

            _makeDesignForm();
            _makeDesignGrid();
            _makeToolbar();
            _makeButtons();
        }

        public void CellButtonClicked(object sender, ButtonEventArgs args)
        {
            Button senderButton = sender as Button;
            senderButton.BackgroundTexture = Utilities.CursorTexture;

            int row = senderButton.Y / _gridTileSize;
            int col = senderButton.X / _gridTileSize;

            _grid[row, col] = new Tile(_cursorPaint.Target, _cursorPaint.State);
        }

        public void ToolbarButtonClicked(object sender, ButtonEventArgs args)
        {
            var senderButton = sender as Button;
            Utilities.CursorTexture = senderButton.BackgroundTexture;

            int senderXCatg = senderButton.X / _toolbarButtonSize;

            switch (senderXCatg)
            {
                case (0):
                    _cursorPaint = new Tile(false, Occpr.WALL);
                    break;
                case (1):
                    _cursorPaint = new Tile(false, Occpr.EMPTY);
                    break;
                case (2):
                    _cursorPaint = new Tile(false, Occpr.CRATE);
                    break;
                case (3):
                    _cursorPaint = new Tile(true, Occpr.EMPTY);
                    break;
            }
        }

        public void SaveButtonClicked(object sender, ButtonEventArgs args)
        {
            List<Tile[,]> allSaved = PuzzleGrid.DeSerializeAll("Puzzles");

            Console.WriteLine("in savebuttonclicked");
            Console.WriteLine("allsaved length: " + allSaved.Count.ToString());

            foreach (Tile [,] tileArr in allSaved)
            {
                if (PuzzleGrid.TilesEqual(tileArr, _grid.Tiles))
                {
                    Console.WriteLine("Making error dialog...");
                    PopupDialog dialog = new PopupDialog("Cannot save puzzle: Already exists", "Error", true, this);
                    Button.ButtonClickCallback callbackFunc = (senderArg, eventArgs) => _gameMgr.DestroyForm(dialog, senderArg, eventArgs);
                    dialog.AddButton(0, 0, _saveButtonsSizeX, _saveButtonsSizeY, callbackFunc, "OK");
                    _gameMgr.centerFormX(dialog);
                    _gameMgr.centerFormY(dialog);
                    forms.Add(dialog);
                    Console.WriteLine("Exiting savebuttondialog....");
                    return;
                }
            }

            Console.WriteLine("Serializing...");

            _grid.Serialize();

            Console.WriteLine("Making success dialog....");


            PopupDialog savedDialog = new PopupDialog("Puzzle successfully saved", "Success", true, this);
            Button.ButtonClickCallback callback = (a, b) => GameMgr.DestroyForm(savedDialog, a, b);
            savedDialog.AddButton(0, 0, _saveButtonsSizeX, _saveButtonsSizeY, callback, "Continue designing");
            savedDialog.AddButton(0, 0, _saveButtonsSizeX, _saveButtonsSizeY, _gameMgr.MainMenuCallback, "Exit to main menu");
            _gameMgr.centerFormX(savedDialog);
            _gameMgr.centerFormY(savedDialog);
            forms.Add(savedDialog);

            Console.WriteLine("Exiting from Savebuttondialog....");
        }


        private void _makeDesignForm()
        {
            _designForm = new Sokoban.XNAForm(0, 0, _gameMgr.ScreenWidth, _gameMgr.ScreenHeight, this, "", false);
        }

        private void _makeToolbar()
        {
            List<string> textureList = new List<string>();
            textureList.Add("BrickWall");
            textureList.Add("Empty");
            textureList.Add("Crate");
            textureList.Add("Target");
            _toolbar = new XNAForm(0, 0, _toolbarButtonSize * textureList.Count, _toolbarButtonSize, _designForm, "m", false);
            _addToolbarButtons(textureList);

            _designForm.AddForm(_toolbar);
        }

        private void _addToolbarButtons(List<string> textures)
        {
            for (int i = 0; i < textures.Count; i++)
            {
                Button newButton = new Button("", i * _toolbarButtonSize, 0, _toolbarButtonSize, _toolbarButtonSize, ToolbarButtonClicked, _toolbar);
                newButton.newBackground(_gameMgr.Content.Load<Texture2D>(textures[i]));
                newButton.newActiveColor(Color.Gray);
                newButton.newInactiveColor(Color.White);
                _toolbar.AddButton(newButton);
            }
        }

        private void _makeDesignGrid()
        {
            _designGridForm = new Sokoban.XNAForm(100, 100, _cols * _gridTileSize, _rows * _gridTileSize, _designForm, "", false);
            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _cols; col++)
                {
                    Button newButton = new Sokoban.Button("", col * _gridTileSize, row * _gridTileSize, _gridTileSize, _gridTileSize, 
                                                            CellButtonClicked, _designGridForm);
                    newButton.newActiveColor(Color.Gray);
                    newButton.newInactiveColor(Color.White);

                    newButton.MouseButtonReleaseEvent = false;
                    _designGridForm.AddButton(newButton);
                }
            }

            _designForm.AddForm(_designGridForm);
        }

        private void _makeButtons()
        {
            int bottomY = _designGridForm.Y + _designGridForm.Height;

            int buttonsY = (_designForm.Height - bottomY - _saveButtonsSizeY) / 2 + bottomY;

            int buttonsStartX = (_designGridForm.Width - _saveButtonsSpaceX - 2 * _saveButtonsSizeX)/2 + _designGridForm.X;

            _designForm.AddButton(buttonsStartX, buttonsY, _saveButtonsSizeX, _saveButtonsSizeY, SaveButtonClicked, "Save");
            _designForm.AddButton(buttonsStartX + _saveButtonsSizeX + _saveButtonsSpaceX, buttonsY, _saveButtonsSizeX, _saveButtonsSizeY,
                                    _gameMgr.MainMenuCallback, "Cancel");
        }

        public override void Update(GameTime gameTime)
        {
            _designForm.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _designForm.Draw(gameTime);

            base.Draw(gameTime);
        }

        protected override void ImportTextures()
        {
        }
    }
}
