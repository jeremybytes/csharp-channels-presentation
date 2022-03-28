namespace digits
{
    public record Record (int Value, int[] Image);
    public record Prediction (Record Actual, Record Predicted);
}