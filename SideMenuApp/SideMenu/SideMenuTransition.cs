using System;
using CoreAnimation;
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

            UIView.Animate(TransitionDurationTime, 0.0f, UIViewAnimationOptions.CurveEaseOut, () =>
            {
                //offset to the right
                ShowMenuOffsetAnimationAction(toViewController.View);

            }, null);

            //Need separate timing function for scaling
            CATransaction.Begin();
            CATransaction.AnimationTimingFunction = new CAMediaTimingFunction(0.25f, 0.1f, 0.25f, 1);

            UIView.Animate(TransitionDurationTime, () =>
            {
                //scaling for target view
                ShowMenuScaleAnimationAction(toViewController.View);
            }, () =>
            {
                container.UserInteractionEnabled = true;
                IsMenuOpen = true;
                transitionContext.CompleteTransition(true);
            });

            CATransaction.Commit();

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
                _originalSuperview = null;
                MainViewController = null;
                IsMenuOpen = false;
                transitionContext.CompleteTransition(true);
                menuView.RemoveFromSuperview();
            });
        }

        public void MenuOrientationChanged()
        {
            if (MainViewController == null)
            {
                Console.WriteLine("Menu is closed no need to handle orientation changes");
                return;
            }

            //After device rotation main view's position will be wrongly translated
            //Reset Main view's transform and bounds, and set to values that will be correct after show animation
            MainViewController.View.Transform = CGAffineTransform.MakeIdentity();
            MainViewController.View.Frame = MainViewController.View.Superview.Bounds;
            ShowMenuAnimationActions(MainViewController.View);
        }

        private void ShowMenuAnimationActions(UIView view)
        {
            ShowMenuScaleAnimationAction(view);
            ShowMenuOffsetAnimationAction(view);
        }

        private void ShowMenuScaleAnimationAction(UIView view)
        {
            var t = view.Transform;
            t.Scale(_targetViewControllerEndScale, _targetViewControllerEndScale);
            view.Transform = t;
        }

        private void ShowMenuOffsetAnimationAction(UIView view)
        {
            var t = view.Transform;
            t.Translate(UIScreen.MainScreen.Bounds.Width * _targetViewControllerOffset, 0);
            view.Transform = t;
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
