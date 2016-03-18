using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BackstoryManager : MonoBehaviour {

    public GameObject text;
    public AudioSource music;

	// Use this for initialization
	void Start () {
        music = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		if (text.transform.position.y >= 1500 || (AirconsoleLogic.skip[0] && AirconsoleLogic.skip[1]))
        {
            SceneManager.LoadScene("versusAI");
        }
        else
        {
            Vector3 pos = text.transform.position;
			pos.y += (float) 45 * Time.deltaTime;
            text.transform.position = pos;
        }

        // Fade audio as we get close to transition
        if (text.transform.position.y >= 1450)
        {
            fadeOut();
        }
	}

    // for impatient people
    public void skipCutscene ()
    {
        SceneManager.LoadScene("versusAI");
    }

    public void fadeOut()
    {
        if (music.volume > 0.0)
        {
            music.volume -= Time.deltaTime;
        }
    }
}
