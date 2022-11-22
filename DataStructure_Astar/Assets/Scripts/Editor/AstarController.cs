using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class AstarController : EditorWindow
{
    const float SPACE_WIDTH = 15.0f;
    const float BUTTON_WIDTH = 200.0f;
    const float BUTTON_HEIGHT = 30.0f;

    Vector2 scrollPos;


    //grid setup info
    int width;
    int height;
    Vector2 cellSize;

    //Debuging with Coordinates
    Color coordsColor = Color.magenta;
    float coordsFontSize = 8;

    //VisualiseGrid
    float lineThickness = 2;
    Color lineColor = Color.green;


    bool settingWalls;

    
    //Player stuff
    Vector2Int playerPosition;
    
    
    
    Player testPlayer;


    GridGenerator gridGenerator;
    GridGeneratorScriptableObject gridGeneratorScriptableObject;

    CustomGrid<Node> nodeGrid => generator.NodeGrid;


    #region Properties
    

    GridGenerator generator
    {
        get
        {
            if (gridGenerator == null)
            {
                gridGenerator = FindObjectOfType<GridGenerator>();
                if (gridGenerator == null)
                {
                    gridGenerator = Instantiate(generatorScriptable.GridGeneratorPrefab);
                    gridGenerator.gameObject.name = "Grid Generator";
                }
            }

            return gridGenerator;
        }
    }


    GridGeneratorScriptableObject generatorScriptable
    {
        get
        {
            if (gridGeneratorScriptableObject == null)
            {
                gridGeneratorScriptableObject = Resources.Load<GridGeneratorScriptableObject>("Generator scriptable");
            }

            return gridGeneratorScriptableObject;
        }
    }
    #endregion

    #region Editor Window Initialization and Setup

    [MenuItem("A* Controller/Open Window")]
    static void Open()
    {
        AstarController win = GetWindow<AstarController>();
        win.titleContent = new GUIContent("A* Controller");
        win.Show();
    }


    void OnDidOpenScene()
    {
        SetupPlayer(true);
    }


    void OnEnable()
    {
        LoadData();
        
        SetupPlayer(true);

        SceneView.duringSceneGui += OnSceneGUI;
    }


    void OnDisable()
    {
        RemoveGridCoordinates();

        SaveData();
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    #endregion

    #region ON INSPECTOR GUI

    #region Gui making stuff more appealing --- HELPERS

    void InsertSpace(float spaceWidth = SPACE_WIDTH)
    {
        EditorGUILayout.Space(spaceWidth);
    }


    void ButtonGUI(string buttonName, Action buttonAction)
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button(buttonName, GUILayout.Height(BUTTON_HEIGHT), GUILayout.Width(BUTTON_WIDTH)))
        {
            buttonAction();
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    void InsertLabel(string labelName)
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("---");

        GUILayout.Label(labelName);
        GUILayout.Label("---");

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        InsertSpace();
    }

    #endregion


    void OnGUI()
    {
        InsertLabel($"Node grid is null: {StringsGUI.YesNoTextFromBool(nodeGrid==null)}");
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);


        GridGUI();

        SettingIsWalkable();


        GridVisualizationValues();
        DisplayCoordinatesOptions();


        PlayerControls();
        EditorGUILayout.EndScrollView();
        
    }


    void PlayerControls()
    {

        InsertLabel("Player Controls");

 
            playerPosition = EditorGUILayout.Vector2IntField("Player Start Pos", playerPosition);
            ButtonGUI("Create Player", () =>
            {
                Coordinates playerStartCoords = new Coordinates(playerPosition);
                Node startPlayerPosNode = nodeGrid.GetGridType(playerStartCoords);
                if (startPlayerPosNode == null || !startPlayerPosNode.IsWalkable)
                {
                    return;
                }

                if (testPlayer == null)
                {
                    testPlayer = Resources.Load<Player>("Player");
                    testPlayer = Instantiate(testPlayer);
                }
                Undo.RecordObject(testPlayer, "Generator to save");

                
                testPlayer.CreatePlayer(generator,startPlayerPosNode);

            });
        

    }
    void SetupPlayer(bool tryToFind = false)
    {
        if (tryToFind)
        {
            testPlayer = FindObjectOfType<Player>();
        }
        if (testPlayer !=null)
        {
            //testPlayer.ReassignGrid(nodeGrid);
        }

    }
    void SettingIsWalkable()
    {
        if (nodeGrid == null) return;
        InsertLabel("Set walls");

        ButtonGUI($"Setting Walls: {StringsGUI.YesNoTextFromBool(settingWalls)}", () =>
        {
            settingWalls = !settingWalls;
        });
        InsertSpace();
    }


    void GridGUI()
    {
        InsertLabel("Grid Settings");
        width = EditorGUILayout.IntField("Width", width);
        height = EditorGUILayout.IntField("Height", height);
        cellSize = EditorGUILayout.Vector2Field("Cell Size", cellSize);

        ButtonGUI("Create New Grid", CreateGrid);

        InsertSpace();
    }

    void CreateGrid()
    {
        Undo.RecordObject(generator, "Generator to save");
        generator.CreateGrid(width, height, cellSize);
        SetupPlayer();

    }

    void GridVisualizationValues()
    {
        InsertLabel("Grid Gizmos");

        lineColor = EditorGUILayout.ColorField("Line Color", lineColor);
        lineThickness = EditorGUILayout.FloatField("Line Thickness", lineThickness);

        InsertSpace();
    }

    void DisplayCoordinatesOptions()
    {
        InsertLabel("Coordinates Debugging");
        if (!generator.DisplayingCoordinates())
        {
            coordsColor = EditorGUILayout.ColorField("Coordinates Color", coordsColor);
            coordsFontSize = EditorGUILayout.FloatField("Coordinates Font Size", coordsFontSize);

            ButtonGUI("Display Coordinates", DisplayGridCoordinates);

            InsertSpace();
            return;
        }


        ButtonGUI("Remove Coordinates", RemoveGridCoordinates);

        InsertSpace();
    }

    void DisplayGridCoordinates()
    {
        RemoveGridCoordinates();
        nodeGrid?.DisplayGridCoordinates(coordsColor, coordsFontSize, generator.ParentForTexts);
    }

    void RemoveGridCoordinates()
    {
        generator.DeleteCoordsText();
    }

    #endregion


    #region ON SCENE GUI

    void OnSceneGUI(SceneView sceneView)
    {
        
        if (nodeGrid == null) return;

        nodeGrid.VisualiseGrid(lineColor, lineThickness);




        if (!settingWalls) return;
        

        Event current = Event.current;
        Vector2 mousePosition = current.mousePosition;
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        mousePosition = ray.origin;

        generatorScriptable.VisualisePointer(mousePosition, Vector3.forward);


        //Register Mouse Click

        if (current.type == EventType.MouseDown && current.button == 0)
        {
            nodeGrid.GetGridCoordsFromWorld(mousePosition, out int x, out int y);
            Node clickedNode = nodeGrid.GetGridType(x, y);
            if (clickedNode != null)
            {
                if (settingWalls)
                {
                    Undo.RecordObject(generator, "Generator to save");
                    clickedNode.SetWalkable(!clickedNode.IsWalkable);

                }
            }
        }


        SceneView.RepaintAll();
    }

    
    //Makes crosses over grid, but not used anymore
    void VisualiseWalls()
    {
        Handles.color = Color.black;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Node node = nodeGrid.GetGridType(x, y);
                if (node == null)
                {
                    continue;
                }
                if (!node.IsWalkable)
                {
                    Vector3 worldPos = nodeGrid.GetWorldPosFromCoords(node.Coordinates);
                    Vector3 leftCorner01 = new Vector3(worldPos.x - cellSize.x * 0.5f, worldPos.y + cellSize.y * 0.5f);
                    Vector3 leftCorner02 = new Vector3(worldPos.x - cellSize.x * 0.5f, worldPos.y - cellSize.y * 0.5f);
                    Vector3 rightCorner01 = new Vector3(worldPos.x + cellSize.x * 0.5f, worldPos.y - cellSize.y * 0.5f);
                    Vector3 rightCorner02 = new Vector3(worldPos.x + cellSize.x * 0.5f, worldPos.y + cellSize.y * 0.5f);
                    Handles.DrawLine(leftCorner01, rightCorner01, 2);
                    Handles.DrawLine(leftCorner02, rightCorner02, 2);
                }
            }
        }
    }

    #endregion


    #region Save Load Data

    void SaveData()
    {
        AstarControllerData astarControllerData = new AstarControllerData(width, height, cellSize);

        astarControllerData.SaveCoordinatesInfo(coordsFontSize, coordsColor);
        astarControllerData.SaveGridLineInfo(lineThickness, lineColor);
        astarControllerData.SavePlayerInfo(playerPosition);
        astarControllerData.Save();
    }

    void LoadData()
    {
        AstarControllerData data = new AstarControllerData();
        data = data.Load();
        data.LoadGridData(ref width, ref height, ref cellSize);
        // height = data.Height;
        // width = data.Width;
        // cellSize = data.CellSize;
        data.LoadCoordinatesInfo(ref coordsFontSize, ref coordsColor);
        data.LoadGridLineInfo(ref lineThickness, ref lineColor);

        // coordsColor = data.CoordinatesGizmosInfo.ColorValue;
        // coordsFontSize = data.CoordinatesGizmosInfo.FloatValue;
        //
        //
        // lineColor = data.GridLineInfo.ColorValue;
        // lineThickness = data.GridLineInfo.FloatValue;

        
        data.LoadPlayerInfo(ref playerPosition);

        // playerPosition = data.PlayerPosition;
    }

    #endregion
}
