/*
********************************************
* Date: 7/4/2022
* Purpose: Script that allows the user to alter a raycast box to their liking
*  and then print the code required to cast the ray.
********************************************
*/
using UnityEngine;

public class RayCastTester : MonoBehaviour
{
    #region Variables
    #region Primary Settings
    [Header("Primary Settings")]

    [Tooltip("The offset from the gameobject's origin for the raycast position.")]
    public Vector2 Offset;

    [Tooltip("The length of the raycast.")]
    public float Length;

    [Tooltip("The rotation/angle of raycast")]
    [Range(0, 360f)]
    public float Rotation;

    // enum for the RelativeToObject Variable, for user ease of use
    public enum RelativeityTypes { None, Horizontal, Verticle }

    [Tooltip("Make raycast radius relative to the gameobject's height or width")]
    public RelativeityTypes RelativeToObject = RelativeityTypes.None;

    // enum for the FlipTypes Variable, for user ease of use
    public enum FlipTypes { None, Mirror, ScaleFlip, RotationFlip }

    [Tooltip("FlipType is not included in code print\n\n" +
    "Mirror: Creates a mirrored version of the cast\n\n" +
    "ScaleFlip: Flips cast when the x scale is -1\n\n" +
    "RotationFlip: Flips cast when y rotation of object is 180")]
    public FlipTypes FlipCast = FlipTypes.None;

    [Tooltip("If rotation or mirror is enabled, also flip rotation of the ray.")]
    public bool FlipRotation = false;

    [Tooltip("What layers the raycast will collide with. (set to everything by default)")]
    public LayerMask ColidableMask = ~0;
    #endregion

    #region Colors
    [Header("Colors")]

    [Tooltip("The color for when the raycast is not colliding with anything within the mask.")]
    public Color NormalColor = Color.red;

    [Tooltip("The color for the raycast when it is colliding with something within the mask.")]
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
    private float RelativeAddition;
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
                        RelativeAddition = ObjCollider.bounds.size.x;
                        break;

                    case RelativeityTypes.Verticle:
                        RelativeAddition = ObjCollider.bounds.size.y;
                        break;
                }
            else
                DetectCollider();
        }
        else
            RelativeAddition = 0;

        // Checks the FlipCast variable and checks the raycast appropriately
        RaycastHit2D raycastHit = new RaycastHit2D();
        RaycastHit2D raycastHit2 = new RaycastHit2D();
        switch (FlipCast)
        {
            case FlipTypes.None:
                // Checks raycast normally
                raycastHit = Physics2D.Raycast(new Vector2(transform.position.x + Offset.x, transform.position.y + Offset.y), new Vector2(Mathf.Cos(Rotation * Mathf.Deg2Rad), Mathf.Sin(Rotation * Mathf.Deg2Rad)), Length + RelativeAddition, ColidableMask);
                break;

            case FlipTypes.Mirror:
                // Checks raycast and then Checks a mirrored version of it
                raycastHit = Physics2D.Raycast(new Vector2(transform.position.x + Offset.x, transform.position.y + Offset.y), new Vector2(Mathf.Cos(Rotation * Mathf.Deg2Rad), Mathf.Sin(Rotation * Mathf.Deg2Rad)), Length + RelativeAddition, ColidableMask);
                if (FlipRotation)
                    raycastHit2 = Physics2D.Raycast(new Vector2(transform.position.x - Offset.x, transform.position.y + Offset.y), new Vector2(Mathf.Cos((180 - Rotation) * Mathf.Deg2Rad), Mathf.Sin((180 - Rotation) * Mathf.Deg2Rad)), Length + RelativeAddition, ColidableMask);
                else
                    raycastHit2 = Physics2D.Raycast(new Vector2(transform.position.x - Offset.x, transform.position.y + Offset.y), new Vector2(Mathf.Cos(Rotation * Mathf.Deg2Rad), Mathf.Sin(Rotation * Mathf.Deg2Rad)), Length + RelativeAddition, ColidableMask);

                break;

            case FlipTypes.ScaleFlip:
                // Checks raycast, flips if the object's X scale is negative
                if (transform.localScale.x < 0)
                {
                    if (FlipRotation)
                        raycastHit = Physics2D.Raycast(new Vector2(transform.position.x - Offset.x * Mathf.Sign(transform.localScale.x), transform.position.y + Offset.y), new Vector2(Mathf.Cos((180 - Rotation) * Mathf.Deg2Rad), Mathf.Sin((180 - Rotation) * Mathf.Deg2Rad)), Length + RelativeAddition, ColidableMask);
                    else
                        raycastHit = Physics2D.Raycast(new Vector2(transform.position.x - Offset.x * Mathf.Sign(transform.localScale.x), transform.position.y + Offset.y), new Vector2(Mathf.Cos(Rotation * Mathf.Deg2Rad), Mathf.Sin(Rotation * Mathf.Deg2Rad)), Length + RelativeAddition, ColidableMask);
                }
                else
                {
                    raycastHit = Physics2D.Raycast(new Vector2(transform.position.x + Offset.x * Mathf.Sign(transform.localScale.x), transform.position.y + Offset.y), new Vector2(Mathf.Cos(Rotation * Mathf.Deg2Rad), Mathf.Sin(Rotation * Mathf.Deg2Rad)), Length + RelativeAddition, ColidableMask);
                }

                break;

            case FlipTypes.RotationFlip:
                // Cheks raycast, flips if object's Y rotation is equal to 180
                if (transform.rotation.eulerAngles.y > 90 && transform.rotation.eulerAngles.y < 270)
                {
                    if (FlipRotation)
                        raycastHit = Physics2D.Raycast(new Vector2(transform.position.x - Offset.x * Mathf.Sign(transform.localScale.x), transform.position.y + Offset.y), new Vector2(Mathf.Cos((180 - Rotation) * Mathf.Deg2Rad), Mathf.Sin((180 - Rotation) * Mathf.Deg2Rad)), Length + RelativeAddition, ColidableMask);
                    else
                        raycastHit = Physics2D.Raycast(new Vector2(transform.position.x - Offset.x * Mathf.Sign(transform.localScale.x), transform.position.y + Offset.y), new Vector2(Mathf.Cos(Rotation * Mathf.Deg2Rad), Mathf.Sin(Rotation * Mathf.Deg2Rad)), Length + RelativeAddition, ColidableMask);
                }
                else
                {
                    raycastHit = Physics2D.Raycast(new Vector2(transform.position.x + Offset.x * Mathf.Sign(transform.localScale.x), transform.position.y + Offset.y), new Vector2(Mathf.Cos(Rotation * Mathf.Deg2Rad), Mathf.Sin(Rotation * Mathf.Deg2Rad)), Length + RelativeAddition, ColidableMask);
                }
                break;
        }

        // Changes the RayColor based on if the circle cast hitting an object or not
        if (raycastHit.collider != null || raycastHit2.collider != null)
            RayColor = CollidingColor;
        else
            RayColor = NormalColor;

        // draw the ray
        switch (FlipCast)
        {
            case FlipTypes.None:
                // Checks raycast normally
                Debug.DrawRay(new Vector2(transform.position.x + Offset.x, transform.position.y + Offset.y), new Vector2(Mathf.Cos(Rotation * Mathf.Deg2Rad), Mathf.Sin(Rotation * Mathf.Deg2Rad)) * (Length + RelativeAddition), RayColor);
                break;

            case FlipTypes.Mirror:
                // Checks raycast and then Checks a mirrored version of it
                Debug.DrawRay(new Vector2(transform.position.x + Offset.x, transform.position.y + Offset.y), new Vector2(Mathf.Cos(Rotation * Mathf.Deg2Rad), Mathf.Sin(Rotation * Mathf.Deg2Rad)) * (Length + RelativeAddition), RayColor);
                if(FlipRotation)
                    Debug.DrawRay(new Vector2(transform.position.x - Offset.x, transform.position.y + Offset.y), new Vector2(Mathf.Cos((180 - Rotation) * Mathf.Deg2Rad), Mathf.Sin((180 - Rotation) * Mathf.Deg2Rad)) * (Length + RelativeAddition), RayColor);
                else
                    Debug.DrawRay(new Vector2(transform.position.x - Offset.x, transform.position.y + Offset.y), new Vector2(Mathf.Cos(Rotation * Mathf.Deg2Rad), Mathf.Sin(Rotation * Mathf.Deg2Rad)) * (Length + RelativeAddition), RayColor);

                break;

            case FlipTypes.ScaleFlip:
                // Checks raycast, flips if the object's X scale is negative

                if(transform.localScale.x < 0)
                {
                    if (FlipRotation)
                        Debug.DrawRay(new Vector2(transform.position.x - Offset.x, transform.position.y + Offset.y), new Vector2(Mathf.Cos((180 - Rotation) * Mathf.Deg2Rad), Mathf.Sin((180 - Rotation) * Mathf.Deg2Rad)) * (Length + RelativeAddition), RayColor);
                    else
                        Debug.DrawRay(new Vector2(transform.position.x - Offset.x, transform.position.y + Offset.y), new Vector2(Mathf.Cos(Rotation * Mathf.Deg2Rad), Mathf.Sin(Rotation * Mathf.Deg2Rad)) * (Length + RelativeAddition), RayColor);
                }
                else
                {
                    Debug.DrawRay(new Vector2(transform.position.x + Offset.x, transform.position.y + Offset.y), new Vector2(Mathf.Cos(Rotation * Mathf.Deg2Rad), Mathf.Sin(Rotation * Mathf.Deg2Rad)) * (Length + RelativeAddition), RayColor);
                }
                break;

            case FlipTypes.RotationFlip:
                // Cheks raycast, flips if object's Y rotation is equal to 180
                if (transform.rotation.eulerAngles.y > 90 && transform.rotation.eulerAngles.y < 270)
                {
                    if (FlipRotation)
                        Debug.DrawRay(new Vector2(transform.position.x - Offset.x, transform.position.y + Offset.y), new Vector2(Mathf.Cos((180 - Rotation) * Mathf.Deg2Rad), Mathf.Sin((180 - Rotation) * Mathf.Deg2Rad)) * (Length + RelativeAddition), RayColor);
                    else
                        Debug.DrawRay(new Vector2(transform.position.x - Offset.x, transform.position.y + Offset.y), new Vector2(Mathf.Cos(Rotation * Mathf.Deg2Rad), Mathf.Sin(Rotation * Mathf.Deg2Rad)) * (Length + RelativeAddition), RayColor);
                }
                else
                {
                    Debug.DrawRay(new Vector2(transform.position.x + Offset.x, transform.position.y + Offset.y), new Vector2(Mathf.Cos(Rotation * Mathf.Deg2Rad), Mathf.Sin(Rotation * Mathf.Deg2Rad)) * (Length + RelativeAddition), RayColor);
                }
                break;
        }
    }

    // Methods to print code and copy it to the clipboard (if "CopyToClipboard" is enabled)
    #region PrintMethods
    public void PrintCode()
    {
        // Start of raycast code
        string CodeString = "RaycastHit2D raycastHit = Physics2D.Raycast(new Vector2(transform.position.x";


        // detects if the offset.x isn't 0
        if (Offset.x != 0)
            CodeString += " + " + Offset.x + "f";

        CodeString += ", transform.position.y";

        // detects if the offset.y isn't 0
        if (Offset.y != 0)
            CodeString += " + " + Offset.y + "f";

        CodeString += "), ";

        switch (Rotation)
        {
            case 0:
            case 360f:
                CodeString += "Vector2.right";
                break;

            case 90f:
                CodeString += "Vector2.up";
                break;

            case 180f:
                CodeString += "Vector2.left";
                break;

            case 270f:
                CodeString += "Vector2.down";
                break;

            default:
                CodeString += "new Vector2(Mathf.Cos(" + Rotation + " * Mathf.Deg2Rad), Mathf.Sin(" + Rotation + " * Mathf.Deg2Rad))";
                break;
        }

        CodeString += ", " + Length + "f";

        switch(RelativeToObject)
        {
            case RelativeityTypes.Horizontal:
                CodeString += " + ObjCollider.bounds.size.x";
                break;

            case RelativeityTypes.Verticle:
                CodeString += " + ObjCollider.bounds.size.y";
                break;
        }

        CodeString += ", ColidableMask);";

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
        string CodeString = "Debug.DrawRay(new Vector2(transform.position.x";

        // Checks if Offset.x is not equal to zero
        if (Offset.x != 0)
            CodeString += " + " + Offset.x + "f";

        CodeString += ", transform.position.y";

        // Checks if Offset.y is not equal to zero
        if (Offset.y != 0)
            CodeString += " + " + Offset.y + "f";

        CodeString += "), ";

        switch (Rotation)
        {
            case 0:
            case 360f:
                CodeString += "Vector2.right";
                break;

            case 90f:
                CodeString += "Vector2.up";
                break;

            case 180f:
                CodeString += "Vector2.left";
                break;

            case 270f:
                CodeString += "Vector2.down";
                break;

            default:
                CodeString += "new Vector2(Mathf.Cos(" + Rotation + " * Mathf.Deg2Rad), Mathf.Sin(" + Rotation + " * Mathf.Deg2Rad))";
                break;
        }

        if (RelativeToObject != RelativeityTypes.None)
            CodeString += " * (Length";
        else
            CodeString += " * Length";

        // Adds code for relativity

        switch (RelativeToObject)
        {
            case RelativeityTypes.Horizontal:
                CodeString += " + ObjCollider.bounds.size.x)";
                break;

            case RelativeityTypes.Verticle:
                CodeString += " + ObjCollider.bounds.size.y)";
                break;
        }

        CodeString += ", RayColor);";



        // Prints code in console
        Debug.Log("\n" + CodeString);

        // Copies code to clipboard if "CopyToClipboard" is enabled
        if (CopyToClipboard)
            GUIUtility.systemCopyBuffer = CodeString;
    }

    public void PrintFlexableCode()
    {
        string CodeString = "RaycastHit2D raycastHit = Physics2D.Raycast(new Vector2(transform.position.x + Offset.x, transform.position.y + Offset.y), ";

        switch (Rotation)
        {
            case 0:
            case 360f:
                CodeString += "Vector2.right";
                break;

            case 90f:
                CodeString += "Vector2.up";
                break;

            case 180f:
                CodeString += "Vector2.left";
                break;

            case 270f:
                CodeString += "Vector2.down";
                break;

            default:
                CodeString += "new Vector2(Mathf.Cos(Rotation * Mathf.Deg2Rad), Mathf.Sin(Rotation * Mathf.Deg2Rad))";
                break;
        }

        CodeString += ", Length";

        switch (RelativeToObject)
        {
            case RelativeityTypes.Horizontal:
                CodeString += " + ObjCollider.bounds.size.x)";
                break;

            case RelativeityTypes.Verticle:
                CodeString += " + ObjCollider.bounds.size.y)";
                break;
        }

        CodeString += ", ColidableMask);";

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
        string CodeString = "Debug.DrawRay(new Vector2(transform.position.x + Offset.x, transform.position.y + Offset.y), ";

        switch (Rotation)
        {
            case 0:
            case 360f:
                CodeString += "Vector2.right";
                break;

            case 90f:
                CodeString += "Vector2.up";
                break;

            case 180f:
                CodeString += "Vector2.left";
                break;

            case 270f:
                CodeString += "Vector2.down";
                break;

            default:
                CodeString += "new Vector2(Mathf.Cos(Rotation * Mathf.Deg2Rad), Mathf.Sin(Rotation * Mathf.Deg2Rad))";
                break;
        }


        if (RelativeToObject != RelativeityTypes.None)
            CodeString += " * (Length";
        else
            CodeString += " * Length";

        switch (RelativeToObject)
        {
            case RelativeityTypes.Horizontal:
                CodeString += " + ObjCollider.bounds.size.x)";
                break;

            case RelativeityTypes.Verticle:
                CodeString += " + ObjCollider.bounds.size.y)";
                break;
        }

        CodeString += ", RayColor);";

        // Prints code in console
        Debug.Log("\n" + CodeString);

        // Copies code to clipboard if "CopyToClipboard" is enabled
        if (CopyToClipboard)
            GUIUtility.systemCopyBuffer = CodeString;
    }

    public void PrintVariables()
    {
        string CodeString = "#region raycast Variables\n" +
            "\t[Tooltip(\"Vector2 to offset the raycast\")]\n" +
            "public Vector2 Offset = new Vector2(" + Offset.x + "f, " + Offset.y + "f);\n\n" +
            "\t[Tooltip(\"Vector to define the length of the raycast\")]\n" +
            "public float Length = " + Length + "f;\n\n" +
            "[Tooltip(\"The rotation/angle of raycast\")]" +
            "[Range(0, 360f)]" +
            "public float Rotation = " + Rotation + "f;\n\n" +
            "\t[Tooltip(\"What layers the raycast will collide with.\")]\n" +
            "public LayerMask ColidableMask = ~0;\n\n" +
            "// Variable that controles the color of the drawn raycast\n" +
            "private Color RayColor;\n\n" +
            "// Collider2D for the raycast to scale in relation to;\n" +
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