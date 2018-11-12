using System;
using UIKit;

namespace SideMenuApp.SideMenu
{
    public class SideMenuManager
    {
        private SideMenuTransition _transition;

        /// <summary>
        /// Checking if current UIViewController is presenting menu
        /// </summary>
        /// <value><c>true</c> if is menu open; otherwise, <c>false</c>.</value>
        public bool IsMenuOpen { get => _transition?.IsMenuOpen ?? false; }

        private static SideMenuManager _instance = new SideMenuManager();
        public static SideMenuManager Instance
        {
            get => _instance;
        }

        private SideMenuManager()
        {
        }

        SideMenuNavigationController _menuController;
        public SideMenuNavigationController MenuController
        {
            get
            {
                if (_menuController == null)
                {
                    Console.WriteLine("SideMenuManager: Creating SideMenuNavigationController from MenuController getter");
                    _menuController = CreateMenuController();
                }
                return _menuController;
            }
        }

        private SideMenuNavigationController CreateMenuController()
        {
            var menuController = new SideMenuNavigationController();
            _transition = new SideMenuTransition();
            menuController.TransitioningDelegate = _transition;
            menuController.ModalPresentationStyle = UIModalPresentationStyle.Custom;

            return menuController;
        }

        /// <summary>
        /// Push UIViewController when menu in open state
        /// </summary>
        /// <param name="viewController">View controller to push.</param>
        public void Push(UIViewController viewController)
        {
            if (_transition.MainViewController == null)
            {
                Console.WriteLine("Menu must be open for pushing new view controllers");
                return;
            }

            var navigationVC = _transition.MainViewController as UINavigationController;

            if (navigationVC == null)
            {
                Console.WriteLine("Host view controller must be UINavigationController");
                return;
            }

            navigationVC.PushViewController(viewController, false);
        }

    }
}
