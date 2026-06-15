using UnityEngine;

// 플레이어 캐릭터를 사용자 입력에 따라 움직이는 스크립트
public class PlayerMovement : MonoBehaviour {
    public float moveSpeed = 5f; // 앞뒤 움직임의 속도
    public float rotateSpeed = 180f; // 좌우 회전 속도

    private Animator playerAnimator; // 플레이어 캐릭터의 애니메이터
    private PlayerInput playerInput; // 플레이어 입력을 알려주는 컴포넌트
    private Rigidbody playerRigidbody; // 플레이어 캐릭터의 리지드바디

    private void Start() {
        // 사용할 컴포넌트들의 참조를 가져오기
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
    }

    // FixedUpdate는 물리 갱신 주기에 맞춰 실행됨
    private void FixedUpdate() {
        // 회전 실행
        Rotate();
        // 움직임 실행
        Move();

        // 입력값의 크기에 따라 애니메이터의 Move 파라미터 값을 변경
        float moveMagnitude = new Vector2(playerInput.side, playerInput.move).magnitude;
        playerAnimator.SetFloat("Move", moveMagnitude);
    }

    // 입력값에 따라 캐릭터를 상하좌우로 움직임
    private void Move() {
        // 이동 방향 계산
        Vector3 moveDirection = new Vector3(playerInput.side, 0f, playerInput.move).normalized;
        // 상대적으로 이동할 거리 계산
        Vector3 moveDistance = moveDirection * moveSpeed * Time.deltaTime;
        // 리지드바디를 통해 게임 오브젝트 위치 변경
        playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);

        // 이동 중일 때 이동 방향을 바라보게 함 (공격 중이 아닐 때)
        if (!playerInput.fire && moveDirection.magnitude > 0.1f)
        {
            Quaternion newRotation = Quaternion.LookRotation(moveDirection);
            playerRigidbody.rotation = Quaternion.Slerp(playerRigidbody.rotation, newRotation, rotateSpeed * Time.deltaTime);
        }
    }

    // 캐릭터가 마우스 포인터 혹은 공격 방향을 바라보게 함
    private void Rotate() {
        if (playerInput.fire)
        {
            // 마우스 광선을 이용해 바닥과의 교차점 찾기
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;

            if (groundPlane.Raycast(ray, out rayDistance))
            {
                Vector3 lookPoint = ray.GetPoint(rayDistance);
                Vector3 lookDirection = lookPoint - transform.position;
                lookDirection.y = 0; // 높이 차이 무시

                if (lookDirection.magnitude > 0.1f)
                {
                    Quaternion newRotation = Quaternion.LookRotation(lookDirection);
                    playerRigidbody.rotation = newRotation;
                }
            }
        }
    }
}