using System;
using System.Collections.Generic;
using System.Linq;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using SideMenuApp.Extensions;
using SideMenuApp.SideMenu;
using UIKit;

namespace SideMenuApp
{
    public partial class MenuViewController : UIViewController, IUITableViewDataSource, IUITableViewDelegate
    {
        private List<MenuModel> _menuItems = new List<MenuModel> {
            new MenuModel("Search", "MenuIcon1"),
            new MenuModel("Home", "MenuIcon2"),
            new MenuModel("Sale", "MenuIcon3"),
            new MenuModel("Catalog", "MenuIcon4"),
            new MenuModel("Orders", "MenuIcon5"),
            new MenuModel("Info", "MenuIcon6"),
            new MenuModel("Settings", "MenuIcon7"),
        };

        public MenuViewController() : base("MenuViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            var text = "Emily Blant";
            var attributedString = new NSMutableAttributedString(text);
            attributedString.AddAttribute(UIStringAttributeKey.KerningAdjustment, new NSNumber(2.2f), new NSRange(0, text.Length - 1));
            MenuName.AttributedText = attributedString;

            MenuTableView.RegisterNibForCellReuse(TableMenuCell.Nib, TableMenuCell.Key);
            MenuTableView.RowHeight = 44.0f;
            MenuTableView.WeakDelegate = this;
            MenuTableView.WeakDataSource = this;
            MenuTableView.SelectRow(NSIndexPath.FromRowSection(1, 0), false, UITableViewScrollPosition.None);

            MenuTableView.Alpha = 0.0f;
            MenuName.Alpha = 0.0f;
            MenuImage.Alpha = 0.0f;

            SideMenuManager.Instance.MenuWillAppear += (object sender, EventArgs e) =>
            {
                UIView.Animate(SideMenuManager.Instance.TransitionDuration, () =>
                {
                    MenuTableView.Alpha = 1.0f;
                    MenuName.Alpha = 1.0f;
                    MenuImage.Alpha = 1.0f;
                });
            };

            SideMenuManager.Instance.MenuWillDisappear += (object sender, EventArgs e) =>
            {
                UIView.Animate(SideMenuManager.Instance.TransitionDuration, () =>
                {
                    MenuTableView.Alpha = 0.0f;
                    MenuName.Alpha = 0.0f;
                    MenuImage.Alpha = 0.0f;
                });
            };
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            AddGradientIfNeeded();

            MenuImage.Layer.CornerRadius = MenuImage.Bounds.Width / 2;
            MenuImage.Layer.MasksToBounds = true;
        }

        private void AddGradientIfNeeded()
        {
            RemoveGradientLayers();

            var gradient = new CAGradientLayer();
            gradient.Frame = View.Bounds;
            gradient.Colors = new[] { ColorExtensions.FromHex("#4CA1A3").CGColor, ColorExtensions.FromHex("#4F4BA2").CGColor };
            gradient.StartPoint = new CGPoint(0.0f, 0.5f);
            gradient.EndPoint = new CGPoint(1.0f, 0.5f);
            gradient.Name = "MenuGradient";

            View.Layer.InsertSublayer(gradient, 0);
        }

        private void RemoveGradientLayers()
        {
            var layer = View.Layer.Sublayers.FirstOrDefault(item => item.Name?.Equals("MenuGradient") ?? false);
            layer?.RemoveFromSuperLayer();
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
            cell.Title = item.Name;
            cell.Icon = UIImage.FromBundle(item.IconName);
            var customView = new UIView
            {
                BackgroundColor = ColorExtensions.FromHex("#28ffffff")
            };
            cell.SelectedBackgroundView = customView;
            return cell;
        }

        [Export("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            string name = "";
            if (indexPath.Row == 0)
            {
                name = nameof(SearchViewController);
            }
            if (indexPath.Row == 1)
            {
                name = nameof(HomeViewController);
            }
            if (indexPath.Row == 2)
            {
                name = nameof(SaleViewController);
            }
            if (indexPath.Row == 3)
            {
                name = nameof(CatalogViewController);
            }
            if (indexPath.Row == 4)
            {
                name = nameof(OrdersViewController);
            }
            if (indexPath.Row == 5)
            {
                name = nameof(InfoViewController);
            }
            if (indexPath.Row == 6)
            {
                name = nameof(SettingsViewController);
            }

            var vc = UIStoryboard.FromName("Main", null)?.InstantiateViewController(name);
            PushAndHideMenu(vc);
        }

        private void PushAndHideMenu(UIViewController vc)
        {
            SideMenuManager.Instance.Push(vc);
            vc.DismissViewController(true, null);
        }
    }


    public class MenuModel
    {
        public string Name { get; set; }
        public string IconName { get; set; }

        public MenuModel(string name, string iconName)
        {
            Name = name;
            IconName = iconName;
        }
    }
}

