using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManger : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;
    public GameObject[] Stages;

    public Image[] UIhealth;
    public TextMeshProUGUI UIPoint;
    public TextMeshProUGUI UIStage;
    public GameObject RestartBtn;

    void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }
    public void NextStage()
    {
        // Change Stage
        if (stageIndex < Stages.Length - 1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = "STAGE " + (stageIndex + 1);
        }
        else // Game Clear
        {
            //Player Control Lock
            Time.timeScale = 0;
            // Result UI
            Debug.Log("게임 클리어!");
            // Restart Button UI
            RestartBtn.SetActive(true);
            TextMeshProUGUI btnText = RestartBtn.GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = "Game Clear!";
            RestartBtn.SetActive(true);
        }
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void HealthDown()
    {
        if (health > 1)
        {
            health--;
            UIhealth[health].color = new Color(1, 0, 0, 0.4f);
        }
        else
        {
            // All Health UI Off
            UIhealth[0].color = new Color(1, 0, 0, 0.4f);

            // Player Die Effect
            player.OnDie();
            // Result UI
            Debug.Log("죽");
            // Retry Button UI
            RestartBtn.SetActive(true);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // Player Reposition
            if (health > 1)
            {
                PlayerReposition();
            }
            // Health Down
            HealthDown();
        }
    }

    void PlayerReposition()
    {
        player.VelocityZero();
        player.transform.position = new Vector3(-1, 1, 0);
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
