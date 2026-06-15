using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/GunData", fileName = "Gun Data")]
public class GunData : ScriptableObject
{
    public AudioClip shotClip; // 발사 소리
    public AudioClip reloadClip; // 재장전 소리

    public float damage = 25; // 공격력

    public int startAmmoRemain = 100; // 처음에 주어질 전체 탄약
    public int maxAmmoRemain = 100; // 최대 수용 가능한 전체 탄약
    public int magCapacity = 25; // 탄창 용량

    public float timeBetFire = 0.12f; // 총알 발사 간격
    public float reloadTime = 1.8f; // 재장전 소요 시간

    public int bulletCount = 1; // 한 번에 발사되는 총알 수
    public float spreadAngle = 0f; // 탄퍼짐 각도
    public bool isExplosive = false; // 폭발 여부
    public float explosionRadius = 0f; // 폭발 반경
    public float shotRadius = 0f; // 발사 판정 두께
}