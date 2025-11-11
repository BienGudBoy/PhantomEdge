using UnityEngine;
using UnityEngine.UI;

public class InfiniteScrollingBackground : MonoBehaviour
{
    [SerializeField] private RectTransform[] images;
    [SerializeField] private float scrollSpeed = 50f;
    
    private float imageWidth;
    private RectTransform[] imageTransforms;
    
    void Start()
    {
        // If images array is not assigned, try to find them automatically
        if (images == null || images.Length == 0)
        {
            // Try to find images by name
            GameObject image1 = GameObject.Find("Image");
            GameObject image2 = GameObject.Find("Image (1)");
            
            if (image1 != null && image2 != null)
            {
                imageTransforms = new RectTransform[2];
                imageTransforms[0] = image1.GetComponent<RectTransform>();
                imageTransforms[1] = image2.GetComponent<RectTransform>();
            }
            else
            {
                Debug.LogError("InfiniteScrollingBackground: Could not find Image GameObjects!");
                return;
            }
        }
        else
        {
            imageTransforms = images;
        }
        
        // Get the width of the first image
        if (imageTransforms != null && imageTransforms.Length > 0 && imageTransforms[0] != null)
        {
            imageWidth = imageTransforms[0].rect.width;
        }
    }
    
    void Update()
    {
        if (imageTransforms == null || imageTransforms.Length == 0) return;
        
        // Move each image to the left
        foreach (RectTransform image in imageTransforms)
        {
            if (image == null) continue;
            
            // Move the image left
            image.anchoredPosition += Vector2.left * scrollSpeed * Time.deltaTime;
            
            // If the image has moved completely off the left side, move it to the right
            if (image.anchoredPosition.x <= -imageWidth)
            {
                // Find the rightmost position of all images
                float rightmostX = float.MinValue;
                foreach (RectTransform otherImage in imageTransforms)
                {
                    if (otherImage != null && otherImage != image)
                    {
                        if (otherImage.anchoredPosition.x > rightmostX)
                        {
                            rightmostX = otherImage.anchoredPosition.x;
                        }
                    }
                }
                
                // Place this image to the right of the rightmost image
                image.anchoredPosition = new Vector2(rightmostX + imageWidth, image.anchoredPosition.y);
            }
        }
    }
}
