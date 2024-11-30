using System.Collections.Generic;

public interface IMinigame
{
    public delegate void MinigameStart();
    public delegate void MinigameEnd(bool success);
    public event MinigameStart OnMinigameStart;
    public event MinigameEnd OnMinigameEnd;
    
    public void StartMinigame(List<IngredientSO> ingredients);
    public void Halt();
}