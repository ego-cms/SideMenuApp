[![eGo-CMS](https://rawgithub.com/ego-cms/Resources/master/Badges_by_EGO/by_EGO.svg)](http://ego-cms.com/?utm_source=github)

Side menu App
==================
Pretty simple iOS side menu application with pan, tap gesture supported. Also resistant to orientation changes.

![](https://rawgithub.com/ego-cms/Resources/master/SideMenuApp_images/SideMenuApp.gif)

How to use?
-----------

To show default side menu just call:
```c#
PresentViewController(SideMenuManager.Instance.MenuController as UIViewController, true, null);
```
To hide:
```c#
DismissViewController(true, null);
```

You can add your UIViewController as menu:
```c#
SideMenuManager.Instance.MenuController = new MySideNavigationMenuController();
```
Surely, MySideNavigationMenuController must implement ISideMenuNavigationController interface.

That's all, folks! &#128055;