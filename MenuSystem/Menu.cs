using System;
using System.Collections.Generic;
using System.Linq;
using ColorString;

namespace MenuSystem
{
    public class Menu
    {
        private readonly EMenuLevel _menuLevel;

        private readonly List<MenuItem> _menuItems = new List<MenuItem>();
        private readonly MenuItem _menuItemExit = new MenuItem("E", "Exit", null!);
        private readonly MenuItem _menuItemReturn = new MenuItem("R", "Return", null!);
        private readonly MenuItem _menuItemMain = new MenuItem("M", "Main", null!);

        private readonly HashSet<string> _menuShortCuts = new HashSet<string>();
        private readonly HashSet<string> _menuSpecialShortCuts = new HashSet<string>();

        private readonly string _title;
        private readonly Func<string>? _header;
        public string? Information; 

        public Menu(Func<string> menuHeader, string title, EMenuLevel menuLevel)
        {
            _title = title;
            _menuLevel = menuLevel;
            _header = menuHeader;
            SpecialMenuItemsInitializer(menuLevel);
        }

        public Menu(string title, EMenuLevel menuLevel)
        {
            _title = title;
            _menuLevel = menuLevel;
            SpecialMenuItemsInitializer(menuLevel);
            
        }

        private void SpecialMenuItemsInitializer(EMenuLevel menuLevel)
        {
            switch (menuLevel)
            {
                case EMenuLevel.Root:
                    _menuSpecialShortCuts.Add(_menuItemExit.ShortCut.ToUpper());
                    _menuItems.Add(_menuItemExit);
                    break;
                case EMenuLevel.First:
                    _menuSpecialShortCuts.Add(_menuItemReturn.ShortCut.ToUpper());
                    _menuSpecialShortCuts.Add(_menuItemExit.ShortCut.ToUpper());
                    _menuItems.Add(_menuItemReturn);
                    _menuItems.Add(_menuItemExit);
                    break;
                case EMenuLevel.SecondOrMore:
                    _menuSpecialShortCuts.Add(_menuItemReturn.ShortCut.ToUpper());
                    _menuSpecialShortCuts.Add(_menuItemMain.ShortCut.ToUpper());
                    _menuSpecialShortCuts.Add(_menuItemExit.ShortCut.ToUpper());
                    _menuItems.Add(_menuItemReturn);
                    _menuItems.Add(_menuItemMain);
                    _menuItems.Add(_menuItemExit);
                    break;
                case EMenuLevel.CustomMain:
                    _menuSpecialShortCuts.Add(_menuItemMain.ShortCut.ToUpper());
                    _menuSpecialShortCuts.Add(_menuItemExit.ShortCut.ToUpper());
                    _menuItems.Add(_menuItemMain);
                    _menuItems.Add(_menuItemExit);
                    break;
                case EMenuLevel.CustomReturnOnly:
                    _menuSpecialShortCuts.Add(_menuItemReturn.ShortCut.ToUpper());
                    _menuItems.Add(_menuItemReturn);
                    break;
            }
        }

        public void AddMenuItem(MenuItem item, int position = -1)
        {
            if (_menuSpecialShortCuts.Contains(item.ShortCut.ToUpper()))
            {
                throw new ApplicationException($"Conflicting menu shortcut {item.ShortCut.ToUpper()}");
            }

            if (_menuShortCuts.Contains(item.ShortCut.ToUpper()))
            {
                throw new ApplicationException($"Conflicting menu shortcut {item.ShortCut.ToUpper()}");
            }
            
            if (position == -1)
            {
                _menuItems.Insert(0, item);
            }
            else
            {
                _menuItems.Insert(position, item);
            }

            _menuShortCuts.Add(item.ShortCut);
        }

        public void DeleteMenuItem(int position = 0)
        {
            _menuItems.RemoveAt(position);
        }

        public void AddMenuItems(List<MenuItem> items)
        {
            items.Reverse();
            foreach (var menuItem in items)
            {
                AddMenuItem(menuItem);
            }
            
            InitializeMenu();
        }

        public void ReverseItems()
        {
            
        }

        public void InitializeMenu()
        {
            if (_menuItems.All(item => item.Selected == false))
            {
                _menuItems[0].Selected = true;
            }
        }

        public string Run()
        {
            var runDone = true;
            var input = "";
            do
            {
                Console.Clear();
                OutputMenu();
                Console.WriteLine(input);
                input = "";
                var keyInfo = Console.ReadKey();
                switch (keyInfo.Key)
                {
                    case ConsoleKey.DownArrow:
                        MoveDown();
                        break;
                    case ConsoleKey.UpArrow:
                        MoveUp();
                        break;
                    case ConsoleKey.Enter:
                        var item = _menuItems.FirstOrDefault(t => t.Selected);
                        input = item?.RunMethod == null ? item?.ShortCut : item.RunMethod();
                        
                        if (item?.RunMethod2 != null)
                        {
                            item.RunMethod2();
                            input = "";
                        }
                        item!.Selected = true;
                        break;
                    case ConsoleKey.Escape:
                        input = EReturn.R.ToString();
                        break;
                    default:
                        var key = keyInfo.KeyChar.ToString();
                        if (_menuSpecialShortCuts.Contains(key.ToUpper()))
                        {
                            input = key.ToUpper();
                        }

                        else if (_menuShortCuts.Contains(key))
                        {
                            item = _menuItems.Find(item => item.ShortCut == key);
                            input = item?.RunMethod == null ? item?.ShortCut : item.RunMethod();
                        
                            if (item?.RunMethod2 != null)
                            {
                                item.RunMethod2();
                                input = "";
                            }
                        }
                        else
                        {
                            input = (" \n!!!Use Up and Down arrow for navigation or select correct shortcut!!!");
                        }
                        
                        break;
                }


                runDone = _menuSpecialShortCuts.Contains(input!);
                

            } while (!runDone);

            if (input == _menuItemMain.ShortCut.ToUpper() && _menuLevel != EMenuLevel.Root)
                return _menuItemReturn.ShortCut;
            if (input == _menuItemReturn.ShortCut.ToUpper()) return "";
            return input!;
        }

        private void OutputMenu()
        {
            Console.WriteLine("====> " + _title + " <====");
            Console.WriteLine("-------------------");
            if (_header != null)
            {
                var info = _header();
                if (info != null)
                {
                    ColoredString.WriteLineString($"{info}", ConsoleColor.Blue);
                }
            }

            if (Information != null)
            {
                Console.WriteLine(Information);
            }
           
            foreach (var t in _menuItems)
            {
               
                if (t.Selected)
                {
                    ColoredString.WriteLineString($"{t}  <----", ConsoleColor.Green);
                }
                else
                {
                    Console.WriteLine(t);
                }
            }

            Console.WriteLine("-------------------");
            Console.WriteLine("===================");
        }


        private void MoveDown()
        {
            var index = FindActiveItem();
            var currentItem = _menuItems[index];
            currentItem.Selected = false;
            index = index == _menuItems.Count - 1 ? -1 : index;
            _menuItems[index + 1].Selected = true;
        }

        private void MoveUp()
        {
            var index = FindActiveItem();
            var currentItem = _menuItems[index];
            currentItem.Selected = false;
            index = index == 0 ? _menuItems.Count : index;
            _menuItems[index - 1].Selected = true;
        }

        private int FindActiveItem()
        {
            var index = 0;

            foreach (var t in _menuItems)
            {
                if (t.Selected)
                {
                    return index;
                }

                index++;
            }

            return index;
        }
    }
}