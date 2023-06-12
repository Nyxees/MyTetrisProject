using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TetrisProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //-------- arrey with tile imagges
        private readonly ImageSource[] tileImages = new ImageSource[]
        {
            new BitmapImage(new Uri("TetrisAssets/TileEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("TetrisAssets/LightBlue.png", UriKind.Relative)),
            new BitmapImage(new Uri("TetrisAssets/Orange.png", UriKind.Relative)),
            new BitmapImage(new Uri("TetrisAssets/Blue.png", UriKind.Relative)),
            new BitmapImage(new Uri("TetrisAssets/Red.png", UriKind.Relative)),
            new BitmapImage(new Uri("TetrisAssets/Yellow.png", UriKind.Relative)),
            new BitmapImage(new Uri("TetrisAssets/Green.png", UriKind.Relative)),
            new BitmapImage(new Uri("TetrisAssets/Purple.png", UriKind.Relative)),
        };

        //-------- same arrey with block images
        private readonly ImageSource[] blockImages = new ImageSource[]
        {
            new BitmapImage(new Uri("TetrisAssets/BlockEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("TetrisAssets/I.png", UriKind.Relative)),
            new BitmapImage(new Uri("TetrisAssets/L.png", UriKind.Relative)),
            new BitmapImage(new Uri("TetrisAssets/J.png", UriKind.Relative)),
            new BitmapImage(new Uri("TetrisAssets/Z.png", UriKind.Relative)),
            new BitmapImage(new Uri("TetrisAssets/O.png", UriKind.Relative)),
            new BitmapImage(new Uri("TetrisAssets/S.png", UriKind.Relative)),
            new BitmapImage(new Uri("TetrisAssets/T.png", UriKind.Relative)),
        };

        //-------- 2 dimensional arrey to control images
        private readonly Image[,] imageControls;

        //--------Controling delay---------//
        private readonly int maxDelay = 900;
        private readonly int minDelay = 90;
        private readonly int decreaseDelay = 30;

        private TetrisState tetrisState = new TetrisState();

        public MainWindow()
        {
            InitializeComponent();
            imageControls = SetupCanvas(tetrisState.Grid);
        }

        //-------- correct control in canvas
        private Image[,] SetupCanvas(Grid grid)
        {
            Image[,] imageControls = new Image[grid.Rows, grid.Columns];
            //--------- veriable for cell siize
            int cellSize = 25; //-- couse canvas width = 250, height = 500

            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    Image imageControl = new Image
                    {
                        Width = cellSize,
                        Height = cellSize
                    };

                    Canvas.SetTop(imageControl, (r - 2) * cellSize + 10); //------ 10 - number of pixels to see which block killed us
                    Canvas.SetLeft(imageControl, c * cellSize);
                    GameCanvas.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;
                }
            }

            return imageControls;
        }

        //--------- draw game grid
        private void DrawGrid(Grid grid)
        {
            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    int Id = grid[r, c];
                    imageControls[r, c].Opacity = 1;
                    imageControls[r, c].Source = tileImages[Id];
                }
            }
        }

        //--------- draw block
        private void DrawBlock(Block block)
        {
            foreach (Position p in block.TilePositions())
            {
                imageControls[p.Row, p.Column].Opacity = 1;
                imageControls[p.Row, p.Column].Source = tileImages[block.Id];
            }
        }

        //--------- next block
        private void DrawNext(Queue queue)
        {
            Block next = queue.NextBlock;
            NextImage.Source = blockImages[next.Id];
        }

        //------ draw hold block
        public void DrawHold(Block HoldBlock)
        {
            if(HoldBlock == null)
            {
                HoldImage.Source = blockImages[0];
            }
            else
            {
                HoldImage.Source = blockImages[HoldBlock.Id];
            }
        }

        //------------- Ghost block to see block drop position
        private void GhostBlock(Block block)
        {
            int dropDistance = tetrisState.BlockDistance();

            foreach (Position p in block.TilePositions())
            {
                imageControls[p.Row + dropDistance, p.Column].Opacity = 0.30; //---- need to reset opacity when drawing grid and current block in DrawGrid/Block method
                imageControls[p.Row + dropDistance, p.Column].Source = tileImages[block.Id];
            }
        }

        //-------------Method to draw grid and current block + next block + score. Call when canvas loaded
        private void DrawBoth(TetrisState tetrisState)
        {
            DrawGrid(tetrisState.Grid);
            GhostBlock(tetrisState.CurrentBlock);//----- need to be called before DrawBlock
            DrawBlock(tetrisState.CurrentBlock);
            DrawNext(tetrisState.Queue);
            DrawHold(tetrisState.HoldBlock);
            Score.Text = $"Score: {tetrisState.Score}";
        }

        //------- loop for tetris start to work + score gameover menu
        private async Task GameLoop()
        {
            DrawBoth(tetrisState);

            while (!tetrisState.GameOver)
            {
                int delay = Math.Max(minDelay, maxDelay - (tetrisState.Score * decreaseDelay));//---------not necessary but give us some sort of hardcore
                await Task.Delay(650);
                tetrisState.MoveDown();
                DrawBoth(tetrisState);
            }

            GameOverMenu.Visibility = Visibility.Visible;
            FinalScore.Text = $"Score: {tetrisState.Score}";
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //------ pressing the key no work
            if (tetrisState.GameOver)
            {
                return;
            }
            //------ movement
            switch (e.Key)
            {
                case Key.Left:
                    tetrisState.MoveLeft();
                    break;
                case Key.Right:
                    tetrisState.MoveRight();
                    break;
                case Key.Down:
                    tetrisState.MoveDown();
                    break;
                case Key.Up:
                    tetrisState.RotateCW();
                    break;
                case Key.X:
                    tetrisState.Hold();
                    break;
                case Key.Z:
                    tetrisState.RotateCCW();
                    break;
                case Key.Space:
                    tetrisState.Drop();
                    break;
                default:
                    return;
            }

            DrawBoth(tetrisState);
        }

        private async void CanvasLoaded(object sender, RoutedEventArgs e)
        {
            await GameLoop();
        }

        //-------- hide gameover menu & restart game loop
        private async void PlayAgain(object sender, RoutedEventArgs e)
        {
            tetrisState = new TetrisState();
            GameOverMenu.Visibility = Visibility.Hidden;
            await GameLoop();
        }
    }
}
