﻿namespace DigitDisplay;

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

        // START: Use this section to compare parallel / non-parallel
        //var panel1Recognizer = new ParallelChannelRecognizerControl(
        //    "Manhattan Classifier", displayMultipler);
        //LeftPanel.Children.Add(panel1Recognizer);

        //MessageBox.Show("Ready to start panel #1");
        //await panel1Recognizer.Start(rawValidation, manhattanClassifier);

        var panel2Recognizer = new NonParallelRecognizerControl(
            "Manhattan Classifier", displayMultipler);
        RightPanel.Children.Add(panel2Recognizer);

        MessageBox.Show("Ready to start panel #2");
        await panel2Recognizer.Start(rawValidation, manhattanClassifier);
        // END: Use this section to compare parallel / non-parallel

        // START: Use this section to compare Manhattan / Euclidean distance algos
        //var panel2Recognizer = new ParallelChannelRecognizerControl(
        //    "Euclidean Classifier", displayMultipler);
        //RightPanel.Children.Add(panel2Recognizer);

        //MessageBox.Show("Ready to start panel #2");
        //await panel2Recognizer.Start(rawValidation, euclideanClassifier);
        // END: Use this section to compare Manhattan / Euclidean distance algos
    }
}
