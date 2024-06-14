using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TimeButtonsAnimationScript : MonoBehaviour
{
    public Transform buttonParentTransform;
    private void Awake() {
        float y = buttonParentTransform.localPosition.y;
        buttonParentTransform.localPosition = new Vector3(buttonParentTransform.localPosition.x, 100, buttonParentTransform.localPosition.z);
        buttonParentTransform.DOLocalMoveY(y, 0.5f).SetEase(Ease.OutCubic);
    }
}
