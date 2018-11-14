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
        private CALayer _shadowLayer;
        double _shadowLayerAnimationTime;

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

        #region Menu open/close animations

        private void ShowMenu(IUIViewControllerContextTransitioning transitionContext, UIView container, UIViewController fromViewController, UIViewController toViewController)
        {
            //Adding views to transition view
            container.AddSubview(fromViewController.View);
            container.AddSubview(toViewController.View);
            //Adding layer with shadow
            CreateShadowLayer(container, toViewController);

            UIView.Animate(TransitionDurationTime, 0.0f, UIViewAnimationOptions.CurveEaseOut, () =>
            {
                //offset to the right
                ShowMenuOffsetAnimationAction(toViewController.View);
                //animation for layer offset
                ShowMenuOffsetShadowLayerAnimation();
            }, null);

            //Need separate timing function for scaling
            CATransaction.Begin();
            CATransaction.AnimationTimingFunction = new CAMediaTimingFunction(0.25f, 0.1f, 0.25f, 1);

            UIView.Animate(TransitionDurationTime, () =>
            {
                //scaling for target view
                ShowMenuScaleAnimationAction(toViewController.View);
                //animation for layer scaling
                ShowMenuScaleShadowLayerAnimation();
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
            //move mainView layer above shadow layer
            mainView.Layer.ZPosition = 2;
            //move shadow layer below mainView, but above other layers
            _shadowLayer.ZPosition = 1;

            //Need separate handling when interactive transition
            if (_panGestureIsActive)
            {
                CATransaction.Begin();
                HideMenuLayerAnimation(CAMediaTimingFunction.Linear);
                UIView.Animate(TransitionDurationTime, TransitionDurationTime, UIViewAnimationOptions.CurveLinear, () =>
                {
                    HideMenuAnimation(mainView);
                }, () =>
                {
                    if (transitionContext.TransitionWasCancelled)
                    {
                        transitionContext.CompleteTransition(false);
                        _shadowLayer.ZPosition = 1;
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

                HideMenuLayerAnimation();
                UIView.Animate(TransitionDurationTime, () =>
                {
                    HideMenuAnimation(mainView);
                }, () =>
                {
                    HideMenuComplete(transitionContext, container, menuView, mainView);
                });
            }
        }

        #endregion

        #region Shadow layer

        void CreateShadowLayer(UIView container, UIViewController toViewController)
        {
            //Create layer with shadow parameters
            _shadowLayer = new CALayer
            {
                Frame = toViewController.View.Frame,
                ShadowRadius = 15.0f,
                ShadowOpacity = 0.3f,
                ShadowColor = UIColor.Black.CGColor,
                BackgroundColor = UIColor.Black.CGColor,
            };
            container.Layer.InsertSublayer(_shadowLayer, 1);
        }

        private void ShowMenuShadowLayerActions(UIView view)
        {
            //Hide layer because it's solid color will be visible during orientation translation
            _shadowLayer.Hidden = true;
            //Reset layer to it's original state
            _shadowLayer.RemoveAllAnimations();
            _shadowLayer.Transform = CATransform3D.Identity;

            //Reapply all layer's effects again
            _shadowLayer.Frame = view.Frame;
            _shadowLayer.Transform = _shadowLayer.Transform.Scale(0.8f, 0.8f, 1.0f);
            var f = _shadowLayer.Frame;
            f.X += UIScreen.MainScreen.Bounds.Width * _targetViewControllerOffset;
            _shadowLayer.Frame = f;
        }

        void ShowMenuScaleShadowLayerAnimation()
        {
            //Need two separate animations for scaling x & y
            //Array crashes when trying to set ZPosition
            var group = new CAAnimationGroup();
            var animationScaleX = new CABasicAnimation();
            animationScaleX.KeyPath = new NSString("transform.scale.x");
            animationScaleX.From = NSValue.FromObject(1.0f);
            animationScaleX.To = NSValue.FromObject(0.8f);

            var animationScaleY = new CABasicAnimation();
            animationScaleY.KeyPath = new NSString("transform.scale.y");
            animationScaleY.From = NSValue.FromObject(1.0f);
            animationScaleY.To = NSValue.FromObject(0.8f);
            group.Duration = TransitionDurationTime;
            group.FillMode = CAFillMode.Forwards;
            group.RemovedOnCompletion = false;
            group.TimingFunction = new CAMediaTimingFunction(0.25f, 0.1f, 0.25f, 1);
            group.Animations = new[] { animationScaleX, animationScaleY };

            _shadowLayer.AddAnimation(group, "ShowScaleLayerAnimation");
        }

        void ShowMenuOffsetShadowLayerAnimation()
        {
            var f = _shadowLayer.Frame;
            var animationOffset = new CABasicAnimation();
            animationOffset.KeyPath = new NSString("position");
            animationOffset.From = NSValue.FromCGPoint(new CGPoint(f.GetMidX(), f.GetMidY()));
            animationOffset.To = NSValue.FromCGPoint(new CGPoint(f.GetMidX() + UIScreen.MainScreen.Bounds.Width * _targetViewControllerOffset, f.GetMidY()));
            animationOffset.Duration = TransitionDurationTime;
            animationOffset.FillMode = CAFillMode.Forwards;
            animationOffset.RemovedOnCompletion = false;
            animationOffset.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseOut);
            //Must set layer frame explicitly, to animate it back when menu will hide
            f.X += UIScreen.MainScreen.Bounds.Width * _targetViewControllerOffset;
            _shadowLayer.Frame = f;

            _shadowLayer.AddAnimation(animationOffset, "ShowOffsetLayerAnimation");
        }

        void ShowMenuCancelLayerAnimation(double offset)
        {
            var group = new CAAnimationGroup();

            var animationOffset = new CABasicAnimation();
            animationOffset.KeyPath = new NSString("position");
            //Calculate position of shadow layer's X when animation was cancelled
            var offsetX = UIScreen.MainScreen.Bounds.Width * _targetViewControllerOffset;
            var originalX = _shadowLayer.Frame.GetMidX() - ((offset / TransitionDurationTime) * offsetX);
            //Start animatiom from cancelled X position 
            animationOffset.From = NSValue.FromCGPoint(new CGPoint(originalX, _shadowLayer.Frame.GetMidY()));
            //to position when animation began
            animationOffset.To = NSValue.FromCGPoint(new CGPoint(_shadowLayer.Frame.GetMidX(), _shadowLayer.Frame.GetMidY()));

            var scaleDiff = _targetViewControllerInitialScale - _targetViewControllerEndScale;
            //scaling of view when animation was cancelled
            var cancelledScalePosition = _targetViewControllerInitialScale - (scaleDiff - (offset / TransitionDurationTime) * scaleDiff);

            //Animate x/y positions from size when was cancelled
            //To size before interaction begin
            var animationScaleX = new CABasicAnimation();
            animationScaleX.KeyPath = new NSString("transform.scale.x");
            animationScaleX.From = NSValue.FromObject(cancelledScalePosition);
            animationScaleX.To = NSValue.FromObject(_targetViewControllerEndScale);

            var animationScaleY = new CABasicAnimation();
            animationScaleY.KeyPath = new NSString("transform.scale.y");
            animationScaleY.From = NSValue.FromObject(cancelledScalePosition);
            animationScaleY.To = NSValue.FromObject(_targetViewControllerEndScale);

            group.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.Linear);

            //It's pretty much random
            //with small animation time, it's looks good 
            //With long animation time it's not really fit, but still not so bad
            group.Duration = offset / 6;
            group.FillMode = CAFillMode.Forwards;
            group.RemovedOnCompletion = false;
            group.Animations = new[] { animationScaleX, animationScaleY, animationOffset };

            _shadowLayer.AddAnimation(group, "ShowScaleLayerAnimation");
        }


        void HideMenuLayerAnimation(NSString timingFunctionName = null)
        {
            CGRect f = _shadowLayer.Frame;
            var group = new CAAnimationGroup();
            var animationOffset = new CABasicAnimation();
            animationOffset.KeyPath = new NSString("position");
            animationOffset.From = NSValue.FromCGPoint(new CGPoint(f.GetMidX(), f.GetMidY()));
            animationOffset.To = NSValue.FromCGPoint(new CGPoint(f.GetMidX() - UIScreen.MainScreen.Bounds.Width * _targetViewControllerOffset, f.GetMidY()));

            var animationScaleX = new CABasicAnimation();
            animationScaleX.KeyPath = new NSString("transform.scale.x");
            animationScaleX.From = NSValue.FromObject(0.8f);
            animationScaleX.To = NSValue.FromObject(1.0f);

            var animationScaleY = new CABasicAnimation();
            animationScaleY.KeyPath = new NSString("transform.scale.y");
            animationScaleY.From = NSValue.FromObject(0.8f);
            animationScaleY.To = NSValue.FromObject(1.0f);

            //during pan gesture animation curve should be linear
            //In other cases EaseInOut
            group.TimingFunction = CAMediaTimingFunction.FromName(timingFunctionName ?? CAMediaTimingFunction.EaseInEaseOut);
            group.Duration = TransitionDurationTime;
            group.Animations = new[] { animationOffset, animationScaleX, animationScaleY };

            _shadowLayer.AddAnimation(group, "HideShadowLayerAnimation");
        }
        #endregion

        #region Rotation handling

        public void MenuOrientationWillChange()
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

        public void MenuOrientationDidChange()
        {
            //Show shadow layer again
            _shadowLayer.Hidden = false;
        }

        #endregion


        #region View animation actions

        private void ShowMenuAnimationActions(UIView view)
        {
            ShowMenuShadowLayerActions(view);

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
            var f = view.Frame;
            f.X += UIScreen.MainScreen.Bounds.Width * _targetViewControllerOffset;
            view.Frame = f;
        }

        void HideMenuAnimation(UIView mainView)
        {
            mainView.Transform = CGAffineTransform.MakeIdentity();
            var f = mainView.Frame;
            f.X -= UIScreen.MainScreen.Bounds.Width * _targetViewControllerOffset;
            mainView.Frame = f;
        }

        #endregion


        private void HideMenuComplete(IUIViewControllerContextTransitioning transitionContext, UIView container, UIView menuView, UIView mainView)
        {
            container.UserInteractionEnabled = true;
            _originalSuperview.Add(mainView);
            _originalSuperview = null;
            MainViewController = null;
            _gestureRecognizerView.RemoveFromSuperview();
            _gestureRecognizerView = null;
            _shadowLayer.RemoveFromSuperLayer();
            _shadowLayer = null;
            IsMenuOpen = false;
            transitionContext.CompleteTransition(true);
            menuView.RemoveFromSuperview();
        }

        #region Gesture recognizers

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

        #endregion

        #region Layer interactive transition

        public void StartLayerInteractiveTransition(CALayer layer)
        {
            _shadowLayerAnimationTime = layer.ConvertTimeFromLayer(0.0f, null);
            //Reset layer's speed for operating TimeOffset through interaction updates
            layer.Speed = 0.0f;
            layer.TimeOffset = _shadowLayerAnimationTime;
        }

        public void FinishLayerInteractiveTransition(CALayer layer)
        {
            var time = layer.TimeOffset;
            //Reset layers properties
            layer.Speed = 1.0f;
            layer.TimeOffset = 0.0f;
            layer.BeginTime = 0.0f;
            var timeSincePause = layer.ConvertTimeFromLayer(0.0f, null) - time;
            layer.BeginTime = timeSincePause;
        }

        public void UpdateLayerInteractiveTransition(CALayer layer, nfloat percentComplete)
        {
            //Manually update TimeOffset for layer animation during interaction
            layer.TimeOffset = _shadowLayerAnimationTime + TransitionDurationTime * percentComplete;
        }

        public void CancelLayerInteractiveTransition(CALayer layer)
        {
            layer.Speed = 1.0f;
            //Animate changes back when cancelling
            ShowMenuCancelLayerAnimation(layer.TimeOffset);
            layer.BeginTime = 0.0f;
            layer.TimeOffset = 0.0f;
        }


        #endregion

        #region Interactive transition overrides

        public override void StartInteractiveTransition(IUIViewControllerContextTransitioning transitionContext)
        {
            base.StartInteractiveTransition(transitionContext);
            StartLayerInteractiveTransition(_shadowLayer);
        }

        public override void UpdateInteractiveTransition(nfloat percentComplete)
        {
            base.UpdateInteractiveTransition(percentComplete);
            UpdateLayerInteractiveTransition(_shadowLayer, percentComplete);
        }

        public override void FinishInteractiveTransition()
        {
            base.FinishInteractiveTransition();
            FinishLayerInteractiveTransition(_shadowLayer);
        }

        public override void CancelInteractiveTransition()
        {
            base.CancelInteractiveTransition();
            CancelLayerInteractiveTransition(_shadowLayer);
        }

        #endregion


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
