using UnityEngine;

public class FallingAmmo : MonoBehaviour {
    private void Start() {
        // Rigidbody가 없으면 추가
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        
        // 크게 만들기
        transform.localScale = Vector3.one * 5f;
        
        // 3초 뒤 파괴
        Destroy(gameObject, 3f);
    }
}