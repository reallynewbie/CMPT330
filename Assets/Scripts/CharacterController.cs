using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    private float maxHealth = 1000f;
    [SerializeField]
    private float currentHealth = 1000f;

    private UIManager _uiManager;

    // Start is called before the first frame update
    void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //updateCurrentHealth(-1);
    }

    private void FixedUpdate()
    {
        
    }

    public void updateCurrentHealth(int healthMod)
    {
        currentHealth += healthMod;
        _uiManager.updateHealthPercentage(currentHealth / maxHealth);
    }
}
