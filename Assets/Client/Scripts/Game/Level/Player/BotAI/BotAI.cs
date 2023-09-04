using System.Collections.Generic;
using UnityEngine;

public class BotAI : MonoBehaviour
{
    [SerializeField] private BotCheckGround _targetBotCheckGround;
    [SerializeField] private BotCheckGround _jumpCheckGround;
    [SerializeField] private List<BotCheckGround> _firstCheckGrounds;
    [SerializeField] private List<BotCheckGround> _secondCheckGrounds;

    [SerializeField] private Transform _obstacleAheadRayTransform;

    [SerializeField] private float _enemyDetectionRadius;
    [SerializeField] private LayerMask _enemyLayer;

    private bool _jump;
    private bool _isJump;

    private float _updateTimeoutDelta;

    private float _jumpTimeoutDelta;
    private float _randomJumpTimeoutDelta;
    private float _randomizeDirectionTimeoutDelta;
    private float _enemySearchTimeoutDelta;

    private Vector2 _moveDirection;
    private Transform _targetEnemy;

    private PlayerInputs _input;
    private Transform _transform;
    private PlayerController _controller;

    private void Start()
    {
        _input = GetComponent<PlayerInputs>();
        _transform = GetComponent<Transform>();
        _controller = GetComponent<PlayerController>();

        _randomJumpTimeoutDelta = Random.Range(1f, 2f);
        _enemySearchTimeoutDelta = Random.Range(1f, 5f);
        _randomizeDirectionTimeoutDelta = Random.Range(0.5f, 2f);

        SetRandomDirection();
    }

    public void MyUpdate()
    {
        _updateTimeoutDelta -= Time.deltaTime;

        Jump();
        EnemySearch();

        if (_updateTimeoutDelta <= 0f)
        {
            Move();
            CheckObstacleAhead();

            _updateTimeoutDelta = 0.05f;
        }

        RandomizeDirection();
        RandomJump();
    }

    private void CheckObstacleAhead()
    {
        if (_isJump || !_controller.Grounded || !_targetBotCheckGround.CheckGround())
            return;

        Vector3 forwardDirection = _obstacleAheadRayTransform.forward;
        RaycastHit hit;

        bool isHit = Physics.Raycast(_obstacleAheadRayTransform.position, forwardDirection, out hit, 0.3f);

        if (isHit)
        {
            if (hit.collider.tag == "Player")
            {
                _jump = false;
                _input.SetAttack(true);
            }
            else
            {
                List<int> rand = new List<int> { 45, 90, 135, 180, 225, 270, 315 };
                float angle = Mathf.Atan2(_moveDirection.y, _moveDirection.x) * Mathf.Rad2Deg;

                _moveDirection = new Vector2(Mathf.Cos((angle + rand[Random.Range(0, rand.Count)]) * Mathf.Deg2Rad), 1);
            }
        }
        else
        {

        }
    }

    private void EnemySearch()
    {
        _enemySearchTimeoutDelta -= Time.deltaTime;

        if (Random.Range(0, 100) == 0)
        {
            _targetEnemy = null;
            _enemySearchTimeoutDelta = Random.Range(1, 5);
        }

        if (_enemySearchTimeoutDelta <= 0f)
        {
            _enemySearchTimeoutDelta = Random.Range(1, 5);

            Collider[] enemies = Physics.OverlapSphere(_transform.position, _enemyDetectionRadius, _enemyLayer);

            if (enemies.Length > 0)
            {
                for (int i = 0; i < enemies.Length; i++)
                {
                    if (enemies[i].transform != _transform)
                    {
                        _targetEnemy = enemies[i].transform;
                        break;
                    }
                }
            }
        }
    }

    private Vector2 GetMoveDirectionToEnemy()
    {
        if (_targetEnemy == null)
            return _moveDirection;

        Vector3 directionToTarget = _targetEnemy.position - _transform.position;
        directionToTarget = new Vector2(directionToTarget.x, directionToTarget.z).normalized;
        directionToTarget = Vector2.Lerp(_moveDirection, directionToTarget, 0.5f);

        return directionToTarget;
    }

    private void Move()
    {
        if (!_isJump)
        {
            if (!_targetBotCheckGround.CheckGround())
            {
                _targetEnemy = null;
                _moveDirection = GetMoveDirection();
            }
            else
                _moveDirection = GetMoveDirectionToEnemy();

            if (_moveDirection == Vector2.zero)
                SetRandomDirection();
        }

        _input.SetSprint(true);
        _input.SetMove(_moveDirection);
    }

    private Vector2 GetMoveDirection()
    {
        Vector2 direction = GetBlockDirectionFirstCheckGrounds();

        if (direction == null)
        {
            direction = GetBlockDirectionSecondCheckGrounds();

            if (direction != null)
                _jump = true;
        }

        if (direction == null)
        {
            List<int> rand = new List<int> { 45, 90, 135, 180, 225, 270, 315 };
            float angle = Mathf.Atan2(_moveDirection.y, _moveDirection.x) * Mathf.Rad2Deg;

            direction = new Vector2(Mathf.Cos((angle + rand[Random.Range(0, rand.Count)]) * Mathf.Deg2Rad), 1);
        }

        return direction;
    }

    private void SetRandomDirection()
    {
        _moveDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    private void Jump()
    {
        if (_jump)
        {
            if (CanJump())
            {
                _isJump = true;
                _jumpTimeoutDelta = 0.2f;
                _moveDirection = GetJumpDirection();
                _input.SetJump(true);
            }

            _jump = false;
        }

        if (_isJump)
        {
            _jumpTimeoutDelta -= Time.deltaTime;

            if (_jumpTimeoutDelta <= 0f)
            {
                if (_controller.Grounded)
                {
                    _jump = false;
                    _isJump = false;
                }
            }
        }
    }

    private Vector2 GetJumpDirection()
    {
        Transform targetTransform = _jumpCheckGround.GetGroundTransform();

        if (targetTransform == null)
            return Vector2.zero;

        Vector3 directionToTarget = targetTransform.position - _transform.position;
        directionToTarget = new Vector2(directionToTarget.x, directionToTarget.z).normalized;

        return directionToTarget;
    }

    private void RandomizeDirection()
    {
        if (_isJump || !_controller.Grounded || _targetEnemy != null || !_targetBotCheckGround.CheckGround())
        {
            if (_randomizeDirectionTimeoutDelta < 1f)
                _randomizeDirectionTimeoutDelta = Random.Range(0.5f, 2f);

            return;
        }

        _randomizeDirectionTimeoutDelta -= Time.deltaTime;

        if (_randomizeDirectionTimeoutDelta <= 0f)
        {
            SetRandomDirection();

            _randomizeDirectionTimeoutDelta = Random.Range(0.5f, 2f);
        }
    }

    private void RandomJump()
    {
        _randomJumpTimeoutDelta -= Time.deltaTime;

        if (_randomJumpTimeoutDelta <= 0f)
        {
            _jump = true;
            _randomJumpTimeoutDelta = Random.Range(1f, 2f);
        }
    }

    private Vector2 GetBlockDirectionFirstCheckGrounds()
    {
        Vector3 directionToTarget = Vector3.zero;

        for (int i = 0; i < _firstCheckGrounds.Count; i++)
        {
            if (_firstCheckGrounds[i].CheckGround())
            {
                directionToTarget = _firstCheckGrounds[i].transform.position - _transform.position;
                directionToTarget = new Vector2(directionToTarget.x, directionToTarget.z).normalized;

                return directionToTarget;
            }
        }

        return directionToTarget;
    }

    private Vector2 GetBlockDirectionSecondCheckGrounds()
    {
        Vector3 directionToTarget = Vector3.zero;

        for (int i = 0; i < _secondCheckGrounds.Count; i++)
        {
            if (_secondCheckGrounds[i].CheckGround())
            {
                directionToTarget = _secondCheckGrounds[i].transform.localPosition - Vector3.zero;
                directionToTarget = new Vector2(directionToTarget.x, directionToTarget.z).normalized;

                return directionToTarget;
            }
        }

        return directionToTarget;
    }


    private bool CanJump()
    {
        if (_isJump || !_controller.Grounded)
            return false;

        if (_jumpCheckGround.CheckGround())
            return true;

        return false;
    }
}
