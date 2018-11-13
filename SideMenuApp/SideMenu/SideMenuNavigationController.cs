using System;
using CoreGraphics;
using UIKit;
namespace SideMenuApp.SideMenu
{
    public class SideMenuNavigationController : UINavigationController, ISideMenuNavigationController
    {
        public event EventHandler<SideMenuNavigationControllerOrientationChangedEventArgs> OrientationChanged;

        public SideMenuNavigationController(Type navigationBarType, Type toolbarType) : base(navigationBarType, toolbarType)
        {
        }

        public SideMenuNavigationController()
        {
            NavigationBarHidden = true;
            SetViewControllers(new[] { new MenuViewController() }, false);
        }


        public override void ViewWillTransitionToSize(CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
        {
            base.ViewWillTransitionToSize(toSize, coordinator);

            if (View.Hidden)
                return;

            OrientationChanged?.Invoke(this, new SideMenuNavigationControllerOrientationChangedEventArgs(coordinator, toSize));
        }

        public SideMenuNavigationController(Foundation.NSCoder coder) : base(coder)
        {
        }

        public SideMenuNavigationController(Foundation.NSObjectFlag t) : base(t)
        {
        }

        public SideMenuNavigationController(IntPtr handle) : base(handle)
        {
        }

        public SideMenuNavigationController(string nibName, Foundation.NSBundle bundle) : base(nibName, bundle)
        {
        }

        public SideMenuNavigationController(UIViewController rootViewController) : base(rootViewController)
        {
        }

    }
}
