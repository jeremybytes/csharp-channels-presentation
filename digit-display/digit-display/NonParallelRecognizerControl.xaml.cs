using Microsoft.FSharp.Core;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static Recognizers;

namespace DigitDisplay
{
    public partial class NonParallelRecognizerControl : UserControl
    {
        #region Control Setup

        private DetailControl DetailPopup;
        private double DisplayMultiplier;

        private class RecognizerResult
        {
            public string prediction { get; set; }
            public string actual { get; set; }
            public string imageString { get; set; }
        }

        string controlTitle;

        DateTimeOffset startTime;
        SolidColorBrush redBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 150, 150));
        SolidColorBrush whiteBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
        int errors = 0;

        public NonParallelRecognizerControl(string controlTitle,
            double displayMultiplier = 1.0)
        {
            InitializeComponent();
            this.controlTitle = controlTitle + " (Non-Parallel)";
            DisplayMultiplier = displayMultiplier;

            Loaded += RecognizerControl_Loaded;
        }

        private void RecognizerControl_Loaded(object sender, RoutedEventArgs e)
        {
            ClassifierText.Text = controlTitle;
        }

        #endregion

        private class Result
        {
            public string Prediction { get; }
            public string Actual { get; }
            public string ImageString { get; }

            public Result(string prediction, string actual, string imageString)
            {
                Prediction = prediction;
                Actual = actual;
                ImageString = imageString;
            }
        }

        private async Task Listen(ChannelReader<Result> reader)
        {
            await foreach (Result result in reader.ReadAllAsync())
            {
                CreateUIElements(result.Prediction, result.Actual, result.ImageString, DigitsBox);
            }
        }

        private Task Produce(ChannelWriter<Result> writer, string[] rawData,
            FSharpFunc<int[], Observation> classifier)
        {
            return Task.Run(() =>
            {
                foreach(var imageString in rawData)
                {
                    int actual = imageString.Split(',').Select(x => Convert.ToInt32(x)).First();
                    int[] ints = imageString.Split(',').Select(x => Convert.ToInt32(x)).Skip(1).ToArray();

                    var observation = Recognizers.predict<Observation>(ints, classifier);
                    var result = new Result(observation.Label, actual.ToString(), imageString);
                    writer.WriteAsync(result);
                }
            });
        }

        public async Task Start(string[] rawData, FSharpFunc<int[], Observation> classifier)
        {
            startTime = DateTime.Now;

            var channel = Channel.CreateUnbounded<Result>();

            var listener = Listen(channel.Reader);
            var producer = Produce(channel.Writer, rawData, classifier);
            await producer;

            //var producer1 = Produce(channel.Writer, rawData.Take(rawData.Length / 2).ToArray(), classifier);
            //var producer2 = Produce(channel.Writer, rawData.Skip(rawData.Length / 2).ToArray(), classifier);
            //await producer1;
            //await producer2;

            channel.Writer.Complete();

            await listener;
        }

        #region UI Controls
        private void CreateUIElements(string prediction, string actual, string imageData,
            Panel panel)
        {
            Bitmap image = DigitBitmap.GetBitmapFromRawData(imageData);

            var imageControl = new System.Windows.Controls.Image();
            imageControl.Source = image.ToWpfBitmap();
            imageControl.Stretch = Stretch.UniformToFill;
            imageControl.Width = imageControl.Source.Width * DisplayMultiplier;
            imageControl.Height = imageControl.Source.Height * DisplayMultiplier;

            var textBlock = new TextBlock();
            textBlock.Height = imageControl.Height;
            textBlock.Width = imageControl.Width;
            textBlock.FontSize = 12 * DisplayMultiplier;
            textBlock.TextAlignment = TextAlignment.Center;
            textBlock.Text = prediction;

            var button = new Button();
            var backgroundBrush = whiteBrush;
            button.Background = backgroundBrush;
            button.Tag = new DetailRecord() { prediction = prediction, actual = actual, image = image };
            button.MouseEnter += Button_MouseEnter;
            button.MouseLeave += Button_MouseLeave;

            var buttonContent = new StackPanel();
            buttonContent.Orientation = Orientation.Horizontal;
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
            var detail = button.Tag as DetailRecord;
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
            DetailPopup.Visibility = Visibility.Hidden;
        }

        private Window GetParentWindow(FrameworkElement element)
        {
            if (element?.Parent != null)
                if (element.Parent is Window)
                    return element.Parent as Window;
                else
                    return GetParentWindow(element.Parent as FrameworkElement);

            return null;
        }
        #endregion
    }
}
