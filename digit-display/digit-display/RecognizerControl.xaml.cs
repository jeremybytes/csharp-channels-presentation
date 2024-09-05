using digits;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Media;

namespace DigitDisplay;

public abstract partial class RecognizerControl : UserControl
{
    public async Task Start(Record[] rawData, Classifier classifier)
    {
        timer.Start();
        await Run(rawData, classifier);
        timer.Stop();
    }

    protected abstract Task Run(Record[] rawData, Classifier classifier);

    public RecognizerControl(string controlTitle,
        double displayMultiplier = 1.0)
    {
        InitializeComponent();
        this.controlTitle = controlTitle;
        DisplayMultiplier = displayMultiplier;
        Loaded += (s, e) => ClassifierText.Text = this.controlTitle;
    }

    private DetailControl? DetailPopup;
    private readonly double DisplayMultiplier;

    protected record RecognizerResult(string Prediction, string Actual, Bitmap Image);

    protected string controlTitle;

    //protected DateTimeOffset startTime;
    protected Stopwatch timer = new();
    protected readonly SolidColorBrush redBrush = new(System.Windows.Media.Color.FromRgb(255, 150, 150));
    protected readonly SolidColorBrush whiteBrush = new(System.Windows.Media.Color.FromRgb(255, 255, 255));
    private int errors = 0;


    protected void CreateUIElements(Prediction prediction, Panel panel)
    {
        int predicted = prediction.Predicted.Value;
        int actual = prediction.Actual.Value;
        int[] imageData = prediction.Actual.Image;

        Bitmap image = DigitBitmap.GetBitmapFromRawData(imageData);

        var imageControl = new System.Windows.Controls.Image
        {
            Source = image.ToWpfBitmap(),
            Stretch = Stretch.UniformToFill
        };
        imageControl.Width = imageControl.Source.Width * DisplayMultiplier;
        imageControl.Height = imageControl.Source.Height * DisplayMultiplier;

        var textBlock = new TextBlock
        {
            Height = imageControl.Height,
            Width = imageControl.Width,
            FontSize = 16 * DisplayMultiplier,
            TextAlignment = TextAlignment.Center,
            Text = $"{predicted}",
        };

        var button = new Button();
        var backgroundBrush = whiteBrush;
        button.Background = backgroundBrush;
        button.Tag = new DetailRecord(predicted, actual, image);
        button.MouseEnter += Button_MouseEnter;
        button.MouseLeave += Button_MouseLeave;

        var buttonContent = new StackPanel
        {
            Orientation = Orientation.Horizontal
        };
        button.Content = buttonContent;

        if (predicted != actual)
        {
            button.Background = redBrush;
            errors++;
            ErrorBlock.Text = $"Errors: {errors}";
        }

        buttonContent.Children.Add(imageControl);
        buttonContent.Children.Add(textBlock);

        panel.Children.Add(button);

        TimeSpan duration = timer.Elapsed;
        TimingBlock.Text = $"Duration (seconds): {duration:s\\.fff}";
    }

    private void Button_MouseEnter(object sender, MouseEventArgs e)
    {
        var button = (Button)sender;
        var detail = (DetailRecord)button.Tag;
        if (DetailPopup == null)
        {
            DetailPopup = new DetailControl(detail);
            DigitsCanvas.Children.Add(DetailPopup);
        }

        var point = e.GetPosition(DigitsCanvas);
        bool invertY = (point.Y + 200) > DigitsCanvas.ActualHeight;
        bool invertX = (point.X + 200) > DigitsCanvas.ActualWidth;

        int yOffset = invertY ? -150 : 10;
        int xOffset = invertX ? -200 : 10;

        Canvas.SetTop(DetailPopup, e.GetPosition(DigitsCanvas).Y + yOffset);
        Canvas.SetLeft(DetailPopup, e.GetPosition(DigitsCanvas).X + xOffset);
        DetailPopup.Data = detail;
        DetailPopup.Visibility = Visibility.Visible;
    }

    private void Button_MouseLeave(object sender, MouseEventArgs e)
    {
        DetailPopup!.Visibility = Visibility.Hidden;
    }
}
