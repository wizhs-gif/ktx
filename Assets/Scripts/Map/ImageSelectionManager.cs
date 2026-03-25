using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ImageSelectionManager : MonoBehaviour
{
    public static ImageSelectionManager Instance { get; private set; }
    private List<SelectableImage> allImages = new List<SelectableImage>();
    private SelectableImage currentSelected;

    void Awake()
    {
        Instance = this;
    }

    public void RegisterImage(SelectableImage img)
    {
        if (!allImages.Contains(img))
            allImages.Add(img);
    }

    public void SelectImage(SelectableImage selected)
    {
        // 取消之前的
        if (currentSelected != null && currentSelected != selected)
            currentSelected.SetSelected(false);

        currentSelected = selected;
        currentSelected.SetSelected(true);
    }

    public void currentSelectedScene()
    {
        SceneManager.LoadScene(currentSelected.sceneName);
    }
}
