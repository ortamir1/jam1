using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playeractions : MonoBehaviour
{
    //settings
    [SerializeField] private int mInitPlayers = 1;
    [SerializeField] private float mTileSize = 2;
    [SerializeField] private int mTilesPerTurn = 3;
    [SerializeField] private float mMoveSpeed = 1;
    //used to determine how long the move animations is, might be unncessery
    //[SerializeField] float mMovetime = 1.0f;//seconds
    [SerializeField] float mTurntime = 30.0f;//seconds
    //objects
    [SerializeField] private GameObject mBomb;
    [SerializeField] private GameObject[] players;
    //controls:
    [SerializeField] private KeyCode mSkipAct = KeyCode.KeypadEnter;
    [SerializeField] private KeyCode mShootAct = KeyCode.Mouse0;
    [SerializeField] private KeyCode mUpAct = KeyCode.UpArrow;
    [SerializeField] private KeyCode mRightAct = KeyCode.RightArrow;
    [SerializeField] private KeyCode mLeftAct = KeyCode.LeftArrow;
    [SerializeField] private KeyCode mDownAct = KeyCode.DownArrow;


    //uses the enum to determaine wether we are moveing shooting or waiting 
    public enum turn_state{ move, shoot, end};
    private turn_state state;
    //how many turns passed:
    private int mTurns;
    //how many players are currently at play
    private int mPlayers;
    //signifies whether the players are alive or not:
    private bool[] pAlive;
    //how many blocks have we moved this turn:
    private int CurrMoved;
    // can calcutlate whos turn it is by % with mPlayers 
    private int WhosTurn;
    //going to position:
    private Vector3 target;
    //if moving we will disable other functions
    private bool isMove;
    //curr rigidbody
    private Rigidbody CurrRigid;

    // Start is called before the first f   rame update
    void Start()
    {
        state = turn_state.move;
        mTurns = 0;
        mPlayers = mInitPlayers;
        pAlive = new bool[mPlayers];
        //might be unnecssery
        for(int i = 0; i < mPlayers; i++)
        {
            pAlive[i] = true;
        }
        CurrMoved = 0;
        WhosTurn = 0;
        CurrRigid = players[WhosTurn].GetComponent<Rigidbody>();
        target = CurrRigid.position;
        isMove = false;
        StartCoroutine(turnswitch(mTurns));
    }

// Update is called once per frame
void Update()
    {
        //if move it moves the currents player rigidBody, then it will change to shoot
        //until someone fires then it will switch the players turn, also i will be conting on changing the  
        // death flag in the array via trigger on impact by hitting the water 
        switch (state)
        {
            case turn_state.move:
                {
                    Move(players[WhosTurn]);
                    break;
                }
            case turn_state.shoot:
                {
                    Shoot();
                    break;
                }
            case turn_state.end:
                {
                    End();
                    break;
                }
        }
    }




    void Move(GameObject mPlayerCurr)
    {
        if (isMove)
        {
            CurrRigid.MovePosition(Vector3.MoveTowards(CurrRigid.position, target, Time.deltaTime * mMoveSpeed));
            if (CurrRigid.position.Equals(target))
            {
                isMove = false;
            }
            return;
        }
        //checks if we wanna/ have to end the turn
        if(CurrMoved >= mTilesPerTurn || Input.GetKey(mSkipAct))
        {
            Debug.Log("finshed moving");
            state = turn_state.shoot;
            CurrMoved = 0;
            return;
        }
        //todo: add a function that by location will detarmaine if you can go in that certain directiond
        if (Input.GetKey(mUpAct))
        {
            target += new Vector3(0, mTileSize, 0);
            CurrMoved += 1;
            isMove = true;
            return;
        }
        if (Input.GetKey(mDownAct))
        {
            target += new Vector3(0, -mTileSize, 0);
            CurrMoved += 1;
            isMove = true;
            return;
        }
        if (Input.GetKey(mRightAct))
        {
            target += new Vector3(mTileSize, 0, 0);
            CurrMoved += 1;
            isMove = true;
            return;

        }
        if (Input.GetKey(mLeftAct))
        {
            target += new Vector3(-mTileSize, 0, 0);
            CurrMoved += 1;
            isMove = true;
            return;
        }

    }

    void Shoot()
    {
        //checks if we left clicked and fired out shot
        // can also be implemented with the tiles as buttons but for now i will base it on cursor location
        // using default con
        if (Input.GetKey(mShootAct))
        {
            //implement ValidLoc by the map
            //will only work for a singular bomb rn but i will add weapon selection
            if (ValidLoc())
            {

                Debug.Log("finshed shooting");
                //todo: summon the bomb/wanted weapon                
                state = turn_state.end;
            }
        }
    }

    //to be implemented: if the clicked location is valid
    bool ValidLoc()
    {
        return true;
    }

    //returns whether we are in the middle of something or if we can end the turn
    bool canSwitch()
    {
        //for now we only have to check isMove
        return isMove;
    }



    void End()
    {
        mTurns += 1;
        state = turn_state.move;
        WhosTurn = (WhosTurn + 1) % mPlayers;
        while (!pAlive[WhosTurn])
        {
            WhosTurn = (WhosTurn + 1) % mPlayers;
        }
        Debug.Log("next player:" + WhosTurn);
        CurrRigid = players[WhosTurn].GetComponent<Rigidbody>();
        target = CurrRigid.position;
        //todo: end the game when pAlive is 1, maybe add a public int that says how many are alive and then and it
        StartCoroutine(turnswitch(mTurns));
    }

    private IEnumerator turnswitch(int currturn)
    {
        yield return new WaitForSeconds(mTurntime);
        // should wait for isMove
        if (currturn == mTurns)
        {
            Debug.Log("next turn");
            state = turn_state.end;
        }
    }
}
