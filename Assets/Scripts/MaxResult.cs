using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxResult : MonoBehaviour
{
   [SerializeField] private TextMesh pastResultScoreText;
   [SerializeField] private int pastResultScore;
   
   [SerializeField] private int pastResultCristall;
   [SerializeField] private TextMesh pastResultCristallText;
   
   [SerializeField] private TextMesh scoreText;
   [SerializeField] private int score;
   
   [SerializeField] private int cristall;
   [SerializeField] private TextMesh cristallText;
   
   [SerializeField] private TextMesh maxScoreText;
   [SerializeField] private int maxScore;
   
   [SerializeField] private TextMesh maxCristallText;
   [SerializeField] private int maxCristall;

   [SerializeField] private GameObject gameOverText;

   [SerializeField] private GameObject maxCristallObjectText;
   [SerializeField] private GameObject maxScoreObjectText;
   

   private void Start()
   {
      maxCristallObjectText.SetActive(false);
      maxScoreObjectText.SetActive(false);
   }
   
   private void Update()
   {
      score = int.Parse(scoreText.text);
      if (gameOverText.transform.position.y == 0)
      {
         maxCristallObjectText.SetActive(true);
         maxScoreObjectText.SetActive(true);
         pastResultScore = score;
      }

      if (pastResultScore >= score)
      {
         maxScore = pastResultScore;
      }
      
      cristall = int.Parse(cristallText.text);
      if (gameOverText.transform.position.y == 0)
      {
         print("GameOver");
         pastResultCristall = cristall;
      }

      if (pastResultCristall >= cristall)
      {
         maxCristall = pastResultCristall;
      }

      maxScoreText.text = maxScore.ToString();
      maxCristallText.text = maxCristall.ToString();
   }
}
