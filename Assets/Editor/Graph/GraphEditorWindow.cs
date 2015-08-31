using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class GraphEditorWindow : EditorWindow
{
	public static string GetGraphColorKey(Graph graph) { return string.Format("Graph.{0}.color", graph.graphName); }

	public static float GraphNodePinRadius { get { return 0.2f; } }
	public static Vector3 GraphNodePinShift { get { return Vector3.up * 5.0f; } }

	public static Color GenerateGraphColor() { return new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)); }


	public static Dictionary<Graph, int> graphPinLevels = new Dictionary<Graph, int>();
	public static int GetGraphPinLevel(Graph graph)
	{
		if (graph == null)
			return -2;

		int level;
		if (!graphPinLevels.TryGetValue(graph, out level)) {
			level = graphPinLevels.Count;
            graphPinLevels.Add(graph, level);
		}
		return level;
	}


	public static Color GetGraphColor(Graph graph)
	{
		if (graph == null)
			return Color.gray;

		Color graphColor;

		string colorKey = GetGraphColorKey(graph);
		if (EditorPrefs.HasKey(colorKey)) {
			string colorString = EditorPrefs.GetString(colorKey);
			if (!Color.TryParseHexString(colorString, out graphColor)) {
				graphColor = GenerateGraphColor();
                SetGraphColor(graph, graphColor);
			}
		}
		else {
			graphColor = GenerateGraphColor();
			SetGraphColor(graph, graphColor);
		}

		return graphColor;
    }

	public static void SetGraphColor(Graph graph, Color color)
	{
		string colorKey = GetGraphColorKey(graph);
		EditorPrefs.SetString(colorKey, color.ToHexStringRGB());
	}

	public static Vector3 GetGraphNodePinShift(GraphNode node)
	{
		int pinLevel = GetGraphPinLevel(node.graph);
        Vector3 pinShift = GraphNodePinShift * (1.0f + (pinLevel * GraphNodePinRadius * 2.0f) / GraphNodePinShift.magnitude);
		return pinShift;
    }
}