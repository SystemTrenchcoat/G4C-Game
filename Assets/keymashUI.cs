using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keymashUI : MonoBehaviour
{

    public GameObject wavePromptSpr;

    void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Fish fish = collision.GetComponent<Fish>();
        if (fish != null)
        {
            wavePromptSpr.SetActive(true);
            return;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Fish fish = collision.GetComponent<Fish>();
        if (fish != null && fish.GetComponentInParent<Boat>() == null)
        {
            wavePromptSpr.SetActive(false);
            return;
        }
    }
}
