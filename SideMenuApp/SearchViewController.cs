using System;
using UIKit;

namespace SideMenuApp
{
    public partial class SearchViewController : BaseViewController
    {
        private string _bgPortraitIcon = "SearchBG";
        private string _bgLandscapeIcon = "SearchLandscapeBG";

        public override void SetBackgroundImage(bool landscape)
        {
            BGImage.Image = UIImage.FromBundle(landscape ? _bgLandscapeIcon : _bgPortraitIcon);
        }

        public SearchViewController() : base("SearchViewController", null)
        {
        }

        public SearchViewController(IntPtr handle) : base(handle)
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

