using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGrid : MonoBehaviour
{
    //Public variables to be shown in the editor to create a grid to be used with A*
    public bool showDebugGrid = true;
    public float nodeDiameter;
    public Vector2 gridSize;
    public LayerMask mask;
    public int obstacleProximityCost = 10;

    //Private variables to be used within the grid class
    private PathNode[,] m_grid;
    private int m_gridSizeX, m_gridSizeY;
    private int m_MinCost = int.MinValue;
    private int m_MaxCost = int.MaxValue;
    private Vector3 m_gridWorldPos;

    //Public variable getters
    public int MaxGridSize
    {
        get { return m_gridSizeX * m_gridSizeY; }
    }

    private void Awake()
    {
        //Set up variables
        m_gridSizeX = Mathf.RoundToInt(gridSize.x / nodeDiameter);
        m_gridSizeY = Mathf.RoundToInt(gridSize.y / nodeDiameter);
    }

    private void Start()
    {
        m_gridWorldPos = gameObject.transform.position;
        //Generate the grid
        GenerateGrid();
    }

    //Function to generate a grid with the given variables
    private void GenerateGrid()
    {
        //Variables setup
        m_grid = new PathNode[m_gridSizeY, m_gridSizeY];
        float nodeRadius = nodeDiameter * 0.5f;
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridSize.x * 0.5f - Vector3.up * gridSize.y *0.5f;

        //Generate grid
        for (int y = 0; y < m_gridSizeY; y++)
        {
            for (int x = 0; x < m_gridSizeX; x++)
            {
                //Set up variables for a new node
                Vector3 worldPos = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                bool is_walkeable = !(Physics2D.OverlapBox(new Vector2(worldPos.x, worldPos.y), new Vector2(nodeRadius, nodeRadius), 0, mask));

                //Calculate movement cost
                int movementCost = 0;
                if (!is_walkeable)
                {
                    movementCost += obstacleProximityCost;
                }

                //Add square to grid
                m_grid[y, x] = new PathNode(is_walkeable, worldPos, x, y, movementCost);
            }
        }
    }

    //Get a node from a world position
    public PathNode GetPathNodeFromWorldPos(Vector3 worldPos)
    {
        Vector3 posToGrid = worldPos - m_gridWorldPos;
        //Get the porcent in the X and Y direciton of the curent pos relative to the grid
        float percentX = (posToGrid.x + gridSize.x * 0.5f) / gridSize.x;
        float percentY = (posToGrid.y + gridSize.y * 0.5f) / gridSize.y;

        //Clamp between 0-1 in case the porcent went outside it probally floating point presicion error or the player is outside the grid
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        //Get the position in the grid 
        int x = Mathf.RoundToInt((m_gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((m_gridSizeY - 1) * percentY);

        return m_grid[y,x];
    } 

    public List<PathNode> GetPathNodeNeighbours(PathNode node)
    {
        //Set up list
        List<PathNode> returnNodeList = new List<PathNode>();

        //Get the Neighbours
        //-1 to 1 for x and Y so if both are 0 its the current node
        for (int y = -1; y <= 1; y++)
        {
            for (int x= -1; x <= 1; x++)
            {
                if (0 == y && 0 == y)
                    continue;

                int newNodeX = node.gridXPos + x;
                int newNodeY = node.gridYPos + y;
                if (newNodeX >= 0 && newNodeX < m_gridSizeX && newNodeY >= 0 && newNodeY < m_gridSizeY)
                {
                    returnNodeList.Add(m_grid[newNodeY, newNodeX]);
                }
            }
        }
        //Return the list of neighbours
        return returnNodeList;
    }

    //Draw Gizmos so we can view the grid in the editor to debug stuff
    void OnDrawGizmos()
    {
        //Draw big grid to show where the grid is beeing generated
        Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, gridSize.y, 1));
        //Depending on debug status draw the different grids
        if (m_grid != null && showDebugGrid)
        {
            foreach (PathNode n in m_grid)
            {
                Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(m_MinCost, m_MaxCost, n.movementCost));
                Gizmos.color = (n.walkeable) ? Gizmos.color : Color.red;
                Gizmos.DrawCube(n.nodeWorldPos, Vector3.one * (nodeDiameter/2.0f));
            }
        }
    }
}