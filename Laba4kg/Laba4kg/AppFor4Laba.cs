using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Laba4kg
{
    public partial class AppFor4Laba : Form
    {
        public AppFor4Laba()
        {
            InitializeComponent();
        }
        //может возникать ошибка из-за неправильно указанного пути. перепроверьте его
        public static Bitmap inputImg = new Bitmap(@"E:\Contrast.jpg");
        public static Bitmap first = new Bitmap(inputImg.Width, inputImg.Height);


        private void buttonForStartPic_Click(object sender, EventArgs e)
        {
            pictureBoxForStartPic.Image = inputImg;
            pictureBoxForStartPic.SizeMode = PictureBoxSizeMode.Zoom;
            buttonForExercise1.Enabled = true;
        }

        private void buttonForExercise1_Click(object sender, EventArgs e)
        {
            int v;
            for (int i = 1; i < inputImg.Width; i++)
            {
                for (int j = 1; j < inputImg.Height; j++)
                {
                    v = Convert.ToInt32(0.3 * inputImg.GetPixel(i, j).R + 0.59 * inputImg.GetPixel(i, j).G + 0.11 * inputImg.GetPixel(i, j).B);
                    first.SetPixel(i, j, Color.FromArgb(v, v, v));
                }
            }
            pictureBoxFor1Exercise.Image = first;
            pictureBoxFor1Exercise.SizeMode = PictureBoxSizeMode.Zoom;
            buttonForExercise2.Enabled = true;
            buttonForExercise3.Enabled = true;
            textBox1.Enabled = true;
            textBox2.Enabled = true;
        }
        private void buttonForExercise2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Введите все значения", "Ошибка");
                return;
            }

            int f_min = Int32.Parse(textBox1.Text);
            int f_max = Int32.Parse(textBox2.Text);

            Bitmap inputImg2 = new Bitmap(pictureBoxFor1Exercise.Image);
            Bitmap resultImg = new Bitmap(inputImg2.Width, inputImg2.Height);
            for (int i = 0; i < inputImg2.Width; i++)
            {
                for (int j = 0; j < inputImg2.Height; j++)
                {
                    Color pixelColor = inputImg2.GetPixel(i, j);
                    int grayValue = (int)(0.3 * pixelColor.R + 0.59 * pixelColor.G + 0.11 * pixelColor.B);

                    if (grayValue > f_min && grayValue < f_max)
                    {
                        // Если значение яркости в заданном диапазоне, устанавливаем белый пиксель
                        resultImg.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                    }
                    else
                    {
                        // Иначе устанавливаем черный пиксель
                        resultImg.SetPixel(i, j, Color.FromArgb(255, 0, 0, 0));
                    }
                }
            }

            // Устанавливаем полученное изображение в pictureBox
            pictureBoxFor2Exercise.Image = resultImg;
            pictureBoxFor2Exercise.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void buttonForExercise3_Click(object sender, EventArgs e)
        {
            int[,] M = { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
            Bitmap inputImg2 = new Bitmap(pictureBoxFor1Exercise.Image);
            Bitmap third = new Bitmap(inputImg2.Width, inputImg2.Height);
            ProcessMainPixels(inputImg2, third, M);
            ProcessBorderPixels(inputImg2, third, M);
            pictureBoxFor3Exercise.Image = third;
            pictureBoxFor3Exercise.SizeMode = PictureBoxSizeMode.Zoom;
        }
        private void ProcessMainPixels(Bitmap input, Bitmap output, int[,] M)
        {
            for (int i = 2; i < input.Width - 1; i++)
            {
                for (int j = 2; j < input.Height - 1; j++)
                {
                    Color color = GetBrightness(input, M, i, j);
                    output.SetPixel(i, j, color);
                }
            }
        }
        private void ProcessBorderPixels(Bitmap input, Bitmap output, int[,] M)
        {
            for (int j = 1; j < input.Height - 1; j++)
            {

                Color leftBorderColor = input.GetPixel(0, j);
                output.SetPixel(0, j, leftBorderColor);

                Color rightBorderColor = GetBrightness(input, M, input.Width - 1, j);
                output.SetPixel(input.Width - 1, j, rightBorderColor);
            }

            
            for (int i = 1; i < input.Width - 1; i++)
            {
                Color topBorderColor = input.GetPixel(i, 0);
                output.SetPixel(i, 0, topBorderColor); 

                Color bottomBorderColor = GetBrightness(input, M, i, input.Height - 1);
                output.SetPixel(i, input.Height - 1, bottomBorderColor);
            }
        }
        private Color GetBrightness(Bitmap input, int[,] M, int x, int y)
        {
            double brightnessR = 0, brightnessG = 0, brightnessB = 0;
            double A = 0;
            double B = 1.0 / 9.0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int offsetX = x - 1 + i;
                    int offsetY = y - 1 + j;
                    if (offsetX < 0 || offsetY < 0 || offsetX >= input.Width || offsetY >= input.Height) continue;

                    Color pixelColor = input.GetPixel(offsetX, offsetY);
                    brightnessR += M[i, j] * pixelColor.R;
                    brightnessG += M[i, j] * pixelColor.G;
                    brightnessB += M[i, j] * pixelColor.B;
                }
            }
            int bR = Convert.ToInt32((brightnessR * B) + A);
            int bG = Convert.ToInt32((brightnessB * B) + A);
            int bB = Convert.ToInt32((brightnessG * B) + A);
            return Color.FromArgb(Clamp(bR), Clamp(bG), Clamp(bB));
        }
        private int Clamp(int value)
        {
            return Math.Max(0, Math.Min(255, value));
        }
        public static Bitmap LinearContrast(Bitmap inputImg2, int threshold)
        {
            int width = inputImg2.Width;
            int height = inputImg2.Height;
            Bitmap outputImg = new Bitmap(width, height);
            int fmin = 255;
            int fmax = 0;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color pixelColor = inputImg2.GetPixel(x, y);
                    int gray = pixelColor.R;

                    if (gray < fmin)
                        fmin = gray;
                    if (gray > fmax)
                        fmax = gray;
                }
            }

            //если изображение состоит из пикселей одного цвета
            if (fmin == fmax)
            {
                return (Bitmap)inputImg2.Clone();
            }

            int gmin = 0;
            int gmax = 255;

            float a = (float)(gmax - gmin) / (fmax - fmin);
            float b = gmin - a * fmin;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color pixelColor = inputImg2.GetPixel(x, y);
                    int gray = pixelColor.R;

                    int newGray;
                    if (gray < threshold)
                    {
                        newGray = gmin;
                    }
                    else
                    {
                        newGray = (int)(a * gray + b);
                        newGray = Math.Max(0, Math.Min(255, newGray));
                    }
                    outputImg.SetPixel(x, y, Color.FromArgb(newGray, newGray, newGray));
                }
            }

            return outputImg;
        }
    }
}
