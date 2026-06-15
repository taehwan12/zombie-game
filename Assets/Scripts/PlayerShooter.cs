using UnityEngine;

// 주어진 Gun 오브젝트를 쏘거나 재장전
// 알맞은 애니메이션을 재생하고 IK를 사용해 캐릭터 양손이 총에 위치하도록 조정
public class PlayerShooter : MonoBehaviour {
    public Gun gun; // 사용할 총
    public Gun defaultGunPrefab; // 기본 무기 프리팹
    public Transform gunPivot; // 총 배치의 기준점
    public Transform leftHandMount; // 총의 왼쪽 손잡이, 왼손이 위치할 지점
    public Transform rightHandMount; // 총의 오른쪽 손잡이, 오른손이 위치할 지점

    private PlayerInput playerInput; // 플레이어의 입력
    private Animator playerAnimator; // 애니메이터 컴포넌트
    private Coroutine weaponTimeoutCoroutine; // 무기 제한 시간 코루틴

    private void Start() {
        // 사용할 컴포넌트들을 가져오기
        playerInput = GetComponent<PlayerInput>();
        playerAnimator = GetComponent<Animator>();
    }

    private void OnEnable() {
        // 슈터가 활성화될 때 총도 함께 활성화
        gun.gameObject.SetActive(true);
    }

    private void OnDisable() {
        // 슈터가 비활성화될 때 총도 함께 비활성화
        gun.gameObject.SetActive(false);
    }

    private void Update() {
        // 입력을 감지하고 총 발사하거나 재장전
        if (playerInput.fire)
        {
            // 발사 입력 감지시 총 발사
            if (gun != null) gun.Fire();
        }
        else if (playerInput.reload)
        {
            // 재장전 입력 감지시 재장전
            if (gun != null && gun.Reload())
            {
                // 재장전 성공시에만 재장전 애니메이션 재생
                playerAnimator.SetTrigger("Reload");
            }
        }

        // 남은 탄약 UI를 갱신
        UpdateUI();
    }

    // 새로운 총으로 교체하는 메서드
    public void ChangeGun(Gun newGunPrefab, bool isTemporary = true) {
        if (gun != null)
        {
            // 기존 총이 있다면 파괴
            Destroy(gun.gameObject);
        }

        // 새로운 총을 Gun Pivot의 자식으로 생성
        gun = Instantiate(newGunPrefab, gunPivot);
        gun.gameObject.name = newGunPrefab.name;
        gun.transform.localPosition = Vector3.zero;
        gun.transform.localRotation = Quaternion.identity;

        // 새로운 총의 IK 위치 설정
        // 총 프리팹 내부에 설정된 Handle들을 찾아서 연결
        Transform leftHandle = gun.transform.Find("Left Handle");
        Transform rightHandle = gun.transform.Find("Right Handle");

        if (leftHandle != null) leftHandMount = leftHandle;
        if (rightHandle != null) rightHandMount = rightHandle;

        // 이전에 실행 중이던 타이머가 있다면 정지
        if (weaponTimeoutCoroutine != null)
        {
            StopCoroutine(weaponTimeoutCoroutine);
            weaponTimeoutCoroutine = null;
        }

        // 임시 무기인 경우 10초 타이머 시작
        // 기본 무기로 교체할 때는 isTemporary를 false로 전달해야 함
        if (isTemporary && defaultGunPrefab != null && newGunPrefab != defaultGunPrefab)
        {
            weaponTimeoutCoroutine = StartCoroutine(WeaponTimeoutRoutine());
        }
        
        // 총이 교체되었으므로 UI 갱신
        UpdateUI();
    }

    private System.Collections.IEnumerator WeaponTimeoutRoutine() {
        // 10초 대기
        yield return new WaitForSeconds(10f);
        
        // 기본 무기로 복구 (임시 아님)
        if (defaultGunPrefab != null)
        {
            ChangeGun(defaultGunPrefab, false);
        }
        
        weaponTimeoutCoroutine = null;
    }

    // 탄약 UI 갱신
    private void UpdateUI() {
        if (gun != null && UIManager.instance != null)
        {
            // UI 매니저의 탄약 텍스트에 탄창의 탄약과 남은 전체 탄약을 표시
            UIManager.instance.UpdateAmmoText(gun.magAmmo, gun.ammoRemain);
        }
    }

    // 애니메이터의 IK 갱신
    private void OnAnimatorIK(int layerIndex) {
        // 총의 기준점 gunPivot을 3D 모델의 오른쪽 팔꿈치 위치로 이동
        gunPivot.position =
            playerAnimator.GetIKHintPosition(AvatarIKHint.RightElbow);

        // IK를 사용하여 왼손의 위치와 회전을 총의 오른쪽 손잡이에 맞춘다
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

        playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand,
            leftHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand,
            leftHandMount.rotation);

        // IK를 사용하여 오른손의 위치와 회전을 총의 오른쪽 손잡이에 맞춘다
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

        playerAnimator.SetIKPosition(AvatarIKGoal.RightHand,
            rightHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.RightHand,
            rightHandMount.rotation);
    }
}