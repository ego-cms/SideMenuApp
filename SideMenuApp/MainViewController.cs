using System;

using UIKit;
using SideMenuApp.SideMenu;

namespace SideMenuApp
{
    public partial class MainViewController : UIViewController
    {
        protected MainViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            PresentButton.TouchUpInside += (object sender, EventArgs e) =>
            {
                PresentViewController(SideMenuManager.Instance.MenuController, true, null);
            };
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }


    }
}
