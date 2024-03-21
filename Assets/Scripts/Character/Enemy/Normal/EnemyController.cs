using System;
using System.Collections;
using System.Collections.Generic;
using Character.Enemy.Normal.AttackLogic;
using Character.Enemy.Normal.DeadLogic;
using Character.Enemy.Normal.StateCheckLogic;
using Character.Player;
using Enemy.IdleLogic;
using Enemy.StateCheckLogic.DetectLogic;
using Enemy.TraceLogic;
using UnityEngine;

namespace Character.Enemy.Normal
{
    public abstract class EnemyController : MonoBehaviour , IHitAble
    {
        internal Rigidbody2D rigid2D;
        internal GameObject player;
        private LayerMask platformLayerMask;
        internal BoxCollider2D boxCollider;
        internal float originGravityScale;
        internal SpriteRenderer spriteRenderer;

        public enum EnemyState { Idle, Trace, Attack, Dead }
        [SerializeField] internal EnemyState curState = EnemyState.Idle;
        [SerializeField] private int health = 10;
        [SerializeField] private int attackDamage;
        private int money;
        internal bool isDead;
        private List<Vector2> guardDirList = new List<Vector2>();

        [SerializeField] internal float stateCheckTime = 0.2f;
        [SerializeField] private float accelerate = 10f;
        protected Vector2 moveDir;

        protected EnemyIdleLogic idleLogic;
        protected EnemyTraceLogic traceLogic;
        protected EnemyAttackLogic attackLogic;
        protected EnemyDetectLogic detectLogic;
        protected EnemyStateSelectLogic stateSelectLogic;
        protected EnemyDeadLogic deadLogic;
        
        internal EnemyStateSelectLogic.AttackAbleDir attackAbleDir;

        public bool debugDrawOn;
        //trace 관련

        [Header("Attacked")]
        [SerializeField] private float knockBackPower = 1f;

        protected virtual void Start()
        {
            rigid2D = GetComponent<Rigidbody2D>();
            boxCollider = GetComponent<BoxCollider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            player = GameObject.FindWithTag("Player");
            platformLayerMask = LayerMask.GetMask("Platform");
            originGravityScale = rigid2D.gravityScale;
            
            SetLogic();
            idleLogic.InitIdleState(this);
            StartCoroutine(CheckState());
            StartCoroutine(CheckStateForAction());
        }


        private void OnDrawGizmosSelected()
        {
            if (debugDrawOn)
            {
                detectLogic.DrawDebug(this);
                traceLogic.DrawDebug(this);
                idleLogic.DrawDebug(this);
            }
        }

        protected abstract void SetLogic();
        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                Vector2 knockBackDir;
                if (transform.position.x <= player.transform.position.x)
                    knockBackDir = new Vector2(1,0);
                else
                    knockBackDir = new Vector2(-1,0);
                collision.collider.GetComponent<PlayerController>().Hit(1, knockBackDir);
            }
        }

        private IEnumerator CheckState()
        {
            while (!isDead)
            {
                yield return new WaitForSeconds(stateCheckTime);
                var nextState = health > 0 ? stateSelectLogic.GetEnemyState(this) : EnemyState.Dead;
                if(curState != nextState)
                {
                    if (nextState == EnemyState.Idle)
                        idleLogic.InitIdleState(this);
                }
                curState = nextState;
            }
        }

        private IEnumerator CheckStateForAction()
        {
            while(!isDead)
            {
                yield return null;
                Vector2 force;
                switch (curState)
                {
                    case EnemyState.Idle:
                        moveDir = idleLogic.GetIdleDir(this);
                        force = GetForce(moveDir, accelerate);
                        rigid2D.AddForce(force);
                        break;
                    case EnemyState.Trace:
                        moveDir = traceLogic.GetTraceDir(this);
                        force = GetForce(moveDir, accelerate);
                        rigid2D.AddForce(force);
                        break;
                    case EnemyState.Attack:
                        StartCoroutine(attackLogic.Attack(this));
                        break;
                    case EnemyState.Dead:
                        StartCoroutine(deadLogic.Dead(this));
                        isDead = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public virtual IEnumerator Hit( int attackDamage,Vector2 knockbackDir, float attackForceScale)
        {
            if (guardDirList.Contains(knockbackDir))
                yield return Blocked(attackForceScale, knockbackDir);
            else
                yield return Damaged(attackForceScale, knockbackDir, attackDamage);
        }

        protected virtual IEnumerator Blocked(float attackForce, Vector2 knockbackDir)
        {
            throw new NotImplementedException();
        }
        private IEnumerator Damaged(float attackForce, Vector2 knockbackDir, int attackDamage)
        {
            health -= attackDamage;
            float knockBackTime = 0;
            while (knockBackTime < 0.05f)
            {
                rigid2D.AddForce(knockbackDir.normalized * knockBackPower * attackForce);
                knockBackTime += Time.deltaTime;
                yield return null;
            }
            
        }

        internal bool IsHeading(Vector2 moveDir)
        {
            var moveDirLength = Mathf.Sqrt(moveDir.x * moveDir.x + moveDir.y * moveDir.y);
            var enemySize = boxCollider.size;
            var maxColliderSize = Mathf.Sqrt(enemySize.x * enemySize.x + enemySize.y * enemySize.y);
            var headingCheck = GetHeadingHit(moveDir, maxColliderSize + 0.03f);
            return headingCheck.collider != null;
        }
        internal bool IsOnPlatform(int platformCheckerPos)
        {
            var position = transform.position;
            var enemySize = boxCollider.size;
            var frontEnemyPos = new Vector2(position.x + platformCheckerPos * enemySize.x / 2, position.y);
            var onPlatformHit = Physics2D.BoxCast(frontEnemyPos, new Vector2(0.03f, enemySize.y), 0, Vector2.down, 0.02f, platformLayerMask);
            return onPlatformHit.collider != null;
        }

        private RaycastHit2D GetHeadingHit(Vector2 moveDir, float distance)
        {
            return Physics2D.BoxCast(rigid2D.position,boxCollider.size,0f, moveDir, distance, platformLayerMask);
        }
        private Vector2 GetForce(Vector2 moveDir, float accelerate)
        {
            return new Vector2(moveDir.x - rigid2D.velocity.x, moveDir.y - rigid2D.velocity.y) * accelerate;
        }
    }
}
