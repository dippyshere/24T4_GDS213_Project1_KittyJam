using TMPro;
using UnityEngine;

/// <summary>
/// A validator class that validates the input field for a username to be compatible with Unity Game Services
/// </summary>
[CreateAssetMenu(menuName = "Kitty Jam/TMPro Validators/Username")]
public class UsernameValidator : TMP_InputValidator
{
    /// <summary>
    /// Override Validate method to implement username validation
    /// </summary>
    /// <param name="text">This is a reference pointer to the actual text in the input field; changes made to this text argument will also result in changes made to text shown in the input field</param>
    /// <param name="pos">This is a reference pointer to the input field's text insertion index position (your blinking caret cursor); changing this value will also change the index of the input field's insertion position</param>
    /// <param name="ch">This is the character being typed into the input field</param>
    /// <returns>Return the character you'd allow into the input field; if you want to block the character, return '\0'</returns>
    public override char Validate(ref string text, ref int pos, char ch)
    {
        // The username is case insensitive; it requires a minimum of 3 and a maximum of 20 characters and only supports letters, numbers and symbols like ., -, @ or _.
        if ((char.IsLetterOrDigit(ch) || ch == '.' || ch == '-' || ch == '@' || ch == '_') && text.Length < 20)
        {
            // If the character is valid, return it
            text = text.Insert(pos, ch.ToString());
            pos++;
            return ch;
        }
        else
        {
            // If the character is invalid, return null
            return '\0';
        }
    }
}

/// <summary>
/// A validator class that validates the input field for a password to be compatible with Unity Game Services
/// </summary>
[CreateAssetMenu(menuName = "Kitty Jam/TMPro Validators/Password")]
public class PasswordValidator : TMP_InputValidator
{
    /// <summary>
    /// Override Validate method to implement password validation
    /// </summary>
    /// <param name="text">This is a reference pointer to the actual text in the input field; changes made to this text argument will also result in changes made to text shown in the input field</param>
    /// <param name="pos">This is a reference pointer to the input field's text insertion index position (your blinking caret cursor); changing this value will also change the index of the input field's insertion position</param>
    /// <param name="ch">This is the character being typed into the input field</param>
    /// <returns>Return the character you'd allow into the input field; if you want to block the character, return '\0'</returns>
    public override char Validate(ref string text, ref int pos, char ch)
    {
        // The password is case sensitive; it requires a minimum of 8 and a maximum of 30 characters and at least 1 lowercase letter, 1 uppercase letter, 1 number, and 1 symbol.
        if (char.IsLetterOrDigit(ch) || char.IsSymbol(ch) || char.IsPunctuation(ch))
        {
            // If the character is valid, return it
            text = text.Insert(pos, ch.ToString());
            pos++;
            return ch;
        }
        else
        {
            // If the character is invalid, return null
            return '\0';
        }
    }
}