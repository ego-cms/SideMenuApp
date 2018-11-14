using System;
using CoreGraphics;
using SideMenuApp.SideMenu;
using UIKit;

namespace SideMenuApp
{
    public abstract class BaseViewController : UIViewController
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

            //Set transparent NavigationBar
            NavigationController.NavigationBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
            NavigationController.NavigationBar.ShadowImage = new UIImage();
            NavigationController.NavigationBar.Translucent = true;
            NavigationController.View.BackgroundColor = UIColor.Clear;

            //Create custom button
            //For standard UIBarButton with image iOS make deafult offset
            //For avoid this create custom view with button within
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

        public override void ViewWillTransitionToSize(CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
        {
            base.ViewWillTransitionToSize(toSize, coordinator);

            coordinator.AnimateAlongsideTransition((obj) =>
            {
                var orientation = UIApplication.SharedApplication.StatusBarOrientation;
                var isLandscape = orientation == UIInterfaceOrientation.LandscapeLeft || orientation == UIInterfaceOrientation.LandscapeRight;
                SetBackgroundImage(isLandscape);

            },null);
        }

        public abstract void SetBackgroundImage(bool landscape);
    }
}
