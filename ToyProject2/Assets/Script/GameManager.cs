using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public GameObject[] Stages;
    public PlayerMove player;


    public Image[] UIhealth;
    public TextMeshProUGUI UIPoint;
    public TextMeshProUGUI UIStage;
    public GameObject RestartButton;


    void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();

    }
    public void NextStage()
    {
        if (stageIndex < Stages.Length-1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerRepositon();
            UIStage.text = "STAGE " + (stageIndex+1);
        }
        else
        {
            Time.timeScale = 0;

            Debug.Log("게임 클리어");


            TextMeshProUGUI btntext = RestartButton.GetComponentInChildren<TextMeshProUGUI>();
            btntext.text = "Game Clear!";
            RestartButton.SetActive(true);

        }

        stagePoint += totalPoint;
        stagePoint = 0;
    }

    public void HealthDown()
    {
        if (health > 1)
        {
            health--;
            UIhealth[health].color = new Color(1, 1, 1, 0.1f);
        }
        else
        {
            UIhealth[0].color = new Color(1, 1, 1, 0.1f);
            player.OnDie();
            Debug.Log("죽었습니다.");
            RestartButton.SetActive(true);

        
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (health > 1)
            {
                HealthDown();
                PlayerRepositon();
            }
            else
                player.OnDie();

        }
    }
    void PlayerRepositon()
    {
        player.transform.position = new Vector3(0, 0, -1);
        player.VelocityZero();

    }
    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
