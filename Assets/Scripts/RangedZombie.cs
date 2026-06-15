using System.Collections;
using UnityEngine;

public class RangedZombie : Zombie {
    public GameObject projectilePrefab; // 발사체 프리팹
    public Transform fireTransform; // 발사 위치
    public float attackRange = 3f; // 공격 사거리

    protected override IEnumerator UpdatePath() {
        while (!dead) {
            if (hasTarget) {
                float distance = Vector3.Distance(transform.position, targetEntity.transform.position);

                if (distance <= attackRange) {
                    // 사거리 안이면 정지하고 공격
                    navMeshAgent.isStopped = true;
                    LookAtTarget();
                    
                    if (Time.time >= lastAttackTime + timeBetAttack) {
                        lastAttackTime = Time.time;
                        StartCoroutine(ShootBurst());
                    }
                } else {
                    // 사거리 밖이면 추적
                    navMeshAgent.isStopped = false;
                    navMeshAgent.SetDestination(targetEntity.transform.position);
                }
            } else {
                navMeshAgent.isStopped = true;
                // 타겟 찾기 (부모 로직 활용)
                Collider[] colliders = Physics.OverlapSphere(transform.position, 20f, whatIsTarget);
                for (int i = 0; i < colliders.Length; i++) {
                    LivingEntity livingEntity = colliders[i].GetComponent<LivingEntity>();
                    if (livingEntity != null && !livingEntity.dead) {
                        targetEntity = livingEntity;
                        break;
                    }
                }
            }
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void LookAtTarget() {
        Vector3 direction = (targetEntity.transform.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 10f * Time.deltaTime);
        }
    }

    private IEnumerator ShootBurst() {
        // 3발 발사
        for (int i = 0; i < 3; i++) {
            if (dead || !hasTarget) yield break;

            GameObject projectile = Instantiate(projectilePrefab, fireTransform.position, fireTransform.rotation);
            ZombieProjectile projScript = projectile.GetComponent<ZombieProjectile>();
            
            Vector3 shootDir = (targetEntity.transform.position + Vector3.up * 1f - fireTransform.position).normalized;
            projScript.Setup(shootDir, damage);

            yield return new WaitForSeconds(0.1f); // 3발 사이 간격
        }
    }
}