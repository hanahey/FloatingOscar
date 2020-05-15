using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    private const float MOVE_SPEED = 60f;
    private const float BRANCH_WIDTH = 35.4f;
    private const float BRANCH_HEIGHT = 7.8f;
    private const float BRANCH_SPAWN_Y_POSITION = 50f;
    private const float BRANCH_DESTROY_Y_POSITION = -50f;
    private const float BRANCH_BOX_COLLIDER_WIDTH_SCALE = 0.8418f;
    private const float BRANCH_BOX_COLLIDER_HEIGHT_SCALE = 0.47668f;
    private const float BRANCH_BOX_COLLIDER_X_OFFSET_SCALE = -0.07627f;
    private const float TREE_BODY_WIDTH = 5f;
    private const float TREE_BODY_HEIGHT = 120f;
    private const float CLOUD_DESTROY_Y_POSITION = -80f;
    private const float CLOUD_SPAWN_Y_POSITION = 80f;

    private static Level instance;

    public static Level GetInstance() //to access the current Level instance in the score window
    {
        return instance;
    }
    
    private List<Transform> cloudList;
    private float cloudSpawnTimer;
    private float cloudMoveSpeed = MOVE_SPEED*0.2f;
    private float cloudSpawnTimerMax = 3f;

    private List<Branch> branchList;
    private int branchesPassed = 0;
    private int branchesSpawned;
    private float branchSpawnTimer;
    private float branchSpawnTimerMax;

    private State state;

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Brutal,
        Nightmare1,
        Nightmare2,
        Nightmare3,
        Impossible,
    }
    
    public enum BranchType
    {
        Left,
        Right,
        Pair,
    }

    public enum State
    {
        WaitingToStart,
        Playing,
        OscarHit,
    }

    private void Awake()
    {
        instance = this;
        branchList = new List<Branch>();
        cloudList = new List<Transform>();
        CreateTreeBodyRightLeft();
        SetDifficulty(Difficulty.Easy);
        state = State.WaitingToStart;
    }

    private void Start()
    {
        Oscar.GetInstance().OnHit += Oscar_OnHit;
        Oscar.GetInstance().OnStartPlaying += Oscar_OnStartPlaying;
    }

    private void Update()
    {
        if (state == State.Playing)
        {
            HandleBranchSpawning();
            HandleBranchMovement();
            HandleCloudsSpawningAndMovement();
        }
    }

    private void Oscar_OnStartPlaying(object sender, System.EventArgs e)
    {
        state = State.Playing;
    }

    private void Oscar_OnHit(object sender, System.EventArgs e)
    {
        state = State.OscarHit;
    }
    private void SetDifficulty(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                branchSpawnTimerMax = 1.6f;
                break;
            case Difficulty.Medium:
                branchSpawnTimerMax = 1.4f;
                cloudMoveSpeed = MOVE_SPEED*0.3f;
                cloudSpawnTimerMax = 2.6f;
                break;
            case Difficulty.Hard:
                branchSpawnTimerMax = 1.2f;
                cloudMoveSpeed = MOVE_SPEED * 0.4f;
                cloudSpawnTimerMax = 2.2f;
                break;
            case Difficulty.Brutal:
                branchSpawnTimerMax = 1.0f;
                cloudMoveSpeed = MOVE_SPEED * 0.5f;
                cloudSpawnTimerMax = 1.9f;
                break;
            case Difficulty.Nightmare1:
                branchSpawnTimerMax = 0.8f;
                cloudMoveSpeed = MOVE_SPEED * 0.6f;
                cloudSpawnTimerMax = 1.6f;
                //make Oscar faster as branch spawn time gets smaller 
                Oscar.GetInstance().SetOscarMoveSpeed(40f);
                break;
            case Difficulty.Nightmare2:
                branchSpawnTimerMax = 0.6f;
                cloudMoveSpeed = MOVE_SPEED * 0.8f;
                cloudSpawnTimerMax = 1.3f;
                break;
            case Difficulty.Nightmare3:
                Oscar.GetInstance().SetOscarMoveSpeed(100f);
                branchSpawnTimerMax = 0.5f;
                cloudMoveSpeed = MOVE_SPEED*0.9f;
                cloudSpawnTimerMax = 1.0f;
                break;
            case Difficulty.Impossible:
                Oscar.GetInstance().SetOscarMoveSpeed(200f);
                branchSpawnTimerMax = 0.45f;
                cloudMoveSpeed = MOVE_SPEED;
                cloudSpawnTimerMax = 0.8f;
                break;
        }
    }
    private Difficulty GetDifficulty()
    {
        if (branchesSpawned >= 100) return Difficulty.Impossible;
        else if (branchesSpawned >= 60) return Difficulty.Nightmare3;
        else if (branchesSpawned >= 50) return Difficulty.Nightmare2;
        else if (branchesSpawned >= 40) return Difficulty.Nightmare1;
        else if (branchesSpawned >= 30) return Difficulty.Brutal;
        else if (branchesSpawned >= 20) return Difficulty.Hard;
        else if (branchesSpawned >= 10) return Difficulty.Medium;
        return Difficulty.Easy;
    }
    public float GetCameraHalfWidth()
    {
        float cameraHalfWidth = (2f * Camera.main.orthographicSize) * Camera.main.aspect * 0.5f;
        return cameraHalfWidth;
    }

    public float GetCameraHeight()
    {
        float cameraHeight = 2f * Camera.main.orthographicSize;
        return cameraHeight;
    }

    //BRANCH METHODS

    private BranchType GetRandomBranchType() //get left or right branch
    {
        BranchType[] branchTypeArray = { BranchType.Left, BranchType.Right };
        int branchTypeIndex = Random.Range(0, branchTypeArray.Length);
        return branchTypeArray[branchTypeIndex];
    }

    private bool TryGeneratBranchPair()
    {
        //ensure that chances to spawn a branch pair is higher
        BranchType[] branchTypeArray = { BranchType.Left, BranchType.Right, BranchType.Pair, BranchType.Pair, BranchType.Pair };
        int branchTypeIndex = Random.Range(0, branchTypeArray.Length);
        if (branchTypeArray[branchTypeIndex] == BranchType.Pair)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void CreateBranch(float width, float yPosition, BranchType branchType)
    {
        //Set up Branch
        Transform branchTransform = Instantiate(GameAssets.GetInstance().pfBranch);
        SpriteRenderer branchSpriteRenderer = branchTransform.GetComponent<SpriteRenderer>();
        BoxCollider2D branchBoxCollider = branchTransform.GetComponent<BoxCollider2D>();
        float branchXPosition; //x-position to spawn branch

        //set size of branch
        float branchWidthHeightScale = BRANCH_WIDTH / BRANCH_HEIGHT;
        float height = width / branchWidthHeightScale;

        //set up sprite renderer and box collider
        branchSpriteRenderer.size = new Vector2(width, height);
        branchBoxCollider.size = new Vector2(width * BRANCH_BOX_COLLIDER_WIDTH_SCALE,
           BRANCH_BOX_COLLIDER_HEIGHT_SCALE);
        branchBoxCollider.offset = new Vector2(width * BRANCH_BOX_COLLIDER_X_OFFSET_SCALE, 0);

        if (branchType == BranchType.Left)
        {
            //position branch at extreme left of screen
            branchXPosition = -GetCameraHalfWidth() + width * 0.5f;
        }
        else
        {
            //position branch at extreme right of screen
            branchXPosition = GetCameraHalfWidth() - width * 0.5f;
            branchTransform.localRotation = Quaternion.Euler(0, 180, 0); //flip the object horizontally
        }

        branchTransform.position = new Vector3(branchXPosition, yPosition);

        if (branchType == BranchType.Right)
        {
            Branch branch = new Branch(branchTransform, BranchType.Right);
            branchList.Add(branch);
        }
        else
        {
            Branch branch = new Branch(branchTransform, BranchType.Left);
            branchList.Add(branch);
        }
    }

    private void CreateBranchPair(float rightWidth, float leftWidth, float yPosition)
    {
        CreateBranch(rightWidth, yPosition, BranchType.Right);
        CreateBranch(leftWidth, yPosition, BranchType.Left);
    }

    private void HandleBranchSpawning()
    {
        branchSpawnTimer -= Time.deltaTime;
        if (branchSpawnTimer < 0)
        {
            //spawn another branch
            branchSpawnTimer += branchSpawnTimerMax; //reset timer
            float minWidth = Oscar.GetInstance().GetOscarWidth(); //set minimum branch width
            float maxWidth = GetCameraHalfWidth() * 2f -
               Oscar.GetInstance().GetOscarWidth(); //set maximum branch width

            //set up single branch
            float width = Random.Range(minWidth, maxWidth);

            //set up branch pair
            float pairRightWidth = Random.Range(minWidth, maxWidth);
            float pairLeftWidth = maxWidth - pairRightWidth;

            if (TryGeneratBranchPair() == true)
            {
                CreateBranchPair(pairRightWidth, pairLeftWidth, BRANCH_SPAWN_Y_POSITION);
                branchesSpawned++;
                branchesSpawned++;
            }
            else
            {
                CreateBranch(width, BRANCH_SPAWN_Y_POSITION, GetRandomBranchType());
                branchesSpawned++;
            }
            SetDifficulty(GetDifficulty());
        }
    }

    private void HandleBranchMovement() //branches are moving while Oscar always remains in the same position on the y-axis
    {
        for (int i = 0; i < branchList.Count; i++)
        {
            Branch branch = branchList[i];

            //check that branches have not moved below Oscar before moving
            bool branchIsaboveOscar = branch.GetYPosition() > Oscar.GetInstance().GetOscarYPosition();
            branch.Move();

            //check if branch that was previously above Oscar is now below him
            if (branchIsaboveOscar && branch.GetYPosition() < Oscar.GetInstance().GetOscarYPosition())
            {
                branchesPassed++;
                //SoundManager.PlaySound(SoundManager.Sound.Score);
                SoundManager.PlaySound(SoundManager.Sound.Bark);
            }
            if (branch.GetYPosition() < BRANCH_DESTROY_Y_POSITION)
            {
                //Destroy branch
                branch.DestroySelf();
                branchList.Remove(branch);
                i--;
            }
        }
    }

    public int GetBranchesPassed()
    {
        return branchesPassed;
    }


    //TREE BODY
    private void CreateTreeBodyRightLeft()
    {
        //set up tree body 
        Transform treeBodyRight = Instantiate(GameAssets.GetInstance().pfTreeBodyRight,
            new Vector3((float)(GetCameraHalfWidth() - (TREE_BODY_WIDTH * 0.5)), 0, 0), Quaternion.identity);

        Transform treeBodyLeft = Instantiate(GameAssets.GetInstance().pfTreeBodyLeft,
            new Vector3((float)(-GetCameraHalfWidth() + (TREE_BODY_WIDTH * 0.5)), 0, 0), Quaternion.identity);

    }

    //CLOUDS
    public void HandleCloudsSpawningAndMovement()
    {
        float cloudXSpawnRange = Random.Range(-30f, 30f);
        //Handle clouds spawning
        cloudSpawnTimer -= Time.deltaTime;
        if (cloudSpawnTimer < 0) //spawn another cloud
        {
            cloudSpawnTimer = cloudSpawnTimerMax;
            //create array and random selector to generate 
            Transform[] differentClouds = {GameAssets.GetInstance().pfClouds_1, GameAssets.GetInstance().pfClouds_2,
                GameAssets.GetInstance().pfClouds_3};
            int randomCloudIndex = Random.Range(0, differentClouds.Length);
            //spawn clouds at random
            Transform cloud = Instantiate(differentClouds[randomCloudIndex],
            new Vector3(cloudXSpawnRange, CLOUD_SPAWN_Y_POSITION, 0), Quaternion.identity);
            cloudList.Add(cloud);
        }

        //HandleClouds Clouds Moving
        for (int i = 0; i < cloudList.Count; i++)
        {
            Transform cloud = cloudList[i];
            //move clouds slower than branches for variation
            cloud.position += new Vector3(0, -1, 0) * cloudMoveSpeed * Time.deltaTime;

            // destroy clouds past destroy position
            if (cloud.position.y < CLOUD_DESTROY_Y_POSITION)
            {
                //Destroy cloud
                Destroy(cloud.gameObject);
                cloudList.Remove(cloudList[i]);
                i--;
            }
        }
    }

    private class Branch
    {
        private Transform branchTransform;
        private BranchType branchType;
        public Branch(Transform branchTransform, BranchType branchType)
        {
            this.branchTransform = branchTransform;
            this.branchType = branchType;
        }
        public void Move()
        {
            branchTransform.position += new Vector3(0, -1, 0) * MOVE_SPEED * Time.deltaTime;
        }

        public float GetYPosition()
        {
            return branchTransform.position.y;
        }
        public void DestroySelf()
        {
            Destroy(branchTransform.gameObject);
        }
    }
}
