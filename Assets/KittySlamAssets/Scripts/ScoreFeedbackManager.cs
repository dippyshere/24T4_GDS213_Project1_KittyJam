using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class ScoreFeedbackManager : MonoBehaviour
{

    public TextMeshProUGUI scoreText;
    public int scoreValue = 100;
    public Vector3 destination;
    public float speed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        UpdateScore();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, destination, speed * Time.deltaTime);
    }

    public void UpdateScore()
    {
        // localised number formatting
        scoreText.text = scoreValue.ToString("N0", CultureInfo.InvariantCulture);
    }
}
