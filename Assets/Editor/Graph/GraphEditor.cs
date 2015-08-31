using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Graph))]
public class GraphEditor : Editor
{
	private Graph graph;
	private Color graphColor;

	public void OnEnable()
	{
		graph = (Graph)target;
		graphColor = GraphEditorWindow.GetGraphColor(graph);

		UpdateGraphNodes(null);
		GraphEditorWindow.graphPinLevels.Clear();
		GraphEditorWindow.graphPinLevels.Add(graph, 0);
    }

	public void OnDisable()
	{
	}

	public override void OnInspectorGUI()
	{
		graphColor = EditorGUILayout.ColorField(graphColor);
		GraphEditorWindow.SetGraphColor(graph, graphColor);

		base.OnInspectorGUI();
	}

	[DrawGizmo(GizmoType.Selected)]
	public static void DrawGraphGizmo(Graph graph, GizmoType type)
	{
		Color graphColor = GraphEditorWindow.GetGraphColor(graph);
        using (new GizmosColor(graphColor)) {
			foreach (GraphNode node in graph.Nodes) {
				GraphNodeEditor.DrawGraphNodeGizmo(node, GizmoType.NonSelected);
			}


			Color linkColor = Color.gray;
			if ((type & GizmoType.Selected) != 0) {
				linkColor = graphColor * 0.75f;
			}
			else if ((type & GizmoType.NonSelected) != 0) {
				linkColor = graphColor * 0.25f;
			}

			using (new GizmosColor(linkColor)) {
				foreach (var links in graph.Links) {
					GraphNode nodeFrom = links.Key;
					foreach (GraphNode nodeTo in links.Value) {
						Vector3 pinShift = GraphEditorWindow.GetGraphNodePinShift(nodeTo);
						Gizmos.DrawLine(nodeFrom.transform.position + pinShift, nodeTo.transform.position + pinShift);
					}
				}
			}
		}
	}

	protected void UpdateGraphNodes(GameObject root)
	{
		graph.Clear();

		foreach (GraphNode node in GameObject.FindObjectsOfType<GraphNode>()) {
			if (node.graph == graph) {
				graph.AddNode(node);
			}
		}
	}
}