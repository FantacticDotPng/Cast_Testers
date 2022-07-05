/*
********************************************
* Date: 7/1/2022
* Purpose: Script that allows the user to alter a circlecast circle to their liking
*  and then print the code required to cast the circle.
********************************************
*/
using UnityEngine;

public class CircleCastTester : MonoBehaviour
{
    #region Variables
    #region Primary Settings
    [Header("Primary Settings")]

        [Tooltip("The offset from the gameobject's origin for the circlecast position.")]
    public Vector2 Offset;

        [Tooltip("The radius of the circlecast.")]
    public float Radius;
    

        // enum for the RelativeToObject Variable, for user ease of use
    public enum RelativeityTypes{ None, Horizontal, Verticle, LongestSide }

        [Tooltip("Make circlecast radius relative to the gameobject's height or width")]
    public RelativeityTypes RelativeToObject = RelativeityTypes.None;

        // enum for the FlipTypes Variable, for user ease of use
    public enum FlipTypes { None, Mirror, ScaleFlip, RotationFlip }

        [Tooltip("FlipType is not included in code print\n\n" +
        "Mirror: Creates a mirrored version of the cast\n\n" +
        "ScaleFlip: Flips cast when the x scale is -1\n\n" +
        "RotationFlip: Flips cast when y rotation of object is 180")]
    public FlipTypes FlipCast = FlipTypes.None;

        [Tooltip("What layers the circlecast will collide with. (set to everything by default)")]
    public LayerMask ColidableMask = ~0;
    #endregion

    #region Colors
    [Header("Colors")]

        [Tooltip("The color for when the circlecast is not colliding with anything within the mask.")]
    public Color NormalColor = Color.red;

        [Tooltip("The color for the circlecast when it is colliding with something within the mask.")]
    public Color CollidingColor = Color.green;
    #endregion

    #region Code printing options
    [Header("Code printing options")]

        [Tooltip("Whether to copy the code to the clipboard or not when printing.")]
    public bool CopyToClipboard = true;

        [Tooltip("If Relative To Object is set to Longest Side then print out the code for detecting the longest side,\n" +
        "if disabled then instead makes code relevent to current longest side")]
    public bool PrintLongestSideCode = false;

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
    private float RelativeAddition = 0;
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
                        RelativeAddition = ObjCollider.bounds.size.x * .5f;
                        break;

                    case RelativeityTypes.Verticle:
                        RelativeAddition = ObjCollider.bounds.size.y * .5f;
                        break;

                    case RelativeityTypes.LongestSide:
                        if (ObjCollider.bounds.size.x > ObjCollider.bounds.size.y)
                            RelativeAddition = ObjCollider.bounds.size.x * .5f;
                        else
                            RelativeAddition = ObjCollider.bounds.size.y * .5f;
                        break;
                }
            else
                DetectCollider();
        }
        else
            RelativeAddition = 0;

        // Checks the FlipCast variable and checks the CircleCast appropriately
        RaycastHit2D raycastHit = new RaycastHit2D();
        RaycastHit2D raycastHit2 = new RaycastHit2D();
        switch (FlipCast)
        {
            case FlipTypes.None:
                // Checks CircleCast normally
                raycastHit = Physics2D.CircleCast(new Vector2(transform.position.x + Offset.x, transform.position.y + Offset.y), Radius + RelativeAddition, Vector2.down, 0, ColidableMask);
                break;

            case FlipTypes.Mirror:
                // Checks CircleCast and then Checks a mirrored version of it
                raycastHit = Physics2D.CircleCast(new Vector2(transform.position.x + Offset.x, transform.position.y + Offset.y), Radius + RelativeAddition, Vector2.down, 0, ColidableMask);
                raycastHit2 = Physics2D.CircleCast(new Vector2(transform.position.x - Offset.x, transform.position.y + Offset.y), Radius + RelativeAddition, Vector2.down, 0, ColidableMask);
                break;

            case FlipTypes.ScaleFlip:
                // Checks CircleCast, flips if the object's X scale is negative
                raycastHit = Physics2D.CircleCast(new Vector2(transform.position.x + Offset.x * Mathf.Sign(transform.localScale.x), transform.position.y + Offset.y), Radius + RelativeAddition, Vector2.down, 0, ColidableMask);
                break;

            case FlipTypes.RotationFlip:
                // Cheks CircleCast, flips if object's Y rotation is equal to 180
                if (transform.rotation.eulerAngles.y > 90 && transform.rotation.eulerAngles.y < 270)
                    raycastHit2 = Physics2D.CircleCast(new Vector2(transform.position.x - Offset.x, transform.position.y + Offset.y), Radius + RelativeAddition, Vector2.down, 0, ColidableMask);
                else
                    raycastHit = Physics2D.CircleCast(new Vector2(transform.position.x + Offset.x, transform.position.y + Offset.y), Radius + RelativeAddition, Vector2.down, 0, ColidableMask);
                break;
        }

        // Changes the RayColor based on if the circle cast hitting an object or not
        if (raycastHit.collider != null || raycastHit2.collider != null)
            RayColor = CollidingColor;
        else
            RayColor = NormalColor;
        
    }

        // Method to draw the circlecast as a gizmo
    private void OnDrawGizmos()
    {
        Gizmos.color = RayColor;

        // Checks the FlipCast variable and draws the CircleCast appropriately
        switch (FlipCast)
        {
            case FlipTypes.None:
                // Checks CircleCast normally
                Gizmos.DrawSphere(new Vector2(transform.position.x + Offset.x, transform.position.y + Offset.y), Radius + RelativeAddition);
                break;

            case FlipTypes.Mirror:
                // Checks CircleCast and then Checks a mirrored version of it
                Gizmos.DrawSphere(new Vector2(transform.position.x + Offset.x, transform.position.y + Offset.y), Radius + RelativeAddition);
                Gizmos.DrawSphere(new Vector2(transform.position.x - Offset.x, transform.position.y + Offset.y), Radius + RelativeAddition);
                break;

            case FlipTypes.ScaleFlip:
                // Checks CircleCast, flips if the object's X scale is negative
                Gizmos.DrawSphere(new Vector2(transform.position.x + Offset.x * Mathf.Sign(transform.localScale.x), transform.position.y + Offset.y), Radius + RelativeAddition);
                break;

            case FlipTypes.RotationFlip:
                // Cheks CircleCast, flips if object's Y rotation is equal to 180
                if (transform.rotation.eulerAngles.y > 90 && transform.rotation.eulerAngles.y < 270)
                    Gizmos.DrawSphere(new Vector2(transform.position.x - Offset.x, transform.position.y + Offset.y), Radius + RelativeAddition);
                else
                    Gizmos.DrawSphere(new Vector2(transform.position.x + Offset.x, transform.position.y + Offset.y), Radius + RelativeAddition);
                break;
        }
    }

        // Methods to print code and copy it to the clipboard (if "CopyToClipboard" is enabled)
    #region PrintMethods
    public void PrintCode()
    {
        // Creates the string that will be added onto and later be printed
        string CodeString = "";

        // Add code for detecting longest side if the option is enabled
        if (PrintLongestSideCode && RelativeToObject == RelativeityTypes.LongestSide)
            CodeString += "if (ObjCollider.bounds.size.x > ObjCollider.bounds.size.y)\n" +
                "\tRelativeAddition = ObjCollider.bounds.size.x * .5f;\n" +
                "else\n" +
                "\tRelativeAddition = ObjCollider.bounds.size.y * .5f;\n\n";

        // Start of CircleCast code
        CodeString += "RaycastHit2D raycastHit = Physics2D.CircleCast(new Vector2(transform.position.x";



        // Checks if the X Offset is not zero and if so adds it to the CodeString
        if (Offset.x != 0)
            CodeString += " + " + Offset.x + "f";

        CodeString += ", transform.position.y";

        // Checks if the Y Offset is not zero and if so adds it to the CodeString
        if (Offset.y != 0)
            CodeString += " + " + Offset.y + "f";



        // Adds code for relativity
        string RelativityText = "";
        switch (RelativeToObject)
        {
            case RelativeityTypes.Horizontal:
                RelativityText = "(ObjCollider.bounds.size.x * .5f)";
                break;

            case RelativeityTypes.Verticle:
                RelativityText = "(ObjCollider.bounds.size.y * .5f)";
                break;

            case RelativeityTypes.LongestSide:
                if (!PrintLongestSideCode)
                    if (ObjCollider.bounds.size.x > ObjCollider.bounds.size.y)
                        RelativityText = "(ObjCollider.bounds.size.x * .5f)";
                    else
                        RelativityText = "(ObjCollider.bounds.size.y * .5f)";
                else
                    RelativityText = "RelativeAddition";
                break;
        }

        if (RelativityText != "" && Radius == 0)
            CodeString += "), " + RelativityText;
        else if(RelativityText != "")
            CodeString += "), " + RelativityText + " + " + Radius;
        else
            CodeString += "), " + Radius;

        if (Radius != 0)
            CodeString += "f";

        CodeString += ", Vector2.down, 0, ColidableMask);";
    
            // Prints the color changing code if PrintColorChangeCode is enabled
        if(PrintColorChangeCode)
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

        CodeString += "\tGizmos.DrawSphere(new Vector2(transform.position.x";

        // Checks if Offset.x is not equal to zero
        if (Offset.x != 0)
            CodeString += " + " + Offset.x + "f";

        CodeString += ", transform.position.y";

        // Checks if Offset.y is not equal to zero
        if (Offset.y != 0)
            CodeString += " + " + Offset.y + "f), ";


        // Adds code for relativity
        string RelativityText = "";
        switch (RelativeToObject)
        {
            case RelativeityTypes.Horizontal:
                RelativityText = "(ObjCollider.bounds.size.x * .5f)";
                break;

            case RelativeityTypes.Verticle:
                RelativityText = "(ObjCollider.bounds.size.y * .5f)";
                break;

            case RelativeityTypes.LongestSide:
                if (!PrintLongestSideCode)
                    if (ObjCollider.bounds.size.x > ObjCollider.bounds.size.y)
                        RelativityText = "(ObjCollider.bounds.size.x * .5f)";
                    else
                        RelativityText = "(ObjCollider.bounds.size.y * .5f)";
                else
                    RelativityText = "RelativeAddition";
                break;
        }

        // Checks if RelativeAddition is equal to zero
        if (RelativityText != "")
            CodeString += RelativityText;

        // Checks if radius equals zero
        if (Radius != 0)
        {
            if (RelativityText != "")
                CodeString += " + ";
            CodeString += Radius + "f";
        }  
        else if (RelativityText == "")
            CodeString += Radius;


        CodeString += ");\n" +
            "}";



        // Prints code in console
        Debug.Log("\n" + CodeString);

        // Copies code to clipboard if "CopyToClipboard" is enabled
        if (CopyToClipboard)
            GUIUtility.systemCopyBuffer = CodeString;
    }

    public void PrintFlexableCode()
    {
        string CodeString = "";

        if (PrintLongestSideCode)
            CodeString += "if (ObjCollider.bounds.size.x > ObjCollider.bounds.size.y)\n" +
                "\tRelativeAddition = ObjCollider.bounds.size.x * .5f;\n" +
                "else\n" +
                "\tRelativeAddition = ObjCollider.bounds.size.y * .5f;\n\n";

        CodeString += "RaycastHit2D raycastHit = Physics2D.CircleCast(new Vector2(transform.position.x + Offset.x, transform.position.y + Offset.y),";

        // Adds code for relativity
        string RelativityText = "";
        switch (RelativeToObject)
        {
            case RelativeityTypes.Horizontal:
                RelativityText = " (ObjCollider.bounds.size.x * .5f) +";
                break;

            case RelativeityTypes.Verticle:
                RelativityText = " (ObjCollider.bounds.size.y * .5f) +";
                break;

            case RelativeityTypes.LongestSide:
                if (!PrintLongestSideCode)
                    if (ObjCollider.bounds.size.x > ObjCollider.bounds.size.y)
                        RelativityText = " (ObjCollider.bounds.size.x * .5f) +";
                    else
                        RelativityText = " (ObjCollider.bounds.size.y * .5f) +";
                else
                    RelativityText = " RelativeAddition +";
                break;
        }

        if (RelativityText != "")
            CodeString += RelativityText;

        CodeString += " Radius, Vector2.down, 0, ColidableMask);";

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
            "\tGizmos.DrawSphere(new Vector2(transform.position.x + Offset.x, transform.position.y + Offset.y),";

        // Adds code for relativity
        string RelativityText = "";
        switch (RelativeToObject)
        {
            case RelativeityTypes.Horizontal:
                RelativityText = " (ObjCollider.bounds.size.x * .5f) +";
                break;

            case RelativeityTypes.Verticle:
                RelativityText = " (ObjCollider.bounds.size.y * .5f) +";
                break;

            case RelativeityTypes.LongestSide:
                if (!PrintLongestSideCode)
                    if (ObjCollider.bounds.size.x > ObjCollider.bounds.size.y)
                        RelativityText = " (ObjCollider.bounds.size.x * .5f) +";
                    else
                        RelativityText = " (ObjCollider.bounds.size.y * .5f) +";
                else
                    RelativityText = " RelativeAddition +";
                break;
        }

        CodeString += RelativityText + " Radius);\n" +
            "}";


        // Prints code in console
        Debug.Log("\n" + CodeString);

        // Copies code to clipboard if "CopyToClipboard" is enabled
        if (CopyToClipboard)
            GUIUtility.systemCopyBuffer = CodeString;
    }

    public void PrintVariables()
    {
        string CodeString = "#region CircleCast Variables\n" +
            "\t[Tooltip(\"Vector2 to offset the circlecast\")]\n" +
            "public Vector2 Offset = new Vector2(" + Offset.x + "f, " + Offset.y + "f);\n\n" +
            "\t[Tooltip(\"Float to define the radius of the circlecast\")]\n" +
            "public float Radius = " + Radius + "f;\n\n" +
            "\t[Tooltip(\"What layers the circlecast will collide with.\")]\n" +
            "public LayerMask ColidableMask = ~0;\n\n" +
            "// Variable that controles the color of the drawn circlecast\n" +
            "private Color RayColor;\n\n" +
            "// Collider2D for the circlecast to scale in relation to;\n" +
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