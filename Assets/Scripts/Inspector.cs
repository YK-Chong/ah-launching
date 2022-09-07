using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]
public class Inspector : MonoBehaviour
{
    public List<Graphic> raycastTargets;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    #if UNITY_EDITOR
    void Update()
    {
        GetRaycastTargets();
        TurnOffTMPRaycastTarget();
    }
    #endif


    private void GetRaycastTargets()
    {
        raycastTargets = FindObjectsOfType<Graphic>().Where(g => g.raycastTarget == true).ToList();
    }

    private void TurnOffTMPRaycastTarget()
    {
        FindObjectsOfType<TextMeshProUGUI>().Where(t => t.raycastTarget == true).ToList().ForEach(t => t.raycastTarget = false);
    }
}
