using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{

    [Header("Minimap Generation")]
    [SerializeField] private RawImage minimapImage;
    [SerializeField] private int minimapPixelSize;
    [SerializeField] private float minimapWorldSize;
    [SerializeField] private Vector3 minimapWorldOrigin;
    [SerializeField] private float castHeight;
    [SerializeField] private float castDistance;
    [SerializeField] private Color flatTrackColor;
    [SerializeField] private Color verticalTrackColor;
    [SerializeField] private LayerMask trackLayers;
    [Header("Minimap Tracking")]
    [SerializeField] private Transform[] cartWorldTransforms;
    [SerializeField] private Transform[] cartDots;

    private Texture2D minimapTexture;

    private void Start()
    {

        minimapTexture = new Texture2D(minimapPixelSize, minimapPixelSize, TextureFormat.ARGB32, false);

        for (int x = 0; x < minimapPixelSize; x++)
        {

            for (int y = 0; y < minimapPixelSize; y++)
            {

                minimapTexture.SetPixel(x, y, GetMinimapPixel(x, y));

            }

        }

        minimapTexture.Apply();

        minimapImage.texture = minimapTexture;

    }

    private void Update()
    {

        for (int i = 0; i < cartWorldTransforms.Length; i++)
        {

            Vector3 worldPosition = cartWorldTransforms[i].position - minimapWorldOrigin;

            float minimapImageSize = minimapImage.rectTransform.sizeDelta.x;

            cartDots[i].localPosition = new Vector2((worldPosition.x / minimapWorldSize) * minimapImageSize, (worldPosition.z / minimapWorldSize) * minimapImageSize);

        }

    }

    private Color GetMinimapPixel(int x, int y)
    {

        Color pixelColor = Color.clear;

        Vector3 castPosition = new Vector3((x / (float)minimapPixelSize) * minimapWorldSize, castHeight, (y / (float)minimapPixelSize) * minimapWorldSize) + minimapWorldOrigin;

        if (Physics.Raycast(castPosition, Vector3.down, out RaycastHit rayHit, castDistance, trackLayers))
        {

            pixelColor = Color.Lerp(verticalTrackColor, flatTrackColor, Mathf.Abs(Vector3.Dot(Vector3.up, rayHit.normal)));

        }

        return pixelColor;

    }

    private void OnDrawGizmosSelected()
    {

        Vector3 center = minimapWorldOrigin + new Vector3(0, castHeight, 0) + new Vector3(minimapWorldSize / 2, -castDistance / 2, minimapWorldSize / 2);

        Vector3 size = new Vector3(minimapWorldSize, castDistance, minimapWorldSize);

        Gizmos.color = Color.blue;

        Gizmos.DrawWireCube(center, size);

    }

}
