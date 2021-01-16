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
    /// Клетка минного поля
    /// </summary>
    public partial class Cell : UserControl
    {

        /// <summary>
        /// Карта к которой принадлежит данная клетка
        /// </summary>
        Map MainMap;

        /// <summary>
        /// Свойство, определяющее содержит ли клетка мину или нет
        /// </summary>
        public bool IsMine { get; set; }

        /// <summary>
        /// Координаты клетки
        /// </summary>
        public Position CellPosition { get; set; }

        /// <summary>
        /// Свойство, определяющее, открыта клетка или нет
        /// </summary>
        public bool Destroied { get; private set; } = false;

        /// <summary>
        /// Строка содержащая данные о содержании клетки, если "*" - клетка с миной
        /// </summary>
        public string CellStatus { get; set; } = "";

        /// <summary>
        /// Массив клеток находящихся рядом с данной
        /// </summary>
        public Cell[] CellsAround {
            get
            {
                Cell[] Cells = {
                MainMap.GetCell(new Position(CellPosition.x + 1, CellPosition.y)),
                MainMap.GetCell(new Position(CellPosition.x + 1, CellPosition.y + 1)),
                MainMap.GetCell(new Position(CellPosition.x, CellPosition.y + 1)),
                MainMap.GetCell(new Position(CellPosition.x - 1, CellPosition.y + 1)),
                MainMap.GetCell(new Position(CellPosition.x - 1, CellPosition.y)),
                MainMap.GetCell(new Position(CellPosition.x - 1, CellPosition.y - 1)),
                MainMap.GetCell(new Position(CellPosition.x, CellPosition.y - 1)),
                MainMap.GetCell(new Position(CellPosition.x + 1, CellPosition.y - 1)),
                };
                return Cells;
            }
        }
        
        
        public Cell(Map Parent, Position Position)
        {
            InitializeComponent();
            CellPosition = Position;
            this.MainMap = Parent;
            
            
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            if (MainMap.IsFirstClick)
                MainMap.SpawnMines(this);
            if (MainMap.IsWin == null) 
                Destroy(this);
            if(MainMap.DestroiedCount == MainMap.CellData.Length - MainMap.MinesCount)
                MainMap.Win();  
        }

        /// <summary>
        /// Метод вскрывающий клетку
        /// </summary>
        /// <param name="cell">Клетка для открытия</param>
        private void Destroy(Cell cell)
        {
            switch (cell.CellStatus)
            {
                case "1":
                    cell.CellText.Foreground = Brushes.Blue;
                    break;
                case "2":
                    cell.CellText.Foreground = Brushes.Green;
                    break;
                case "3":
                    cell.CellText.Foreground = Brushes.DarkRed;
                    break;
                case "4":
                    cell.CellText.Foreground = Brushes.DarkBlue;
                    break;
                case "5":
                    cell.CellText.Foreground = Brushes.Brown;
                    break;
                case "6":
                    cell.CellText.Foreground = Brushes.DarkBlue;
                    break;
                case "7":
                    cell.CellText.Foreground = Brushes.DarkGreen;
                    break;
                case "8":
                    cell.CellText.Foreground = Brushes.Black;
                    break;
                case "*":
                    cell.CellImg.Source = cell.ExplodedMineImg.Source;
                    MainMap.GameOver();
                    return;
                default:
                    break;
            }
            cell.CellGrid.Children.Remove(cell.CellButton);
            MainMap.DestroiedCount++;
            cell.Destroied = true;
            if (cell.CellText.Text == "")
            {
                for(int i = 0; i < cell.CellsAround.Length; i++)
                {
                    if (cell.CellsAround[i] != null && !cell.CellsAround[i].Destroied)
                    {
                        Destroy(cell.CellsAround[i]);
                    }
                }
            }
        }
    }
    /// <summary>
    /// Класс содержащий позицию клетки x и y
    /// </summary>
    public class Position
    {
        /// <summary>
        /// X координата
        /// </summary>
        public int x { get; set; }
        /// <summary>
        /// Y координата
        /// </summary>
        public int y { get; set; }
        public  Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
