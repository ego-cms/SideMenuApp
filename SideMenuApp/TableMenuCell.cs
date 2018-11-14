using System;

using Foundation;
using UIKit;

namespace SideMenuApp
{
    public partial class TableMenuCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("TableMenuCell");
        public static readonly UINib Nib;

        static TableMenuCell()
        {
            Nib = UINib.FromName("TableMenuCell", NSBundle.MainBundle);
        }

        public string Title
        {
            get => Name.Text;
            set => Name.Text = value;
        }

        public UIImage Icon
        {
            get => Image.Image;
            set => Image.Image = value;
        }

        protected TableMenuCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
    }
}
