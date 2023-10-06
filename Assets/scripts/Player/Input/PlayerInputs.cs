using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    [HideInInspector] public bool CanChange = true;

    private Vector2 _move;
    private bool _sprint;
    private Vector2 _look;
    private bool _jump;

    public Vector2 Move => _move;
    public bool Sprint => _sprint;
    public Vector2 Look => _look;
    public bool Jump => _jump;

    public void SetMove(Vector2 move)
    {
        if (!CanChange)
            return;

        _move = move;
    }

    public void SetLook(Vector2 look)
    {
        _look = look;
    }

    public void SetJump(bool jump)
    {
        if (!CanChange)
            return;

        _jump = jump;
    }

    public void ResetValues()
    {
        SetMove(Vector2.zero);
        SetLook(Vector2.zero);

        SetJump(false);
    }
}
