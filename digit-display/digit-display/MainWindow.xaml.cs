using System;
using System.Threading.Tasks;
using System.Windows;

namespace DigitDisplay
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Offset.Text = 6000.ToString();
            RecordCount.Text = 375.ToString();
            OutputSize.Text = "1.0";
        }

        private async void GoButton_Click(object sender, RoutedEventArgs e)
        {
            LeftPanel.Children.Clear();
            RightPanel.Children.Clear();

            string fileName = AppDomain.CurrentDomain.BaseDirectory + "train.csv";

            int offset = int.Parse(Offset.Text);
            int recordCount = int.Parse(RecordCount.Text);
            double displayMultipler = double.Parse(OutputSize.Text);

            string[] rawTrain = await Task.Run(() => Loader.trainingReader(fileName, offset, recordCount));
            string[] rawValidation = await Task.Run(() => Loader.validationReader(fileName, offset, recordCount));

            var manhattanClassifier = Recognizers.manhattanClassifier(rawTrain);
            var euclideanClassifier = Recognizers.euclideanClassifier(rawTrain);

            var panel1Recognizer = new ParallelChannelRecognizerControl(
                "Manhattan Classifier", displayMultipler);
            LeftPanel.Children.Add(panel1Recognizer);

            var panel2Recognizer = new NonParallelRecognizerControl(
                "Manhattan Classifier", displayMultipler);
            RightPanel.Children.Add(panel2Recognizer);

            MessageBox.Show("Ready to start panel #1");
            await panel1Recognizer.Start(rawValidation, manhattanClassifier);

            MessageBox.Show("Ready to start panel #2");
            await panel2Recognizer.Start(rawValidation, manhattanClassifier);
        }
    }
}
