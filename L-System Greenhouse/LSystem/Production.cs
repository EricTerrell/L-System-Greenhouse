namespace L_System_Greenhouse.LSystem;

public class Production(char letter, string replacementLetters)
{
    public char Letter
    {
        get;
    } = letter;

    public string ReplacementLetters
    {
        get;
    } = replacementLetters;
}