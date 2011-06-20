using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ascend.Core
{
    public interface IMenuItemContainer
    {
        IList<MenuItem> Items { get; set; }
        void AddItem(MenuItem i);
    }

    public class Menu : Entity, IMenuItemContainer
    {
        public string Name { get; set; }
        public IList<MenuItem> Items { get; set; }

        public void AddItem(MenuItem i)
        {
            (Items ?? (Items = new List<MenuItem>())).Add(i);
        }
    }

    public class MenuItem : IMenuItemContainer
    {
        public string Name { get; set; }
        public MenuItemType Type { get; set; }
        public string Location { get; set;}
        public IList<MenuItem> Items { get; set; }

        public void AddItem(MenuItem i)
        {
            (Items ?? (Items = new List<MenuItem>())).Add(i);
        }
    }

    public enum MenuItemType
    {
        None,
        Url,
        Catalog,
        Page,
        Quiz,
        Award,
        Survey,
        Game,
    }
}
