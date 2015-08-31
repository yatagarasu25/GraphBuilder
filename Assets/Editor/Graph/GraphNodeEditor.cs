using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(GraphNode))]
public class GraphNodeEditor : Editor
{
	static List<string> graphNames = new List<string>();
	static Dictionary<string, Graph> graphs = new Dictionary<string, Graph>();

	GraphNode graphNode;
	int graphIndex;

	static GraphNode selectedAddLinkObjectFrom;
    static GameObject selectedAddLinkObjectTo;

	public void OnEnable()
	{
		graphNode = (GraphNode)target;

		UpdateGraphList(null);
    }

	public void OnDisable()
	{
	}

	public void OnDestroy()
	{
		if (!target) {
			graphNode.OnDestroy();
        }
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		int newGraphIndex = EditorGUILayout.Popup(graphIndex, graphNames.ToArray<string>());
		if (newGraphIndex >= 0 && newGraphIndex != graphIndex) {
			graphIndex = newGraphIndex;
            graphNode.graph = graphs[graphNames[graphIndex]];
		}

		int toolbarButton = GUILayout.Toolbar(-1, new string[] { "+", "-" });
		if (toolbarButton == 0) {
			selectedAddLinkObjectFrom = graphNode;
            EditorGUIUtility.ShowObjectPicker<GraphNode>(null, true, "", 0);
        }

		if (Event.current.commandName == "ObjectSelectorUpdated") {
			selectedAddLinkObjectTo = EditorGUIUtility.GetObjectPickerObject() as GameObject;
			SceneView.RepaintAll();
		}
		if (Event.current.commandName == "ObjectSelectorClosed") {
			if (selectedAddLinkObjectFrom != null && selectedAddLinkObjectFrom != selectedAddLinkObjectTo) {
				GraphNode nodeFrom = selectedAddLinkObjectFrom;
				GraphNode nodeTo = null;

				foreach (GraphNode nodeToCandidate in selectedAddLinkObjectTo.GetComponents<GraphNode>()) {
					if (nodeToCandidate.graph == nodeFrom.graph) {
						nodeTo = nodeToCandidate;
						break;
					}
				}

				if (nodeTo == null) {
					bool result = EditorUtility.DisplayDialog("Add new GraphNode component to an Object?"
						, "GameObject does not have GraphNode component belonging to a Graph. Should I add a new GraphNode component to the GameObject?"
						, "Add", "Do not add");
					if (result) {
						nodeTo = selectedAddLinkObjectTo.gameObject.AddComponent<GraphNode>();
						nodeTo.graph = nodeFrom.graph;
					}
				}

				nodeFrom.LinkToNode(nodeTo);
			}

			selectedAddLinkObjectFrom = null;
			selectedAddLinkObjectTo = null;
			SceneView.RepaintAll();
		}
	}

	public void OnSceneGUI()
	{
		if (selectedAddLinkObjectFrom != null && selectedAddLinkObjectTo != null) {
			Vector3 pinShift = GraphEditorWindow.GetGraphNodePinShift(selectedAddLinkObjectFrom);
			Handles.DrawLine(selectedAddLinkObjectFrom.transform.position + pinShift, selectedAddLinkObjectTo.transform.position + pinShift);
		}
	}

	[DrawGizmo(GizmoType.Selected)]
	public static void DrawGraphNodeGizmo(GraphNode node, GizmoType type)
	{
		using (new GizmosColor(GraphEditorWindow.GetGraphColor(node.graph))) {
			if (node.graph == null) {
				Vector3 nodePinPosition = node.transform.position + GraphEditorWindow.GetGraphNodePinShift(node);
				Gizmos.DrawLine(node.transform.position, nodePinPosition);
				Gizmos.DrawWireSphere(nodePinPosition, GraphEditorWindow.GraphNodePinRadius * 1.1f);

				return;
			}

			if ((type & GizmoType.Selected) != 0) {
				GraphEditor.DrawGraphGizmo(node.graph, GizmoType.NonSelected);

				Vector3 pinShift = GraphEditorWindow.GetGraphNodePinShift(node);
				Vector3 nodePinPosition = node.transform.position + pinShift;
				Gizmos.DrawLine(node.transform.position, nodePinPosition);
				Gizmos.DrawWireSphere(nodePinPosition, GraphEditorWindow.GraphNodePinRadius * 1.1f);

				foreach (GraphNode linkedNode in node.EnumLinkedNodes()) {
					Gizmos.DrawLine(nodePinPosition, linkedNode.transform.position + pinShift);
				}
			}
			else if (type == GizmoType.NonSelected) {
				Vector3 pinShift = GraphEditorWindow.GetGraphNodePinShift(node);
				Vector3 nodePinPosition = node.transform.position + pinShift;
				Gizmos.DrawLine(node.transform.position, nodePinPosition);
				Gizmos.DrawSphere(nodePinPosition, GraphEditorWindow.GraphNodePinRadius);
			}
		}
    }

	protected void UpdateGraphList(GameObject root)
	{
		graphIndex = -1;
        graphs.Clear();
		
		foreach (Graph graph in GameObject.FindObjectsOfType<Graph>()) {
			graphNames.Add(graph.graphName);
            graphs.Add(graph.graphName, graph);

			if (graphNode.graph == graph) {
				graphIndex = graphs.Count - 1;
            }
        }
	}
}