using UnityEngine;

public class DrawInSpace : MonoBehaviour {

    public static DrawInSpace Instance;

    public GameObject resizableQuadPrefab;

    [SerializeField]
    private Material[] materials;

    private bool drawingOnGoing = false;
    private GameObject quadOnDrawing;

    private void Awake()
    {
        Instance = this;
    }
    // Use this for initialization
    void Start () {
        //For testing
        //DrawStaticRectangle(new Vector3(0, 0, 0), 1, 0.5f);
    }
	
	public GameObject DrawStaticRectangle(Vector3 position, float width, float height)
    {
        GameObject quad = Instantiate(resizableQuadPrefab, position, Quaternion.identity);

        //GameObject quad = Instantiate(QuadPrefab, transform.position, Quaternion.identity);
        //GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        //quad.transform.position = position;

        //Set size of RectTransform
        RectTransform rectTrans = quad.AddComponent<RectTransform>();
        rectTrans.sizeDelta = new Vector2(width, height);

        //Change scale depending on RectTransform width and height
        Resize(quad);

        //Debug.Log($"Rect : Size={rectTrans.rect.width};{rectTrans.rect.height}, Scale={quad.transform.localScale}, Bounds={quad.GetComponent<Renderer>().bounds.size}");

        //Set the first material as default
        quad.GetComponent<Renderer>().material = materials[0];

        return quad;
    }

    public void DrawDynamicRectangleStart(Vector3 topLeftCorner)
    {
        quadOnDrawing = GameObject.CreatePrimitive(PrimitiveType.Quad);
        
        drawingOnGoing = true;

        if (drawingOnGoing)
        {

        }
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
