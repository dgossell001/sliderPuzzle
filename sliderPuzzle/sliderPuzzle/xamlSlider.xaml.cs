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
    }
}
