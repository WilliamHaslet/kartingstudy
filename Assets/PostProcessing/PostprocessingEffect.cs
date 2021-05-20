using UnityEngine;

public class PostprocessingEffect : MonoBehaviour
{

    [SerializeField] private Material postprocessMaterial;

    private Camera cam;

    private void Start()
    {

        cam = GetComponent<Camera>();

        cam.depthTextureMode |= DepthTextureMode.DepthNormals;

    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        Graphics.Blit(source, destination, postprocessMaterial);
    
    }

}
