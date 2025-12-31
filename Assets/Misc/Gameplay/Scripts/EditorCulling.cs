using UnityEditor;
using UnityEngine;

public class EditorCulling : MonoBehaviour
{
    [SerializeField] LayerMask cullingLayer;
    
    void Start()
    {
        Tools.visibleLayers = cullingLayer;
    }
}
