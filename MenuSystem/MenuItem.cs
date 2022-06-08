using System;

namespace MenuSystem
{
    public class MenuItem
    {
        public MenuItem(string shortCut, string title, Func<string> runMethod, bool selected = false)
        {
            if (string.IsNullOrEmpty(shortCut))
            {
                throw new ArgumentException("shortCut cannot be empty!");
            }

            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentException("title cannot be empty!");
            }

            ShortCut = shortCut.Trim();
            Title = title.Trim();
            RunMethod = runMethod;
            Selected = selected;
        }
        
        public MenuItem(string shortCut, string title, Action runMethod, bool selected = false)
        {
            if (string.IsNullOrEmpty(shortCut))
            {
                throw new ArgumentException("shortCut cannot be empty!");
            }

            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentException("title cannot be empty!");
            }

            ShortCut = shortCut.Trim();
            Title = title.Trim();
            RunMethod2 = runMethod;
            Selected = selected;
        }
        
        public string ShortCut { get; private set; }
        public string Title { get; private set; }

        public bool Selected { get; set; }

        public Func<string>? RunMethod { get; private set; }
        
        public Action? RunMethod2 { get; private set; }
        

        public override string ToString()
        {
            return ShortCut + ") " + Title;
        }
    }
}