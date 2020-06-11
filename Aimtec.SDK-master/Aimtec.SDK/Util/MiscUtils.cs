using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aimtec.SDK.Util
{
    public class MiscUtils
    {
        /// <summary>
        ///     The League of Legends font
        /// </summary>
        public static Font LeagueFont = new Font("Arial", 11);

        /// <summary>
        ///     Calculates the width of the text (not 100% accurate)
        /// </summary>
        public static float MeasureTextWidth(string text)
        {
            float textWidth = 0;

            using (var bmp = new Bitmap(1, 1))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    textWidth = g.MeasureString(text, LeagueFont).Width;
                }
            }

            return textWidth;
        }
    }
}
