using System.Collections;
using System.Linq;
using UnityEngine;

public class ZoomNodeHandler : MonoBehaviour
{
    public int[] sequence;
    public InteractionNodeAndDrop[] nodes;
    public bool canStart = true;
    public int step = 0;
    public int hasInteracted = -1;
    public GameObject turnOn;
    public GameObject turnOff;

    private void Start()
    {
        turnOn.SetActive(false);
        turnOff.SetActive(true);
    }

    private void FixedUpdate()
    {
        hasInteracted = -1;

        foreach (InteractionNodeAndDrop node in nodes)
        {
            if (node.interacted)
                hasInteracted = nodes.ToList().IndexOf(node);
        }

        if (hasInteracted != -1)
        {
            foreach (InteractionNodeAndDrop node in nodes)
            {
                if (nodes.ToList().IndexOf(node) != hasInteracted)
                    node.ForceEvent(false);
            }
            if (canStart && nodes.ToList().IndexOf(nodes[hasInteracted]) == sequence[step] && FailCheck())
            {
                step++;
            }
            else
            {
                step = 0;

                StartCoroutine(WaitToReset());
            }

            nodes[hasInteracted].interacted = false;
        }
        if (step >= sequence.Length)
        {
            turnOn.SetActive(true);
            turnOff.SetActive(false);
        }
    }

    IEnumerator WaitToReset()
    {
        canStart = false;
        while (!canStart)
        {
            canStart = true;
            foreach (InteractionNodeAndDrop node in nodes)
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
