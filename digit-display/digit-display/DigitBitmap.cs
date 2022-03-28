using System.Drawing;

namespace DigitDisplay;

public class DigitBitmap
{
    public static int[][] GenerateDigitArray(int[] input)
    {
        var output = new int[28][];
        for (int i = 0; i < 28; i++)
        {
            output[i] = input
                .Skip(i * 28)
                .Take(28)
                .ToArray();
        }
        return output;
    }

    public static Bitmap GetBitmapFromRawData(int[] input)
    {
        var digitArray = GenerateDigitArray(input);

        var digitBitmap = new Bitmap(28, 28);

        for (int i = 0; i < 28; i++)
            for (int j = 0; j < 28; j++)
            {
                var colorValue = 255 - digitArray[i][j];
                digitBitmap.SetPixel(j, i,
                    Color.FromArgb(colorValue, colorValue, colorValue));
            }
        digitBitmap.MakeTransparent(Color.White);
        return digitBitmap;
    }
}
