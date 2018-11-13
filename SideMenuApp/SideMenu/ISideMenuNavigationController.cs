using System;
using CoreGraphics;
using UIKit;

namespace SideMenuApp.SideMenu
{
    /// <summary>
    /// Interface for side menu navigation controller.
    /// Provide supports for device orientation changes
    /// </summary>
    public interface ISideMenuNavigationController
    {
        event EventHandler<SideMenuNavigationControllerOrientationChangedEventArgs> OrientationChanged;
    }

    /// <summary>
    /// Represents event data for orientation changing
    /// </summary>
    public class SideMenuNavigationControllerOrientationChangedEventArgs : EventArgs
    {
        public IUIViewControllerTransitionCoordinator Coordinator { get; set; }
        public CGSize ToSize { get; set; }

        public SideMenuNavigationControllerOrientationChangedEventArgs(IUIViewControllerTransitionCoordinator coordinator, CGSize toSize)
        {
            Coordinator = coordinator;
            ToSize = toSize;
        }
    }

}
