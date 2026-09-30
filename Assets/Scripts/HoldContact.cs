using UnityEngine;
using UnityEngine.InputSystem;

public class HoldContact : MonoBehaviour
{
    public float climbSpeed = 2.5f;
    public float fallY = -1f;
    public Vector3 startPosition = new Vector3(0f, 1f, -2f);

    public bool IsHanging { get; private set; }
    public bool IsClimbing => climbing;

    Rigidbody body;
    Hold currentHold;
    Hold climbFromHold;
    bool climbing;
    bool waitingForSpaceRelease;
    bool leftGround;
    bool hasRoute;
    RouteColor chosenRoute;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Ground>() != null)
        {
            if (leftGround)
            {
                ReturnToStart();
            }

            return;
        }

        Hold hold = collision.gameObject.GetComponent<Hold>();
        if (hold == null)
        {
            return;
        }

        Say("碰到 " + hold.name);

        if (IsWrongRoute(hold))
        {
            Say("路線失敗");
            ReturnToStart();
            return;
        }

        if (climbing && hold != climbFromHold)
        {
            HangOn(hold);
            return;
        }

        if (IsHanging && hold != currentHold)
        {
            HangOn(hold);
            return;
        }

        if (!climbing && !IsHanging)
        {
            currentHold = hold;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (climbing || IsHanging)
        {
            return;
        }

        Hold hold = collision.gameObject.GetComponent<Hold>();
        if (hold == null)
        {
            return;
        }

        currentHold = hold;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<Ground>() != null)
        {
            leftGround = true;
            return;
        }

        Hold hold = collision.gameObject.GetComponent<Hold>();
        if (hold == null || climbing || IsHanging || hold != currentHold)
        {
            return;
        }

        currentHold = null;
    }

    void FixedUpdate()
    {
        if (body.position.y < fallY)
        {
            Say("路線失敗");
            ReturnToStart();
            return;
        }

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        if (IsHanging)
        {
            body.useGravity = false;

            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
            {
                LetGo();
                return;
            }

            float sideways = 0f;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            {
                sideways -= 1f;
            }

            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            {
                sideways += 1f;
            }

            Vector3 hangVelocity = Vector3.zero;
            hangVelocity.x = sideways * climbSpeed;
            body.linearVelocity = hangVelocity;

            if (!keyboard.spaceKey.isPressed)
            {
                waitingForSpaceRelease = false;
                return;
            }

            if (waitingForSpaceRelease)
            {
                return;
            }

            StartClimb(currentHold);
            return;
        }

        if (!keyboard.spaceKey.isPressed)
        {
            if (climbing)
            {
                climbing = false;
                climbFromHold = null;
                currentHold = null;
            }

            return;
        }

        if (climbing)
        {
            ApplyClimbVelocity();
            return;
        }

        if (currentHold != null)
        {
            StartClimb(currentHold);
        }
    }

    void StartClimb(Hold hold)
    {
        if (IsWrongRoute(hold))
        {
            Say("路線失敗");
            ReturnToStart();
            return;
        }

        ChooseRoute(hold);
        climbing = true;
        IsHanging = false;
        waitingForSpaceRelease = false;
        climbFromHold = hold;
        currentHold = hold;
        body.useGravity = true;
        ApplyClimbVelocity();
    }

    void ApplyClimbVelocity()
    {
        Keyboard keyboard = Keyboard.current;
        float sideways = 0f;
        if (keyboard != null)
        {
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            {
                sideways -= 1f;
            }

            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            {
                sideways += 1f;
            }
        }

        Vector3 velocity = body.linearVelocity;
        velocity.x = sideways * climbSpeed;
        velocity.y = climbSpeed;
        velocity.z = 0f;
        body.linearVelocity = velocity;
    }

    void HangOn(Hold hold)
    {
        climbing = false;
        IsHanging = true;
        waitingForSpaceRelease = true;
        climbFromHold = null;
        currentHold = hold;
        body.useGravity = false;
        body.linearVelocity = Vector3.zero;
        ChooseRoute(hold);

        if (hold.isFinish)
        {
            Say("路線完成");
        }
    }

    void ChooseRoute(Hold hold)
    {
        if (hasRoute)
        {
            return;
        }

        hasRoute = true;
        chosenRoute = hold.routeColor;
        Say("選擇 " + chosenRoute);
    }

    bool IsWrongRoute(Hold hold)
    {
        return hasRoute && hold.routeColor != chosenRoute;
    }

    void Say(string message)
    {
        Debug.Log(message);
        GameMessage.Show(message);
    }

    void LetGo()
    {
        climbing = false;
        IsHanging = false;
        waitingForSpaceRelease = false;
        climbFromHold = null;
        currentHold = null;
        body.useGravity = true;
    }

    void ReturnToStart()
    {
        LetGo();
        hasRoute = false;
        leftGround = false;
        body.position = startPosition;
        body.linearVelocity = Vector3.zero;
    }
}
