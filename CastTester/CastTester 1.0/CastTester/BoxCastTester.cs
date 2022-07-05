/*
********************************************
* Date: 7/2/2022
* Purpose: Script that allows the user to alter a boxcast box to their liking
*  and then print the code required to cast the box.
********************************************
*/
using UnityEngine;

public class BoxCastTester : MonoBehaviour
{
    #region Variables
    #region Primary Settings
    [Header("Primary Settings")]

    [Tooltip("The offset from the gameobject's origin for the boxcast position.")]
    public Vector2 Offset;

    [Tooltip("The size of the boxcast.")]
    public Vector2 size;

    // enum for the RelativeToObject Variable, for user ease of use
    public enum RelativeityTypes { None, Horizontal, Verticle, Both }

    [Tooltip("Make boxcast radius relative to the gameobject's height or width")]
    public RelativeityTypes RelativeToObject = RelativeityTypes.None;

    // enum for the FlipTypes Variable, for user ease of use
    public enum FlipTypes { None, Mirror, ScaleFlip, RotationFlip }

    [Tooltip("FlipType is not included in code print\n\n" +
    "Mirror: Creates a mirrored version of the cast\n\n" +
    "ScaleFlip: Flips cast when the x scale is -1\n\n" +
    "RotationFlip: Flips cast when y rotation of object is 180")]
    public FlipTypes FlipCast = FlipTypes.None;

    [Tooltip("What layers the boxcast will collide with. (set to everything by default)")]
    public LayerMask ColidableMask = ~0;
    #endregion
    
    #region Colors
    [Header("Colors")]

    [Tooltip("The color for when the boxcast is not colliding with anything within the mask.")]
    public Color NormalColor = Color.red;

    [Tooltip("The color for the boxcast when it is colliding with something within the mask.")]
    public Color CollidingColor = Color.green;
    #endregion

    #region Code printing options
    [Header("Code printing options")]

    [Tooltip("Whether to copy the code to the clipboard or not when printing.")]
    public bool CopyToClipboard = true;

    [Tooltip("Whether or not to include the code to change the draw color when colliding with object within mask.")]
    public bool PrintColorChangeCode = false;

    [Tooltip("If Print Color Change Code is enabled, use exact color codes instead of color.red and color.green.")]
    public bool UseExactColors = false;
    #endregion

    #region Private variables
    // The current color of the drawn circle gizmo.
    private Color RayColor;

    // The object's collider, only relevent if "RelativeToObject" is anything but "none"
    private Collider2D ObjCollider;

    // Float that keeps track of the amount to add to the radius if Relativity is enabled
    private Vector2 RelativeAddition;
    #endregion
    #endregion

    // Checks if relativity is on and trys to fetch a colider if there is
    private void Awake()
    {
        if (RelativeToObject != RelativeityTypes.None)
            DetectCollider();
    }

    // Trys to fetch the object's collider
    private void DetectCollider()
    {
        try
        {
            ObjCollider = gameObject.GetComponent(typeof(Collider2D)) as Collider2D;
        }
        catch
        {
            RelativeToObject = RelativeityTypes.None;
            Debug.LogError("To use any Relativity to objects you must have a Collider2D attached to gameobject.");
        }
    }

    // Update method to cast the circle
    void Update()
    {
        // Checks if "RelativeToObject" is enabled
        if (RelativeToObject != RelativeityTypes.None)
        {
            // Checks if there is a colider and trys to fetch one if there is not
            if (ObjCollider != null)
                // Changes the "RelativeAddition" variable to reflect the selected relativity
                switch (RelativeToObject)
                {
                    case RelativeityTypes.Horizontal:
                        RelativeAddition.x = ObjCollider.bounds.size.x;
                        RelativeAddition.y = 0;
                        break;

                    case RelativeityTypes.Verticle:
                        RelativeAddition.x = 0;
                        RelativeAddition.y = ObjCollider.bounds.size.y;
                        break;

                    case RelativeityTypes.Both:
                        RelativeAddition.x = ObjCollider.bounds.size.x;
                        RelativeAddition.y = ObjCollider.bounds.size.y;
                        break;
                }
            else
                DetectCollider();
        }
        else
            RelativeAddition = Vector2.zero;

        // Checks the FlipCast variable and checks the boxcast appropriately
        RaycastHit2D raycastHit = new RaycastHit2D();
        RaycastHit2D raycastHit2 = new RaycastHit2D();
        switch (FlipCast)
        {
            case FlipTypes.None:
                // Checks boxcast normally
                raycastHit = Physics2D.BoxCast(new Vector2(transform.position.x + Offset.x, transform.position.y + Offset.y), size + RelativeAddition, 0, Vector2.down, 0, ColidableMask);
                break;

            case FlipTypes.Mirror:
                // Checks boxcast and then Checks a mirrored version of it
                raycastHit = Physics2D.BoxCast(new Vector2(transform.position.x + Offset.x, transform.position.y + Offset.y), size + RelativeAddition, 0, Vector2.down, 0, ColidableMask);
                raycastHit2 = Physics2D.BoxCast(new Vector2(transform.position.x - Offset.x, transform.position.y + Offset.y), size + RelativeAddition, 0, Vector2.down, 0, ColidableMask);
                break;

            case FlipTypes.ScaleFlip:
                // Checks boxcast, flips if the object's X scale is negative
                raycastHit = Physics2D.BoxCast(new Vector2(transform.position.x + Offset.x * Mathf.Sign(transform.localScale.x), transform.position.y + Offset.y), size + RelativeAddition, 0, Vector2.down, 0, ColidableMask);                break;

            case FlipTypes.RotationFlip:
                // Cheks boxcast, flips if object's Y rotation is equal to 180
                if (transform.rotation.eulerAngles.y > 90 && transform.rotation.eulerAngles.y < 270)
                    raycastHit = Physics2D.BoxCast(new Vector2(transform.position.x - Offset.x, transform.position.y + Offset.y), size + RelativeAddition, 0, Vector2.down, 0, ColidableMask);
                else
                    raycastHit = Physics2D.BoxCast(new Vector2(transform.position.x + Offset.x, transform.position.y + Offset.y), size + RelativeAddition, 0, Vector2.down, 0, ColidableMask);
                break;
        }

        // Changes the RayColor based on if the circle cast hitting an object or not
        if (raycastHit.collider != null || raycastHit2.collider != null)
            RayColor = CollidingColor;
        else
            RayColor = NormalColor;
    }

    // Method to draw the boxcast as a gizmo
    private void OnDrawGizmos()
    {
        Gizmos.color = RayColor;

        // Checks the FlipCast variable and draws the boxcast appropriately
        switch (FlipCast)
        {
            case FlipTypes.None:
                // Checks boxcast normally
                Gizmos.DrawCube(new Vector2(transform.position.x + Offset.x, transform.position.y + Offset.y), new Vector3(size.x + RelativeAddition.x, size.y + RelativeAddition.y, 0));
                break;

            case FlipTypes.Mirror:
                // Checks boxcast and then Checks a mirrored version of it
                Gizmos.DrawCube(new Vector2(transform.position.x + Offset.x, transform.position.y + Offset.y), new Vector3(size.x + RelativeAddition.x, size.y + RelativeAddition.y, 0));
                Gizmos.DrawCube(new Vector2(transform.position.x - Offset.x, transform.position.y + Offset.y), new Vector3(size.x + RelativeAddition.x, size.y + RelativeAddition.y, 0));
                break;

            case FlipTypes.ScaleFlip:
                // Checks boxcast, flips if the object's X scale is negative
                Gizmos.DrawCube(new Vector2(transform.position.x + Offset.x * Mathf.Sign(transform.localScale.x), transform.position.y + Offset.y), new Vector3(size.x + RelativeAddition.x, size.y + RelativeAddition.y, 0));
                break;

            case FlipTypes.RotationFlip:
                // Cheks boxcast, flips if object's Y rotation is equal to 180
                if (transform.rotation.eulerAngles.y > 90 && transform.rotation.eulerAngles.y < 270)
                    Gizmos.DrawCube(new Vector2(transform.position.x - Offset.x, transform.position.y + Offset.y), new Vector3(size.x + RelativeAddition.x, size.y + RelativeAddition.y, 0));
                else
                    Gizmos.DrawCube(new Vector2(transform.position.x + Offset.x, transform.position.y + Offset.y), new Vector3(size.x + RelativeAddition.x, size.y + RelativeAddition.y, 0));
                break;
        }
    }

    // Methods to print code and copy it to the clipboard (if "CopyToClipboard" is enabled)
    #region PrintMethods
    public void PrintCode()
    {
        // Creates the string that will be added onto and later be printed
        string CodeString = "";

        // Start of boxcast code
        CodeString += "RaycastHit2D raycastHit = Physics2D.BoxCast(new Vector2(transform.position.x";

        // detects if the offset.x isn't 0
        if (Offset.x != 0)
            CodeString += " + " + Offset.x + "f";

        CodeString += ", transform.position.y";

        // detects if the offset.y isn't 0
        if (Offset.y != 0)
            CodeString += " + " + Offset.y + "f";

        CodeString += "), new Vector2(" + size.x + "f";

        // detects if the boxcast is realative horizontally
        if (RelativeToObject == RelativeityTypes.Horizontal || RelativeToObject == RelativeityTypes.Both)
            CodeString += " + ObjCollider.bounds.size.x";

        CodeString += ", " + size.y + "f";

        // detects if the boxcast is relative vertically
        if (RelativeToObject == RelativeityTypes.Verticle || RelativeToObject == RelativeityTypes.Both)
            CodeString += " + ObjCollider.bounds.size.y";

        CodeString += "), 0, Vector2.down, 0, ColidableMask);";

        // Prints the color changing code if PrintColorChangeCode is enabled
        if (PrintColorChangeCode)
        {
            CodeString += "\n\nif(raycastHit.collider != null)\n";
            // Prints exact colors if UseExactColors is enabled
            if (UseExactColors)
                CodeString += "\tRayColor = new Color(" + CollidingColor.r + ", " + CollidingColor.g + ", " + CollidingColor.b + ");\n" +
                    "else\n" +
                    "\tRayColor = new Color(" + NormalColor.r + ", " + NormalColor.g + ", " + NormalColor.b + ");";
            else
                CodeString += "\tRayColor = Color.green;\n" +
                    "else\n" +
                    "\tRayColor = Color.red;";
        }


        // Prints code in console
        Debug.Log("\n" + CodeString);

        // Copies code to clipboard if "CopyToClipboard" is enabled
        if (CopyToClipboard)
            GUIUtility.systemCopyBuffer = CodeString;
    }

    public void PrintDrawCode()
    {
        
        // Creates string and starts method in string
        string CodeString = "private void OnDrawGizmos()\n" +
            "{\n" +
            "\tGizmos.color = RayColor;\n\n";


        CodeString += "\tGizmos.DrawCube(new Vector2(transform.position.x";

        // Checks if Offset.x is not equal to zero
        if (Offset.x != 0)
            CodeString += " + " + Offset.x + "f";

        CodeString += ", transform.position.y";

        // Checks if Offset.y is not equal to zero
        if (Offset.y != 0)
            CodeString += " + " + Offset.y + "f";

        CodeString += "), new Vector3(" + size.x + "f";

        // Adds code for relativity

        if (RelativeToObject == RelativeityTypes.Horizontal || RelativeToObject == RelativeityTypes.Both)
            CodeString += " + ObjCollider.bounds.size.x";

        CodeString += ", " + size.y + "f";

        if (RelativeToObject == RelativeityTypes.Verticle || RelativeToObject == RelativeityTypes.Both)
            CodeString += " + ObjCollider.bounds.size.y";

        CodeString += ", 0));\n" +
            "}";



        // Prints code in console
        Debug.Log("\n" + CodeString);

        // Copies code to clipboard if "CopyToClipboard" is enabled
        if (CopyToClipboard)
            GUIUtility.systemCopyBuffer = CodeString;
    }

    public void PrintFlexableCode()
    {
        string CodeString = "RaycastHit2D raycastHit = Physics2D.BoxCast(new Vector2(transform.position.x + Offset.x, transform.position.y + Offset.y), size";

        switch (RelativeToObject)
        {
            case RelativeityTypes.Horizontal:
                CodeString += " + new Vector2(ObjCollider.bounds.size.x, 0)";
                break;

            case RelativeityTypes.Verticle:
                CodeString += " + new Vector2(0, ObjCollider.bounds.size.y)";
                break;

            case RelativeityTypes.Both:
                CodeString += " + new Vector2(ObjCollider.bounds.size.x, ObjCollider.bounds.size.y)";
                break;
        }

        CodeString += ", 0, Vector2.down, 0, ColidableMask);";

        if (PrintColorChangeCode)
        {
            CodeString += "\n\nif(raycastHit.collider != null)\n";
            // Prints exact colors if UseExactColors is enabled
            if (UseExactColors)
                CodeString += "\tRayColor = new Color(" + CollidingColor.r + ", " + CollidingColor.g + ", " + CollidingColor.b + ");\n" +
                    "else\n" +
                    "\tRayColor = new Color(" + NormalColor.r + ", " + NormalColor.g + ", " + NormalColor.b + ");";
            else
                CodeString += "\tRayColor = Color.green;\n" +
                    "else\n" +
                    "\tRayColor = Color.red;";
        }


        // Prints code in console
        Debug.Log("\n" + CodeString);

        // Copies code to clipboard if "CopyToClipboard" is enabled
        if (CopyToClipboard)
            GUIUtility.systemCopyBuffer = CodeString;
    }

    public void PrintFlexableDrawCode()
    {
        // Creates string and starts method in string
        string CodeString = "private void OnDrawGizmos()\n" +
            "{\n" +
            "\tGizmos.color = RayColor;\n\n" +
            "\tGizmos.DrawCube(new Vector2(transform.position.x + Offset.x, transform.position.y + Offset.y), new Vector3(size.x";

        if (RelativeToObject == RelativeityTypes.Horizontal || RelativeToObject == RelativeityTypes.Both)
            CodeString += " + ObjCollider.bounds.size.x";

        CodeString += ", size.y";

        if (RelativeToObject == RelativeityTypes.Verticle || RelativeToObject == RelativeityTypes.Both)
            CodeString += " + ObjCollider.bounds.size.y";

        CodeString += ", 0));\n" +
            "}";


        // Prints code in console
        Debug.Log("\n" + CodeString);

        // Copies code to clipboard if "CopyToClipboard" is enabled
        if (CopyToClipboard)
            GUIUtility.systemCopyBuffer = CodeString;
    }

    public void PrintVariables()
    {
        string CodeString = "#region boxcast Variables\n" +
            "\t[Tooltip(\"Vector2 to offset the boxcast\")]\n" +
            "public Vector2 Offset = new Vector2(" + Offset.x + "f, " + Offset.y + "f);\n\n" +
            "\t[Tooltip(\"Vector to define the size of the boxcast\")]\n" +
            "public Vector2 size = new Vector2(" + size.x + "f, " + size.y + "f);\n\n" +
            "\t[Tooltip(\"What layers the boxcast will collide with.\")]\n" +
            "public LayerMask ColidableMask = ~0;\n\n" +
            "// Variable that controles the color of the drawn boxcast\n" +
            "private Color RayColor;\n\n" +
            "// Collider2D for the boxcast to scale in relation to;\n" +
            "private Collider2D ObjCollider;\n" +
            "#endregion";

        // Prints code in console
        Debug.Log("\n" + CodeString);

        // Copies code to clipboard if "CopyToClipboard" is enabled
        if (CopyToClipboard)
            GUIUtility.systemCopyBuffer = CodeString;
    }
    #endregion
}
