using System;
using UIKit;

namespace SideMenuApp.SideMenu
{
    public class SideMenuManager
    {
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
            menuController.TransitioningDelegate = new SideMenuTransition();
            menuController.ModalPresentationStyle = UIModalPresentationStyle.Custom;

            return menuController;
        }
    }
}
