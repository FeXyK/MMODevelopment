using Assets.Scripts.Character;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetSelector : MonoBehaviour
{
    List<GameObject> targets = new List<GameObject>();
    int targetSelectedCounter = 0;
    public GameObject targetCircle;
    private GameObject selectedTarget;
    public float maxSelectableDistance = 100f;
    TargetFrameController targetFrameController;
    UIManager uiManager;
    void Start()
    {
        targetCircle = Instantiate(Resources.Load<GameObject>("SelectionIndicator"));
        targetCircle.SetActive(false);
        uiManager = GameObject.FindObjectOfType<UIManager>();
        targetFrameController = uiManager.TargetFrame.GetComponent<TargetFrameController>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //create a ray cast and set it to the mouses cursor position in game
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxSelectableDistance))
            {
                if (hit.transform.tag == "Entity")
                {
                    selectedTarget = hit.transform.gameObject;
                    targetFrameController.Set(hit.transform.GetComponent<EntityContainer>());
                    targetCircle.SetActive(true);
                    targetCircle.transform.SetParent(hit.transform);
                    targetCircle.transform.localPosition = Vector3.zero;
                    uiManager.TargetFrame.SetActive(true);
                    targetFrameController.Set(hit.transform.GetComponent<EntityContainer>());
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            for (int i = targets.Count - 1; i >= 0; i--)
            {
                if (!IsVisibleOnScreen(targets[i]))
                {
                    targets.Remove(targets[i]);
                }
            }
            GameObject[] entities = GameObject.FindGameObjectsWithTag("Entity");
            foreach (var entity in entities)
            {
                if (IsVisibleOnScreen(entity))
                {
                    if (!targets.Contains(entity))
                        targets.Add(entity);
                }
            }
            targets = targets.OrderBy(target => Vector3.Distance(target.transform.position, this.transform.position)).ToList();
            if (targets.Count > 0)
            {
                targetSelectedCounter++;
                if (targetSelectedCounter >= targets.Count)
                {
                    targetSelectedCounter = 0;
                }
                selectedTarget = targets[targetSelectedCounter];

                targetCircle.SetActive(true);
                targetCircle.transform.SetParent(selectedTarget.transform);
                targetCircle.transform.localPosition = Vector3.zero;

                uiManager.TargetFrame.SetActive(true);
                targetFrameController.Set(selectedTarget.GetComponent<EntityContainer>());
            }
            Debug.Log(targets.Count);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            selectedTarget = null;
        }
        if (selectedTarget == null)
        {
            targetCircle.SetActive(false);
            uiManager.TargetFrame.SetActive(false);
        }

    }
    private bool IsBetween(float value, float b1, float b2)
    {
        if (value > b1 && value < b2)
            return true;
        return false;
    }
    private bool IsVisibleOnScreen(GameObject entity)
    {
        Vector3 entityScreenPosition = Camera.main.WorldToScreenPoint(entity.transform.position);
        //if (IsBetween(entityScreenPosition.x, 0, 1) && IsBetween(entityScreenPosition.y, 0, 1))
        {
            if (Vector3.Distance(this.transform.position, entity.transform.position) < maxSelectableDistance)
            {
                return true;
            }
        }
        return false;
    }
}
