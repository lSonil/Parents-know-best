using Pathfinding;
using UnityEngine;
public class NPCCat : SearchAI
{
    [Header("NPC")]
    private int mxHeigth;
    private int mxWidth;
    private bool inBed = false;
    private bool caged = false;
    private bool move = true;
    private MatrixHandler catBrain;
    [System.NonSerialized] public Vector2 currentCell;
    public Vector2 startingCell;
    public float distance;
    public int range;
    public int annoy;
    public Seeker seeker;
    public GameObject remains;

    Vector2[] runFromClock = { new Vector2(-1, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1) };
    Vector2[] runToClock = { new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0) };
    public void Start()
    {
        remains.SetActive(false);
        catBrain = FindFirstObjectByType<MatrixHandler>();
        mxWidth = catBrain.matrix[0].point.Length;
        mxHeigth = catBrain.matrix.Count;
        seeker.graphMask = 3;
        ResetToStart((int)startingCell.x, (int)startingCell.y);
    }

    public void SetCaged(bool value)
    {
        caged = value;
    }
    public void Update()
    {
        if (!move)
        {
            if (SameRoom(GlobalInfo.i.GetRoom("Livingroom")))
            {
                if (SameRoom(GlobalInfo.i.bby.room))
                {
                    if (!inBed)
                    {
                        move = true;
                        ResetSearch(GlobalInfo.i.bby.transform);

                        seeker.graphMask = 3;
                    }
                }
                else
                if (SameRoom(PlayerHandler.i.room))
                {
                    seeker.graphMask = 2;

                    if (!inBed)
                        CheckForPlayer();
                }
                else
                {
                    if (!caged)
                    {
                        inBed = false;
                        ResetToStart((int)startingCell.x, (int)startingCell.y);
                    }
                }
            }
        }

        AnimationHandle();
    }
    public void AnimationHandle()
    {
        anim.SetBool("isMoving", move);
        anim.SetFloat("moveX", moveX);
        anim.SetFloat("aware", inBed? 2: Vector2.Distance((Vector2)PlayerHandler.i.transform.position, transform.position)<=1 ? 1 :0);
    }
    public void MoveToCell(int x, int y)
    {
        move = true;
        ResetSearch(catBrain.CatCell(x, y).transform);
        currentCell = new Vector2(x, y);
    }

    public void ResetToStart(int x, int y)
    {
        if (currentCell != new Vector2(x, y))
        {
            transform.position = catBrain.CellPosition(x, y).transform.position;
            currentCell = new Vector2(x, y);
            MoveToCell(x, y);
        }
    }

    public override void TargetIsInRange()
    {
        move = false;
        transform.position = target.transform.position;

        switch(target.name)
        {
            case "VaseUsePos":
                GlobalInfo.i.GetKeyPos("Vase").GetComponent<InteractionVase>().Break();
                break;
            case "LivingroomToKitchenDoorPosition":
                Stop();
                transform.position = GlobalInfo.i.GetKeyPos("BasementWToBasementNWDoorPosition").transform.position;
                room = GlobalInfo.i.GetRoom("BasementW");
                ResetSearch(GlobalInfo.i.GetKeyPos("CatEatenUsePos").transform);
                break;
            case "CatEatenUsePos":
                anim.SetBool("dead", true);
                GlobalInfo.i.GetKeyPos("CrawlerTrigger").gameObject.SetActive(false);
                remains.SetActive(true);
                break;
            case "CatSpot1UsePos":
                annoy++;
                move = true;
                ResetSearch(GlobalInfo.i.GetKeyPos("CatSpot2UsePos").transform);
                break;
            case "CatSpot2UsePos":
                move = true;
                annoy++;
                if (annoy >= 6)
                {
                    annoy = 0;
                    MoveToCell((int)startingCell.x, (int)startingCell.y);
                }
                else
                {
                    ResetSearch(GlobalInfo.i.GetKeyPos("CatSpot1UsePos").transform);
                }
                break;
            case "BbyNPC":
                GlobalInfo.i.bby.Scare();
                if (GlobalInfo.i.bby.room != GlobalInfo.i.GetRoom("Livingroom"))
                    MoveToCell((int)startingCell.x, (int)startingCell.y);
                break;
        }

    }
    public void CheckForPlayer()
    {
        float dist = 0.1f;
        Vector2 foundPlayerAt = new Vector2(-1,-1);


        int startX = currentCell.x - range < 0 ? 0 : (int)currentCell.x - range;
        int startY = currentCell.y - range < 0 ? 0 : (int)currentCell.y - range;
        int endX = currentCell.x + range >= mxHeigth ? mxHeigth-1 : (int)currentCell.x + range;
        int endY = currentCell.y + range >= mxWidth ? mxWidth-1 : (int)currentCell.y + range;

        for (int i = startX; i <= endX; i++)
        {
            for (int j = startY; j <= endY; j++)
            {
                if (catBrain.Blocked(i, j) || (i == currentCell.x && j == currentCell.y) || (i != currentCell.x && j != currentCell.y) || Obstructed(currentCell, i, j))
                    continue;
                float testDist = Vector2.Distance((Vector2)PlayerHandler.i.transform.position, catBrain.CellPosition(i, j).transform.position);
                if (testDist <= 0.075f)
                {
                    dist = testDist;
                    foundPlayerAt = new Vector2(i, j);
                }
            }
        }
        if (foundPlayerAt != new Vector2(-1, -1))
            MoveToLocation(currentCell, foundPlayerAt);
    }

    public void MoveToLocation(Vector2 start, Vector2 end)
    {
        int startX = (int)start.x, startY = (int)start.y;
        int endX = (int)end.x, endY = (int)end.y;
        Vector2 dir;

        if (startX != endX)
        {
            dir = new Vector2(startX > endX ? -1 : 1, 0);

        }
        else
        {
            dir = new Vector2(0, startY > endY ? -1 : 1);
        }

        int index = System.Array.IndexOf(runFromClock, dir);

        for (int i = 0; i < 4; i++)
        {
            int newIndex = (index + i) % 4;


            if (runToClock[newIndex].x == dir.x && runToClock[newIndex].y == dir.y)
                continue;

            int checkX = (int)(currentCell.x + runToClock[newIndex].x);
            int checkY = (int)(currentCell.y + runToClock[newIndex].y);

            if (checkX >= mxHeigth || checkY >= mxWidth || checkX < 0 || checkY < 0)
                continue;

            if (catBrain.Blocked(checkX, checkY))
                continue;
            CheckDirection(currentCell, runToClock[newIndex]);

            return;
        }
    }

    public void CheckDirection(Vector2 cell, Vector2 add)
    {
        Vector2 newCell = cell + add;
        if (newCell.x >= mxHeigth || newCell.y >= mxWidth || newCell.x < 0 || newCell.y < 0 || catBrain.Blocked((int)newCell.x, (int)newCell.y))
        {
            MoveToCell((int)cell.x, (int)cell.y);
            CheckForEvents(cell, add);
        }
        else
        {
            CheckDirection(newCell, add);
        }
    }
    public void CheckForEvents(Vector2 cell, Vector2 direction)
    {
        //go to bed
        if (cell == new Vector2(7,6))
        {
            inBed = true;
            return;
        }

        //go to bathroom/ attic
        if (cell == new Vector2(2, 15) && direction == new Vector2(0, 1))
        {
            seeker.graphMask = 3;

            if (!GlobalInfo.i.GetDoor("DoorHallToAttic").GetComponent<Door>().locked)
            {
                ResetSearch(GlobalInfo.i.GetRoom("Attic").transform);
            }
            else
            {
                ResetSearch(GlobalInfo.i.GetRoom("Bathroom").transform);
            }
            return;
        }

        //go to kitchen
        if (cell == new Vector2(0, 3) && direction == new Vector2(-1, 0))
        {
            seeker.graphMask = 3;

            if (!GlobalInfo.i.GetDoor("DoorKitchenToBasementStairs").GetComponent<Door>().locked)
            {
                ResetSearch(GlobalInfo.i.GetKeyPos("LivingroomToKitchenDoorPosition").transform);
            }
            else
            {
                ResetSearch(GlobalInfo.i.GetKeyPos("CatSpot1UsePos").transform);
            }
            return;
        }

        //go to vase
        if (cell == new Vector2(3, 13) && direction == new Vector2(-1, 0))
        {
            seeker.graphMask = 3;
            ResetSearch(GlobalInfo.i.GetKeyPos("VaseUsePos").transform);
            return;
        }
    }
    public bool Obstructed(Vector2 start, int endX, int endY)
    {
        int min, max, line;
        int startX = (int)start.x, startY = (int)start.y;

        if (startX != endX)
        {
            min = startX > endX ? endX : startX;
            max = startX > endX ? startX : endX;
            line = startY;

            for (int i = 1; i < max - min; i++)
            {
                if(catBrain.Blocked(min+i, line))
                    return true;
            }
        }
        else
        {
            min = startY > endY ? endY : startY;
            max = startY > endY ? startY : endY;
            line = startX;

            for (int i = 1; i < max - min; i++)
            {
                if (catBrain.Blocked(line, min+i))
                    return true;
            }
        }

        return false;
    }


    public void MorningRoutine()
    {
    }
    public void DayEndRoutine()
    {

    }

    public override bool HuntPreconditionsCheck()
    {
        return false;
    }

}