using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Shared.Infra.CrossCutting
{
    public static class Cor
    {

        /// <summary>
        /// Creates color with corrected brightness.
        /// </summary>
        /// <param name="hexColor">Hex Color Color to correct.</param>
        /// <param name="correctionFactor">The brightness correction factor. Must be between -1 and 1. 
        /// Negative values produce darker colors.</param>
        /// <returns>
        /// Hex Color.
        /// </returns>
        public static string ChangeColorBrightness(string hexColor, float correctionFactor)
        {
            try
            {
                Color color = System.Drawing.ColorTranslator.FromHtml(hexColor);

                float red = (float)color.R;
                float green = (float)color.G;
                float blue = (float)color.B;

                if (correctionFactor < 0)
                {
                    correctionFactor = 1 + correctionFactor;
                    red *= correctionFactor;
                    green *= correctionFactor;
                    blue *= correctionFactor;
                }
                else
                {
                    red = (255 - red) * correctionFactor + red;
                    green = (255 - green) * correctionFactor + green;
                    blue = (255 - blue) * correctionFactor + blue;
                }

                Color final = Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
                return "#" + final.R.ToString("X2") + final.G.ToString("X2") + final.B.ToString("X2");
            }
            catch
            {
                return hexColor;
            }
        }

       

    }
}
