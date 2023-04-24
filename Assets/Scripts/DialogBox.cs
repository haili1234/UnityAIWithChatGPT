using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour
{
    public GameObject dialogBox;
    public Text dialogText;

    public void ShowDialog(string text)
    {
        dialogBox.SetActive(true);
        dialogText.text = text;
    }

    public void HideDialog()
    {
        dialogBox.SetActive(false);
    }
}
