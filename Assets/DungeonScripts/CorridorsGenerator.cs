using System;
using System.Collections.Generic;
using System.Linq;

public class CorridorsGenerator
{
    public List<Node> CreateCorridor(List<RoomNode> allNodesCollection, int corridorWidth)
    {
        List<Node> corridorList = new List<Node>();
        Queue<RoomNode> structuresToCheck = new Queue<RoomNode>(
            allNodesCollection.OrderByDescending(node => node.TreeLayerIndex).ToList());
        List<int> allID = new List<int>();
        while (structuresToCheck.Count > 0)
        {

            var node = structuresToCheck.Dequeue();
            int num = StructureHelper.TraverseGraphToExtractLowestLeafes(node).Count;
            if (node.ChildrenNodeList.Count == 0)
            {
                continue;
            }
            for (int i = 0; num > i; i++)
            {

                CorridorNode corridor = new CorridorNode(node.ChildrenNodeList[0], node.ChildrenNodeList[1], corridorWidth);

                if (allID.Contains(corridor.ID) == false)
                {
                    corridorList.Add(corridor);
                    allID.Add(corridor.ID);

                }
            }
        }
        return corridorList;
    }
}