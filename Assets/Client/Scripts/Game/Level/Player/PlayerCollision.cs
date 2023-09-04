using System.Collections;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private ParticleSystem _collectedCoinEffect;

    private PlayerController _controller;

    private void Awake()
    {
        _controller = GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Trap")
        {
            if (other.gameObject.TryGetComponent<Trap>(out Trap trap))
                StartCoroutine(OnTrapHit(trap));
        }
        else if (other.tag == "Border")
        {
            gameObject.SetActive(false);
            GameEvents.InvokePlayerLoseEvent(_controller);
        }
        else if (other.tag == "Coin")
        {
            if (!_controller.IsBot)
            {
                _collectedCoinEffect.Play();
                other.gameObject.SetActive(false);

                GameEvents.InvokePlayerCollectedCoinEvent();
            }
        }
    }

    IEnumerator OnTrapHit(Trap trap)
    {
        _controller.CanMove = false;
        _controller.CanRotation = false;
        _controller.CanJump = false;
        _controller.CanAttack = false;
        _controller.CanHit = false;

        trap.Deactivate();
        Vector3 trapPosition = transform.position;
        trap.transform.position = new Vector3(trapPosition.x, transform.position.y, trapPosition.z);

        yield return new WaitForSeconds(2);

        _controller.CanMove = true;
        _controller.CanRotation = true;
        _controller.CanJump = true;
        _controller.CanAttack = true;
        _controller.CanHit = true;

        trap.DestroyAnim();
    }
}
