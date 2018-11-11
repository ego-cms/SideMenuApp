using System;
using UIKit;
namespace SideMenuApp.SideMenu
{
    public class SideMenuNavigationController : UINavigationController
    {
        public SideMenuNavigationController(Type navigationBarType, Type toolbarType) : base(navigationBarType, toolbarType)
        {
        }

        public SideMenuNavigationController()
        {
            NavigationBarHidden = true;
            SetViewControllers(new[] { new MenuViewController() }, false);
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
