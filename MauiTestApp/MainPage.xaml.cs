
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using MauiTestApp.Models;
using Microsoft.Maui.Layouts;

namespace MauiTestApp
{
    public partial class MainPage : ContentPage
    {
        // Store all the rectangular items the user adds
        private List<RectItem> _rectItems = new List<RectItem>();

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnAddRectangleClicked(object sender, EventArgs e)
        {
            // Parse user input
            if (double.TryParse(WidthEntry.Text, out double w) &&
                double.TryParse(HeightEntry.Text, out double h))
            {
                if (w <= 0 || h <= 0)
                    return;

                // Create a new rectangle item with default position 0,0
                var newItem = new RectItem
                {
                    X = 0,
                    Y = 0,
                    Width = w,
                    Height = h,
                    //Label = $"W:{w}, H:{h}"
                };

                _rectItems.Add(newItem);

                // Add a corresponding Frame to the layout
                CreateFrameForRectItem(newItem);

                // Re-calculate the "paper" size so everything fits
                ResizePaperToFitAll();
            }
        }

        private void CreateFrameForRectItem(RectItem item)
        {
            // We'll use a Frame to represent the rectangle
            var frame = new Frame
            {
                BackgroundColor = Colors.SkyBlue,
                BorderColor = Colors.DarkBlue,
                CornerRadius = 0,
                WidthRequest = item.Width,
                HeightRequest = item.Height,
                Content = new Label
                {
                    //Text = item.Label,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
            };

            // Attach PanGestureRecognizer for dragging
            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += (s, e) => OnDragUpdated(s, e, item, frame);
            frame.GestureRecognizers.Add(panGesture);

            // Position the frame on the AbsoluteLayout
            // Note: We start everything at (0,0) as an example
            AbsoluteLayout.SetLayoutBounds(frame, new Rect(item.X, item.Y, item.Width, item.Height));
            AbsoluteLayout.SetLayoutFlags(frame, AbsoluteLayoutFlags.None);

            // Add to the AbsoluteLayout
            PaperLayout.Children.Add(frame);
        }

        private void OnDragUpdated(object sender, PanUpdatedEventArgs e, RectItem item, Frame frame)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Running:
                    // Update the X/Y based on the translation
                    item.X += e.TotalX;
                    item.Y += e.TotalY;

                    // Move the frame on the layout
                    AbsoluteLayout.SetLayoutBounds(frame, new Rect(item.X, item.Y, item.Width, item.Height));

                    // Reset the translation delta to 0 each time so we accumulate properly
                    // (this approach means each PanUpdated event gives the delta from the *previous* event,
                    // not from the initial position.)
                    break;

                case GestureStatus.Completed:
                case GestureStatus.Canceled:
                    // If you want to do boundary checks or final calculations, do it here
                    break;
            }
        }

        private void ResizePaperToFitAll()
        {
            // For demonstration: 
            //  - The "paper" width is the max X+width among all frames
            //  - The "paper" height is the max Y+height among all frames

            double maxRight = 0;
            double maxBottom = 0;

            foreach (var r in _rectItems)
            {
                double right = r.X + r.Width;
                double bottom = r.Y + r.Height;

                if (right > maxRight) maxRight = right;
                if (bottom > maxBottom) maxBottom = bottom;
            }

            // Add some margin if you like
            PaperLayout.WidthRequest = Math.Max(300, maxRight + 50);
            PaperLayout.HeightRequest = Math.Max(300, maxBottom + 50);
        }
    }
}

