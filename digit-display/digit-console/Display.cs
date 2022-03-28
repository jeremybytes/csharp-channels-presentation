using System.Text;

namespace digit_console;

public class Display
{
    public static void GetImagesAsString(StringBuilder result, int[] image1, int[] image2)
    {
        var first = GetImageAsArray(image1);
        var second = GetImageAsArray(image2);
        for (int i = 0; i < 28; i++)
        {
            result.Append(first[i]);
            result.Append(" | ");
            result.AppendLine(second[i]);
        }
    }

    public static string[] GetImageAsArray(int[] image)
    {
        List<string> result = new();
        StringBuilder line = new();
        for (int i = 0; i < image.Length; i++)
        {
            if (i % 28 == 0 && i != 0)
            {
                result.Add(line.ToString());
                line.Clear();
            }
            var output_char = GetDisplayCharForPixel(image[i]);
            line.Append(output_char);
            line.Append(output_char);
        }
        result.Add(line.ToString());
        return result.ToArray();
    }

    private static char GetDisplayCharForPixel(int pixel)
    {
        return pixel switch
        {
            > 16 and < 32 => '.',
            >= 32 and < 64 => ':',
            >= 64 and < 160 => 'o',
            >= 160 and < 224 => 'O',
            >= 224 => '@',
            _ => ' ',
        };
    }
}
