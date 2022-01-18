using System.Drawing;
using System.Windows.Controls;

namespace DigitDisplay;

public record DetailRecord(string Prediction, string Actual, Bitmap Image);

public partial class DetailControl : UserControl
{
    private DetailRecord? data;
    public DetailRecord? Data
    {
        get { return data; }
        set
        {
            data = value;
            UpdateUI();
        }
    }


    public DetailControl(DetailRecord record)
    {
        InitializeComponent();
        Data = record;
    }

    private void UpdateUI()
    {
        var multiplier = 4;
        DigitImage.Source = data!.Image.ToWpfBitmap();
        DigitImage.Width = DigitImage.Source.Width * multiplier;
        DigitImage.Height = DigitImage.Source.Height * multiplier;

        Prediction.Text = data.Prediction;
        Prediction.Height = DigitImage.Height;
        Prediction.Width = DigitImage.Width;

        Actual.Text = $"Actual: {data.Actual}";
    }
}
