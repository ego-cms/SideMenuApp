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

        /// <summary>
        /// Event which notifies that transition is going to play it's appearing animations
        /// </summary>
        public event EventHandler MenuWillAppear;
        /// <summary>
        /// Event which notifies that transition is finished playing it's appearing animations
        /// </summary>
        public event EventHandler MenuDidAppear;
        /// <summary>
        /// Event which notifies that transition is going to play it's disappearing animations
        /// </summary>
        public event EventHandler MenuWillDisappear;
        /// <summary>
        /// Event which notifies that transition is finished playing it's disappearing animations
        /// </summary>
        public event EventHandler MenuDidDisappear;
        /// <summary>
        /// Gets or sets the duration of the transition.
        /// </summary>
        /// <value>The duration of the transition.</value>
        public double TransitionDuration
        {
            get => _transition.TransitionDurationTime;
            set => _transition.TransitionDurationTime = value;
        }


        private static SideMenuManager _instance = new SideMenuManager();
        public static SideMenuManager Instance
        {
            get => _instance;
        }

        private SideMenuManager()
        {
            CreateTranistion();
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
            e.Coordinator.AnimateAlongsideTransition((IUIViewControllerTransitionCoordinator) => _transition.MenuOrientationWillChange(),
                                                     (IUIViewControllerTransitionCoordinator) => _transition.MenuOrientationDidChange());
        }

        private void CreateTranistion()
        {
            _transition = new SideMenuTransition();
            _transition.MenuDidAppear += (object sender, EventArgs e) => MenuDidAppear?.Invoke(this, e);
            _transition.MenuWillAppear += (object sender, EventArgs e) => MenuWillAppear?.Invoke(this, e);
            _transition.MenuDidDisappear += (object sender, EventArgs e) => MenuDidDisappear?.Invoke(this, e);
            _transition.MenuWillDisappear += (object sender, EventArgs e) => MenuWillDisappear?.Invoke(this, e);
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
