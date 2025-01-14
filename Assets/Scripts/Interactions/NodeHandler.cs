using System.Collections;
using System.Linq;
using UnityEngine;

public class NodeHandler : MonoBehaviour
{
    public int[] sequence;
    public InteractionNode[] nodes;
    public bool canStart = true;
    public int step = 0;
    public int hasInteracted = -1;
    public Item correctItem;
    public Item wrongItem;
    public InteractionNode correctIndex;
    public InteractionNode wrongIndex;
    public GameObject turnOn;
    public GameObject turnOff;

    private void Start()
    {
        turnOn.SetActive(false);
        turnOff.SetActive(true);

        correctIndex = nodes[sequence[0]];

        wrongItem = null;
        correctItem = null;

        foreach (InteractionNode node in nodes)
        {
            if (node.itemToSpawn != null)
            {
                if (nodes.ToList().IndexOf(node) != sequence[0])
                {
                    if (wrongItem == null)
                    {
                        wrongIndex = node;
                        wrongItem = wrongIndex.itemToSpawn;
                    }
                    else
                        Debug.LogError("Warning: More than one item in nodes!");
                }
            }
        }
        correctItem = correctIndex.itemToSpawn;

    }

    private void FixedUpdate()
    {
        hasInteracted = -1;

        foreach (InteractionNode node in nodes)
        {
            if (node.interacted)
                hasInteracted = nodes.ToList().IndexOf(node);
        }

        correctItem = correctIndex.itemchanged ? correctIndex.itemToSpawn : correctItem;
        wrongItem = wrongIndex.itemchanged ? wrongIndex.itemToSpawn : wrongItem;

        if (hasInteracted != -1)
        {
            foreach (InteractionNode node in nodes)
            {
                if (nodes.ToList().IndexOf(node) != hasInteracted)
                    node.ForceEvent(false);
            }
            if (canStart && nodes.ToList().IndexOf(nodes[hasInteracted]) == sequence[step] && FailCheck())
            {
                if (correctItem != null)
                {

                    correctIndex.Drop();
                }

                if (wrongIndex == nodes[hasInteracted])
                {
                    if (wrongItem != null)
                    {

                        wrongIndex.Drop();
                        nodes[hasInteracted == 0 ? 1 : 0].Hold(wrongItem);
                    }
                    wrongIndex = nodes[hasInteracted == 0 ? 1 : 0];
                }

                if (correctItem != null)
                {

                    nodes[hasInteracted].Hold(correctItem);
                }
                correctIndex = nodes[hasInteracted];
                step++;
            }
            else
            {
                if (wrongItem != null)
                {
                    wrongIndex.Drop();
                }

                if (correctIndex == nodes[hasInteracted])
                {

                    if (correctItem != null)
                    {
                        correctIndex.Drop();
                        nodes[hasInteracted == 2 ? 3 : 2].Hold(correctItem);
                    }
                    correctIndex = nodes[hasInteracted == 2 ? 3 : 2];
                }

                wrongIndex = nodes[hasInteracted];
                if (wrongItem != null)
                {

                    wrongIndex.Hold(wrongItem);
                }
                step = 0;

                StartCoroutine(WaitToReset());
            }

            nodes[hasInteracted].interacted = false;
        }
        if (step >= sequence.Length)
        {
            turnOn.SetActive(true);
            turnOff.SetActive(false);
            if (wrongItem != null)
            {
                wrongItem.GetComponentInParent<InteractionNode>().Drop();
                nodes[1].Hold(wrongItem);
            }
            if (correctItem != null)
            {
                correctItem.GetComponentInParent<InteractionNode>().Drop();
                nodes[3].Hold(correctItem);
            }

            this.enabled = false;
        }
    }

    IEnumerator WaitToReset()
    {
        canStart = false;
        while (!canStart)
        {
            canStart = true;
            foreach (InteractionNode node in nodes)
            {
                if(node.opend)
                canStart = false;
            }
            yield return new WaitForSeconds(0.01f);
        }
    }

    public bool FailCheck()
    {
        if(step==0 && nodes[0].interacted && !nodes[0].opend)
            return false;
        return true;
    }
}
