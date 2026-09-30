using UnityEngine;
using UnityEngine.InputSystem;

public class WalkTowardWall : MonoBehaviour
{
    public float speed = 1.5f;

    Rigidbody body;
    HoldContact holdContact;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        holdContact = GetComponent<HoldContact>();
    }

    void FixedUpdate()
    {
        if (holdContact != null && (holdContact.IsHanging || holdContact.IsClimbing))
        {
            return;
        }

        Keyboard keyboard = Keyboard.current;
        Vector3 direction = Vector3.zero;

        if (keyboard != null)
        {
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
            {
                direction.z += 1f;
            }

            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
            {
                direction.z -= 1f;
            }

            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            {
                direction.x -= 1f;
            }

            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            {
                direction.x += 1f;
            }
        }

        Vector2 stick = MoveStick.Axis;
        direction.x += stick.x;
        direction.z += stick.y;
        if (direction.sqrMagnitude > 1f)
        {
            direction.Normalize();
        }

        Vector3 velocity = body.linearVelocity;
        velocity.x = direction.x * speed;
        velocity.z = direction.z * speed;
        body.linearVelocity = velocity;
    }
}
