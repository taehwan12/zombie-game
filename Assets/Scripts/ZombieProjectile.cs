using UnityEngine;

public class ZombieProjectile : MonoBehaviour {
    public float speed = 10f;
    public float damage = 20f;
    private Vector3 direction;

    public void Setup(Vector3 shootDirection, float shootDamage) {
        direction = shootDirection;
        damage = shootDamage;
        // 5초 뒤 자동 파괴
        Destroy(gameObject, 5f);
    }

    private void Update() {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other) {
        // 플레이어 캐릭터와 충돌했는지 확인
        if (other.CompareTag("Player")) {
            IDamageable target = other.GetComponent<IDamageable>();
            if (target != null) {
                target.OnDamage(damage, transform.position, -direction);
            }
            Destroy(gameObject);
        }
        // 환경과 충돌해도 파괴 (Floor, Wall 등)
        else if (other.gameObject.layer == LayerMask.NameToLayer("Default")) {
            Destroy(gameObject);
        }
    }
}