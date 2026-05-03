using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PlayerMove3D_TopDownDialogue : MonoBehaviour
{
    [Header("引用")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private Transform visualRoot;
    [SerializeField] private Animator animator;
    [SerializeField] private Camera mainCamera;

    [Header("移动参数")]
    [SerializeField] private float moveSpeed = 4.5f;
    [SerializeField] private bool faceMoveDirection = true;

    [Header("额外锁定")]
    [SerializeField] private bool extraMoveLock = false;

    private Rigidbody rb;
    private Vector3 inputDir;
    private Vector3 moveDir;

    public bool CanMove
    {
        get
        {
            bool dialogueLock = dialogueManager != null && dialogueManager.IsPlaying;
            return !dialogueLock && !extraMoveLock;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Reset()
    {
        Rigidbody body = GetComponent<Rigidbody>();
        body.useGravity = false;
        body.isKinematic = true;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.constraints =
            RigidbodyConstraints.FreezePositionY |
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationY |
            RigidbodyConstraints.FreezeRotationZ;
    }

    private void Update()
    {
        if (!CanMove)
        {
            inputDir = Vector3.zero;
            moveDir = Vector3.zero;
            UpdateAnimator(0f);
            return;
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        inputDir = new Vector3(h, 0f, v).normalized;
        moveDir = inputDir;

        // UpdateFacing();
        UpdateAnimator(moveDir.magnitude);
    }

    private void FixedUpdate()
    {
        if (!CanMove) return;

        Vector3 targetPos = rb.position + moveDir * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPos);
    }

    // private void UpdateFacing()
    // {
    //     if (!faceMoveDirection || visualRoot == null) return;
    //     if (moveDir.sqrMagnitude < 0.0001f) return;
    //
    //     visualRoot.forward = moveDir;
    // }

    private void UpdateAnimator(float speedValue)
    {
        if (animator == null) return;
        animator.SetFloat("Speed", speedValue);
    }

    public void SetMoveLock(bool value)
    {
        extraMoveLock = value;
        if (value)
        {
            inputDir = Vector3.zero;
            moveDir = Vector3.zero;
            UpdateAnimator(0f);
        }
    }
}