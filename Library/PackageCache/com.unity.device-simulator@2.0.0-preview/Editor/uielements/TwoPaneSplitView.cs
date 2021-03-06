using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.UIElements
{
    /// <summary>
    /// Borrow from https://github.com/Unity-Technologies/UIElementsExamples/tree/2019.1.1f1/Assets/Examples/Editor/TwoPaneSplitView.
    /// </summary>
    internal class TwoPaneSplitView : VisualElement
    {
        private static readonly string s_UssClassName = "unity-two-pane-split-view";
        private static readonly string s_ContentContainerClassName = "unity-two-pane-split-view__content-container";
        private static readonly string s_HandleDragLineClassName = "unity-two-pane-split-view__dragline";
        private static readonly string s_HandleDragLineAnchorClassName = "unity-two-pane-split-view__dragline-anchor";
        private static readonly string s_HorizontalClassName = "unity-two-pane-split-view--horizontal";

        public new class UxmlFactory : UxmlFactory<TwoPaneSplitView, UxmlTraits> {}

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlIntAttributeDescription m_LeftPaneInitialSize = new UxmlIntAttributeDescription { name = "left-pane-initial-size", defaultValue = 100 };
            UxmlIntAttributeDescription m_MinLeftPaneSize = new UxmlIntAttributeDescription { name = "minimum-left-pane-size", defaultValue = 100 };
            UxmlIntAttributeDescription m_MinRightPaneSize = new UxmlIntAttributeDescription { name = "minimum-right-pane-size", defaultValue = 100 };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var leftPaneInitialSize = m_LeftPaneInitialSize.GetValueFromBag(bag, cc);
                var minLeftPaneSize = m_MinLeftPaneSize.GetValueFromBag(bag, cc);
                var minRightPaneSize = m_MinRightPaneSize.GetValueFromBag(bag, cc);

                ((TwoPaneSplitView)ve).Init(leftPaneInitialSize, minLeftPaneSize, minRightPaneSize);
            }
        }

        public bool LeftPanelHidden { get; set; }

        private VisualElement m_LeftPane;
        private VisualElement m_RightPane;

        private VisualElement m_DragLine;
        private VisualElement m_DragLineAnchor;
        private float m_MinLeftDimension;
        private float m_MinRightDimension;

        private VisualElement m_Content;

        private float m_LeftPaneInitialDimension;

        private SquareResizer m_Resizer;

        private float m_PreviousLeftPaneWidth = 1;

        public TwoPaneSplitView()
        {
        }

        public void Init(float leftPaneStartDimension, float minLeftDimension, float minRightDimension)
        {
            AddToClassList(s_UssClassName);

            m_Content = new VisualElement();
            m_Content.name = "unity-content-container";
            m_Content.AddToClassList(s_ContentContainerClassName);
            m_Content.AddToClassList(s_HorizontalClassName);
            hierarchy.Add(m_Content);

            // Create drag anchor line.
            m_DragLineAnchor = new VisualElement();
            m_DragLineAnchor.name = "unity-dragline-anchor";
            m_DragLineAnchor.AddToClassList(s_HandleDragLineAnchorClassName);
            hierarchy.Add(m_DragLineAnchor);

            // Create drag
            m_DragLine = new VisualElement();
            m_DragLine.name = "unity-dragline";
            m_DragLine.AddToClassList(s_HandleDragLineClassName);
            m_DragLineAnchor.Add(m_DragLine);

            m_LeftPaneInitialDimension = leftPaneStartDimension;
            m_MinLeftDimension = minLeftDimension;
            m_MinRightDimension = minRightDimension;

            style.minWidth = m_LeftPaneInitialDimension;

            // We reply on the UIElement layout engine to add the children defined in the uxml.
            RegisterCallback<GeometryChangedEvent>(OnPostDisplaySetup);
        }

        private void OnPostDisplaySetup(GeometryChangedEvent evt)
        {
            Assert.AreEqual(2, m_Content.childCount);

            PostDisplaySetup();

            // We can only initialize the hidden state after we have the UIElements initialized.
            if (LeftPanelHidden)
                HideLeftPanel(LeftPanelHidden);

            UnregisterCallback<GeometryChangedEvent>(OnPostDisplaySetup);
            RegisterCallback<GeometryChangedEvent>(OnSizeChange);
        }

        private void PostDisplaySetup()
        {
            m_LeftPane = m_Content[0];
            m_LeftPane.style.width = m_LeftPaneInitialDimension;
            m_LeftPane.style.flexShrink = 0;

            m_RightPane = m_Content[1];
            m_RightPane.style.flexGrow = 1;
            m_RightPane.style.flexShrink = 0;
            m_RightPane.style.flexBasis = 0;

            m_DragLineAnchor.style.left = m_LeftPaneInitialDimension;

            m_Resizer = new SquareResizer(this, m_MinLeftDimension, m_MinRightDimension);
            m_DragLineAnchor.AddManipulator(m_Resizer);
        }

        private void OnSizeChange(GeometryChangedEvent evt)
        {
            var maxLength = this.resolvedStyle.width;
            var dragLinePos = m_DragLineAnchor.resolvedStyle.left;

            if (dragLinePos > maxLength)
            {
                var delta = maxLength - dragLinePos;
                m_Resizer.ApplyDelta(delta);
            }
        }

        public void HideLeftPanel(bool hidden)
        {
            LeftPanelHidden = hidden;

            if (LeftPanelHidden)
            {
                m_PreviousLeftPaneWidth = m_LeftPane.style.width.value.value;
                m_LeftPane.style.width = 1;
            }
            else
            {
                m_LeftPane.style.width = m_PreviousLeftPaneWidth;
                m_PreviousLeftPaneWidth = 1;
            }

            m_DragLineAnchor.style.visibility = LeftPanelHidden ? Visibility.Hidden : Visibility.Visible;
        }

        public override VisualElement contentContainer => m_Content;

        class SquareResizer : MouseManipulator
        {
            private Vector2 m_Start;
            protected bool m_Active;
            private TwoPaneSplitView m_SplitView;
            private VisualElement m_Pane;
            private float m_MinFirstDimension;
            private float m_MinSecondDimension;

            public SquareResizer(TwoPaneSplitView splitView, float minFirstDimension, float minSecondDimension)
            {
                m_MinFirstDimension = minFirstDimension;
                m_MinSecondDimension = minSecondDimension;
                m_SplitView = splitView;
                m_Pane = splitView.m_LeftPane;
                activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
                m_Active = false;
            }

            protected override void RegisterCallbacksOnTarget()
            {
                target.RegisterCallback<MouseDownEvent>(OnMouseDown);
                target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
                target.RegisterCallback<MouseUpEvent>(OnMouseUp);
            }

            protected override void UnregisterCallbacksFromTarget()
            {
                target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
                target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
                target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
            }

            public void ApplyDelta(float delta)
            {
                float oldDimension = m_Pane.resolvedStyle.width;
                float newDimension = oldDimension + delta;

                if (newDimension < oldDimension && newDimension < m_MinFirstDimension)
                    newDimension = m_MinFirstDimension;

                float maxLength = m_SplitView.resolvedStyle.width - m_MinSecondDimension;

                if (newDimension > oldDimension && newDimension > maxLength && maxLength > m_MinFirstDimension)
                    newDimension = maxLength;

                m_Pane.style.width = newDimension;
                target.style.left = newDimension;
            }

            private void OnMouseDown(MouseDownEvent e)
            {
                if (m_Active)
                {
                    e.StopImmediatePropagation();
                    return;
                }

                if (CanStartManipulation(e))
                {
                    m_Start = e.localMousePosition;

                    m_Active = true;
                    target.CaptureMouse();
                    e.StopPropagation();
                }
            }

            private void OnMouseMove(MouseMoveEvent e)
            {
                if (!m_Active || !target.HasMouseCapture())
                    return;

                Vector2 diff = e.localMousePosition - m_Start;
                float mouseDiff = diff.x;

                ApplyDelta(mouseDiff);

                e.StopPropagation();
            }

            private void OnMouseUp(MouseUpEvent e)
            {
                if (!m_Active || !target.HasMouseCapture() || !CanStopManipulation(e))
                    return;

                m_Active = false;
                target.ReleaseMouse();
                e.StopPropagation();
            }
        }
    }
}
