using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.Windows;
using UnityEngine.UI;

public class Aiming : MonoBehaviour
{
    protected GrappleInput input;

    [Header("===== Size =====")]
    [SerializeField] protected float range = 30;
    [SerializeField] protected float radius = 0.5f;

    [Header("===== Cursor =====")]
    [SerializeField] protected GameObject worldCursor;
    [SerializeField] protected float cursorSensitivity = .003f;
    [SerializeField] protected Image screenSpaceCursor;

    protected float screen_x;
    protected float screen_y;

    protected Color defaultColor;

    protected bool validTargetHovered = false;

    private void Start()
    {
        input = new GrappleInput();
        input.Enable();

        Vector3 bounds = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 4));
        screen_x = Mathf.Abs(Camera.main.transform.position.x - bounds.x);
        screen_y = Mathf.Abs(Camera.main.transform.position.y - bounds.y);
        
        print("Cam Bounds: " + screen_x + " : " + screen_y);

        StartCoroutine("SwivelCursor");

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        defaultColor = screenSpaceCursor.color;
    }

    public void Scan(out RaycastHit hit)
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(worldCursor.transform.position);
        Ray ray = Camera.main.ViewportPointToRay(viewPos);
        string[] mask = { "Grapple", "Claw" };

        if (Physics.SphereCast(ray, radius, out hit, range, LayerMask.GetMask(mask)))
        {
            validTargetHovered = true;
            SafeSetScreenSpaceCursorColor( Color.green);
            CursorSnap(hit);
        }
        else
        {
            validTargetHovered = false;
            SafeSetScreenSpaceCursorColor(defaultColor);
        }
    }

    protected void CursorSnap(RaycastHit hit)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(hit.point);

        print("Aim " + screenPos);

        SafeSetScreenSpaceCursorPos( screenPos);
    }

    /// <summary>
    /// Translate the cursor represented by a canvas object.
    /// </summary>
    protected IEnumerator SwivelCursor()
    {
        // Skip the first frame since it usually has a large offset.
        yield return null;

        while (true)
        {
            Vector3 offset = input.Grapple.SwivelCursor.ReadValue<Vector2>();
            offset *= cursorSensitivity * Time.timeScale;
            Vector3.ProjectOnPlane(offset, Camera.main.transform.forward);
            worldCursor.transform.localPosition += new Vector3(offset.x, offset.y, 0);

            Vector3 correction = worldCursor.transform.localPosition;
            if (worldCursor.transform.localPosition.x > screen_x)
            {
                correction.x = screen_x;
            }
            if (worldCursor.transform.localPosition.x < -screen_x)
            {
                correction.x = -screen_x;
            }
            if (worldCursor.transform.localPosition.y > screen_y)
            {
                correction.y = screen_y;
            }
            if (worldCursor.transform.localPosition.y < -screen_y)
            {
                correction.y = -screen_y;
            }

            worldCursor.transform.localPosition = correction;

            if (!validTargetHovered)
            {
                print("Aim swivel ");
                SafeSetScreenSpaceCursorPos( Camera.main.WorldToScreenPoint(worldCursor.transform.position));
            }

            yield return null;
        }
    }


    protected void SafeSetScreenSpaceCursorPos(Vector3 pos)
    {
        if(screenSpaceCursor != null)
        {
            screenSpaceCursor.rectTransform.position = pos;
        }
    }

    protected void SafeSetScreenSpaceCursorColor(Color color)
    {
        if (screenSpaceCursor != null)
        {
            screenSpaceCursor.color = color;
        }
    }

    public void SetRange(float inRange)
    {
        range = inRange;
    }
}
