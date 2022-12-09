using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEditor;

public class PathfindingVisualiser : Pathfinding
{
    [SerializeField] float generalWaitTime = 2;
    [SerializeField] PathfindingInfo infoVisuals;
    Node scanningNeighbourNode;
    Node currentNode;

    public IEnumerator FindPathVisualised(Node startNode, Node endNode)
    {
        if (EncounteredProblem(startNode, endNode)) yield break;

        StartNewSearch(startNode, endNode);
        ResetNodeTexts(); 
        generator.Visualiser.SetTempSpriteColor(endNode, Color.cyan);

        
        while (!openList.IsEmpty())
        {
            currentNode = GetNewCurrentNode();
            MarkCurrentNode(currentNode); 
            yield return Wait(generalWaitTime); 
            
            if (currentNode == endNode)
            {
                yield break;
            }
    
            foreach (Node neighbourNode in generator.GetNeighbourList(currentNode))
            {
                scanningNeighbourNode = neighbourNode;
                if (!NodeIsWalkable(neighbourNode))
                {
                    MarkNonWalkableNode(neighbourNode);
                    continue;
                }
                MarkScannedWalkableNode(neighbourNode);
                
                int newFromStartCost = GetFromStartCost(currentNode, neighbourNode);
    
                yield return Wait(generalWaitTime);
                
                if (!costFromStart.ContainsKey(neighbourNode) || newFromStartCost < costFromStart[neighbourNode])
                {

                    AssignNewNode(currentNode,neighbourNode, endNode, newFromStartCost);
                    int estimateCost = CalculateManhattanCost(neighbourNode, endNode);
                    VisualiseNodeCost(neighbourNode, newFromStartCost, estimateCost);
                }
                RestoreColor(neighbourNode);
                scanningNeighbourNode = null;
            }
            RestoreColor(currentNode);
        }
        RestoreColor(endNode);
        Debug.Log("No Path found");
    }


    IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
    }
    
    public void ResetNodeTexts()
    {
        TextMeshPro textMeshPro = Resources.Load<TextMeshPro>("TextCube");

        generator.Visualiser.ResetSpriteColors();
        foreach (var node in nodeGrid.GridArray)
        {
            if (node.Visualiser.DebugText == null)
            {
                CreateTextObject(textMeshPro, node);
            }
            else
            {
                node.Visualiser.ChangeText(infoVisuals.DefaultText);
            }
        }
    }

    void CreateTextObject(TextMeshPro textMeshPro, Node node)
    {
        TextMeshPro textMeshProClone = Instantiate(textMeshPro, nodeGrid.GetWorldPosFromCoords(node.Coordinates), Quaternion.identity, this.transform);
        node.Visualiser.AssignDebugText(textMeshProClone, infoVisuals.DefaultText, Color.black);
        SceneVisibilityManager.instance.DisablePicking(textMeshProClone.gameObject, false);
    }

    void MarkNonWalkableNode(Node nonWalkableNode)
    {
        nonWalkableNode.Visualiser.ChangeText("X");
        generator.Visualiser.SetSpriteColor(nonWalkableNode, infoVisuals.WallTileColor);
    }

    void MarkCurrentNode(Node newCurrentNode)
    {
        generator.Visualiser.SetTempSpriteColor(newCurrentNode, infoVisuals.CurrentTileColor);
    }
    void MarkScannedWalkableNode(Node node)
    {
        generator.Visualiser.SetTempSpriteColor(node,infoVisuals.NeighbourTileColor);
    }
    void RestoreColor(Node node)
    {
        generator.Visualiser.RestoreColor(node);
    }
    void VisualiseNodeCost(Node node, int fromStartCost, int estimateToGoalCost)
    {
        node.Visualiser.VisualiseCosts(fromStartCost,estimateToGoalCost);
    }

    public void ChangePathfindingPace(float newValue)
    {
        generalWaitTime = newValue;
    }

    void OnDrawGizmos()
    {
        if (generator == null||scanningNeighbourNode == null || currentNode == null)
        {
            return;
        }
        DrawPointingArrow();
    }

    void DrawPointingArrow()
    {
        Handles.color = infoVisuals.PointingArrowColor;

        //Getting coordinates of the nodes
        Vector3 currentNodePos = nodeGrid.GetWorldPosFromCoords(currentNode.Coordinates);
        Vector3 neighbourNodePos = nodeGrid.GetWorldPosFromCoords(scanningNeighbourNode.Coordinates);

        //Drawing line between those nodes
        Gizmos.color = infoVisuals.DotColor;
        Gizmos.DrawSphere(currentNodePos, infoVisuals.DotRadius);
        Handles.DrawLine(currentNodePos, neighbourNodePos, infoVisuals.ArrowThickness);

        //Getting Angle of the direction
        Vector3 currentToNeighbourDir = (currentNodePos - neighbourNodePos).normalized;
        float dirAngle = Mathf.Atan2(currentToNeighbourDir.y, currentToNeighbourDir.x);

        //Adding To The Angle
        float angleRad01 = dirAngle + infoVisuals.ArrowAngleRad;
        float angleRad02 = dirAngle - infoVisuals.ArrowAngleRad;

        //Getting Vectors from new angles
        Vector3 arrowHeadPoint01 = neighbourNodePos + AngleToVector(angleRad01, currentNodePos.z) * infoVisuals.ArrowHeadLength;
        Vector3 arrowHeadPoint02 = neighbourNodePos + AngleToVector(angleRad02, currentNodePos.z) * infoVisuals.ArrowHeadLength;

        // Adding Arrow Heads
        Handles.DrawLine(neighbourNodePos, arrowHeadPoint01, infoVisuals.ArrowThickness);
        Handles.DrawLine(neighbourNodePos, arrowHeadPoint02, infoVisuals.ArrowThickness);
    }

    Vector3 AngleToVector(float angleRad, float zPos)
    {
        float xPos = Mathf.Cos(angleRad);
        float yPos = Mathf.Sin(angleRad);
        return new Vector3(xPos, yPos, zPos);
    }
}
