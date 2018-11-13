using System;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace SideMenuApp.SideMenu
{
    public class SideMenuTransition : UIPercentDrivenInteractiveTransition, IUIViewControllerTransitioningDelegate, IUIViewControllerAnimatedTransitioning
    {
        private const float _targetViewControllerOffset = 0.5f;
        private const float _targetViewControllerInitialScale = 1.0f;
        private const float _targetViewControllerEndScale = 0.8f;
        private double TransitionDurationTime { get; set; } = 0.5f;
        private UIView _originalSuperview;
        private UIView _gestureRecognizerView;
        bool _panGestureIsActive;

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
                InitGestureRecognizerView(toViewController.View, container);
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
            //Need separate handling when interactive transition
            if (_panGestureIsActive)
            {
                CATransaction.Begin();
                UIView.Animate(TransitionDurationTime, TransitionDurationTime, UIViewAnimationOptions.CurveLinear, () =>
                {
                    mainView.Transform = CGAffineTransform.MakeIdentity();
                }, () =>
                {
                    if (transitionContext.TransitionWasCancelled)
                    {
                        transitionContext.CompleteTransition(false);
                        container.UserInteractionEnabled = true;
                        container.BringSubviewToFront(_gestureRecognizerView);
                    }
                    else
                    {
                        HideMenuComplete(transitionContext, container, menuView, mainView);
                    }
                });
                CATransaction.Commit();
            }
            else
            {
                UIView.Animate(TransitionDurationTime, () =>
                {
                    mainView.Transform = CGAffineTransform.MakeIdentity();
                }, () =>
                {
                    HideMenuComplete(transitionContext, container, menuView, mainView);
                });
            }

        }

        private void HideMenuComplete(IUIViewControllerContextTransitioning transitionContext, UIView container, UIView menuView, UIView mainView)
        {
            container.UserInteractionEnabled = true;
            _originalSuperview.Add(mainView);
            _originalSuperview = null;
            MainViewController = null;
            _gestureRecognizerView.RemoveFromSuperview();
            _gestureRecognizerView = null;
            IsMenuOpen = false;
            transitionContext.CompleteTransition(true);
            menuView.RemoveFromSuperview();
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

            //Also need to update bounds of gestureRecognizer view
            InitGestureRecognizerViewBounds(MainViewController.View);

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

        private void InitGestureRecognizerView(UIView mainView, UIView container)
        {
            _gestureRecognizerView = new UIView();
            InitGestureRecognizerViewBounds(mainView);
            _gestureRecognizerView.AddGestureRecognizer(new UITapGestureRecognizer(CloseMenu));
            _gestureRecognizerView.AddGestureRecognizer(new UIPanGestureRecognizer(HandlePanGesture));
            container.InsertSubviewAbove(_gestureRecognizerView, mainView);
        }

        private void InitGestureRecognizerViewBounds(UIView mainView)
        {
            _gestureRecognizerView.Transform = mainView.Transform;
            _gestureRecognizerView.Bounds = mainView.Bounds;
            _gestureRecognizerView.Center = mainView.Center;
        }

        void CloseMenu()
        {
            MainViewController.DismissViewController(true, null);
        }

        private void HandlePanGesture(UIPanGestureRecognizer recognizer)
        {
            var translation = recognizer.TranslationInView(recognizer.View);
            //distance of pan gestire from start position
            var distance = translation.X / UIScreen.MainScreen.Bounds.Width * -1;

            switch (recognizer.State)
            {
                case UIGestureRecognizerState.Began:
                    _panGestureIsActive = true;
                    MainViewController.DismissViewController(true, null);
                    break;
                case UIGestureRecognizerState.Changed:
                    var update = Math.Max(Math.Min((float)distance, 1.0f), 0.0f);
                    UpdateInteractiveTransition(update);
                    break;
                default:
                    _panGestureIsActive = false;
                    var velocity = recognizer.VelocityInView(recognizer.View).X * -1;
                    if (velocity >= 100 || velocity >= -50 && distance >= 0.5)
                    {
                        FinishInteractiveTransition();
                    }
                    else
                    {
                        CancelInteractiveTransition();
                    }
                    break;
            }
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

        [Export("interactionControllerForPresentation:")]
        IUIViewControllerInteractiveTransitioning GetInteractionControllerForPresentation(UIViewControllerAnimatedTransitioning animator)
        {
            return _panGestureIsActive ? this : null;
        }

        [Export("interactionControllerForDismissal:")]
        IUIViewControllerInteractiveTransitioning GetInteractionControllerForDismissal(UIViewControllerAnimatedTransitioning animator)
        {
            return _panGestureIsActive ? this : null;
        }
    }
}
