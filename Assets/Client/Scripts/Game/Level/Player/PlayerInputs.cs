using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    [HideInInspector] public bool CanChange = true;

    private Vector2 _move;
    private Vector2 _look;
    private bool _sprint = true;
    private bool _attack;
    private bool _jump;

    public Vector2 Move => _move;
    public Vector2 Look => _look;
    public bool Sprint => _sprint;
    public bool Attack => _attack;
    public bool Jump => _jump;

    public void SetMove(Vector2 move)
    {
        if (!CanChange)
            return;

        _move = move;
    }

    public void SetLook(Vector2 look)
    {
        if (!CanChange)
            return;

        _look = look;
    }

    public void SetSprint(bool sprint)
    {

    }

    public void SetAttack(bool attack)
    {
        if (!CanChange)
            return;

        _attack = attack;
    }

    public void SetJump(bool jump)
    {
        if (!CanChange)
            return;

        _jump = jump;
    }
}
