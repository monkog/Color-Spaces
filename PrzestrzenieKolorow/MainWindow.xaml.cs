using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
using Color = System.Drawing.Color;

namespace PrzestrzenieKolorow
{
    /// <summary>
    /// Author: Monika Kogut
    /// This demo converts the color space of a picture.
    /// Most of the information was provided from the articles:
    /// http://www.babelcolor.com/download/A%20comparison%20of%20four%20multimedia%20RGB%20spaces.pdf
    /// http://www.babelcolor.com/download/A%20review%20of%20RGB%20color%20spaces.pdf
    /// and the site:
    /// http://www.brucelindbloom.com/index.html?Eqn_RGB_XYZ_Matrix.html
    /// </summary>
    public partial class MainWindow : Window
    {
        Bitmap m_sourceBitmap;
        ImageBrush m_whiteSmokeBitmap;

        public MainWindow()
        {
            InitializeComponent();
            m_sourceBitmap = null;
            Bitmap grayBitmap = new Bitmap(1, 1);
            grayBitmap.SetPixel(0, 0, Color.WhiteSmoke);
            m_whiteSmokeBitmap = createImageBrushFromBitmap(grayBitmap);
        }

        private void m_openButton_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            openFileDialog.Filter = "all image files(*.bmp; *.gif; *.jpeg; *.jpg; *.png)|*.bmp;*.gif; *.jpeg; *.jpg; *.png"
                + "|BMP Files (*.bmp)|*.bmp|GIF Files (*.gif)|*.gif|JPEG Files (*.jpeg)|*.jpeg|JPG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = openFileDialog.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                string fileName = openFileDialog.FileName;
                ImageBrush imageBrush = new ImageBrush();
                BitmapImage bitmapImage = new BitmapImage(new Uri(fileName));
                imageBrush.ImageSource = bitmapImage;
                m_sourcePhoto.Background = imageBrush;
                m_sourceBitmap = createBitmapFromBitmapImage(bitmapImage);
                m_outputPhoto.Background = m_whiteSmokeBitmap;
            }
        }

        private void m_saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (m_sourceBitmap == null || m_outputPhoto.Background == m_whiteSmokeBitmap)
                return;

            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "BMP Files (*.bmp)|*.bmp|GIF Files (*.gif)|*.gif|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png";
            Nullable<bool> result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                string fileName = saveFileDialog.FileName;
                string extension = Path.GetExtension(fileName);

                switch (extension)
                {
                    case ".bmp":
                        saveToBmp(m_outputPhoto, fileName);
                        break;
                    case ".gif":
                        saveToGif(m_outputPhoto, fileName);
                        break;
                    case ".jpeg":
                        saveToJpeg(m_outputPhoto, fileName);
                        break;
                    case ".png":
                        saveToPng(m_outputPhoto, fileName);
                        break;
                }
            }
        }

        private void m_convertToGrayScaleButton_Click(object sender, RoutedEventArgs e)
        {
            if (m_sourceBitmap != null)
                convertToGrayScale();
        }

        private void m_convertToAdobeButton_Click(object sender, RoutedEventArgs e)
        {
            if (m_sourceBitmap != null)
                convertToColorSpace("Adobe RGB");
        }

        private void m_convertToAppleButton_Click(object sender, RoutedEventArgs e)
        {
            if (m_sourceBitmap != null)
                convertToColorSpace("Apple RGB");
        }

        private void m_convertToWideGamutButton_Click(object sender, RoutedEventArgs e)
        {
            if (m_sourceBitmap != null)
                convertToColorSpace("Wide Gamut");
        }

        private void m_reduceButton_Click(object sender, RoutedEventArgs e)
        {
            if (m_sourceBitmap != null)
                reduceColors(Int32.Parse(m_kr.Text), Int32.Parse(m_kg.Text), Int32.Parse(m_kb.Text));
        }
    }
}
