using TMPro;
using UnityEngine;

public class InputFieldGrabber : MonoBehaviour
{
    [Header("The value we got from it")]
    [SerializeField] private string inputText;

    [Header("showing reaction to player")]
    [SerializeField] private GameObject ReactionGroup;
    [SerializeField] private TMP_Text reactionTextBox;

    public void GrabFromInputField(string input)
    {
        inputText = input;
        Debug.Log(inputText);
       

        //DisplayReactionToInput();

    }
   /*
    private void DisplayReactionToInput()
    {
        reactionTextBox.text = InputText;
        ReactionGroup.SetActive(true);
    }
   */

}