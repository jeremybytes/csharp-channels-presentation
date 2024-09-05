using digits;

namespace DigitDisplay;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Offset.Text = 6000.ToString();
        RecordCount.Text = 390.ToString();
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

        (Record[] rawTrain, Record[] rawValidation) = await Task.Run(() => FileLoader.GetData(fileName, offset, recordCount));

        var manhattanClassifier = new ManhattanClassifier(rawTrain);
        var euclideanClassifier = new EuclideanClassifier(rawTrain);
        var k5EuclideanClassifier = new K5EuclideanClassifier(rawTrain);

        // START: Use this section to compare parallel / non-parallel
        //var panel1Recognizer = new NonParallelRecognizerControl(
        //    "Euclidean Classifier", displayMultipler);
        //LeftPanel.Children.Add(panel1Recognizer);

        //MessageBox.Show("Ready to start panel #1");
        //await panel1Recognizer.Start(rawValidation, euclideanClassifier);

        var panel2Recognizer = new ParallelChannelRecognizerControl(
            "Euclidean Classifier", displayMultipler);
        RightPanel.Children.Add(panel2Recognizer);

        MessageBox.Show("Ready to start panel #2");
        await panel2Recognizer.Start(rawValidation, euclideanClassifier);
        // END: Use this section to compare parallel / non-parallel

        // START: Use this section to compare Manhattan / Euclidean distance algos
        //var panel1Recognizer = new ParallelChannelRecognizerControl(
        //    "Manhattan Classifier", displayMultipler);
        //LeftPanel.Children.Add(panel1Recognizer);

        //MessageBox.Show("Ready to start panel #1");
        //await panel1Recognizer.Start(rawValidation, manhattanClassifier);

        //var panel2Recognizer = new ParallelChannelRecognizerControl(
        //    "Euclidean Classifier", displayMultipler);
        //RightPanel.Children.Add(panel2Recognizer);

        //MessageBox.Show("Ready to start panel #2");
        //await panel2Recognizer.Start(rawValidation, euclideanClassifier);
        // END: Use this section to compare Manhattan / Euclidean distance algos
    }
}
