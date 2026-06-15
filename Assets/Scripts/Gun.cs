using System.Collections;
using UnityEngine;

// 총을 구현한다
public class Gun : MonoBehaviour {
    // 총의 상태를 표현하는데 사용할 타입을 선언한다
    public enum State {
        Ready, // 발사 준비됨
        Empty, // 탄창이 빔
        Reloading // 재장전 중
    }

    public State state { get; private set; } // 현재 총의 상태

    public Transform fireTransform; // 총알이 발사될 위치

    public ParticleSystem muzzleFlashEffect; // 총구 화염 효과
    public ParticleSystem shellEjectEffect; // 탄피 배출 효과

    private LineRenderer bulletLineRenderer; // 총알 궤적을 그리기 위한 렌더러

    private AudioSource gunAudioPlayer; // 총 소리 재생기

    public GunData gunData; // 총의 현재 데이터
    
    private float fireDistance = 50f; // 사정거리

    public int ammoRemain = 100; // 남은 전체 탄약
    public int magAmmo; // 현재 탄창에 남아있는 탄약
    
    // 탄약 충전 메서드
    public void AddAmmo(int ammo) {
        // 탄창을 즉시 채움
        magAmmo = gunData.magCapacity;
        
        // 예비 탄약을 추가
        ammoRemain += ammo;

        // 총의 상태가 Empty였다면 Ready로 변경하여 즉시 발사 가능하게 함
        if (state == State.Empty)
        {
            state = State.Ready;
        }
    }
    
    private float lastFireTime; // 총을 마지막으로 발사한 시점
    
    private void Awake() {
        // 사용할 컴포넌트들의 참조를 가져오기
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineRenderer = GetComponent<LineRenderer>();

        // 사용할 점을 두개로 변경
        bulletLineRenderer.positionCount = 2;
        // 라인 렌더러를 비활성화
        bulletLineRenderer.enabled = false;
    }

    private void OnEnable() {
        // 전체 예비 탄약 양을 초기화
        ammoRemain = gunData.startAmmoRemain;
        // 현재 탄창을 가득채우기
        magAmmo = gunData.magCapacity;

        // 총의 현재 상태를 총을 쏠 준비가 된 상태로 변경
        state = State.Ready;
        // 마지막으로 총을 쏜 시점을 초기화
        lastFireTime = 0;
    }

    // 발사 시도
    public void Fire() {
        // 현재 상태가 발사 가능한 상태
        // && 마지막 총 발사 시점에서 timeBetFire 이상의 시간이 지남
        if (state == State.Ready && Time.time >= lastFireTime + gunData.timeBetFire)
        {
            // 마지막 총 발사 시점을 갱신
            lastFireTime = Time.time;
            // 실제 발사 처리 실행
            Shot();
        }
    }

    public GameObject trailPrefab; // 총알 궤적 프리팹
    public GameObject impactEffectPrefab; // 충돌 이펙트 프리팹

    private void Shot() {
        // 총알 수만큼 반복하여 발사
        for (int i = 0; i < gunData.bulletCount; i++)
        {
            // 탄퍼짐 계산
            Vector3 shotDirection = fireTransform.forward;
            if (gunData.spreadAngle > 0)
            {
                shotDirection = Quaternion.Euler(
                    Random.Range(-gunData.spreadAngle, gunData.spreadAngle),
                    Random.Range(-gunData.spreadAngle, gunData.spreadAngle),
                    0) * shotDirection;
            }

            // 레이캐스트에 의한 충돌 정보를 저장하는 컨테이너
            RaycastHit hit;
            // 총알이 맞은 곳을 저장할 변수
            Vector3 hitPosition = Vector3.zero;

            // 판정 두께에 따라 레이캐스트 또는 스피어캐스트 실행
            bool isHit = false;
            if (gunData.shotRadius > 0f) {
                isHit = Physics.SphereCast(fireTransform.position, gunData.shotRadius, shotDirection, out hit, fireDistance);
            } else {
                isHit = Physics.Raycast(fireTransform.position, shotDirection, out hit, fireDistance);
            }

            // 충돌 여부 확인
            if (isHit)
            {
                // 레이가 어떤 물체와 충돌한 경우

                // 폭발 무기인 경우
                if (gunData.isExplosive)
                {
                    Explode(hit.point);
                }
                else
                {
                    // 충돌한 상대방으로부터 IDamageable 오브젝트를 가져오기 시도
                    IDamageable target =
                        hit.collider.GetComponent<IDamageable>();

                    // 상대방으로 부터 IDamageable 오브젝트를 가져오는데 성공했다면
                    if (target != null)
                    {
                        // 상대방의 OnDamage 함수를 실행시켜서 상대방에게 데미지 주기
                        target.OnDamage(gunData.damage, hit.point, hit.normal);
                    }
                }

                // 레이가 충돌한 위치 저장
                hitPosition = hit.point;
            }
            else
            {
                // 레이가 다른 물체와 충돌하지 않았다면
                // 총알이 최대 사정거리까지 날아갔을때의 위치를 충돌 위치로 사용
                hitPosition = fireTransform.position +
                              shotDirection * fireDistance;
            }

            // 모든 총알에 대해 궤적 생성
            CreateTrail(fireTransform.position, hitPosition);
        }

        // 발사 이펙트 및 소리 재생 (전체 발사에 대해 한 번만)
        muzzleFlashEffect.Play();
        shellEjectEffect.Play();
        gunAudioPlayer.PlayOneShot(gunData.shotClip);

        // 남은 탄환의 수를 -1
        magAmmo--;
        if (magAmmo <= 0)
        {
            // 탄창에 남은 탄약이 없다면, 총의 현재 상태를 Empty으로 갱신
            state = State.Empty;
        }
    }

    private void CreateTrail(Vector3 start, Vector3 end) {
        if (trailPrefab != null) {
            GameObject trailObj = Instantiate(trailPrefab, Vector3.zero, Quaternion.identity);
            BulletTrail trail = trailObj.GetComponent<BulletTrail>();
            if (trail != null) {
                // 기존 bulletLineRenderer의 속성을 참고하거나 기본값 사용
                float width = bulletLineRenderer != null ? bulletLineRenderer.startWidth : 0.08f;
                Color startColor = bulletLineRenderer != null ? bulletLineRenderer.startColor : Color.yellow;
                Color endColor = bulletLineRenderer != null ? bulletLineRenderer.endColor : new Color(1, 0.5f, 0, 0.5f);
                trail.Setup(start, end, width, startColor, endColor);
            }
        }
    }

    private void Explode(Vector3 explosionPoint) {
        // 폭발 반경 내의 모든 콜라이더를 가져오기
        Collider[] colliders = Physics.OverlapSphere(explosionPoint, gunData.explosionRadius);

        // RPG용 특별 연출: 큰 Ammo가 떨어지는 연출
        if (impactEffectPrefab != null) {
            Instantiate(impactEffectPrefab, explosionPoint + Vector3.up * 2f, Quaternion.identity);
        }

        for (int i = 0; i < colliders.Length; i++)
        {
            // 충돌한 상대방으로부터 IDamageable 오브젝트를 가져오기 시도
            IDamageable target = colliders[i].GetComponent<IDamageable>();

            // 상대방으로 부터 IDamageable 오브젝트를 가져오는데 성공했다면
            if (target != null)
            {
                // 상대방의 OnDamage 함수를 실행시켜서 상대방에게 데미지 주기
                target.OnDamage(gunData.damage, explosionPoint, (colliders[i].transform.position - explosionPoint).normalized);
            }
        }
    }

    // 기존 ShotEffect 코루틴은 이제 사용하지 않음 (CreateTrail로 대체)
    // 하지만 호환성을 위해 남겨두거나 삭제 가능
    private IEnumerator ShotEffect(Vector3 hitPosition) {
        yield break;
    }

    // 재장전 시도
    public bool Reload() {
        if (state == State.Reloading ||
            ammoRemain <= 0 || magAmmo >= gunData.magCapacity)
        {
            // 이미 재장전 중이거나, 남은 총알이 없거나
            // 탄창에 총알이 이미 가득한 경우 재장전 할수 없다
            return false;
        }

        // 재장전 처리 시작
        StartCoroutine(ReloadRoutine());
        return true;
    }

    // 실제 재장전 처리를 진행
    private IEnumerator ReloadRoutine() {
        // 현재 상태를 재장전 중 상태로 전환
        state = State.Reloading;
        // 재장전 소리 재생
        gunAudioPlayer.PlayOneShot(gunData.reloadClip);

        // 재장전 소요 시간 만큼 처리를 쉬기
        yield return new WaitForSeconds(gunData.reloadTime);

        // 탄창에 채울 탄약을 계산한다
        int ammoToFill = gunData.magCapacity - magAmmo;

        // 탄창에 채워야할 탄약이 남은 탄약보다 많다면,
        // 채워야할 탄약 수를 남은 탄약 수에 맞춰 줄인다
        if (ammoRemain < ammoToFill)
        {
            ammoToFill = ammoRemain;
        }

        // 탄창을 채운다
        magAmmo += ammoToFill;
        // 남은 탄약에서, 탄창에 채운만큼 탄약을 뺸다
        ammoRemain -= ammoToFill;

        // 총의 현재 상태를 발사 준비된 상태로 변경
        state = State.Ready;
    }
}