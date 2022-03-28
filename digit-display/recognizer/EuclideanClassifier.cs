namespace digits;

public class EuclideanClassifier : Classifier
{
    public EuclideanClassifier(Record[] training_data)
        : base("Euclidean Classifier", training_data)
    {
    }

    public override Task<Prediction> Predict(Record input)
    {
        return Task.Run(() =>
        {
            int[] inputImage = input.Image;
            int best_total = int.MaxValue;
            Record best = new(0, Array.Empty<int>());
            foreach (Record candidate in TrainingData)
            {
                int total = 0;
                int[] candidateImage = candidate.Image;
                for (int i = 0; i < 784; i++)
                {
                    int diff = inputImage[i] - candidateImage[i];
                    total += (diff * diff);
                }
                if (total < best_total)
                {
                    best_total = total;
                    best = candidate;
                }
            }

            return new Prediction(input, best);
        });
    }
}
