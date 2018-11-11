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
	[Register ("MainViewController")]
	partial class MainViewController
	{
		[Outlet]
		UIKit.UIButton PresentButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (PresentButton != null) {
				PresentButton.Dispose ();
				PresentButton = null;
			}
		}
	}
}
