using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Keyur.Components.ObjectPooling;
using DG.Tweening;
using TMPro;

public class FloatingText : Singleton<FloatingText>
{
    private ObjectPoolerSimple pool;

    public void Start()
    {
        pool = GetComponent<ObjectPoolerSimple>();
        pool.InitializePool(pool.pooledAmount, pool.pooledObject);
    }
    
    public void AnimateFloatingText(Vector3 targetPos,int textValue, AnimationComplete onComplete = null)
    {
        GameObject go = pool.GetPooledObject();

        go.SetActive(true);
        go.GetComponent<RectTransform>().localPosition = targetPos;
        go.GetComponent<Text>().text = textValue.ToString();

        if (textValue > 0)
        {
            go.GetComponent<Text>().text = "+"+textValue.ToString();
            go.GetComponent<Text>().color = Color.green;
        }
        else if(textValue < 0)
            go.GetComponent<Text>().color = Color.red;

        Sequence mysequence = DOTween.Sequence();
        mysequence.Append(go.GetComponent<CanvasGroup>().DOFade(1,0f));
        mysequence.Append(go.GetComponent<RectTransform>().DOLocalMove(new Vector2(targetPos.x, targetPos.y + 125), .6f));
        mysequence.Append(go.GetComponent<CanvasGroup>().DOFade(0, .4f));
        mysequence.AppendCallback(() =>
        {
            go.SetActive(false);
            onComplete();
        });
    }

    //Usar o FloatingText abaixo para strings
    public void AnimateFloatingString(Vector3 targetPos, string textValue, AnimationComplete onComplete = null)
    {
        GameObject go = pool.GetPooledObject();
        go.SetActive(true);
        go.GetComponent<RectTransform>().localPosition = targetPos;
        go.GetComponent<Text>().text = textValue.ToString();

        go.GetComponent<Text>().color = Color.yellow;

        Sequence mysequence = DOTween.Sequence();
        mysequence.Append(go.GetComponent<CanvasGroup>().DOFade(1, 0f));
        mysequence.Append(go.GetComponent<RectTransform>().DOLocalMove(new Vector2(targetPos.x, targetPos.y + 125), 1f));
        mysequence.Append(go.GetComponent<CanvasGroup>().DOFade(0, 1f));

        mysequence.AppendCallback(() =>
        {
            go.SetActive(false);

            onComplete();
        });
    }
}
