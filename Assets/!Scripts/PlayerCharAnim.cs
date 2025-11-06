using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerCharAnim : MonoBehaviour
{
    public Animator animator;
    public string speedParam = "Speed"; // must match your Animator parameter name

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Check if any movement key is pressed
        bool isMoving = Input.GetKey(KeyCode.W) ||
                        Input.GetKey(KeyCode.A) ||
                        Input.GetKey(KeyCode.S) ||
                        Input.GetKey(KeyCode.D);

        // Immediately set speed to 1 or 0
        animator.SetFloat(speedParam, isMoving ? 1f : 0f);
    }
}
