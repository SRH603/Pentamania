using UnityEngine;

public class RecipeBookPages : MonoBehaviour
{
    public string pageType;
    public RecipeBookManager manager;
    private MeshRenderer mesh;

    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        if (pageType == "Page1")
        {
            mesh.material = manager.page1mat;
        }
        if (pageType == "Page2")
        {
            mesh.material = manager.page2mat;
        }
        if (pageType == "Flip1")
        {
            mesh.material = manager.flip1mat;
        }
        if (pageType == "Flip2")
        {
            mesh.material = manager.flip2mat;
        }
    }
}
