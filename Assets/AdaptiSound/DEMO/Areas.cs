using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdaptiSound
{

public class Areas : MonoBehaviour
{
    public int layer_index;
    public BoxCollider2D col;
    public GameObject parent;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            AudioManager.Instance.muteAllLayer("Theme3", true, 1.0f);
            AudioManager.Instance.muteLayer("Theme3", layer_index, false, 1.0f);
            
            DEMO2 parent_script = parent.GetComponent<DEMO2>();
            parent_script.area = layer_index;
        }
    }
}

}
