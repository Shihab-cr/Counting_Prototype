using UnityEngine;

public class AnimationRelay : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    
    public void SetCanMove(int value)
    {
        bool canMove = value != 0;
        if(playerController != null)
        {
            playerController.SetMoveState(canMove);
        }
    }
}
