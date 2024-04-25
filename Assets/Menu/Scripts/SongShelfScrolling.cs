using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the scrolling of the song shelf
/// </summary>
public class SongShelfScrolling : MonoBehaviour
{
    [SerializeField, Tooltip("Reference to the left button")] private GameObject leftButton;
    [SerializeField, Tooltip("Reference to the right button")] private GameObject rightButton;
    [SerializeField, Tooltip("The speed at which the shelf scrolls")] private float scrollSpeed = 1;
    [SerializeField, Tooltip("How far to move in the x direction per page")] private float pageDistance = 1100;
    [Tooltip("Reference to the rect transform of the shelf")] private RectTransform shelfRectTransform;
    [Tooltip("The current position of the shelf")] private float currentPosition;
    [Tooltip("The target position of the shelf")] private float targetPosition;
    [Tooltip("If the shelf is currently scrolling")] private bool isScrolling;
    [Tooltip("The number of pages in the shelf")] private int numberOfPages;
    [Tooltip("The current page of the shelf")] private int currentPage;

    // Start is called before the first frame update
    void Start()
    {
        shelfRectTransform = GetComponent<RectTransform>();
        numberOfPages = Mathf.CeilToInt((float)transform.childCount / 3);
        currentPage = 0;
        currentPosition = shelfRectTransform.anchoredPosition.x;
        targetPosition = currentPosition;
        leftButton.SetActive(false);
        rightButton.SetActive(numberOfPages > 1);
    }

    /// <summary>
    /// Updates the number of pages in the shelf
    /// </summary>
    public void UpdatePages()
    {
        numberOfPages = Mathf.CeilToInt((float)transform.childCount / 3);
        rightButton.SetActive(numberOfPages > 1);
    }

    /// <summary>
    /// Scrolls the shelf to the left
    /// </summary>
    public void ScrollLeft()
    {
        if (currentPage > 0)
        {
            currentPage--;
            targetPosition = currentPosition + pageDistance;
            leftButton.SetActive(currentPage > 0);
            rightButton.SetActive(true);
            isScrolling = true;
        }
    }

    /// <summary>
    /// Scrolls the shelf to the right
    /// </summary>
    public void ScrollRight()
    {
        if (currentPage < numberOfPages - 1)
        {
            currentPage++;
            targetPosition = currentPosition - pageDistance;
            rightButton.SetActive(currentPage < numberOfPages - 1);
            leftButton.SetActive(true);
            isScrolling = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isScrolling)
        {
            shelfRectTransform.localPosition = new Vector3(Mathf.Lerp(shelfRectTransform.localPosition.x, targetPosition, Time.unscaledDeltaTime * scrollSpeed), shelfRectTransform.localPosition.y, shelfRectTransform.localPosition.z);
            if (Mathf.Abs(shelfRectTransform.localPosition.x - targetPosition) < 1)
            {
                isScrolling = false;
                currentPosition = targetPosition;
            }
        }
    }
}
