using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Saper
{
    /// <summary>
    /// Минное поле
    /// </summary>
    public partial class Map : UserControl
    {
        /// <summary>
        /// Клетки содержащиеся в поле
        /// </summary>
        public Cell[,] CellData { get; set; }

        /// <summary>
        /// Количество мин в поле
        /// </summary>
        public int MinesCount { get; set; }

        /// <summary>
        /// Массив в котором содержатся клетки содержащие мины
        /// </summary>
        public Cell[] MinesCells { get; set; }
        /// <summary>
        /// Длинна поля по X
        /// </summary>
        public int XLength { get; set; } = 10;
        /// <summary>
        /// Длинна поля по Y
        /// </summary>
        public int YLength { get; set; } = 10;
        /// <summary>
        /// Поле определяющее был ли совершен первый клик или нет
        /// </summary>
        public bool IsFirstClick = true;

        /// <summary>
        /// Выйграл ли пользователь?   
        /// null - идет игра.
        /// true - победа.
        /// false - поражение.
        /// </summary>
        public bool? IsWin { get; set; } = null;

        /// <summary>
        /// Количество открытых клеток
        /// </summary>
        public int DestroiedCount { get; set; } = 0;
        public Map()
        {
            InitializeComponent();
            GenerateMap();
        }

        /// <summary>
        /// Создание колонок и заполнение клетками
        /// </summary>
        private void GenerateMap()
        {
            CellData = new Cell[XLength, YLength];
            for (int y = 0; y < YLength; y++)
                MapGrid.RowDefinitions.Add(new RowDefinition());
            for (int x = 0; x < XLength; x++)
                MapGrid.ColumnDefinitions.Add(new ColumnDefinition());

            for (int y = 0; y < YLength; y++)
            {
                for (int x = 0; x < XLength; x++)
                {
                    var currCell = new Cell(this, new Position(x, y));
                    MapGrid.Children.Add(currCell);
                    Grid.SetRow(currCell, y);
                    Grid.SetColumn(currCell, x);
                    CellData[x, y] = currCell;
                }
            }
        }
        /// <summary>
        /// Метод расставляющий мины
        /// </summary>
        /// <param name="ClickedCell">Первая клетка на которуб кликнул пользователь</param>
        public void SpawnMines(Cell ClickedCell)
        {
            MinesCells = new Cell[MinesCount];
            var rnd = new Random();
            int counter = 0;
            while (counter < MinesCount)
            {
                var currCell = CellData[rnd.Next(XLength), rnd.Next(YLength)];
                if (currCell.IsMine == false && currCell != ClickedCell)
                {
                    currCell.CellImg.Visibility = Visibility.Visible;
                    currCell.CellStatus = "*";
                    currCell.IsMine = true;
                    MinesCells[counter] = currCell;
                    counter++;
                }
            }
            IsFirstClick = false;

            foreach (var currCell in CellData)
            {
                if (currCell.IsMine == false)
                {
                    int MinesAround = 0;
                    foreach (var cell in currCell.CellsAround)
                    {
                        if (cell != null)
                            if (cell.IsMine)
                                MinesAround++;
                    }
                    if (MinesAround != 0)
                    {
                        currCell.CellText.Text = MinesAround.ToString();
                        currCell.CellStatus = MinesAround.ToString();
                    }

                }

            }

        }

        /// <summary>
        /// Метод получающий клетку из общего массива принимая ее позицию.
        /// </summary>
        /// <param name="Position">Позиция клетки</param>
        /// <returns>Клетка минного поля</returns>
        public Cell GetCell(Position Position)
        {
            if ((Position.x >= 0 && Position.x < XLength) && (Position.y >= 0 && Position.y < YLength))
                return CellData[Position.x, Position.y];
            else
                return null;
        }

        /// <summary>
        /// Метод вскрывающий все мины и обозначающий поражение
        /// </summary>
        public void GameOver()
        {
            foreach (var Mine in MinesCells)
            {
                Mine.CellGrid.Children.Remove(Mine.CellButton);
            }
            IsWin = false;
            var result = MessageBox.Show("Желаете повторить игру?", "Поражение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Restart();
            }
        }

        /// <summary>
        /// Метод перезапускающий игру
        /// </summary>
        public void Restart()
        {
            MapGrid.Children.Clear();
            MapGrid.ColumnDefinitions.Clear();
            MapGrid.RowDefinitions.Clear();
            CellData = null;
            MinesCells = null;
            IsFirstClick = true;
            DestroiedCount = 0;
            GenerateMap();
            IsWin = null;

        }

        public void Win()
        {
            foreach (var Mine in MinesCells)
            {
                Mine.CellGrid.Children.Remove(Mine.CellButton);
                Mine.CellImg.Source = Mine.DefusedMineImg.Source;
            }
            IsWin = true;
            var result = MessageBox.Show("Желаете повторить игру?", "Победа", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Restart();
            }
        }
    }
}