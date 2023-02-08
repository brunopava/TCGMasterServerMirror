using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public GameObject sleeve;

    public MeshRenderer cardSplashArtPlane;

    public TextMeshPro description;
    public TextMeshPro attack;
    public TextMeshPro hit_points;

    public void SetCardImage(Texture texture)
    {
        cardSplashArtPlane.material.SetTexture("_MainTex", texture);
    }

    public void ToggleSleeve(bool active = false)
    {
        sleeve.SetActive(active);
    }

    public void SetIsPlayable(bool isPlayable)
    {
        // CardLight.SetActive(isPlayable);
        // _cardLightAnim.Play("CardLightTest");
        if (isPlayable)
        {
            // switch (type)
            // {
            //     case (cardType.offBoard):
            //         CardLight.GetComponent<RectTransform>().DOScale(new Vector2(_originalLightScale.x + 0.2f, _originalLightScale.y + 0.2f), .1f).SetLoops(-1, LoopType.Yoyo);
            //         break;
            //     case (cardType.onBoard):
            //         CardLight.GetComponent<RectTransform>().DOScale(new Vector2(_originalLightScale.x + 1, _originalLightScale.y + 1), .1f).SetLoops(-1, LoopType.Yoyo);
            //         break;
            // }
        }
        else
        {
            // DOTween.Kill(CardLight.GetComponent<RectTransform>());
        }
    }
}
