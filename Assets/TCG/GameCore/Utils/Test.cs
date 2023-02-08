using UnityEngine;

public class Test : MonoBehaviour
{
    public float spacing = 1.0f;
    public float parabolaWidth = 1.0f;
    public float parabolaHeight = 1.0f;
    public float rotation = 30.0f;

    void Update()
    {
        ReorderCards();
    }

    public void ReorderCards()
    {
        int childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);

            // calculate x and y positions using parabolic equation
            float x = spacing * i - (childCount - 1) * spacing / 2;
            float y = -parabolaWidth * x * x / ((childCount - 1) * spacing) + parabolaHeight;

            // set position and rotation of child
            child.localPosition = new Vector3(x, child.localPosition.y, y);
            child.localRotation = Quaternion.Euler(0, rotation * x / ((childCount - 1) * spacing), 0);
        }
    }
}
