// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SideMenuApp
{
	[Register ("MenuViewController")]
	partial class MenuViewController
	{
		[Outlet]
		UIKit.UIImageView MenuImage { get; set; }

		[Outlet]
		UIKit.UILabel MenuName { get; set; }

		[Outlet]
		UIKit.UITableView MenuTableView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (MenuTableView != null) {
				MenuTableView.Dispose ();
				MenuTableView = null;
			}

			if (MenuName != null) {
				MenuName.Dispose ();
				MenuName = null;
			}

			if (MenuImage != null) {
				MenuImage.Dispose ();
				MenuImage = null;
			}
		}
	}
}
