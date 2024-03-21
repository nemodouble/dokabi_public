using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Boss.Phase;
using Character;
using FMODUnity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Boss
{
    [RequireComponent(typeof(SpriteFlasher))]
    public abstract class BossController : MonoBehaviour, IHitAble
    {
        internal Rigidbody2D Rigid2D;
        internal Animator Animator;
        private BoxCollider2D boxCollider;
        
        protected ParticleSystem hitPS;
        protected ParticleSystem deadPS;
        
        private SpriteFlasher spriteFlasher;
        private GameObject dangerRange;
        
        protected BossPhase DeadPhase;
        private bool isDead;
        private Dictionary<string, int> phaseSelectCount = new Dictionary<string, int>();
        private Dictionary<string, int> phaseRecentness = new Dictionary<string, int>();
        private int phaseCountSum = 100;
        [SerializeField] public string prevPhase = "";
        private Queue<string> prevPhaseQueue = new Queue<string>();

        protected GameObject Player;

        public EventReference hitEvent;
        public EventReference dead;
        
        [SerializeField]
        internal int health;
        protected virtual void Start()
        {
            Rigid2D = GetComponent<Rigidbody2D>();
            Animator = GetComponent<Animator>();
            boxCollider = GetComponent<BoxCollider2D>();
            spriteFlasher = GetComponent<SpriteFlasher>();
            Player = GameObject.Find("Player");
            dangerRange = transform.Find("DangerRange").gameObject;
            
            StartCoroutine(Action());
        }

        protected IEnumerator Action()
        {
            while (!isDead)
            {
                var ableStateList = GetAblePhaseList();
                var nowPhase = SelectPhase(ableStateList);
                prevPhase = nowPhase.PhaseName;
                yield return nowPhase.DoPhase(this);
            }
        }

        public IEnumerator Hit(int damage, Vector2 attackDir, float attackForceScale)
        {
            health -= damage;
            if (health <= 0)
            {
                yield return Dead();
            }
            else
            {
                if (hitPS != null)
                {
                    var angle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
                    var hitPSTransform = hitPS.transform;
                    hitPSTransform.position = transform.position;
                    hitPSTransform.eulerAngles = new Vector3(0, 0, angle);
                    hitPS.Play();
                }

                spriteFlasher.Flash();
                if (!hitEvent.IsNull)
                {
                    RuntimeManager.PlayOneShot(hitEvent);
                }
            }
        }

        protected virtual IEnumerator Dead()
        {
            isDead = true;
            prevPhase = DeadPhase.PhaseName;
            dangerRange.SetActive(false);
            deadPS.Play();
            RuntimeManager.PlayOneShot(dead);
            yield return DeadPhase.DoPhase(this);
        }
        
        internal RaycastHit2D IsHeading(Vector2 moveDir)
        {
            var length = Mathf.Sqrt(moveDir.x * moveDir.x + moveDir.y * moveDir.y);
            return IsHeading(moveDir, length);
        }
        internal RaycastHit2D IsHeading(Vector2 moveDir, float distance)
        {
            LayerMask platformLayer = LayerMask.GetMask("Platform");
            return Physics2D.BoxCast(transform.position, boxCollider.size, 0f, moveDir, distance, platformLayer);
        }
        protected Vector2 GetBossToPlayerDir()
        {
            throw new NotImplementedException();
        }

        internal void CallInstantiate(GameObject instantiateGameObject, Vector3 relativePos)
        {
            var summonPos = transform.position + (Vector3)relativePos;
            Instantiate(instantiateGameObject, summonPos, Quaternion.identity);
        }

        private BossPhase SelectPhase(IReadOnlyList<BossPhase> phases)
        {
            var recentness = new int[phases.Count];
            var recentRank = new int[phases.Count];
            var max = 0;
            if (phases.Count == 1)
                return phases[0];
            
            // 페이즈가 얼마나 최근에 선택됐는지(최신성) 순위를 매김 
            for (var i = 0; i < phases.Count; i++)
            {
                if(!phaseRecentness.TryGetValue(phases[i].PhaseName, out recentness[i]))
                {
                    phaseRecentness.Add(phases[i].PhaseName, 0);
                }
                if(phaseRecentness[phases[i].PhaseName] > max)
                {
                    max = phaseRecentness[phases[i].PhaseName];
                    for (var j = 0; j < phases.Count; j++)
                    {
                        recentRank[j] += 1;
                    }
                    recentRank[i] = 0;
                }
                else
                {
                    recentRank[i] = i;
                }
            }
            
            // 해당 페이즈 선택될 random값의 범위를 설정함
            var phaseRandomRange = new float[phases.Count];
            var phaseSelectedCount = new int[phases.Count];
            var str = ""; //debug
            for(var i =0; i< phases.Count; i++)
            {
                if (!phaseSelectCount.TryGetValue(phases[i].PhaseName, out phaseSelectedCount[i]))
                    phaseSelectCount.Add(phases[i].PhaseName, 0);
                phaseRandomRange[i] = (float)(phaseCountSum - 2 * phaseSelectedCount[i]) // 페이즈가 선택된 횟수 반영
                    / phases[i].Rarity * // 페이즈별 희귀도 반영
                    ((float) recentRank[i] / phases.Count) + 1; // 페이즈 최신성랭킹 반영

                str += phases[i].PhaseName + " " + phaseRandomRange[i] + " / ";//debug
            }
            
            // 선택된 random값에 맞는 패턴을 실행함
            var randVal = Random.Range(0, phaseRandomRange.Sum());
            str += "\n" + randVal + " : ";//debug
            var sum = 0f;
            for (var i = 0; i < phaseRandomRange.Length; i++)
            {
                sum += phaseRandomRange[i];
                if (randVal < sum)
                {
                    phaseCountSum += 1;
                    phaseSelectCount[phases[i].PhaseName] += 1;
                    phaseRecentness[phases[i].PhaseName] = phaseCountSum;
                    str += phases[i].PhaseName;//debug
                    //Debug.Log(str);//debug
                    return phases[i];
                }
            }
            Debug.Log("패턴 선택 오류");
            return phases[0];
        }
        
        /// <summary>
        /// 액션에서 이 함수를 호출해 리스트 중 하나를 선택해 실행합니다.
        /// </summary>
        /// <returns>가능한 페이즈 리스트 목록</returns>
        protected abstract List<BossPhase> GetAblePhaseList();

    }
}
