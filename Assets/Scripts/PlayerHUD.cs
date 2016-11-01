using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHUD : MonoBehaviour {
    public GameObject assignedPlayer;
    private PlayerController control;
    private CombatController combat;
    private MovementController movement;

    public RectTransform weaponTracker;
    private Image weaponTrackerIcon;
    public RectTransform aimCursor;
    private Image aimCursorIcon;

    private Camera myCamera;
    RectTransform canvasRect;

    void Start()
    {
        myCamera = transform.parent.parent.GetComponentInChildren<Camera>();
        canvasRect = transform.parent.GetComponent<RectTransform>();
        //canvasRect.GetComponent<Canvas>().scaleFactor = myCamera.rect.width;
        weaponTrackerIcon = weaponTracker.GetComponent<Image>();
        aimCursorIcon = aimCursor.GetComponent<Image>();
        if(assignedPlayer != null)
        {
            combat = assignedPlayer.GetComponent<CombatController>();
            movement = assignedPlayer.GetComponent<MovementController>();
            control = assignedPlayer.GetComponent<PlayerController>();
        }
    }

    void Update()
    {
        if(combat.lastUsedWeapon != null)
        {
            weaponTrackerIcon.enabled = true;
            weaponTracker.anchoredPosition = WorldToCanvasPoint(combat.lastUsedWeapon.position);
        }
        else
        {
            weaponTrackerIcon.enabled = false;
        }

        if(control.controlType == PlayerController.ControlType.Controller)
        {
            if (control.lookDirection.x != 0)
            {
                aimCursor.anchoredPosition = control.lookDirection * 50.0f;
                //aimCursor.forward = new Vector3(control.lookDirection.x, 90.0f ,control.lookDirection.z);
                aimCursor.localRotation = Quaternion.LookRotation(Vector3.forward, -control.lookDirection);
            }
        }
        else
        {
            aimCursorIcon.enabled = false;
        }
    }

    public Vector2 WorldToCanvasPoint(Vector3 target)
    {
        Vector3 targetPoint = myCamera.WorldToViewportPoint(target);
        Vector2 screenPosition = new Vector2(
            Mathf.Clamp((targetPoint.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f), 0 - canvasRect.sizeDelta.x * 0.5f + 20, canvasRect.sizeDelta.x * 0.5f - 20),
            Mathf.Clamp((targetPoint.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f), 0 - canvasRect.sizeDelta.y * 0.5f + 20, canvasRect.sizeDelta.y * 0.5f - 20));
        return screenPosition;
    }
}
