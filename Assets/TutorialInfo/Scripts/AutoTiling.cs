using UnityEngine;

[ExecuteAlways] 
public class AutoTiling : MonoBehaviour
{
    public float textureToMeshRatio = 1f;

    private Renderer rend;
    private MaterialPropertyBlock propBlock;

    void Update()
    {
        // Cache komponen 
        if (rend == null) rend = GetComponent<Renderer>();
        if (propBlock == null) propBlock = new MaterialPropertyBlock();

        // get  Scale
        float scaleX = transform.localScale.x;
        float scaleY = transform.localScale.z; 

        Vector4 tilingData = new Vector4(scaleX * textureToMeshRatio, 1, 0, 0);
        rend.GetPropertyBlock(propBlock);

        propBlock.SetVector("_BaseMap_ST", tilingData);
        propBlock.SetVector("_MainTex_ST", tilingData);

        rend.SetPropertyBlock(propBlock);
    }
}