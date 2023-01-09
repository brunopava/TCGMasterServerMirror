using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDisplay : MonoBehaviour
{
    public GameObject sleeve;

    public void ToggleSleeve(bool active)
    {
        sleeve.SetActive(active);
    }
}
