using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public TMP_Text countercoin;
    public int count = 0;

    public AudioClip clip;

    private AudioSource source;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        source = GetComponent<AudioSource>();
        ShowDialogue();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(other.gameObject);
        count++;
        ShowDialogue();
        source.PlayOneShot(clip);

    }
    void ShowDialogue()
    {
        if (countercoin != null)
        {
            Debug.Log("Showing dialogue panel"); // Debug log
            if (countercoin != null)
            {
                countercoin.text = "" + count;

            }
            else
            {

                Debug.LogWarning("Dialogue Text component is missing!"); // Warning if text is missing
            }
        }
    }
}
