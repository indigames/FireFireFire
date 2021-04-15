using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameplay : MonoBehaviour
{
    public Gameplay gameplay;

    public GameObject itemPreview;
    public GameObject grabIcon;
    public Text textRemaining;
    public Text textStage;
    public string textStageFormat = "Stage {0}";

    // Start is called before the first frame update
    void Start()
    {
        gameplay.callbackRestart += Restart;
        gameplay.callbackRemainingMeshblock += ResetRemainingCount;
    }

    private void Update()
    {
        if (gameplay.DraggingMeshBlock == null && grabIcon.activeSelf) grabIcon.SetActive(false);
        if (gameplay.DraggingMeshBlock != null && grabIcon.activeSelf == false) grabIcon.SetActive(true);

        if (gameplay.DraggingMeshBlock != null)
        {
            var screenPoint = Camera.main.WorldToScreenPoint(gameplay.DraggingMeshBlock.transform.position);
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform) grabIcon.transform.parent, screenPoint, Camera.main, out localPoint);
            //var pos = grabIcon.transform.localPosition;
            //pos.z = 0;
            grabIcon.transform.localPosition = localPoint;
        }
    }

    void Restart()
    {
        textStage.text = string.Format(textStageFormat, gameplay.StageNumber);
        StartCoroutine(DelayPreview());
    }

    IEnumerator DelayPreview()
    {
        itemPreview.SetActive(false);
        yield return true;
        yield return true;
        itemPreview.SetActive(true);
    }

    void ResetRemainingCount(int count)
    {

        textRemaining.text = string.Format("x<color=red>{0}</color>", count);


        //if (count > 1)
        //    textRemaining.text = string.Format("<color=red>{0}</color> items remaning", count);
        //else if (count == 1)
        //    textRemaining.text = "<color=red>1</color> item remaning";
        //else
        //    textRemaining.text = "No item remaning";
    }

    public void RestartGame()
    {
        gameplay.RestartGame(false);
    }

}
