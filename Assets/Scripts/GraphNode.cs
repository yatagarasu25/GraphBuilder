using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphNode : MonoBehaviour
{
	[SerializeField]
	public Graph graph;

	// Use this for initialization
	private void Start()
	{
	}

	// Update is called once per frame
	private void Update()
	{
	}

	public void OnDestroy()
	{
		if (graph != null) {
			graph.RemoveNode(this);
		}
	}

	public IEnumerable<GraphNode> EnumLinkedNodes()
	{
		List<GraphNode> links;
		if (graph == null || !graph.Links.TryGetValue(this, out links)) {
			return Enumerable.Empty<GraphNode>();
		}

		return links;
	}

	public bool LinkToNode(GraphNode to)
	{
		if (to.graph != graph)
			return false;

		graph.AddLink(this, to);

		return true;
	}
}