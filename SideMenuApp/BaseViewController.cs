using System;
using SideMenuApp.SideMenu;
using UIKit;
namespace SideMenuApp
{
    public class BaseViewController : UIViewController
    {
        public BaseViewController()
        {
        }

        public BaseViewController(Foundation.NSCoder coder) : base(coder)
        {
        }

        public BaseViewController(Foundation.NSObjectFlag t) : base(t)
        {
        }

        public BaseViewController(IntPtr handle) : base(handle)
        {
        }

        public BaseViewController(string nibName, Foundation.NSBundle bundle) : base(nibName, bundle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationController.NavigationBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
            NavigationController.NavigationBar.ShadowImage = new UIImage();
            NavigationController.NavigationBar.Translucent = true;
            NavigationController.View.BackgroundColor = UIColor.Clear;

            var navButton = new UIButton(UIButtonType.System);
            navButton.SetImage(UIImage.FromBundle("HamburgerIcon"), UIControlState.Normal);
            navButton.TintColor = UIColor.Black;
            navButton.TouchUpInside += HandleHamburgerPressed;
            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(navButton);
            NavigationItem.HidesBackButton = true;
        }

        private void HandleHamburgerPressed(object sender, EventArgs e)
        {
            if (SideMenuManager.Instance.IsMenuOpen)
            {
                DismissViewController(true, null);
            }
            else
            {
                PresentViewController(SideMenuManager.Instance.MenuController as UIViewController, true, null);
            }
        }
    }
}
