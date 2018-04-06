using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace SamplesMeetup.Views
{
    public sealed partial class AnimationsPage : Page
    {
        #region [ Fields ]
        private Compositor compositor;
        private Visual visualRectangle;
        private Visual visualRectangleExpression;

        private CubicBezierEasingFunction cubicBezierEasingFunction;
        private Vector3KeyFrameAnimation vector3KeyFrameAnimation;
        private ScalarKeyFrameAnimation scalarKeyFrameAnimationRefreshEnd;
        #endregion [ Fields ]


        #region [ Constructors ]
        public AnimationsPage()
        {
            this.InitializeComponent();
            this.InitializeAnimation();
        }
        #endregion [ Constructors ]


        #region [ Events ]
        #region [ Events - Controls ]
        private void buttonStoryboard_Click(object sender, RoutedEventArgs e)
        {
            this.StoryboardRectangleManipulation?.Begin();
        }


        private void buttonComposition_Click(object sender, RoutedEventArgs e)
        {
            this.visualRectangle.StartAnimation("Offset", this.vector3KeyFrameAnimation);
            //this.visualRectangle.StartAnimation("Offset.X", this.scalarKeyFrameAnimationRefreshEnd);
            //this.scalarKeyFrameAnimationRefreshEnd.InsertKeyFrame(0.1f, 30f);
            this.visualRectangle.StartAnimation("RotationAngleInDegrees", this.scalarKeyFrameAnimationRefreshEnd);

            #region Expression
            //Animation with expression
            ExpressionAnimation expressionAnimation = this.compositor.CreateExpressionAnimation();

            expressionAnimation.Expression = "visualRectangle.Offset.X > 0 ? visualRectangle.Offset.X * Multiplier : 0.0f";
            expressionAnimation.SetScalarParameter("Multiplier", 2.0f);
            expressionAnimation.SetReferenceParameter("visualRectangle", this.visualRectangle);

            this.visualRectangleExpression.StartAnimation("Offset.X", expressionAnimation);
            #endregion Expresion
        }
        #endregion [ Events - Controls ]
        #endregion [ Events ]


        #region [ Functions ]
        /// <summary>
        /// Initialize data
        /// </summary>
        private void InitializeAnimation()
        {
            this.compositor = ElementCompositionPreview.GetElementVisual(this)?.Compositor;
            this.cubicBezierEasingFunction = this.compositor.CreateCubicBezierEasingFunction(new Vector2(0.8f, 0.8f), new Vector2(1.0f, 1.0f));

            //Get visual rectangle
            this.visualRectangle = ElementCompositionPreview.GetElementVisual(this.rectangleComposition);
            this.visualRectangle.CenterPoint = new Vector3(new Vector2(50f), 1);

            //Create animation offset  Vector 3
            this.vector3KeyFrameAnimation = this.compositor.CreateVector3KeyFrameAnimation();
            this.vector3KeyFrameAnimation.InsertKeyFrame(0f, new Vector3(0, 0, 0), cubicBezierEasingFunction);
            this.vector3KeyFrameAnimation.InsertKeyFrame(0.8f, new Vector3(100, 0, 0), cubicBezierEasingFunction);
            this.vector3KeyFrameAnimation.InsertKeyFrame(1.0f, new Vector3(150, 0, 0), cubicBezierEasingFunction);
            this.vector3KeyFrameAnimation.Duration = TimeSpan.FromMilliseconds(700);
            //vector3KeyFrameAnimation.IterationBehavior = AnimationIterationBehavior.Forever;

            //Create animation rotation Int
            this.scalarKeyFrameAnimationRefreshEnd = this.compositor.CreateScalarKeyFrameAnimation();
            this.scalarKeyFrameAnimationRefreshEnd.InsertKeyFrame(0f, 0);
            this.scalarKeyFrameAnimationRefreshEnd.InsertKeyFrame(0.8f, 0);
            this.scalarKeyFrameAnimationRefreshEnd.InsertKeyFrame(1.0f, 50);
            this.scalarKeyFrameAnimationRefreshEnd.Duration = TimeSpan.FromMilliseconds(700);


            #region Expression
            //Animation with expression
            this.visualRectangleExpression = ElementCompositionPreview.GetElementVisual(this.rectangleCompositionExpression);
            #endregion Expression
        }
        #endregion [ Functions ]
    }
}
