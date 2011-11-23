namespace VirastyarWordAddin.Verifiers.Basics
{
    public interface IBatchModeVerificationWindow
    {
        bool CancelationPending { get; }
        string CurrentStatus { get; set; }
        bool ShowProgressAtDocument { get; set; }
    }
}
