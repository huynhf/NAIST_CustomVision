using UnityEngine;

public class DrawInSpace : MonoBehaviour {

    public static DrawInSpace Instance;

    [SerializeField]
    private Material[] materials;

    private void Awake()
    {
        Instance = this;
    }
    // Use this for initialization
    void Start () {
        //For testing
        //DrawRectangle(new Vector3(0, 0, 0), 1, 0.5f);
    }
	
	public GameObject DrawRectangle(Transform transform, float width, float height)
    {
        //GameObject quad = Instantiate(QuadPrefab, transform.position, Quaternion.identity);
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.transform.position = transform.position;
        quad.transform.rotation = transform.rotation;

        //Set size of RectTransform
        RectTransform rectTrans = quad.AddComponent<RectTransform>();
        rectTrans.sizeDelta = new Vector2(width, height);
        //quad.GetComponent<Renderer>().bounds = new Bounds(position, new Vector3(width, height, 0));

        //Change scale depending on RectTransform width and height
        Resize(quad);

        //Debug.Log($"Rect : Size={rectTrans.rect.width};{rectTrans.rect.height}, Scale={quad.transform.localScale}, Bounds={quad.GetComponent<Renderer>().bounds.size}");

        return quad;
    }

    public void ChooseMaterial(GameObject geometricForm2D, string materialName)
    {
        foreach (Material mat in materials)
        {
            if(mat.name == materialName)
            {
                // assign the material to the renderer
                geometricForm2D.GetComponent<Renderer>().material = mat;
            }
        }
    }

    private void Resize(GameObject obj)
    {
        //Initially a GameObject is 1x1 Unity units and Scale (1,1,1)
        float ExpectedWidth = obj.GetComponent<RectTransform>().rect.width;
        float ExpectedHeight = obj.GetComponent<RectTransform>().rect.height;

        if (ExpectedWidth != obj.transform.localScale.x || ExpectedHeight != obj.transform.localScale.y)
        {
            obj.transform.localScale = new Vector3(ExpectedWidth, ExpectedHeight, transform.localScale.z);
        }
    }
}
