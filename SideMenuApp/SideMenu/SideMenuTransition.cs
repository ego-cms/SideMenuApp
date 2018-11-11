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

        public void AnimateTransition(IUIViewControllerContextTransitioning transitionContext)
        {
            var container = transitionContext.ContainerView;
            container.UserInteractionEnabled = false;

            //Getting target and source VC
            var fromViewController = transitionContext.GetViewControllerForKey(UITransitionContext.ToViewControllerKey);
            var toViewController = transitionContext.GetViewControllerForKey(UITransitionContext.FromViewControllerKey);
            //Adding views to transition view
            container.AddSubview(fromViewController.View);
            container.AddSubview(toViewController.View);

            UIView.Animate(TransitionDurationTime, () =>
            {
                //scaling for target view
                var transform = CGAffineTransform.MakeScale(_targetViewControllerEndScale, _targetViewControllerEndScale);
                transform.Translate(UIScreen.MainScreen.Bounds.Width * _targetViewControllerOffset,0);
                toViewController.View.Transform = transform;
            }, () =>
            {
                container.UserInteractionEnabled = true;
                transitionContext.CompleteTransition(true);
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
