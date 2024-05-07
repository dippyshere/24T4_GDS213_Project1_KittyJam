using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AutoSelectText : MonoBehaviour
{
    [SerializeField, Tooltip("The text entry field to have selected by default on enable of this game object")] private TMP_InputField inputField;
    [SerializeField, Tooltip("The delay before selecting the text entry field")] private float delay = 0.1f;

    private void OnEnable()
    {
        StartCoroutine(SelectText());
    }

    private IEnumerator SelectText()
    {
        yield return new WaitForSeconds(delay);
        inputField.Select();
        inputField.ActivateInputField();
    }
}
