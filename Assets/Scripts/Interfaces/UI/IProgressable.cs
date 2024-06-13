public interface IProgressable
{
    int MaxValue { get; }
    int CurrentValue { get; set; }
    void UpdateProgress(int value);
}