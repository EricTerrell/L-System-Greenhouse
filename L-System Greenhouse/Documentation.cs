using System.Collections.Generic;

namespace L_System_Greenhouse;

public class Documentation
{
    public List<DocumentationItem> Items { get; set; }

    public Documentation()
    {
        DocumentationItem[] items =
        [
            new ("F", "Move forward and draw a line"),
            new ("f", "Move forward invisibly"),
            new ("+", "Turn left"),
            new ("-", "Turn right"),
            new ("|", "Turn 180\u00b0"),
            new ("[", "Begin branch (save turtle state)"),
            new ("]", "End branch (restore turtle state)"),
            new ("#", "Increase line width"),
            new ("!", "Decrease line width"),
            new (">", "Multiply line length by scaling factor"),
            new ("<", "Divide line length by scaling factor"),
            new ("(", "Decrease turn angle by angle increment (towards the left)"),
            new (")", "Increase turn angle angle increment (towards the right)")
        ];

        Items = new List<DocumentationItem>(items);
    }
}