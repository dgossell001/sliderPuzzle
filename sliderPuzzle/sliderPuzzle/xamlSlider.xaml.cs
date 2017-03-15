using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace sliderPuzzle
{
    public partial class xamlSlider : ContentPage
    {
        public xamlSlider()
        {
            InitializeComponent();
        }

        void OnBtnLoadPuzzle(object sender, EventArgs args)
        {
            /* *************************************************** **
             * clear out any existing master image and load the new one
             * *************************************************** */
            cvMasterPic.Content = null;

            Image img = new Image
            {
                Source = ImageSource.FromResource("sliderPuzzle.Images.9-9.jpg"),
                BackgroundColor = Color.Aqua
            };

            cvMasterPic.Content = img;

            /* *************************************************** **
             * Load the names of the image files into a list
             * *************************************************** */
            // building file names which is based on 4x4 row-column
            List<Image> lstImageFiles = new List<Image>();
            string strFileName = "";
            
            for(int r=0; r<4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    // going to skip 3-0.jpeg, that's the lower left square
                    if (r != 3 | c != 0)
                    {
                        strFileName = "sliderPuzzle.Images." + r.ToString() + "-" + c.ToString() + ".jpeg";

                        Image imgArrayImage = new Image
                        {
                            Source = ImageSource.FromResource(strFileName)
                        };

                        lstImageFiles.Add(imgArrayImage);
                    }
                }
            }

            Random rndm = new Random();
            int intRandomNumber = -1;
            int intTileToSkip = rndm.Next(0, 16);

            ContentView cvContentView = new ContentView();

            string strStatus = lstImageFiles.Count().ToString() + "\n" + statusLabel.Text;
            for (int i = 0; i < 16; i++)
            {
                cvContentView = (ContentView)grdPictureGrid.Children[i];
                
                //skipping one randomly chose tile which will remain blank
                if (i != intTileToSkip)
                {
                    intRandomNumber = rndm.Next(0, lstImageFiles.Count);

                    cvContentView.Content = lstImageFiles[intRandomNumber];
                    lstImageFiles.Remove(lstImageFiles[intRandomNumber]);
                }
                else
                {
                    cvContentView.Content = null;
                }
            }

            statusLabel.Text = strStatus;

            lstImageFiles = null;
            cvContentView = null;


        }

        void OnCVTapped(object sender, EventArgs args)
        {
            // identify the blank cell number, row, and column
            int intBlankNumber = -1;
            int intBlankRow = -1;
            int intBlankColumn = -1;

            for(int i=0; i<16; i++)
            {
                ContentView cvBlankCheck = (ContentView)grdPictureGrid.Children[i];
                if (cvBlankCheck.Content == null) { intBlankNumber = i; }
            }

            intBlankColumn = intBlankNumber % 4;
            intBlankRow = intBlankNumber / 4;

            statusLabel.Text = "blank column = " + intBlankColumn.ToString() + "\n" + statusLabel.Text;
            statusLabel.Text = "blank row = " + intBlankRow.ToString() + "\n" + statusLabel.Text;

            /* *************************************************** **
             * Identify the tile that was clicked, then it's column and row.
             * Then check whether it is eligible to move.
             * *************************************************** */
            ContentView cvTile = (ContentView)sender;
            string strName = (string)cvTile.StyleId;

            int intSelectedRow = -1;
            int intSelectedColumn = -1;

            // a non-blank cell was clicked
            if (cvTile.Content != null)
            {
                // get the row and column for the tile clicked
                statusLabel.Text = "CV location = " + strName+ "\n" + statusLabel.Text;
                intSelectedColumn = Convert.ToInt16(strName.Substring(3, 1));
                intSelectedRow = Convert.ToInt16(strName.Substring(2, 1));

                statusLabel.Text = "tile column = " + intSelectedColumn.ToString() + "\n" + statusLabel.Text;
                statusLabel.Text = "tile row = " + intSelectedRow.ToString() + "\n" + statusLabel.Text;

                // check if it can move into the blank
                if((intBlankRow == intSelectedRow && Math.Abs(intBlankColumn - intSelectedColumn) == 1)   // tile is in same row, next column to blank
                    || (intBlankColumn == intSelectedColumn && Math.Abs(intBlankRow - intSelectedRow) == 1)) // tile is in same column, next row to blank
                {
                    statusLabel.Text = "Swap them!\n" + statusLabel.Text;
                }
                else
                {
                    statusLabel.Text = "CANNOT swap them!!!!!\n" + statusLabel.Text;
                }
            }
            // the blank cell was clicked
            else
            {
                statusLabel.Text = "Click a tile that can move into the blank space.";
            }
            
        }
    }
}
