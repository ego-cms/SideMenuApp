using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using SideMenuApp.SideMenu;
using UIKit;

namespace SideMenuApp
{
    public partial class MenuViewController : UIViewController, IUITableViewDataSource, IUITableViewDelegate
    {
        List<string> _menuItems = new List<string>
        {
            "Search",
            "Home",
            "Sale"
        };


        public MenuViewController() : base("MenuViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            MenuTableView.RegisterNibForCellReuse(TableMenuCell.Nib, TableMenuCell.Key);
            MenuTableView.RowHeight = 44.0f;
            MenuTableView.WeakDelegate = this;
            MenuTableView.WeakDataSource = this;

        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public nint RowsInSection(UITableView tableView, nint section)
        {
            return _menuItems.Count;
        }

        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (TableMenuCell)tableView.DequeueReusableCell(TableMenuCell.Key);
            var item = _menuItems.ElementAt(indexPath.Row);
            cell.Title = item;
            return cell;
        }

        [Export("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Row == 0)
            {
                var vc = UIStoryboard.FromName("Main", null)?.InstantiateViewController(nameof(SearchViewController));
                SideMenuManager.Instance.Push(vc);
                vc.DismissViewController(true, null);

            }
            if (indexPath.Row == 1)
            {
                var vc = UIStoryboard.FromName("Main", null)?.InstantiateViewController(nameof(HomeViewController));
                SideMenuManager.Instance.Push(vc);
                vc.DismissViewController(true, null);
            }
            if (indexPath.Row == 2)
            {
                var vc = UIStoryboard.FromName("Main", null)?.InstantiateViewController(nameof(SaleViewController));
                SideMenuManager.Instance.Push(vc);
                vc.DismissViewController(true, null);
            }
        }
    }
}