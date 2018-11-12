using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace SideMenuApp.SideMenu
{
    public class SideMenuTransition : NSObject, IUIViewControllerTransitioningDelegate, IUIViewControllerAnimatedTransitioning
    {
        private const float _targetViewControllerOffset = 0.5f;
        private const float _targetViewControllerInitialScale = 1.0f;
        private const float _targetViewControllerEndScale = 0.8f;
        private double TransitionDurationTime { get; set; } = 0.5f;
        private UIView _originalSuperview;

        /// <summary>
        /// ViewController which starts current transition
        /// </summary>
        /// <value>View controller.</value>
        public UIViewController MainViewController { get; private set; }

        public bool IsMenuOpen { get; private set; }


        public void AnimateTransition(IUIViewControllerContextTransitioning transitionContext)
        {
            var container = transitionContext.ContainerView;
            container.UserInteractionEnabled = false;

            //Getting target and source VC
            var fromViewController = transitionContext.GetViewControllerForKey(UITransitionContext.ToViewControllerKey);
            var toViewController = transitionContext.GetViewControllerForKey(UITransitionContext.FromViewControllerKey);
            if (!IsMenuOpen)
            {
                _originalSuperview = toViewController.View.Superview;
                MainViewController = toViewController;
                ShowMenu(transitionContext, container, fromViewController, toViewController);
            }
            else
            {
                HideMenu(transitionContext, container, fromViewController, toViewController);
            }
        }

        private void ShowMenu(IUIViewControllerContextTransitioning transitionContext, UIView container, UIViewController fromViewController, UIViewController toViewController)
        {
            //Adding views to transition view
            container.AddSubview(fromViewController.View);
            container.AddSubview(toViewController.View);

            UIView.Animate(TransitionDurationTime, () =>
            {
                //scaling for target view
                var transform = CGAffineTransform.MakeScale(_targetViewControllerEndScale, _targetViewControllerEndScale);
                transform.Translate(UIScreen.MainScreen.Bounds.Width * _targetViewControllerOffset, 0);
                toViewController.View.Transform = transform;
            }, () =>
            {
                container.UserInteractionEnabled = true;
                IsMenuOpen = true;
                transitionContext.CompleteTransition(true);
            });
        }

        private void HideMenu(IUIViewControllerContextTransitioning transitionContext, UIView container, UIViewController fromViewController, UIViewController toViewController)
        {
            var menuView = toViewController.View;
            var mainView = fromViewController.View;

            container.AddSubview(menuView);
            container.AddSubview(mainView);
            UIView.Animate(TransitionDurationTime, () =>
            {
                mainView.Transform = CGAffineTransform.MakeIdentity();
            }, () =>
            {
                container.UserInteractionEnabled = true;
                _originalSuperview.Add(mainView);
                MainViewController = null;
                IsMenuOpen = false;
                transitionContext.CompleteTransition(true);
                menuView.RemoveFromSuperview();
            });
        }


        public double TransitionDuration(IUIViewControllerContextTransitioning transitionContext)
        {
            return TransitionDurationTime;
        }

        [Export("animationControllerForPresentedController:presentingController:sourceController:")]
        IUIViewControllerAnimatedTransitioning GetAnimationControllerForPresentedController(UIViewController presented, UIViewController presenting, UIViewController source)
        {
            return this;
        }

        [Export("animationControllerForDismissedController:")]
        IUIViewControllerAnimatedTransitioning GetAnimationControllerForDismissedController(UIViewController dismissed)
        {
            return this;
        }

    }
}
