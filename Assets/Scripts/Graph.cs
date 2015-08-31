using System;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour, ISerializationCallbackReceiver
{
	[SerializeField]
	public string graphName;

	[SerializeField, HideInInspector]
	private List<GraphNode> nodes = new List<GraphNode>();
	private Dictionary<GraphNode, List<GraphNode>> links = new Dictionary<GraphNode, List<GraphNode>>();

	[SerializeField, HideInInspector]
	private List<int> flattedLinkIndices = new List<int>();
	[SerializeField, HideInInspector]
	private List<GraphNode> flattedLinks = new List<GraphNode>();


	public List<GraphNode> Nodes { get { return nodes; } }
	public Dictionary<GraphNode, List<GraphNode>> Links { get { return links; } }


	// Use this for initialization
	private void Start()
	{
	}

	// Update is called once per frame
	private void Update()
	{
	}

	public void Clear()
	{
		nodes.Clear();
	}

	public void AddNode(GraphNode node)
	{
		nodes.Add(node);
		node.graph = this;
	}

	public void RemoveNode(GraphNode node)
	{
		if (nodes.Remove(node)) {
			links.Remove(node);

			foreach (var link in links) {
				link.Value.Remove(node);
			}
		}
	}

	public void AddLink(GraphNode a, GraphNode b)
	{
		List<GraphNode> nodeLinks;
		if (links.TryGetValue(a, out nodeLinks)) {
			nodeLinks.Add(b);
		}
		else {
			links.Add(a, new List<GraphNode> { b });
		}
	}

	public void OnBeforeSerialize()
	{
		flattedLinkIndices = new List<int>();
		flattedLinks = new List<GraphNode>();

		foreach (var link in links) {
			flattedLinkIndices.Add(flattedLinks.Count);
            flattedLinks.Add(link.Key);
			flattedLinks.AddRange(link.Value);
        }

		flattedLinkIndices.Add(flattedLinks.Count);
	}

	public void OnAfterDeserialize()
	{
		for (int i = 0; i < flattedLinkIndices.Count - 1; i++) {
			int linksBegin = flattedLinkIndices[i];
			int linksEnd = flattedLinkIndices[i + 1];

			if (linksBegin != linksEnd) {
				GraphNode nodeFrom = flattedLinks[linksBegin];
				for (int li = linksBegin + 1; li < linksEnd; li++) {
					AddLink(nodeFrom, flattedLinks[li]);
				}
			}
		}

		flattedLinkIndices = null;
		flattedLinks = null;
    }
}