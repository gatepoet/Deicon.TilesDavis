﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace TilesDavis.Wpf.Util
{
    public class MarginSetter
    {
        public static Thickness GetMargin(DependencyObject obj)
        {
            return (Thickness)obj.GetValue(MarginProperty);
        }

        public static void SetMargin(DependencyObject obj, Thickness value)
        {
            obj.SetValue(MarginProperty, value);
        }

        // Using a DependencyProperty as the backing store for Margin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarginProperty = DependencyProperty.RegisterAttached("Margin", typeof (Thickness), typeof (MarginSetter), new UIPropertyMetadata(new Thickness(), MarginChangedCallback));
        public static void MarginChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
        {
            var panel = sender as Panel;
            if (panel == null)
                return;

            if (DesignerProperties.GetIsInDesignMode(panel))
            {
                Panel_Loaded(sender, new RoutedEventArgs());
            }
            else
            {
                panel.Loaded += new RoutedEventHandler(Panel_Loaded);
            }
        }

        private static void Panel_Loaded(object sender, RoutedEventArgs e)
        {
            var panel = sender as Panel;
            foreach (var child in panel.Children)
            {
                var fe = child as FrameworkElement;
                if (fe == null)
                    continue;
                fe.Margin = MarginSetter.GetMargin(panel);
            }
        }
    }
}