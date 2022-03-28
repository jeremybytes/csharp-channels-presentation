namespace digits;

public abstract class Classifier
{
    public string Name { get; set; }
    public Record[] TrainingData { get; set; }

    public Classifier(string name, Record[] trainingData)
    {
        Name = name;
        TrainingData = trainingData;
    }

    public abstract Task<Prediction> Predict(Record input);
}

