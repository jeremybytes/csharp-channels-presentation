using System.Drawing;
using System.Windows.Media;
using static Recognizers;

namespace DigitDisplay;

public partial class ParallelTaskRecognizerControl : UserControl
{
    #region Control Setup

    private DetailControl? DetailPopup;
    private readonly double DisplayMultiplier;

    private readonly string controlTitle;

    private DateTimeOffset startTime;
    private readonly SolidColorBrush redBrush = new(System.Windows.Media.Color.FromRgb(255, 150, 150));
    private readonly SolidColorBrush whiteBrush = new(System.Windows.Media.Color.FromRgb(255, 255, 255));
    private int errors = 0;

    public ParallelTaskRecognizerControl(string controlTitle,
        double displayMultiplier = 1.0)
    {
        InitializeComponent();
        this.controlTitle = controlTitle;
        DisplayMultiplier = displayMultiplier;

        Loaded += RecognizerControl_Loaded;
    }

    private void RecognizerControl_Loaded(object sender, RoutedEventArgs e)
    {
        ClassifierText.Text = controlTitle + " (Parallel Task)";
    }

    #endregion

    public Task Start(string[] rawData, FSharpFunc<int[], Observation> classifier)
    {
        var allTasks = new List<Task>();
        foreach (var imageString in rawData)
        {
            int actual = imageString.Split(',').Select(x => Convert.ToInt32(x)).First();
            int[] ints = imageString.Split(',').Select(x => Convert.ToInt32(x)).Skip(1).ToArray();

            var task = Task.Run<Observation>(() =>
            {
                return Recognizers.predict<Observation>(ints, classifier);
            }
            );
            allTasks.Add(task);
            var continuation = task.ContinueWith(t =>
                {
                    CreateUIElements(t.Result.Label, actual.ToString(), imageString, DigitsBox);
                },
                TaskScheduler.FromCurrentSynchronizationContext()
            );
            allTasks.Add(continuation);
        }
        Task.WhenAny(allTasks).ContinueWith(t => startTime = DateTime.Now);
        return Task.WhenAll(allTasks);
    }

    #region UI Controls
    private void CreateUIElements(string prediction, string actual, string imageData,
        Panel panel)
    {
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
            FontSize = 12 * DisplayMultiplier,
            TextAlignment = TextAlignment.Center,
            Text = prediction
        };

        var button = new Button();
        var backgroundBrush = whiteBrush;
        button.Background = backgroundBrush;
        button.Tag = new DetailRecord(prediction, actual, image);
        button.MouseEnter += Button_MouseEnter;
        button.MouseLeave += Button_MouseLeave;

        var buttonContent = new StackPanel
        {
            Orientation = Orientation.Horizontal
        };
        button.Content = buttonContent;

        if (prediction != actual)
        {
            button.Background = redBrush;
            errors++;
            ErrorBlock.Text = $"Errors: {errors}";
        }

        buttonContent.Children.Add(imageControl);
        buttonContent.Children.Add(textBlock);

        panel.Children.Add(button);

        TimeSpan duration = DateTimeOffset.Now - startTime;
        TimingBlock.Text = $"Duration (seconds): {duration.TotalSeconds:0}";
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
    #endregion
}
