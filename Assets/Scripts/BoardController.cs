using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class BoardController : Singleton<BoardController>
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private GameObject promptToPlacePanel;
    [SerializeField] private GameObject boardPrefab;

    private Camera _camera;
    private List<ARRaycastHit> _hits = new List<ARRaycastHit>();
    private GameObject board;

    public bool isPlacing { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    public void StartPlacingBoard()
    {
        if(!_camera)
        {
            _camera = Camera.main;
        }
        StartCoroutine(_StartPlacingBoard());

       
    }
    public IEnumerator _StartPlacingBoard()
    {
        yield return new WaitForSeconds(1f);

        _PromptToPlace(true);
        isPlacing = true;
    }

    private void _PromptToPlace(bool show)
    {
        promptToPlacePanel?.SetActive(show);
    }

    private void Update()
    {
        if(!isPlacing)
        {
            return;
        }
        if (!TryGetTouchPosition(out Vector2 touchPosition)) return;

        if (!raycastManager.Raycast(touchPosition,_hits, TrackableType.PlaneWithinPolygon)) return;

        ScreenLog.Log("raycast = true");
        var hitPose = _hits[0].pose;

        if (board == null)
        {
            board = Instantiate(boardPrefab, hitPose.position, hitPose.rotation);
        }
        else
        {
            board.transform.position = hitPose.position;
        }

        board.transform.LookAt(_camera.transform.position);
        board.transform.eulerAngles =
            new Vector3(0, board.transform.eulerAngles.y, 0);

        isPlacing = false;
        _PromptToPlace(false);

    }

    private bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.GetMouseButton(0))
        {
            var mousePosition = Input.mousePosition;
            touchPosition = new Vector2(mousePosition.x, mousePosition.y);
            return true;
        }
        if(Input.touchCount >0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
        touchPosition = default;
        return false;
    }

    public void DestroyBoard()
    {
        Destroy(board);
    }
}
