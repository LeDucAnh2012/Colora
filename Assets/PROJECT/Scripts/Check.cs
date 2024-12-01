using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Check : MonoBehaviour
{

    [SerializeField] private Camera mainCamera;
    [SerializeField] private float zoomModifierSpeed = 0.1f;
    [SerializeField] private float speedMove = 0.05f;
    [SerializeField] private float speedAutozoom = 0.5f;

    public bool isTouch = true;
    public bool isSetColor = false;
    public bool isCheckSetColor = false;
    public bool isPaintColor = true; // có dc phép tô màu hay ko
    public bool isClaimPos = false;

    public float maxSizeCam = 50;
    public float sizeAutoZoom = 35;

    public float minSizeCam = 2;
    public float radiusPoint = 0.3f;
    public float cameraMoveSpeed;
    public float speedLerp = 5;

    public Vector3 targetPosition;

    private Vector2 firstTouchPrevPos, secondTouchPrevPos;
    private Vector2 startPos;
    private Vector2 startMove;
    private Vector2 beginPosPaint;

    private bool isAutoZoom = false; // tự động zoom
    private bool isCanMove = true; // cam có thể dc di chuyển hay chưa
    private bool isMoveCam = true; // cam đang di chuyển hay dừng
    private bool isClickDown = true; // click có trúng shape chưa tô màu ko
    private bool isMoving = false; // check cam đang di chuyển
    private bool isPainting = true; // có đang kéo tô màu hay ko
    private bool isZoooming = false;

    private float touchesPrevPosDifference;
    private float touchesCurPosDifference;
    private float zoomModifier;
    private float valZoomEditor = 20;
    private float distanceBetweenPoints = 0.4f;

    private EventSystem eventSystem;
    private string layerShape = "ShapePixel";
    private int idShapeClickDown = -1;
    private IEnumerator IE_AUTO_ZOOM = null;

    private void Start()
    {
        eventSystem = EventSystem.current;
        ResetCam();
    }
    public void ResetCam()
    {
        mainCamera.transform.position = new Vector3(0, 0, -10);
        mainCamera.orthographicSize = maxSizeCam;
        valSpeedZoom = 8;
    }
    private float valSpeedZoom = 4;
    void Update()
    {

        if (Input.touchCount == 2)
        {
            Touch firstTouch = Input.GetTouch(0);
            Touch secondTouch = Input.GetTouch(1);

            firstTouchPrevPos = firstTouch.position - firstTouch.deltaPosition;
            secondTouchPrevPos = secondTouch.position - secondTouch.deltaPosition;

            touchesPrevPosDifference = (firstTouchPrevPos - secondTouchPrevPos).magnitude;
            touchesCurPosDifference = (firstTouch.position - secondTouch.position).magnitude;

            zoomModifier = (firstTouch.deltaPosition - secondTouch.deltaPosition).magnitude * zoomModifierSpeed / valSpeedZoom;

            if (touchesPrevPosDifference > touchesCurPosDifference) // thu nho
            {
                isCheckSetColor = true;
                isZoooming = true;
                mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, mainCamera.orthographicSize + zoomModifier, 0.5f);

                if (mainCamera.orthographicSize >= 25)
                    mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, Time.deltaTime * cameraMoveSpeed);
            }

            if (touchesPrevPosDifference < touchesCurPosDifference) // phong to
            {
                isAutoZoom = false;
                isCheckSetColor = true;
                isZoooming = true;

                mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, mainCamera.orthographicSize - zoomModifier, 0.5f);
            }

            if ((int)mainCamera.orthographicSize % 5 == 0 && isCheckSetColor)
                isSetColor = true;

            isPaintColor = false;
        }
        else
            isZoooming = false;

        // set color

        if (isTouch)
            ControlMove();
        mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, minSizeCam, maxSizeCam);
    }

   

    public bool IsPointerOverGameObject()
    {
        //check mouse
#if UNITY_EDITOR
        if (eventSystem.IsPointerOverGameObject())
            return true;
#else
        //check touch
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            if (eventSystem.IsPointerOverGameObject(Input.touches[0].fingerId))
                return true;
        }
#endif

        return false;
    }
    private void ControlMove()
    {

        if (ActionHelper.IsEditor())
        {
            if (Input.GetMouseButtonDown(0))
                BeginMove(Input.mousePosition);
            if (isMoving && Input.GetMouseButton(0))
                Move(Input.mousePosition);
            if (Input.GetMouseButtonUp(0))
                EndMove(Input.mousePosition);


            if (mainCamera.orthographic)
            {
                var val = mainCamera.orthographicSize - Input.GetAxis("Mouse ScrollWheel") * valZoomEditor;
                mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, val, 0.5f);

                if ((int)mainCamera.orthographicSize % 5 == 0)
                    isSetColor = true;

                if (mainCamera.orthographicSize >= 25 && isClaimPos)
                    mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, Time.deltaTime * cameraMoveSpeed);
            }
            else
                mainCamera.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * valZoomEditor;
        }
        else
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                    BeginMove(touch.position);
                else if (touch.phase == TouchPhase.Moved && Input.touchCount < 2 && isMoving)
                    Move(touch.position);
                else if (touch.phase == TouchPhase.Ended)
                    EndMove(touch.position);
            }
        }
    }

    #region MoveCam

    private void BeginMove(Vector3 posMouse)
    {
        beginPosPaint = mainCamera.ScreenToWorldPoint(posMouse);
        startPos = posMouse;
        startMove = transform.position;

        isMoveCam = true;
        isClickDown = false;
        isPainting = false;
        if (mainCamera.orthographicSize > sizeAutoZoom)
            isAutoZoom = true;

        if (IsPointerOverGameObject())
            return;

        isPaintColor = true;

        isMoving = true;
        RaycastHit2D hit = Physics2D.Raycast(beginPosPaint, Vector2.down, 0.01f, LayerMask.GetMask(layerShape));
    }
    private void Move(Vector3 posMouse)
    {
        if (isMoveCam && startPos != (Vector2)posMouse)
        {
            if (isAutoZoom && isCanMove)
            {
                isCanMove = false;

                // audo zoom
                StartCoroutine(IE_AutoZoomCam());
                return;
            }

            if (IE_AUTO_ZOOM != null)
                StopCoroutine(IE_AUTO_ZOOM);

            isPaintColor = false;
            isCanMove = true;

            if (mainCamera.orthographicSize >= 25)
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, Time.deltaTime * cameraMoveSpeed);
            else
            {

                var pos = -((Vector2)posMouse - startPos) * speedMove * mainCamera.orthographicSize / maxSizeCam + startMove;

                if (!isZoooming)
                {
                    transform.position = new Vector3(pos.x, pos.y, -10);
                }
            }
        }

        else
        {
            if (startPos != (Vector2)posMouse)
            {
                // paint color
                isPainting = true;
                PaintColorPixel();
            }
        }
    }
    private void EndMove(Vector2 point)
    {

        if (IsPointerOverGameObject())
            return;

        point = mainCamera.ScreenToWorldPoint(point);
       
        isMoving = false;
        idShapeClickDown = -1;
    }

    #endregion

    private void PaintColorPixel()
    {
        var posCurrent = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastPoint(posCurrent);

        Vector2 AB = new Vector2(posCurrent.x - beginPosPaint.x, posCurrent.y - beginPosPaint.y);
        Vector2 unitAB = AB.normalized;

        float totalDistance = Vector2.Distance(beginPosPaint, posCurrent);
        int numberOfPoints = Mathf.FloorToInt(totalDistance / distanceBetweenPoints);

        for (int i = 0; i <= numberOfPoints; i++)
        {
            Vector2 newPosition = (Vector2)beginPosPaint + unitAB * distanceBetweenPoints * i;
            PaintColorCircle(newPosition, 0.2f, false, true);
        }

        beginPosPaint = mainCamera.ScreenToWorldPoint(Input.mousePosition);

    }
    private void PaintColorCircle(Vector2 point, float radius, bool isPaintAllColor = true, bool isSound = false)
    {
        RaycastHit2D hitPoint = Physics2D.Raycast(point, Vector2.down, 0.01f, LayerMask.GetMask(layerShape));
        if (hitPoint.collider != null)
            point = hitPoint.collider.gameObject.GetComponent<ElementShape>().transform.position;
        else
            return;

        RaycastHit2D[] hit = Physics2D.CircleCastAll(point, radius, Vector2.down, 0.01f, LayerMask.GetMask(layerShape));
        var listEl = new List<ElementShape>();

        if (hit != null)
        {
            if (hit.Length > 0)
            {
                for (int i = 0; i < hit.Length; i++)
                {
                    var eleShape = hit[i].collider.gameObject.GetComponent<ElementShape>();
                    if (eleShape != null)
                    {
                        listEl.Add(eleShape);

                    
                    }
                }
            }
        }
    }
    private ElementShape GetEleRaycastPoint(Vector2 point)
    {
        RaycastHit2D hit = Physics2D.Raycast(point, Vector2.down, 0.01f, LayerMask.GetMask(layerShape));
        if (hit.collider != null)
        {
            var eleShape = hit.collider.gameObject.GetComponent<ElementShape>();
            if (eleShape != null)
                return eleShape;
        }
        return null;
    }
    private void RaycastPoint(Vector2 point)
    {
        if (!isPaintColor) return;

        var ele = GetEleRaycastPoint(point);
    }

    private IEnumerator IE_AutoZoomCam()
    {
        while (mainCamera.orthographicSize > 8)
        {
            var lerpTimer = Time.deltaTime * speedLerp;
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, 7.5f, lerpTimer);// -= speedAutozoom;
            if ((int)mainCamera.orthographicSize % 5 == 0)
                isCheckSetColor = true;
            yield return new WaitForEndOfFrame();
        }
        isCanMove = true;
        isAutoZoom = false;
    }

    private IEnumerator IE_AutoZoomCam(float size, Vector3 posZoom)
    {
        isCanMove = false;
        isAutoZoom = true;

        if (mainCamera.orthographicSize > size)
        {
            while (mainCamera.orthographicSize >= size)
            {
                var siz = mainCamera.orthographicSize - speedAutozoom;
                var lerpTimer = Time.deltaTime * speedLerp;
                mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, size - 0.5f, lerpTimer);

                transform.position = Vector3.Lerp(transform.position, posZoom, lerpTimer);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            // while (Vector3.Distance(posZoom, transform.position) > 0.2f)
            while (transform.position != posZoom)
            {
                var lerpTimer = Time.deltaTime * speedLerp;
                transform.position = Vector3.Lerp(transform.position, posZoom, lerpTimer);
                yield return new WaitForEndOfFrame();
            }
        }
        isCanMove = true;
        isAutoZoom = false;
    }
    private IEnumerator IE_AutoZoomCam(Vector3 posZoom, float size, UnityAction callback = null)
    {
        isCanMove = false;
        isAutoZoom = true;

        while (mainCamera.orthographicSize < size)
        {
            var siz = mainCamera.orthographicSize + speedAutozoom;
            var lerpTimer = Time.deltaTime * speedLerp;
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, size + 0.5f, lerpTimer);

            transform.position = Vector3.Lerp(transform.position, posZoom, lerpTimer);
            yield return new WaitForEndOfFrame();
        }

        isCanMove = true;
        isAutoZoom = false;
        this.enabled = false;
        callback?.Invoke();
    }

    public void AutoZoomCam(float size, Vector3 pos)
    {
        if (IE_AUTO_ZOOM != null)
            StopCoroutine(IE_AUTO_ZOOM);
        IE_AUTO_ZOOM = IE_AutoZoomCam(size, pos);
        StartCoroutine(IE_AUTO_ZOOM);
    }
    public void AutoZoomCam(float size)
    {
        if (IE_AUTO_ZOOM != null)
            StopCoroutine(IE_AUTO_ZOOM);
        IE_AUTO_ZOOM = IE_Auto_Zoom_Button(size);
        StartCoroutine(IE_AUTO_ZOOM);
    }

    private IEnumerator IE_Auto_Zoom_Button(float size)
    {
        if (size > 20)
        {
            while (mainCamera.orthographicSize < size)
            {
                var lerpTimer = Time.deltaTime * speedLerp;
                mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, size + 0.5f, lerpTimer);
                //  transform.position = Vector3.Lerp(transform.position, posZoom, 0.5f);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (mainCamera.orthographicSize > size)
            {
                var lerpTimer = Time.deltaTime * speedLerp;
                mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, size - 0.5f, lerpTimer);
                //  transform.position = Vector3.Lerp(transform.position, posZoom, 0.5f);
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public void SetSizeAndPosCam(Vector3 pos)
    {
        mainCamera.orthographicSize = maxSizeCam;
        transform.position = pos;
    }
}
