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
            Application.Current.Properties["GameStatus"] = "Startup";
        }

        void OnBtnLoadPuzzle(object sender, EventArgs args)
        {
            Application.Current.Properties["GameStatus"] = "Playing";

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
                            Source = ImageSource.FromResource(strFileName),
                            StyleId = r.ToString() + c.ToString()
                        };

                        lstImageFiles.Add(imgArrayImage);
                    }
                }
            }

            Random rndm = new Random();
            int intRandomNumber = -1;
            int intTileToSkip = rndm.Next(0, 16);

            ContentView cvContentView = new ContentView();

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

            statusLabel.Text = "Click a tile to move it.";

            lstImageFiles = null;
            cvContentView = null;


        }

        void OnCVTapped(object sender, EventArgs args)
        {
            // only do this routine if we are playing the game, otherwise display should tell us to load a picture
            if (Application.Current.Properties["GameStatus"].ToString() == "Playing")
            {
                bool booCanMove = false;
                ContentView cvSourceCV = (ContentView)sender;
                ContentView cvDestinationCV = null;

                /* *************************************************** **
                 * identify the blank cell number, row, and column
                 *  when found, set the destination content view as the blank one
                 * *************************************************** */
                int intBlankNumber = -1;
                int intBlankRow = -1;
                int intBlankColumn = -1;

                for (int i = 0; i < 16; i++)
                {
                    ContentView cvBlankCheck = (ContentView)grdPictureGrid.Children[i];
                    if (cvBlankCheck.Content == null)
                    {
                        intBlankNumber = i;
                        cvDestinationCV = cvBlankCheck;
                    }
                }

                intBlankColumn = intBlankNumber % 4;
                intBlankRow = intBlankNumber / 4;

                /* *************************************************** **
                 * Identify the tile that was clicked, then it's column and row.
                 * Then check whether it is eligible to move.
                 * *************************************************** */
                string strName = (string)cvSourceCV.StyleId;

                int intSelectedRow = -1;
                int intSelectedColumn = -1;

                // a non-blank cell was clicked
                if (cvSourceCV.Content != null)
                {
                    // get the row and column for the tile clicked
                    intSelectedColumn = Convert.ToInt16(strName.Substring(3, 1));
                    intSelectedRow = Convert.ToInt16(strName.Substring(2, 1));

                    // check if it can move into the blank
                    if ((intBlankRow == intSelectedRow && Math.Abs(intBlankColumn - intSelectedColumn) == 1)   // tile is in same row, next column to blank
                        || (intBlankColumn == intSelectedColumn && Math.Abs(intBlankRow - intSelectedRow) == 1)) // tile is in same column, next row to blank
                    {
                        booCanMove = true;
                    }
                    else
                    {
                        statusLabel.Text = "You can't move that piece! Pick one next to the blank space.";
                    }
                }
                // the blank cell was clicked
                else
                {
                    statusLabel.Text = "Click a tile that can move into the blank space.";
                }

                /* ************************************************** **
                 * If you can move it, then do so by swapping the image
                 *  with the blank space.
                 * ************************************************** */
                if (booCanMove)
                {
                    // Here's the image I'm going to move
                    Image imgMoveMe = (Image)cvSourceCV.Content;

                    // move it
                    cvSourceCV.Content = null;  // ctile ell we move FROM becomes blank
                    cvDestinationCV.Content = imgMoveMe;    // tile we move TO gets the image that was in the source

                    CheckIfDone();

                }
            }
            else
            {
                statusLabel.Text = "Click the button to begin.";
            }

            
        }

        void CheckIfDone()
        {
            bool booAllDone = true;

            /* **************************************** **
             *  Check if we solved it
             * **************************************** */
            
            for (int tile=0; tile < 16; tile++)
            {
                ContentView cvTileToCheck = (ContentView)grdPictureGrid.Children[tile];

                // only check image if we are NOT doing tile #
                if (tile != 12)
                {
                    // don't try to check if we have no image
                    if (cvTileToCheck.Content != null)
                    {
                        Image imgPicToCheck = (Image)cvTileToCheck.Content;

                        // set a column and row name for the tile (cv) and the image it contains
                        string cvRow = cvTileToCheck.StyleId.Substring(2, 1);
                        string cvColumn = cvTileToCheck.StyleId.Substring(3, 1);
                        string imgRow = imgPicToCheck.StyleId.Substring(0, 1);
                        string imgColumn = imgPicToCheck.StyleId.Substring(1, 1);

                        if ((cvRow != imgRow) || (cvColumn != imgColumn))
                        {
                            booAllDone = false;
                        }
                    }
                    // else our image is null (and outer loop means it's not #12)
                    else
                    {
                        booAllDone = false;
                    }
                }
                // otherwise, for tile #12 make sure there is no content/image
                else
                {
                    if(cvTileToCheck.Content != null) { booAllDone = false; }
                }
            }

            if (booAllDone)
            {
                statusLabel.Text = "YAY, YOU WON!";
                Application.Current.Properties["GameStatus"] = "Done";
            }
            else
            {
                statusLabel.Text = "Not done yet. Keep trying.";
            }
        }
    }
}
