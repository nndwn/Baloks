using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{

    public static int gridWidth = 10;
    public static int gridHeight = 25;

    public static Transform[,] grid = new Transform[gridWidth, gridHeight];

    public static int AddSpeedFall;
    //public static bool MoveRight, MoveLeft, MoveDown, MoveRotate = false;

    public int scoreOneLine = 5;
    public int scoreTwoLine = 10;
    public int scoreThreeLine = 20;
    public int scoreFourLine = 50;

    public static int currentLevel;
    public static int customLevel;
    public static int numLinesCleared = 0;

    public static float fallSpeed = 1.0f;
    public float maxSpeedFall = 9;

    public Slider SliderSpeed;
    public TextMeshProUGUI AddSpeedSlide;

    public Text GameOver_score;

    public TextMeshProUGUI scoreUi;
    public TextMeshProUGUI HighScoreUi;
    public TextMeshProUGUI LevelUi;
    public TextMeshProUGUI LinesUi;

    public static int numberOfRowsThisTurn = 0;
    public static int currentScore = 0;

    private GameObject previewTetromino;
    public static GameObject nextTetromino;
    private GameObject savedTetromno;
    private GameObject ghosttetromino;

    private bool gameStarted = false;

    public static Vector2 positionMino = new Vector2(5.0f, 25.0f);
    private Vector2 previewTetrominoPosition = new Vector2(-1.7f, 11.64f);
    private Vector2 savedTetrominoPosition = new Vector2(-1.7f, 8.4f);

    public static int startingHighScore;

    private float animScore;
    private float animHighScore;
    private float updateAnimValueHighScore;
    //public float speedcount = 2.0f;

    public int maxSwaps = 2;
    private int currentSwaps = 1;

    public static bool gameover;


    public static int tempScore;

    [Header("Game Over")]
    public GameObject gameOverPanel;
    public AudioSource Audio;

    float lines;

    void Start()
    {
        StartingGame();
        SpawnNextTetromino();


    }
    void Update()
    {
        if (!(currentScore > 99999))
            UpdateScore();
        UpdateUI();
        UpdateLevel(numLinesCleared);
        UpdateSpeed(PlayerPrefs.GetInt(PlayerData.speedsettings.ToString()));
        UpdateNewHighScore();
        CheckUserInput();

    }

    void CheckUserInput()
    {

        if (Input.GetKeyUp(KeyCode.Space) || ButtonFocus.swap)
        {
            GameObject tempNexTetromino = GameObject.FindGameObjectWithTag("currentActiveTetromino");
            SaveTetromino(tempNexTetromino.transform);
            ButtonFocus.swap = false;
        }
    }


    void StartingGame()
    {
        currentScore = 0;
        scoreUi.text = "0";
        //SliderSpeed.maxValue = maxSpeedFall;
        //SliderSpeed.value = PlayerPrefs.GetInt("speedsettings");
        startingHighScore = PlayerPrefs.GetInt("highscore");
        HighScoreUi.text = startingHighScore.ToString();
        numLinesCleared = 0;
        Pause.isPaused = false;
        customLevel = 1;
        PlayerPrefs.SetInt("tempscore", startingHighScore);
        tempScore = PlayerPrefs.GetInt("tempscore");

    }
    public void UpdateHighScore()
    {
        if (currentScore > startingHighScore)
        {
            PlayerPrefs.SetInt("highscore", currentScore);
        }
    }

    private int AnimCountScore(int value, float fast)
    {
        if (animScore <= value)
        {
            animScore += (fast * Time.deltaTime);
            return (int)Mathf.Floor(animScore);
        }
        return value;
    }

    public void UpdateUI()
    {
        scoreUi.text = AnimCountScore(currentScore, scoreFourLine).ToString();
        LevelUi.text = currentLevel.ToString();
        //lines = numLinesCleared;

        if (lines <= numLinesCleared)
        {
            lines += (4 * Time.deltaTime);
        }
        LinesUi.text = ((int)Mathf.Floor(lines)).ToString();
       // SliderSpeed.minValue = currentLevel;
    }

    void UpdateNewHighScore()
    {
        if (startingHighScore < currentScore && AnimCountScore(currentScore, scoreFourLine) == currentScore)
        {
            if (startingHighScore == 0)
            {

                if (animHighScore < currentScore)
                {
                    animHighScore += (1 * Time.deltaTime) * 30;
                }
                HighScoreUi.text = Mathf.Floor(animHighScore).ToString();
            }
            else
            {
                //animHighScore + startingHighScore;

                if (animHighScore < (currentScore - startingHighScore))
                {

                    animHighScore += (1 * Time.deltaTime) * 30;
                    updateAnimValueHighScore = startingHighScore + Mathf.Floor(animHighScore);
                }

                HighScoreUi.text = updateAnimValueHighScore.ToString();
            }


        }
    }
    public void UpdateLevel(int value)
    {
        currentLevel = customLevel;
        if (value >= (customLevel * 10))
        {
            customLevel = (value / 10) + 1;
        }
        return;
    }
    public void SaveSetting(int value)
    {
        PlayerPrefs.SetInt("speedsettings", value);
    }

    void UpdateSpeed(int value)
    {

        if (value > currentLevel)
        {

            fallSpeed = 1.0f - (value * 0.1f);
        }
        else
        {
            fallSpeed = 1.0f - (currentLevel * 0.1f);
            if (maxSpeedFall < currentLevel)
            {
                fallSpeed = 1.0f - (maxSpeedFall * 0.1f);
            }
        }

    }
    public void ChangedValue(float value)
    {
        AddSpeedFall = (int)value;

        AddSpeedSlide.text = value.ToString();

    }

    public void UpdateScore()
    {
        if (numberOfRowsThisTurn > 0)
        {
            if (numberOfRowsThisTurn == 1)
            {
                ClearedOneLine();
            }
            else if (numberOfRowsThisTurn == 2)
            {
                ClearedTwoLines();
            }
            else if (numberOfRowsThisTurn == 3)
            {
                ClearedThreeLines();
            }
            else if (numberOfRowsThisTurn == 4)
            {
                ClearedFourLines();
            }
            numberOfRowsThisTurn = 0;
        }
    }

    public void ClearedOneLine()
    {
        currentScore += scoreOneLine + (currentLevel * 5);
        numLinesCleared++;

    }
    public void ClearedTwoLines()
    {
        currentScore += scoreTwoLine + (currentLevel * 10);
        numLinesCleared += 2;
    }
    public void ClearedThreeLines()
    {
        currentScore += scoreThreeLine + (currentLevel * 20);
        numLinesCleared += 3;

    }
    public void ClearedFourLines()
    {
        currentScore += scoreFourLine + (currentLevel * 30);
        numLinesCleared += 4;

    }


    bool CheckIsValidPosition(GameObject tetromino)
    {
        foreach (Transform mino in tetromino.transform)
        {
            Vector2 pos = Round(mino.position);
            if (!CheckIsInsideGrid(pos))
                return false;
            if (GetTransformAtGridPosition(pos) != null && GetTransformAtGridPosition(pos).parent != tetromino.transform)
                return false;
        }
        return true;
    }
    public bool CheckIsAboveGrid(Tetrimino tetrimino)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            foreach (Transform mino in tetrimino.transform)
            {
                Vector2 pos = Round(mino.position);
                if (pos.y > gridHeight - 1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsFullRowAt(int y)
    {
        // - The parameter y, is the row we will interate over in the grid array to check each x position for a transform.
        for (int x = 0; x < gridWidth; ++x)
        {
            // - If we find a position that return NULL instead of a tranform, then we know that the row is not full.
            if (grid[x, y] == null)
            {
                // - so we return false
                return false;
            }
        }
        //- Since we found a full row, we increment the full row variable.
        numberOfRowsThisTurn++;
        return true;
    }
    public bool IsFullRowAtanim(int y)
    {
        // - The parameter y, is the row we will interate over in the grid array to check each x position for a transform.
        for (int x = 0; x < gridWidth; ++x)
        {
            // - If we find a position that return NULL instead of a tranform, then we know that the row is not full.
            if (grid[x, y] == null)
            {
                // - so we return false
                return false;
            }
        }
        // rowsoundanim++;
        return true;
    }

    public void FullAnimatehide()
    {
        for (int y = 0; y < gridHeight; ++y)
        {
            if (IsFullRowAtanim(y))
            {
                AnimateColorhide(y);

            }
        }
    }
    public bool FullAnimateshowbool()
    {
        for (int y = 0; y < gridHeight; ++y)
        {
            if (IsFullRowAtanim(y))
            {
                return true;
            }

        }
        return false;
    }
    public void FullAnimateshow()
    {
        for (int y = 0; y < gridHeight; ++y)
        {
            if (IsFullRowAtanim(y))
            {

                AnimateColorshow(y);
            }
        }
    }
    public void AnimateColorshow(int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            Game.grid[x, y].gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        }
    }

    public void AnimateColorhide(int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            Game.grid[x, y].gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0);
        }
    }
    public void DeleteMinoAt(int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }
    public void MoveRowDown(int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            if (grid[x, y] != null)
            {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }

    }
    public void MoveAllRowsDown(int y)
    {
        for (int i = y; i < gridHeight; ++i)
        {
            MoveRowDown(i);
            --y;
        }
    }

    public void DeleteRow()
    {
        for (int y = 0; y < gridHeight; ++y)
        {
            if (IsFullRowAt(y))
            {
                //AnimationRow(y);
                //StartCoroutine(Blink(2f, y));
                DeleteMinoAt(y);
                MoveAllRowsDown(y + 1);
                --y;
            }
        }
    }
    public void UpdateGrid(Tetrimino tetrimino)
    {
        for (int y = 0; y < gridHeight; ++y)
        {
            for (int x = 0; x < gridWidth; ++x)
            {
                if (grid[x, y] != null)
                {
                    if (grid[x, y].parent == tetrimino.transform)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }
        foreach (Transform mino in tetrimino.transform)
        {
            Vector2 pos = Round(mino.position);
            if (pos.y < gridHeight)
            {
                grid[(int)pos.x, (int)pos.y] = mino;
            }
        }
    }

    public Transform GetTransformAtGridPosition(Vector2 pos)
    {
        if (pos.y > gridHeight - 1)
        {
            return null;
        }
        else
        {
            return grid[(int)pos.x, (int)pos.y];
        }
    }


    public void SpawnNextTetromino()
    {

        if (!gameStarted)
        {
            gameStarted = true;
            nextTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), positionMino, Quaternion.identity);
            previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
            previewTetromino.GetComponent<Tetrimino>().enabled = false;
            previewTetromino.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
            nextTetromino.tag = "currentActiveTetromino";

            SpawnGhostTetromino();

        }
        else
        {
            previewTetromino.transform.localScale = new Vector3(1f, 1f, 1f);
            previewTetromino.transform.localPosition = positionMino;
            nextTetromino = previewTetromino;
            nextTetromino.GetComponent<Tetrimino>().enabled = true;
            nextTetromino.tag = "currentActiveTetromino";
            previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);

            previewTetromino.GetComponent<Tetrimino>().enabled = false;
            previewTetromino.transform.localScale = new Vector3(0.3f, 0.3f, 1f);


            SpawnGhostTetromino();
        }
        currentSwaps = 0;

    }



    public void SpawnGhostTetromino()
    {
        if (GameObject.FindGameObjectWithTag("currentGhostTetromino") != null)
            Destroy(GameObject.FindGameObjectWithTag("currentGhostTetromino"));
        ghosttetromino = (GameObject)Instantiate(nextTetromino, nextTetromino.transform.position, Quaternion.identity);
        Destroy(ghosttetromino.GetComponent<Tetrimino>());
        ghosttetromino.AddComponent<GhostTetromino>();

    }

    public void SaveTetromino(Transform t)
    {
        currentSwaps++;
        if (currentSwaps > maxSwaps)
            return;
        if (savedTetromno != null)
        {
            GameObject tempSavedTetromino = GameObject.FindGameObjectWithTag("currentSaveTetromino");

            tempSavedTetromino.transform.localPosition = new Vector2(gridWidth / 2, gridHeight);
            tempSavedTetromino.transform.localScale = new Vector3(1f, 1f, 1f);
            if (!CheckIsValidPosition(tempSavedTetromino))
            {
                tempSavedTetromino.transform.localPosition = savedTetrominoPosition;
                tempSavedTetromino.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
                return;
            }
            savedTetromno = (GameObject)Instantiate(t.gameObject);
            savedTetromno.GetComponent<Tetrimino>().enabled = false;
            savedTetromno.transform.localPosition = savedTetrominoPosition;
            savedTetromno.transform.localScale = new Vector3(1f, 1f, 1f);
            savedTetromno.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
            savedTetromno.tag = "currentSaveTetromino";

            nextTetromino = (GameObject)Instantiate(tempSavedTetromino);
            nextTetromino.GetComponent<Tetrimino>().enabled = true;
            nextTetromino.transform.localPosition = new Vector2(gridWidth / 2, gridHeight);
            nextTetromino.transform.localScale = new Vector3(1f, 1f, 1f);
            nextTetromino.tag = "currentActiveTetromino";
            DestroyImmediate(t.gameObject);
            DestroyImmediate(tempSavedTetromino);

            SpawnGhostTetromino();

        }
        else
        {
            savedTetromno = (GameObject)Instantiate(GameObject.FindGameObjectWithTag("currentActiveTetromino"));
            savedTetromno.GetComponent<Tetrimino>().enabled = false;
            savedTetromno.transform.localPosition = savedTetrominoPosition;
            savedTetromno.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
            savedTetromno.tag = "currentSaveTetromino";

            DestroyImmediate(GameObject.FindGameObjectWithTag("currentActiveTetromino"));

            SpawnNextTetromino();
        }
    }
    public bool CheckIsInsideGrid(Vector2 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.y >= 0);
    }
    public Vector2 Round(Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    public bool CheatMino = false;
    string GetRandomTetromino()
    {
        int randomTetromino = Random.Range(1, 8);
        string randomTetrominoName = "Prefabs/mino_i";
        if (!CheatMino)
        {
            switch (randomTetromino)
            {
                case 1:
                    randomTetrominoName = "Prefabs/mino_yellow";
                    break;
                case 2:
                    randomTetrominoName = "Prefabs/mino_blue";
                    break;
                case 3:
                    randomTetrominoName = "Prefabs/mino_i";
                    break;
                case 4:
                    randomTetrominoName = "Prefabs/mino_red";
                    break;
                case 5:
                    randomTetrominoName = "Prefabs/mino_orange";
                    break;
                case 6:
                    randomTetrominoName = "Prefabs/mino_cyan";
                    break;
                case 7:
                    randomTetrominoName = "Prefabs/mino_magenta";
                    break;
                    //case 8:
                    //    randomTetrominoName = "Prefabs/Tetrimono_V";
                    //    break;
            }
        }
        return randomTetrominoName;
    }

    //public void MoveRightButton () {
    //   MoveRight = true;
    //}
    //public void MoveRightButtonUp () {
    //   MoveRight = false;
    //    Tetrimino.movedImmediateHorizontal = false;
    //}
    //public void MoveLeftButton(){
    //    MoveLeft = true;
    //}
    //public void MoveLeftButtonUp(){
    //    MoveLeft = false;
    //    Tetrimino.movedImmediateHorizontal = false;
    //}
    //public void MoveDownButton() {
    //    MoveDown = true;

    //}
    //public void MoveDownButtonUp() {
    //    MoveDown = false;

    //    Tetrimino.movedImmediateVertical = false;
    //}
    //public void MoveRotateButton(){
    //    MoveRotate = true;
    //}
    public void RestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }



    public void GameOver()
    {
        UpdateHighScore();
        gameOverPanel.SetActive(true);
        Pause.isPaused = true;
        Audio.loop = false;

        //CanvasPanelOpen(GameOverPanel);

        Debug.Log("Game Over");

    }

}
