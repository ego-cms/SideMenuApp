using System;
using UIKit;

namespace SideMenuApp
{
    public partial class HomeViewController : BaseViewController
    {
        private string _bgPortraitIcon = "HomeBG";
        private string _bgLandscapeIcon = "HomeLandscapeBG";

        public override void SetBackgroundImage(bool landscape)
        {
            BGImage.Image = UIImage.FromBundle(landscape ? _bgLandscapeIcon : _bgPortraitIcon);
        }

        protected HomeViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public HomeViewController() : base("HomeViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}
