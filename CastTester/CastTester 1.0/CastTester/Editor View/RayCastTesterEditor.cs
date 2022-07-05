/*
********************************************
* Date: 7/5/2022
* Purpose: Alters how the "RayCastTester" script is seen in the Inspector window,
*   adding a "Print Code" button
********************************************
*/
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RayCastTester))]
public class RayCastTesterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the Default Unity Inspector GUI
        DrawDefaultInspector();

        // Get referance to the script we are altering
        RayCastTester myScript = (RayCastTester)target;

        // Create the "Print Code" button and call the PrintCode method
        if (GUILayout.Button("Print Code"))
        {
            myScript.PrintCode();
        }

        // Create the "Print Draw Code" button and call the PrintDrawCode method
        if (GUILayout.Button("Print Draw Code"))
        {
            myScript.PrintDrawCode();
        }


        EditorGUILayout.LabelField("Flexable Code", EditorStyles.boldLabel);

        // Create the "Print Flexable Code" button and call the PrintFlexableCode method
        if (GUILayout.Button("Print Flexable Code"))
        {
            myScript.PrintFlexableCode();
        }

        // Create the "Print Flexable Draw Code" button and call the PrintFlexableDrawCode method
        if (GUILayout.Button("Print Flexable Draw Code"))
        {
            myScript.PrintFlexableDrawCode();
        }

        // Create the "Print Variables" button and call the PrintVarables method
        if (GUILayout.Button("Print Variables"))
        {
            myScript.PrintVariables();
        }
    }
}