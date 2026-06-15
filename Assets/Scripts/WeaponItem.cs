using UnityEngine;

// 획득 시 플레이어의 무기를 교체하는 아이템
public class WeaponItem : MonoBehaviour, IItem {
    public Gun weaponPrefab; // 교체할 무기 프리팹

    public void Use(GameObject target) {
        // 전달 받은 게임 오브젝트로부터 PlayerShooter 컴포넌트를 가져오기 시도
        PlayerShooter playerShooter = target.GetComponent<PlayerShooter>();

        // PlayerShooter 컴포넌트가 존재하면 무기 교체 실행
        if (playerShooter != null && weaponPrefab != null)
        {
            // "Smoke" 무기인 경우 무적 기능만 발동하고 무기 교체는 하지 않음
            if (weaponPrefab.name == "Smoke")
            {
                PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.SetInvulnerable(5f);
                }
            }
            else
            {
                // 일반 무기인 경우 무기 교체 실행
                playerShooter.ChangeGun(weaponPrefab);
            }
        }

        // 사용되었으므로, 자신을 파괴
        Destroy(gameObject);
    }
}