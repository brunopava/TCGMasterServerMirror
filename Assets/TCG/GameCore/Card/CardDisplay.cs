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

    public void UpdateData(CardBehaviour data)
    {
        Color32 red = Color.red;
        Color32 green = Color.green;
        Color32 white = Color.white;

        if(data.attack < data.originalAttack)
        {
            attack.color = red;
        }
        else if(data.attack > data.originalAttack)
        {
            attack.color = green;
        }else{
            attack.color = white;
        }

        if(data.hit_points < data.originalHitpoints)
        {
            hit_points.color = red;
        }
        else if(data.hit_points > data.originalHitpoints)
        {
            hit_points.color = green;
        }else{
            hit_points.color = white;
        }

        attack.text = data.attack.ToString();
        hit_points.text = data.hit_points.ToString();
    }

    public void SetCardImage(Texture texture)
    {
        Debug.Log(texture);
        cardSplashArtPlane.material.SetTexture("_MainTex", texture);
    }

    private void Awake()
    {
        ToggleSleeve(true);
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
