public interface IMinigame
{
    public delegate void MinigameStart();
    public delegate void MinigameEnd(bool success);
    public event MinigameStart OnMinigameStart;
    public event MinigameEnd OnMinigameEnd;
    
    public void StartMinigame();
    public void Halt();
}