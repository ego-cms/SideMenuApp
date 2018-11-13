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
            _transition = new SideMenuTransition();
        }

        ISideMenuNavigationController _menuController;
        public ISideMenuNavigationController MenuController
        {
            get
            {
                if (_menuController == null)
                {
                    Console.WriteLine("SideMenuManager: Creating SideMenuNavigationController from MenuController getter");
                    _menuController = CreateMenuController();
                    SetupController(_menuController);
                }
                return _menuController;
            }
            set
            {
                _menuController = value;
                SetupController(_menuController);
            }

        }

        private SideMenuNavigationController CreateMenuController()
        {
            var menuController = new SideMenuNavigationController();
            return menuController;
        }

        private void SetupController(ISideMenuNavigationController menuController)
        {
            SetupUIViewController(menuController);
            menuController.OrientationChanged += MenuOrientationChanged;
        }

        private void SetupUIViewController(ISideMenuNavigationController menuController)
        {
            if (menuController is UIViewController vc)
            {
                vc.TransitioningDelegate = _transition;
                vc.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            }
            else
            {
                throw new SideMenuInitializationException($"Menu must be {nameof(UIViewController)} type");
            }
        }

        void MenuOrientationChanged(object sender, SideMenuNavigationControllerOrientationChangedEventArgs e)
        {
            e.Coordinator.AnimateAlongsideTransition((IUIViewControllerTransitionCoordinator) => _transition.MenuOrientationChanged(),
                                                     null);
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
